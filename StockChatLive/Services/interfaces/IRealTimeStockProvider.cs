namespace StockChatLive.Services
{
    public interface IRealTimeStockProvider
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}