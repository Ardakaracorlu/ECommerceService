namespace Order.Service.Model.Request
{
    public class StockQueueRequest
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
