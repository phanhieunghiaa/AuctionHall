using JewelryAuction.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Models.ReqpuestDTO
{
    public class ProductRequestDTO
    {
        public string Name { get; set; }
        public ProductStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }


    }
}
