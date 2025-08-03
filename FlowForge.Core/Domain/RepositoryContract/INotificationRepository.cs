using FlowForge.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowForge.Core.Domain.RepositoryContract
{
    public interface INotificationRepository
    {
        Task<bool> SendNotification(Notification notification);
        Task<List<Notification>> GetNotifications(Guid userId);
        Task<Notification?> GetNotificationById(Guid notificationId);
        Task<Notification> EditNotification(Notification notification);
        Task<bool> DeleteNotification(Guid notificationId);
        Task<int> GetUnreadNotificationCount(Guid userId);
        Task<bool> MarkNotificationAsRead(Notification notification);
        Task MarkAllNotificationsAsRead(Guid userId);
    }
}
