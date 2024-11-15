using JewelryAuction.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jewelry_auction_system.Data.Entity
{
    [Table("Products")]
    public class Products
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Price { get; set; }
        public string Description { get; set; }
        public ProductStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        [ForeignKey("UserId")]
        public string UserId { get; set; }
        public Users Users { get; set; }

        public ICollection<Images> Images { get; set; }
        public ICollection<Auctions> Auctions { get; set; }
        public ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
