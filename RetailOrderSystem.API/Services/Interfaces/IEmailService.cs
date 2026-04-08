namespace RetailOrderSystem.API.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendOrderConfirmationAsync(
            string toEmail,
            int orderId,
            decimal totalAmount);
    }
}