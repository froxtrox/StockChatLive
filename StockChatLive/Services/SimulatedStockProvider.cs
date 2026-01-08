using Microsoft.AspNetCore.SignalR;
using StockChatLive.Hubs;

namespace StockChatLive.Services
{
    public class SimulatedStockProvider : IRealTimeStockProvider, IAsyncDisposable
    {
        private readonly IHubContext<StockListingHub> _stockListingHub;
        private readonly ILogger<SimulatedStockProvider> _logger;
        private PeriodicTimer? _timer;
        private Task? _timerTask;
        private CancellationTokenSource? _cts;
        private readonly SemaphoreSlim _stateLock = new SemaphoreSlim(1, 1);
        private bool _disposed;

        public SimulatedStockProvider(IHubContext<StockListingHub> stockHub, ILogger<SimulatedStockProvider> logger)
        {
            _stockListingHub = stockHub;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await _stateLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (_disposed)
                    throw new ObjectDisposedException(nameof(SimulatedStockProvider));

                if (_cts != null)
                {
                    _logger.LogWarning("RealTimeStockProvider already started.");
                    return;
                }

                _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
                _timerTask = RunTimerAsync(_cts.Token);
                _logger.LogInformation("RealTimeStockProvider started.");
            }
            finally
            {
                _stateLock.Release();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await _stateLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (_cts == null)
                {
                    _logger.LogDebug("RealTimeStockProvider already stopped or not started.");
                    return;
                }

                _cts.Cancel();
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

                _cts.Dispose();
                _cts = null;
                _timer = null;

                _logger.LogInformation("RealTimeStockProvider stopped.");
            }
            finally
            {
                _stateLock.Release();
            }
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
            _logger.LogInformation($"Posting stock price: {price}");
            await _stockListingHub.Clients.All.SendAsync("PostStocks", "PostStocks", price);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            await StopAsync().ConfigureAwait(false);
            _stateLock.Dispose();
            _disposed = true;
        }
    }
}