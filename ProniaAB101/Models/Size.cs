using ProniaAB101.Models.Base;

namespace ProniaAB101.Models
{
    public class Size:BaseEntity
    {
        public string Name { get; set; }
        public ICollection<ProductSize>? ProductSizes{ get; set; }
    }
}
