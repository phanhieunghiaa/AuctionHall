using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jewelry_auction_system.Data.Entity
{
    [Table("EmailNotification")]
    public class EmailNotification
    {
        [Key]
        public string ID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? Description { get; set; }
        public string? Detail { get; set; }
        

    }
}
