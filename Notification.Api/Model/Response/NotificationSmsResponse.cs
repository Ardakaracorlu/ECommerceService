namespace Notification.Api.Model.Response
{
    public class NotificationSmsResponse
    {
        public Guid OrderId { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
    }
}
