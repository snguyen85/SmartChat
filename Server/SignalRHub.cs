using Microsoft.AspNetCore.SignalR;
using SmartChat.Shared.ViewModels;

namespace SmartChat.Server
{
    public class SignalRHub : Hub
    {
        /// <summary>
        /// Send message to all connections with the user id
        /// </summary>
        /// <param name="message"></param>
        /// <param name="toUserId"></param>
        /// <returns></returns>
        public async Task BroadcastMessageAsync(ChatMessage message, string toUserId)
        {
            await Clients.User(toUserId).SendAsync("ChatNotification", message);
        }
    }
}
