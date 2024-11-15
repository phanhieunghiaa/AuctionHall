using JewelryAuction.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Models
{
    public class ProductDTO
    {

        public string Name { get; set; }

        public string Price { get; set; }

        public string Description { get; set; }

        //public string UpdatedBy { get; set; } 
        public string UserId { get; set; }
        public List<string> ImageUrls { get; set; }
    }
}
