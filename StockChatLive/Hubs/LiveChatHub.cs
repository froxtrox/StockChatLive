using Microsoft.AspNetCore.SignalR;
using StockChatLive.Hubs.Interfaces;

namespace StockChatLive.Hubs
{
    public class LiveChatHub : Hub<ILiveChatClient>
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.ReceiveMessage(user, message);
        }
    }
}
