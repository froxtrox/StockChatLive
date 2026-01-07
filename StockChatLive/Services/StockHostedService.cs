namespace StockChatLive.Services
{
    public class StockPriceHostedService : IHostedService
    {
        private readonly IRealTimeStockProvider _realTimeStockProvider;

        public StockPriceHostedService(IRealTimeStockProvider realTimeStockProvider)
        {
            _realTimeStockProvider = realTimeStockProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _realTimeStockProvider.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _realTimeStockProvider.StopAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}