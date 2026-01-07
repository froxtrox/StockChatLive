using Microsoft.AspNetCore.SignalR;
using StockChatLive.Hubs;

namespace StockChatLive.Services
{
    public class SimulatedStockProvider : IRealTimeStockProvider
    {
        private readonly IHubContext<StockListingHub> _stockListingHub;
        private readonly ILogger<SimulatedStockProvider> _logger;
        private Timer? _timer;

        public SimulatedStockProvider(IHubContext<StockListingHub> stockHub, ILogger<SimulatedStockProvider> logger)
        {
            _stockListingHub = stockHub;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("RealTimeStockProvider started.");
            _timer = new Timer(async _ => await PostStocks(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            _timer?.Change(Timeout.Infinite, 0);
            DisposeTimer();
            _logger.LogInformation("RealTimeStockProvider stopped.");
            return Task.CompletedTask;
        }

        private void DisposeTimer()
        {
            _timer?.Dispose();
            _timer = null;
        }

        private async Task PostStocks()
        {
            decimal price = Random.Shared.Next(101, 113);
            _logger.LogDebug($"Posting stock price: {price}");
            await _stockListingHub.Clients.All.SendAsync("PostStocks", "PostStocks", price);
        }
    }
}