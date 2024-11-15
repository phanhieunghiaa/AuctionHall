using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Models
{
    public class AuctionsDTO
    {

        public string? StartDate { get; set; }
        //public string? EndDate { get; set; }      
        public float StartingPrice { get; set; }
        //public float? FinalPrice { get; set; } = null;
        public string Description { get; set; }
        public bool Approve { get; set; }
        public string Title { get; set; }
        public string? Detail { get; set; }
    }
}
