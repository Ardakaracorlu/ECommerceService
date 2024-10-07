namespace Stock.Consumer.Model.Response
{
    public class StockQueueResponse
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
