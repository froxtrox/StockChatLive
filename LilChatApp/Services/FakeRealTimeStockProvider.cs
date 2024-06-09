using Microsoft.AspNetCore.SignalR;

namespace LilChatApp.Services
{
    public class FakeRealTimeStockProvider : IRealTimeStockProvider
    {
        private readonly IHubContext<StockListingHub> _stockListingHub;
        private readonly ILogger<FakeRealTimeStockProvider> _logger;
        private Timer? _timer;

        public FakeRealTimeStockProvider(IHubContext<StockListingHub> stockHub, ILogger<FakeRealTimeStockProvider> logger)
        {
            _stockListingHub = stockHub;
            _logger = logger;
        }

        public void Start()
        {
            _logger.LogInformation("RealTimeStockProvider started.");
            _timer = new Timer(async _ => await PostStocks(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        public void Stop()
        {
            DisposeTimer();
            _timer?.Change(Timeout.Infinite, 0);
            _logger.LogInformation("RealTimeStockProvider stopped.");
        }

        private void DisposeTimer()
        {
            _timer?.Change(Timeout.Infinite, 0);
            _timer?.Dispose();
            _timer = null;
        }

        private async Task PostStocks()
        {
            decimal price = new Random().Next(101, 113);
            _logger.LogInformation($"Posting stock price: {price}");
            await _stockListingHub.Clients.All.SendAsync("PostStocks", "PostStocks", price);
        }
    }
}