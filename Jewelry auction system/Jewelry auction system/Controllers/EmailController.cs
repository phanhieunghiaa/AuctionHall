using JewelryAuction.Services.Services;
using JewelryAuction.Services.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jewelry_auction_system.Controllers
{
    [Route("api/Email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        //private readonly EmailServices _emailService;
        private readonly IEmailServices _emailService;

        public EmailController(EmailServices emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("SendMail")]
        public async Task<IActionResult> SendEmail(string toEmail, string Subject, string Content)
        {
            await _emailService.SendEmail(toEmail, "Xác nhận tài khoản", Content);

            return Ok("User registered successfully. A confirmation email has been sent.");
        }

        [HttpPost("SendMail_Denied")]
        public async Task<IActionResult> SendSendEmail_DeniedNotificationEmail(string productId, string Content)
        {
            try
            {
                await _emailService.SendEmail_DeniedNotification(productId, Content);
                return Ok("Email has been sent successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
    }
}
