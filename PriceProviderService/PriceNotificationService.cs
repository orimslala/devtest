
using Microsoft.AspNetCore.SignalR;
using PriceProviderService.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceProviderService
{
    public class PriceNotificationService : IPriceNotificationService
    {
        private readonly IHubContext<PricesHub> _hub;

        public PriceNotificationService(IHubContext<PricesHub> hub)
            => _hub = hub;
        public async Task NotifyPricesUpdated(IEnumerable<(string, string)> prices)
        {
            throw new NotImplementedException();
        }
    }
}
