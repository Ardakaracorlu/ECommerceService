namespace Notification.Consumer.Model.Response
{
    public class NotificationEmailResponse
    {
        public Guid OrderId { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }
}
