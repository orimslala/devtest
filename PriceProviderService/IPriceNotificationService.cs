using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PriceProviderService
{
    interface IPriceNotificationService
    {
        Task NotifyPricesUpdated(IEnumerable<(string, string)> prices);
    }
}