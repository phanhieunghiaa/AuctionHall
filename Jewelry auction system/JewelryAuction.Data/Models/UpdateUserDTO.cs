using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Models
{
    public class UpdateUserDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public int? CID { get; set; }
        public string Password { get; set; }
        public string? Phone { get; set; }
        public string Address { get; set; }
    }
}
