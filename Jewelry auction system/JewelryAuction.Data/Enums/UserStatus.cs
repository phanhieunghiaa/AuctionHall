using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Enums
{
    public enum UserStatus
    {
        New = 0, //tài khoản vừa được khởi tạo
        Validated = 1, //tài khoản đã được xác nhận
        Active = 2, //tài khoản đủ điều kiện tham gia đấu giá
        Disable = 3, //tài khoản bị tạm dừng hoạt động
    }
}
