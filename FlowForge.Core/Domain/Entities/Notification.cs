using Microsoft.Data.SqlClient;
using FlowForge.Core.Domain.IdentityEntities;
using FlowForge.Core.Enums;

namespace FlowForge.Core.Domain.Entities
{
    public class Notification
    {
        public Guid NotificationId { get; set; }
        public Guid? ReceiverId { get; set; }
        public Guid? SenderId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        public NotificationType NotificationType { get; set; }
        public ApplicationUser Receiver { get; set; }
        public ApplicationUser Sender { get; set; }
        public Guid? ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
