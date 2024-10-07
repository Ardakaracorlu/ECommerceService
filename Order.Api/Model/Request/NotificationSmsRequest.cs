namespace Order.Api.Model.Request
{
    public class NotificationSmsRequest
    {
        public Guid OrderId { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
    }
}
