namespace Order.Api.Model.Response
{
    public class OrderStatusResponse
    {
        public Guid OrderId { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
    }
}
