using JewelryAuction.Data.Enums;
using JewelryAuction.Data.Repositories.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jewelry_auction_system.Data.Entity
{
    [Table("Feedbacks")]
    public class FeedBacks
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; }
        public FeedBackStatus Status { get; set; }

        public string Content { get; set; }

        [ForeignKey("UserId")]
        public string UserId { get; set; }
        public Users User { get; set; }

        [ForeignKey("AuctionId")]
        public string AuctionId { get; set; }
        public Auctions Auction { get; set; }


    }
}
