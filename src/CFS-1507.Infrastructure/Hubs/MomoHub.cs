using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Infrastructure.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace CFS_1507.Infrastructure.Hubs
{
    public class MomoHub : Hub, IMomoHub
    {
        public async Task ConnectAsync(string cart_id)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, cart_id);
            System.Console.WriteLine($"Connected to cart_id: {cart_id}");
        }

        public async Task DisconnectAsync(string cart_id)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, cart_id);
            System.Console.WriteLine($"Disconnected to cart_id: {cart_id}");
        }

        // public async Task NotifyPurchaseSuccessfully(string cart_id, string status)
        // {
        //     await Clients.Group(cart_id).SendAsync("PurchaseSuccessfully", cart_id, status);
        //     System.Console.WriteLine($"Purchase successfully cart_id: {cart_id}");
        // }

        // public async Task NotifyUserCancelPurchase(string cart_id, string status)
        // {
        //     await Clients.Group(cart_id).SendAsync("UserCancelOrder", cart_id, status);
        //     System.Console.WriteLine($"User canceled cart_id: {cart_id}");
        // }
    }
}