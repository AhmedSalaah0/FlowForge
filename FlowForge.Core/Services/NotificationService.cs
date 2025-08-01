using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Domain.RepositoryContract;
using FlowForge.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowForge.Core.Services
{
    public class NotificationService(INotificationRepository notificationRepository) : INotificationService
    {
        private readonly INotificationRepository _notificationRepository = notificationRepository;
        public Task<bool> SendNotification(Notification notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification), "Notification cannot be null.");
            }
            if (notification.NotificationId == Guid.Empty)
            {
                notification.NotificationId = Guid.NewGuid();
            }
            var result = _notificationRepository.SendNotification(notification);
            return result;
        }

        public Task<List<Notification>> GetNotifications(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var result = _notificationRepository.GetNotifications(userId);
            return result;
        }

        public Task<bool> DeleteNotification(Guid notificationId)
        {
            if (notificationId == Guid.Empty)
            {
                throw new ArgumentException("Notification ID cannot be empty.", nameof(notificationId));
            }
            var result = _notificationRepository.DeleteNotification(notificationId);
            return result;
        }
        public Task<int> GetUnreadNotificationCount(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var result = _notificationRepository.GetUnreadNotificationCount(userId);
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
    }
}
