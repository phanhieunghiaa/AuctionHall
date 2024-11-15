using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Models.ReqpuestDTO
{
    public class ProductWithCategoryTypesDTO
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public List<string> CategoryTypeIds { get; set; }
    }
}
