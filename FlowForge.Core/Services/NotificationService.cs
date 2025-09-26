using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Domain.RepositoryContract;
using FlowForge.Core.Hubs;
using FlowForge.Core.ServiceContracts;
using Microsoft.AspNetCore.SignalR;

namespace FlowForge.Core.Services
{
    public class NotificationService(INotificationRepository notificationRepository, IHubContext<NotificationHub> hubContext) : INotificationService
    {
        private readonly INotificationRepository _notificationRepository = notificationRepository;
        public async Task<bool> SendNotification(Notification notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification), "Notification cannot be null.");
            }
            if (notification.NotificationId == Guid.Empty)
            {
                notification.NotificationId = Guid.NewGuid();
            }
            
            // First save to database
            var result = await _notificationRepository.SendNotification(notification);
            
            try
            {
                // Then send via SignalR
                await hubContext.Clients.User(notification.ReceiverId.ToString())
                    .SendAsync("ReceiveNotification", new {
                        notification.NotificationId,
                        notification.Message,
                        notification.SenderId,
                        notification.ReceiverId,
                        notification.ProjectId,
                        notification.NotificationType,
                        notification.IsRead,
                        CreatedAt = notification.CreatedAt.ToString("o")
                    });
            }
            catch (Exception ex)
            {
                // Log the error but don't fail the operation since the notification is already saved
                Console.Error.WriteLine($"Error sending notification via SignalR: {ex.Message}");
            }
            
            return result;
        }

        public async Task<List<Notification>> GetNotifications(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var result = await _notificationRepository.GetNotifications(userId);
            return result;
        }

        public async Task<bool> DeleteNotification(Guid notificationId)
        {
            if (notificationId == Guid.Empty)
            {
                throw new ArgumentException("Notification ID cannot be empty.", nameof(notificationId));
            }
            var result = await _notificationRepository.DeleteNotification(notificationId);
            return result;
        }
        public async Task<int> GetUnreadNotificationCount(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var result = await _notificationRepository.GetUnreadNotificationCount(userId);
            return result;
        }

        public async Task<bool> MarkNotificationAsRead(Guid notificationId)
        {
            if (notificationId == Guid.Empty)
            {
                throw new ArgumentException("Notification ID cannot be empty.", nameof(notificationId));
            }
            var notification = await _notificationRepository.GetNotificationById(notificationId);
            if (notification == null)
            {
                throw new ArgumentException("Notification not found.", nameof(notificationId));
            }
            var result = await _notificationRepository.MarkNotificationAsRead(notification);
            return result;
        }

        public async Task<bool> MarkAllNotificationsAsRead(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            
            await _notificationRepository.MarkAllNotificationsAsRead(userId);
            return true;
        }

        public async Task<Notification> EditNotification(Notification notification)
        {
            ArgumentNullException.ThrowIfNull(notification);

            var notify = await _notificationRepository.EditNotification(notification);
            return notify;
        }

        public async Task<bool> DeleteAllNotifications(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var notifications = await _notificationRepository.GetNotifications(userId);
            
            return await _notificationRepository.DeleteAllNotifications(notifications);
        }

        public async Task<bool> DeleteAllUserProjectNotifications(Guid userId, Guid projectId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            if (projectId == Guid.Empty)
            {
                throw new ArgumentException("Project ID cannot be empty.", nameof(projectId));
            }

            return await notificationRepository.DeleteAllUserProjectNotifications(userId, projectId);
        }
    }
}
