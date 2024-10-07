namespace Notification.Api.Data.Entities
{
    public class NotificationInfo
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string Recipient { get; set; }
        public string Message { get; set; }
        public int NotificationType { get; set; }
        public int NotificationStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public NotificationInfo()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }
    }
}
