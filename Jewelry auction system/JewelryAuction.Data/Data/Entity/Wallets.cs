using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jewelry_auction_system.Data.Entity
{
    [Table("Wallets")]
    public class Wallets
    {
        [Key]
        public string ID { get; set; }
        public float Balance { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }

        [ForeignKey("UserId")]
        public string UserId { get; set; }
        public Users Users { get; set; }

        public ICollection<Transactions> Transactions { get; set; }

    }
}
