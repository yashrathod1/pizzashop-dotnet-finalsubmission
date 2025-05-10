namespace pizzashop_service.Interface;

public interface IEmailSender
{
     Task SendEmailAsync(string email, string subject, string message);
}
