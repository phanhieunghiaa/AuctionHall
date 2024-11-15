using JewelryAuction.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Models
{
    public class BidDTO
    {
        public string ID { get; set; }
        public string CreatedDate { get; set; }
        public string? UpdatedDate { get; set; }
        public float Value { get; set; }
        public string Status { get; set; }
        public string AuctionId { get; set; }
        public string UserId { get; set; }

    }
}
