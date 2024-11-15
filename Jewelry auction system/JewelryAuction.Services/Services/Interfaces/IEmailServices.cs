using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Services.Services.Interfaces
{
    public interface IEmailServices
    {
        Task SendEmail(string toEmail, string subject, string content);
        Task SendEmail_DeniedNotification(string productId, string content);
        Task SendEmail_ConfirmCode(string toEmail, string userName, string confirmCode);
        Task SendEmail_WinBid(string bidId);
    }
}
