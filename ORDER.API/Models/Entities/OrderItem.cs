namespace ORDER.API.Models.Entities
{
    public class OrderItem
    {
        public Guid OrderItemID { get; set; }

        public Guid OrderID { get; set; }

        public Guid ProductID { get; set; }

        public int Count { get; set; }

        public decimal Price { get; set; }

        public Order Order { get; set; }
    }
}
