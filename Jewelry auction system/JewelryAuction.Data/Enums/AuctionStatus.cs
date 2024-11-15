using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Enums
{
    public enum AuctionStatus
    {
        Pending = 0, //đang được triển khai
        Active = 1, //đã đủ điều kiện đấu giá
        Done = 2, //đã hoàn thành
        None = 3, //thất bại hoặc bị hủy
    }
}
