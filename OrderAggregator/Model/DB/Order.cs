namespace OrderAggregator.Model.DB
{
    public class Order : Base
    {
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }

        public Product? Product { get; set; }
    }
}
