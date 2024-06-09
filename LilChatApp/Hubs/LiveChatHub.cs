using Microsoft.AspNetCore.SignalR;

namespace LilChatApp.Hubs
{
    public class LiveChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
