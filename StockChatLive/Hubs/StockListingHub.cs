using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace StockChatLive.Hubs
{
    [Authorize]
    public class StockListingHub : Hub
    {
        private readonly ILogger<StockListingHub> _logger;

        public StockListingHub(ILogger<StockListingHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Stock client connected: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception != null)
                _logger.LogWarning(exception, "Stock client disconnected with error: {ConnectionId}", Context.ConnectionId);
            else
                _logger.LogInformation("Stock client disconnected: {ConnectionId}", Context.ConnectionId);
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}
