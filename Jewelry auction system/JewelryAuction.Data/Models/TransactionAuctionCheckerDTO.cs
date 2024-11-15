using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Models
{
    public class TransactionAuctionCheckerDTO
    {
        public float Amount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Payment { get; set; }
        public string WalletId { get; set; }

    }
}
