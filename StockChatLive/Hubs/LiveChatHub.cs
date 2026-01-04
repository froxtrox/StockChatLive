using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StockChatLive.Hubs.Interfaces;
using System.Text.Encodings.Web;

namespace StockChatLive.Hubs
{
    [Authorize]
    public class LiveChatHub : Hub<ILiveChatClient>
    {
        private readonly ILogger<LiveChatHub> _logger;
        private const int MaxMessageLength = 500;
        private const int MaxUsernameLength = 50;

        public LiveChatHub(ILogger<LiveChatHub> logger)
        {
            _logger = logger;
        }

        public async Task SendMessage(string message)
        {
            try
            {
                var user = Context.User?.Identity?.Name ?? "Anonymous";
                
                if (string.IsNullOrWhiteSpace(message))
                    throw new HubException("Message cannot be empty.");
                
                if (message.Length > MaxMessageLength)
                    throw new HubException($"Message too long (max {MaxMessageLength} characters).");

                var sanitizedMessage = HtmlEncoder.Default.Encode(message.Trim());
                
                await Clients.All.ReceiveMessage(user, sanitizedMessage);
            }
            catch (HubException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message");
                throw new HubException("Failed to send message. Please try again.");
            }
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception != null)
                _logger.LogWarning(exception, "Client disconnected with error: {ConnectionId}", Context.ConnectionId);
            else
                _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}
