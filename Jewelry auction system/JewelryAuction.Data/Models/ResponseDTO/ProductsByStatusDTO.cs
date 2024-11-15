﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Models.ResponseDTO
{
    public class ProductsByStatusDTO
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string CreatedDate { get; set; }
        public string? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UserId { get; set; }
    }
}
