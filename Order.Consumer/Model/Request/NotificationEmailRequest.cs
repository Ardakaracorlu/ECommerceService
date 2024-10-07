namespace Order.Consumer.Model.Request
{
    public class NotificationEmailRequest
    {
        public Guid OrderId { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }
}
