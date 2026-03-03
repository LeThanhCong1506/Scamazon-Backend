namespace MV.InfrastructureLayer.Interfaces;

public interface IEmailService
{
    Task SendOtpEmailAsync(string toEmail, string otp);
}
