using JewelryAuction.Data.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jewelry_auction_system.Data.Entity
{
    [Table("Categories")]
    public class Categories
    {
        [Key]
        public string ID { get; set; }
        public string Category { get; set; }

        public ICollection<CategoryType> CategoryType { get; set; }
    }
}
