using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFS_1507.Infrastructure.Interfaces
{
    public interface IMomoHub
    {
        Task ConnectAsync(string cart_id);
        Task DisconnectAsync(string cart_id);
        // Task NotifyPurchaseSuccessfully(string cart_id, string status);
        // Task NotifyUserCancelPurchase(string cart_id, string status);
    }
}