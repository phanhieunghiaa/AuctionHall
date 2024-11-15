using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Models
{
    public class FeedBackDTO
    {
        public string ID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UserId { get; set; }
        public string AuctionId { get; set; }
        public string Content { get; set; }
        public string Status { get; set; }
        public string FullName { get; set; }
    }
}
