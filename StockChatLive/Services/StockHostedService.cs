﻿namespace StockChatLive.Services
{
    public class StockPriceHostedService : IHostedService
    {
        private readonly IRealTimeStockProvider _realTimeStockProvider;

        public StockPriceHostedService(IRealTimeStockProvider realTimeStockProvider)
        {
            _realTimeStockProvider = realTimeStockProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _realTimeStockProvider.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _realTimeStockProvider.Stop();
            return Task.CompletedTask;
        }
    }
}