using JewelryAuction.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jewelry_auction_system.Data.Entity
{
    [Table("Bids")]
    public class Bids
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public float Value { get; set; }
        public BidStatus Status { get; set; }

        [ForeignKey("AuctionId")]
        public string AuctionId { get; set; }
        public Auctions Auctions { get; set; }

        [ForeignKey("UserId")]
        public string UserId { get; set; }
        public Users Users { get; set; }

    }
}
