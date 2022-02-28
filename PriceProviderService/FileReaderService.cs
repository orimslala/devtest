using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PriceProviderService.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace PriceProviderService
{
    public class MissingFileException : Exception
    {
        public MissingFileException(string message) : base(message)
        { }
    }
    public class FileReaderService
    {
        private ILogger _logger;
        private IConfiguration _configurer;
        private string _file;
        private IHubContext<PricesHub> _hub;
        public FileReaderService(IHubContext<PricesHub> hub, ILogger logger, IConfiguration configurer)
        {
            _hub = hub;
            _logger = logger;
            _configurer = configurer;
            _file = _configurer.GetValue<string>("PricesFile");
            if (string.IsNullOrEmpty(_file) || !File.Exists(_file))
                throw new MissingFileException("Prices file not found");
        }

        (string,string) ParseLine( ref string line)
        {
            int separator = line.IndexOf(':');

            var left = line.Substring(0, separator);
            var right = line.Substring(separator + 1);
            //_logger.LogInformation($"Left: {left} Right: {right}");

            return (line.Substring(0, separator), line.Substring(separator + 1));
        }

        public static Func<Queue<MarketPrice>, Dictionary<string,double>> FuncGetAveragePrice = (queue) =>
        {
            var dict = new Dictionary<string, double>();
            var symbols = queue.Select(x => x.Symbol).Distinct();
            symbols.ToList().ForEach(x =>
            {
                var result = queue.Reverse().ToList().Where(mkt => mkt.Symbol == x).Take(5).Average(mkt => double.Parse(mkt.Price));
                dict[x] = result;
            });
            return dict;
        };

        public class MarketPrice
        {
            [JsonProperty("symbol")]
            public string Symbol { get; set; }

            [JsonProperty("price")]
            public string Price { get; set; }


            [JsonProperty("average")]
            public double Average { get; set; }
        }
        public async Task Read()
        {
            try
            {
                var queue = new Queue<MarketPrice>();
   
                for (int i = 0; i < 10; i++)
                {
                    using (var reader = new StreamReader(new FileStream(_file, FileMode.Open, FileAccess.Read, FileShare.Read)))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            (string key, string val) item = ParseLine(ref line);

                            queue.Enqueue(new MarketPrice { Symbol = item.key, Price = item.val });
                        }
                    }

                    FuncGetAveragePrice(queue).ToList().ForEach(av =>
                    {
                        foreach( var item in queue.ToList())
                        {
                            if (item.Symbol == av.Key)
                                item.Average = av.Value;
                        }                     
                    });
                    await _hub.Clients.All.SendAsync("OnPriceUpdated", queue.ToArray().Reverse());
                    queue.Clear();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
    }
}
