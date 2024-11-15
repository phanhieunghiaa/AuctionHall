using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Repositories.Interfaces;
using JewelryAuction.Services.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JewelryAuction.Services.Services
{
    public class EmailServices : IEmailServices
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IBidRepository _bidRepository;
        private readonly IAuctionsRepository _auctionsRepository;


        public EmailServices(IEmailRepository emailRepository, IConfiguration configuration, 
            IUserRepository userRepository, IProductRepository productRepository, 
            IBidRepository bidRepository, IAuctionsRepository auctionsRepository)
        {
            _emailRepository = emailRepository;
            _configuration = configuration;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _bidRepository = bidRepository;
            _auctionsRepository = auctionsRepository;
        }

        #region
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task SendEmail(string toEmail, string subject, string content)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:FromEmail"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = content };
            email.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:Port"]), SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendEmail_ConfirmCode(string toEmail, string userName, string confirmCode)
        {
            string mailBox = _configuration.GetValue<string>("EmailSettings:FromEmail");
            string contentTemplate = "<p>Dear <strong>[User Name]</strong>,</p>\r\n    " +
                "<p>Thank you for registering with our service. To complete your registration, please use the following confirmation code:</p>\r\n    " +
                "<p><strong>Confirmation Code:</strong></p>\r\n    <p><strong>[Confirmation Code]</strong></p>\r\n " +
                "<br><p>If you did not request this code or have any questions, please contact us at: [Email].</p>\r\n    " +
                "<p>Thank you for choosing our service. We look forward to serving you.</p>\r\n    <p>Sincerely,</p>\r\n Auction Hall</p>";

            contentTemplate = Regex.Replace(contentTemplate, @"\[User Name\]", userName);
            contentTemplate = Regex.Replace(contentTemplate, @"\[Confirmation Code\]", confirmCode);
            contentTemplate = Regex.Replace(contentTemplate, @"\[Email]", mailBox);
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:FromEmail"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "Verification Code";

            var bodyBuilder = new BodyBuilder { HtmlBody = contentTemplate };
            email.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:Port"]), SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendEmail_WinBid(string bidId)
        {
            string mailBox = _configuration.GetValue<string>("EmailSettings:FromEmail");
            string contentTemplate = "<p>Dear <strong>[User Name]</strong>,</p>\r\n" +
    "<p>We are pleased to inform you that you have won the auction for the item <strong>[Item Name]</strong> with a bid of <strong>[Bid Amount]</strong>. " +
    "This is a great opportunity for you to own this item.</p>\r\n" +
    "<h2>Auction Details:</h2>\r\n" +
    "<ul>\r\n" +
    "<li><strong>Item Name:</strong> [Item Name]</li>\r\n" +
    "<li><strong>Winning Bid:</strong> [Bid Amount]</li>\r\n" +
    "<li><strong>Auction End Time:</strong> [End Time]</li>\r\n" +
    "</ul>\r\n" +
    "<p>If you have any questions or need further assistance, please contact our customer support team via email at [Email].</p>\r\n" +
    "<p>Thank you for participating in the auction and congratulations on becoming the new owner of <strong>[Item Name]</strong>.</p>\r\n" +
    "<p>Best regards,</p>\r\n" +
    "<p>Auction Hall</p>";

            var bid = await _bidRepository.GetBidById(bidId);
            var user = await _userRepository.GetUserById(bid.UserId);
            var toEmail = user.Email;
            var auction = await _auctionsRepository.GetSimpleAuction(bid.AuctionId);
            var product = await _productRepository.GetSimpleProductByIdAsync(auction.ProductId);

            contentTemplate = Regex.Replace(contentTemplate, @"\[User Name\]", user.FullName);
            contentTemplate = Regex.Replace(contentTemplate, @"\[Item Name\]", product.Name);
            contentTemplate = Regex.Replace(contentTemplate, @"\[Bid Amount\]", bid.Value.ToString());
            contentTemplate = Regex.Replace(contentTemplate, @"\[End Time\]", auction.EndDate.ToString("dd/MM/yyyy HH:mm:ss"));
            contentTemplate = Regex.Replace(contentTemplate, @"\[End Time\]", auction.EndDate.ToString("dd/MM/yyyy HH:mm:ss"));

            contentTemplate = Regex.Replace(contentTemplate, @"\[Email]", mailBox);
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:FromEmail"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "Biding result";

            var bodyBuilder = new BodyBuilder { HtmlBody = contentTemplate };
            email.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:Port"]), SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendEmail_DeniedNotification(string productId, string content)
        {
            if (string.IsNullOrWhiteSpace(productId) || string.IsNullOrEmpty(content))
            {
                throw new ArgumentException("productId of content is empty.");
            }
            else
            {
                var product = await _productRepository.GetSimpleProductByIdAsync(productId);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with this ID does not exist.");
                } else
                if (product.Status != Data.Enums.ProductStatus.Requesting)
                {
                    throw new KeyNotFoundException($"Product status must be requesting to be denied.");
                }
                else
                {
                    var user = await _userRepository.GetUserById(product.UserId);
                    if (user.Email == null)
                    {
                        throw new KeyNotFoundException("This user doesn't have an email.");
                    }
                    else
                    {
                        string mailBox = _configuration.GetValue<string>("EmailSettings:FromEmail");
                        string contentTemplate = "<p>Dear <strong>[User Name]</strong>,</p>\r\n    " +
                            "<p>Thank you for trusting and submitting your product to our auction system. " +
                            "After careful consideration and evaluation according to our criteria, " +
                            "we regret to inform you that your product does not meet the requirements to participate in the upcoming auction.</p>\r\n    " +
                            "<p><strong>Reason for rejection:</strong></p>\r\n    <p>[Denied reason]</p>\r\n    " +
                            "<br>" +
                            "<p>We understand that this may be disappointing and apologize for the inconvenience. " +
                            "We hope you can submit another product that better meets our criteria in the future.</p>\r\n    " +
                            "<p>If you need more detailed information about the reason for the rejection or have any questions, " +
                            "please contact us via email at: [Email] " +
                            "<p>Once again, thank you for choosing our auction system. We look forward to the opportunity to work with you in the future.</p>\r\n    " +
                            "<p>Sincerely,</p>\r\n Auction Hall.</p>";
                        contentTemplate = Regex.Replace(contentTemplate, @"\[User Name\]", user.FullName);
                        contentTemplate = Regex.Replace(contentTemplate, @"\[Denied reason]", content);
                        contentTemplate = Regex.Replace(contentTemplate, @"\[Email]", mailBox);

                        string toEmail = user.Email;
                        string subject = "Product denied";
                        var email = new MimeMessage();
                        email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:FromEmail"]));
                        email.To.Add(MailboxAddress.Parse(toEmail));
                        email.Subject = subject;

                        var bodyBuilder = new BodyBuilder { HtmlBody = contentTemplate };
                        email.Body = bodyBuilder.ToMessageBody();

                        using var smtp = new SmtpClient();
                        await smtp.ConnectAsync(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:Port"]), SecureSocketOptions.StartTls);
                        await smtp.AuthenticateAsync(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]);
                        await smtp.SendAsync(email);
                        await smtp.DisconnectAsync(true);
                        await _productRepository.UpdateStatusProduct(productId, Data.Enums.ProductStatus.Denied);
                    }
                }

            }
            #endregion
        }
    }
}
