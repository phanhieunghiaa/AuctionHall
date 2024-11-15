using JewelryAuction.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Models.ResponseDTO
{
    public class AuctionDetailsDTO
    {
        public string ID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public AuctionStatus Status { get; set; }
        public float StartingPrice { get; set; }
        public float? FinalPrice { get; set; }
        public string Description { get; set; }
        public bool Approve { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public List<ImageDTO> Images { get; set; }
    }

    public class ImageDTO
    {
        public string Id { get; set; }
        public string ImgUrl { get; set; }
    }
}
