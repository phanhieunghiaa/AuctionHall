using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Enums
{
    public enum ProductStatus
    {
        New = 0, //sản phẩm mới tạo
        Requesting = 1, //đang đợi sản phẩm được đăng kiểm
        Accepted = 2, //sản phẩm đã được đăng kiểm
        Denied = 3, //sản phẩm bị từ chối
        Hold = 4, //sản phẩm đấu giá thất bại
        Sold = 5, //sản phẩm đấu giá thành công

    }
}
