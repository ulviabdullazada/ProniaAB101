using ProniaAB101.Models.Base;

namespace ProniaAB101.Models
{
    public class ProductInformation:BaseEntity
    {
        public string Shipping { get; set; }
        public string AboutReturnRequest { get; set; }
        public string Guarantee { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
