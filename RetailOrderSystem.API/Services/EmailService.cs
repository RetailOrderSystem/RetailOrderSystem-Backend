using System.Net;
using System.Net.Mail;
using RetailOrderSystem.API.Services.Interfaces;

namespace RetailOrderSystem.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendOrderConfirmationAsync(
            string toEmail,
            int orderId,
            decimal totalAmount)
        {
            var smtpHost = _config["EmailSettings:SmtpHost"];
            var smtpPort = int.Parse(_config["EmailSettings:SmtpPort"]!);
            var senderEmail = _config["EmailSettings:SenderEmail"];
            var senderPassword = _config["EmailSettings:SenderPassword"];

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(
                    senderEmail,
                    senderPassword),
                EnableSsl = true
            };

            var subject = $"Order Confirmation #{orderId}";
            var body = $@"
                <h2>Order Confirmed ✅</h2>
                <p>Your order <b>#{orderId}</b> has been placed successfully.</p>
                <p>Total Amount: ₹{totalAmount}</p>
                <p>Thank you for shopping with us 🛒</p>";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail!),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}