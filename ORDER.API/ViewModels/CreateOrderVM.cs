namespace ORDER.API.ViewModels
{
    public class CreateOrderVM
    {
        public Guid BuyerID { get; set; }

        public List<CreateOrderItemVM> ? OrderItems { get; set; }



    }

    public class CreateOrderItemVM
    {
        public Guid ProductID { get; set; }

        public int Count { get; set; }

        public decimal Price { get; set; }


    }
}
