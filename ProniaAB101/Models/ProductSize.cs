using ProniaAB101.Models.Base;

namespace ProniaAB101.Models
{
    public class ProductSize:BaseEntity
    {
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public Size? Size { get; set; }
        public Product? Product { get; set; }
    }
}
