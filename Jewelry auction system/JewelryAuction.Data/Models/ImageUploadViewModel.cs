﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Models
{
    public class ImageUploadViewModel
    {
        public IFormFile File { get; set; }
    }
}
