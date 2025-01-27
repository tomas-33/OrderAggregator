namespace OrderAggregator.Model.DB
{
    public class Product : Base
    {
        public string? Name { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }
}
