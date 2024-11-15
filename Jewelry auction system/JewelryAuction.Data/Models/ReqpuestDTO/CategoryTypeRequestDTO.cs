using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Models.ReqpuestDTO
{
    public class CategoryTypeRequestDTO
    {
        public string CategoryId { get; set; }
        public List<CategoryTypeDetailDTO> CategoryTypes { get; set; }
    }
}
