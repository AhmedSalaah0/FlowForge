using FlowForge.Core.Domain.Entities;
using FlowForge.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace FlowForge.Core.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation("Client disconnected: {ConnectionId}, Error: {Error}", 
                Context.ConnectionId, exception?.Message);
            await base.OnDisconnectedAsync(exception);
        }

        // Method to receive notifications from clients if needed
        public async Task SendNotification(Notification notification)
        {
            try
            {
                await Clients.User(notification.ReceiverId.ToString())
                    .SendAsync("ReceiveNotification", notification);
                _logger.LogInformation("Notification sent to user {UserId}", notification.ReceiverId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to user {UserId}", notification.ReceiverId);
                throw;
            }
        }
    }
}
