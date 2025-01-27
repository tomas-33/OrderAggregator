using System.ComponentModel.DataAnnotations;

namespace OrderAggregator.Model
{
    public class AddProduct
    {
        [Required]
        public string Name { get; set; }
    }
}
