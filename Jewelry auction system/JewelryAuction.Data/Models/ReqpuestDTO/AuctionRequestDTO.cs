using JewelryAuction.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Models.ReqpuestDTO
{
    public class AuctionRequestDTO
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public AuctionStatus Status { get; set; }
        public float StartingPrice { get; set; }
        public float FinalPrice { get; set; } 
        public string? Description { get; set; }
        public bool Approve { get; set; }
        public string Title { get; set; }
        public string? Detail { get; set; }

        public string ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
