using FlowForge.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowForge.Core.ServiceContracts
{
    public interface INotificationService
    {
        Task<bool> SendNotification(Notification notification);
        Task<List<Notification>> GetNotifications(Guid userId);
        Task<Notification> EditNotification(Notification notification);
        Task<int> GetUnreadNotificationCount(Guid userId);
        Task<bool> MarkNotificationAsRead(Guid notificationId);
        Task<bool> MarkAllNotificationsAsRead(Guid userId);
        Task<bool> DeleteNotification(Guid notificationId);
    }
}
