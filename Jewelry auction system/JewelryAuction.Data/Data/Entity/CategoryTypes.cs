using Jewelry_auction_system.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Data.Entity
{
    [Table("CategoryType")]
    public class CategoryType
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public string CategoryTypes { get; set; }

        [ForeignKey("CategoryId")]
        public string CategoryId { get; set; }
        public Categories Category { get; set; }

        public ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
