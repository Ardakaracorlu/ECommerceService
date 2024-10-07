namespace Stock.Api.Model.Request
{
    public class OrderStatusRequest
    {
        public Guid OrderId { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }

        public OrderStatusRequest()
        {
            Status = false;
        }
    }
}
