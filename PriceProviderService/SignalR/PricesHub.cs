using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PriceProviderService.SignalR
{
    public class PricesHub : Hub
    {
        public async Task SendMessage((string,string) price)
        {
            await Clients.All.SendAsync("OnPriceUpdated", price);
        }

        public const string PRICES_GROUP = "Prices_";
        public async Task JoinGroup( string id)
        {
            await Groups.AddToGroupAsync(this.Context.ConnectionId, $"{PRICES_GROUP}{id}");
        }

        public async Task LeaveGroup(string id)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{PRICES_GROUP}{id}");
        }
    }
}
