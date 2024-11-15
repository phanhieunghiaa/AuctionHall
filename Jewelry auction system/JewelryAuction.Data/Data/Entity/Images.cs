using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jewelry_auction_system.Data.Entity
{
    [Table("Images")]
    public class Images
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ImgUrl { get; set; }

        [ForeignKey("ProductId")]
        public string ProductId { get; set; }
        public Products Products { get; set; }
    }
}
