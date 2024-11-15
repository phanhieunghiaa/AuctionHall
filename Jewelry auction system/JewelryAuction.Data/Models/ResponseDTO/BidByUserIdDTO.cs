using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Models.ResponseDTO
{
    public class BidByUserIdDTO
    {
        public string ID { get; set; }
        public string CreatedDate { get; set; }
        public string? UpdatedDate { get; set; }
        public float Value { get; set; }
        public string Status { get; set; }
        public string AuctionId { get; set; }
        public string UserId { get; set; }
        public string ProductName { get; set; }
    }
}
