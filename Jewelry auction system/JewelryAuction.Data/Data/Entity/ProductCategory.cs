using JewelryAuction.Data.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jewelry_auction_system.Data.Entity
{
    [Table("ProductCategory")]
    public class ProductCategory
    {
        [Key]
        public string ID { get; set; }

        [ForeignKey("ProductId")]
        public string ProductId { get; set; }
        public Products Products { get; set; }

        [ForeignKey("CategoryTypeId")]
        public string CategoryTypeId { get; set; }
        public CategoryType CategoryType { get; set; }

    }
}
