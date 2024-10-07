namespace Notification.Api.Data.Constants
{
    public class NotificationStatus
    {
        public const int Pending = 1;
        public const int Sent = 2;
        public const int Failed = 3;
    }
    public class NotificationType
    {
        public const int Email = 1;
        public const int Sms = 2;
        public const int PushNotification = 3;
    }
}
