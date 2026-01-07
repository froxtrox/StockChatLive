using Microsoft.AspNetCore.SignalR;
using StockChatLive.Hubs;

namespace StockChatLive.Services
{
    public class SimulatedStockProvider : IRealTimeStockProvider
    {
        private readonly IHubContext<StockListingHub> _stockListingHub;
        private readonly ILogger<SimulatedStockProvider> _logger;
        private PeriodicTimer? _timer;
        private Task? _timerTask;
        private CancellationTokenSource? _cts;

        public SimulatedStockProvider(IHubContext<StockListingHub> stockHub, ILogger<SimulatedStockProvider> logger)
        {
            _stockListingHub = stockHub;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            _timerTask = RunTimerAsync(_cts.Token);
            _logger.LogInformation("RealTimeStockProvider started.");
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            _cts?.Cancel();
            _timer?.Dispose();

            if (_timerTask != null)
            {
                try
                {
                    await _timerTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("OperationCanceledException is caught.");
                }
                _timerTask = null;
            }

            _cts?.Dispose();
            _cts = null;
            _timer = null;

            _logger.LogInformation("RealTimeStockProvider stopped.");
        }

        private async Task RunTimerAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (_timer != null && await _timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
                {
                    await PostStocks();
                }
            }
            catch (OperationCanceledException)
            {
                 _logger.LogInformation("OperationCanceledException is caught.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in stock timer loop");
                throw;
            }
        }

        private async Task PostStocks()
        {
            decimal price = Random.Shared.Next(101, 113);
            _logger.LogDebug($"Posting stock price: {price}");
            await _stockListingHub.Clients.All.SendAsync("PostStocks", "PostStocks", price);
        }
    }
}