namespace Order.Service.Model.Request
{
    public class CreateOrderRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string CustomerName { get; set; }
        public string CustomerSurname { get; set; }
        public string Adress { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
