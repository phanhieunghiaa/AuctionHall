using JewelryAuction.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jewelry_auction_system.Data.Entity
{
    [Table("Auctions")]
    public class Auctions
    {
        [Key]
        public string ID { get; set; } =  Guid.NewGuid().ToString();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public AuctionStatus Status { get; set; } 
        public float BasePrice { get; set; }
        public float StartingPrice { get; set; }
        public float? FinalPrice { get; set; }
        public string Description { get; set; }
        public bool Approve { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }

        [ForeignKey("ProductId")]
        public string ProductId { get; set; }
        public Products Products { get; set; }

        public ICollection<FeedBacks> FeedBacks { get; set; }
        public ICollection<Bids> Bids { get; set; }


    }
}
