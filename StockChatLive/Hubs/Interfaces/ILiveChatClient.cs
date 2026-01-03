namespace StockChatLive.Hubs.Interfaces
{
    public interface ILiveChatClient
    {
        Task ReceiveMessage(string user, string message);
    }
}