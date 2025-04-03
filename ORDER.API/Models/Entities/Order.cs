using ORDER.API.Models.Enums;

namespace ORDER.API.Models.Entities
{
    public class Order
    {
        public Guid OrderID { get; set; }

        public Guid BuyerID { get; set; }


        public decimal TotalPrice { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public DateTime CreatedDate { get; set; } 

    }
}
