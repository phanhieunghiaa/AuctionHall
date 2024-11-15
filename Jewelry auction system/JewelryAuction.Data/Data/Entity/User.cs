using JewelryAuction.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jewelry_auction_system.Data.Entity
{
    [Table("Users")]
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public int? CID { get; set; } // Cho phép null bằng cách sử dụng int?

        public string? Address { get; set; } // String đã mặc định cho phép null

        public UserRole? Role { get; set; } // Cho phép null bằng cách sử dụng int?

        public UserStatus? Status { get; set; } // Cho phép null bằng cách sử dụng int?
        public string? Phone { get; set; }

        public DateTime? CreatedDate { get; set; } // Cho phép null bằng cách sử dụng DateTime?

        public DateTime? UpdatedDate { get; set; } // Cho phép null bằng cách sử dụng DateTime?

        public string UpdatedBy { get; set; } // String đã mặc định cho phép null

        public ICollection<Products> Products { get; set; }
        public Wallets Wallets { get; set; }
        public ICollection<FeedBacks> FeedBacks { get; set; }
        public ICollection<Bids> Bids { get; set; }
    }
}
