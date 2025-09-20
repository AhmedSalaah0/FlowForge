using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Domain.RepositoryContract;
using FlowForge.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowForge.Infrastructure.Repositories
{
    public class NotificationRepository(ApplicationDbContext context, ILogger<NotificationRepository> logger) : INotificationRepository
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<NotificationRepository> _logger = logger;
        public async Task<bool> SendNotification(Notification notification)
        {
            _context.Notifications.Add(notification);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification: {Message}", ex.Message);
                return false;
            }
            return true;
        }
        public async Task<bool> DeleteNotification(Guid notificationId)
        {
            _context.Notifications.RemoveRange(_context.Notifications.Where(n => n.NotificationId == notificationId));
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification: {Message}", ex.Message);
                return false;
            }
            return true;
        }

        public async Task<List<Notification>> GetNotifications(Guid userId)
        {
            return await _context.Notifications
                .Where(n => n.ReceiverId == userId)
                .Include(n => n.Project)
                    .ThenInclude(pm => pm.ProjectMembers)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public Task<Notification?> GetNotificationById(Guid notificationId)
        {
            return _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId);
        }

        public Task<int> GetUnreadNotificationCount(Guid userId)
        {
            return _context.Notifications
                .CountAsync(n => n.ReceiverId == userId && !n.IsRead);
        }

        public Task<bool> MarkNotificationAsRead(Notification notification)
        {
            notification.IsRead = true;
            _context.Notifications.Update(notification);
            try
            {
                return _context.SaveChangesAsync().ContinueWith(t => t.IsCompletedSuccessfully);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read: {Message}", ex.Message);
                return Task.FromResult(false);
            }
        }

        public async Task MarkAllNotificationsAsRead(Guid userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.ReceiverId == userId && !n.IsRead)
                .ToListAsync();
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }
            _context.Notifications.UpdateRange(notifications);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read: {Message}", ex.Message);
            }
        }

        public async Task<Notification> EditNotification(Notification notification)
        {
            _context.Update(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public Task<bool> DeleteAllNotifications(IEnumerable<Notification> notifications)
        {
            _context.Notifications.RemoveRange(notifications);
            try
            {
                return _context.SaveChangesAsync().ContinueWith(t => t.IsCompletedSuccessfully);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting all notifications: {Message}", ex.Message);
                return Task.FromResult(false);
            }
        }
    }
}
