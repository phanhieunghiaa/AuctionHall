using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jewelry_auction_system.Data.Entity
{
    [Table("Transactions")]
    public class Transactions
    {
        [Key]
        public string ID { get; set; }
        public float Amount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Payment { get; set; }

        [ForeignKey("WalletId")]
        public string WalletId { get; set; }
        public Wallets Wallets { get; set; }

    }
}
