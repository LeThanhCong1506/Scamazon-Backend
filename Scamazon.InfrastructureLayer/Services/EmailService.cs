using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MV.InfrastructureLayer.Interfaces;

namespace MV.InfrastructureLayer.Services;

public class EmailService : IEmailService
{
    private readonly string _host;
    private readonly int _port;
    private readonly string _username;
    private readonly string _password;
    private readonly string _senderEmail;
    private readonly string _senderName;

    public EmailService(IConfiguration configuration)
    {
        _host        = configuration["Smtp:Host"] ?? "smtp.supabase.com";
        _port        = int.Parse(configuration["Smtp:Port"] ?? "587");
        _username    = configuration["Smtp:Username"] ?? string.Empty;
        _password    = configuration["Smtp:Password"] ?? string.Empty;
        _senderEmail = configuration["Smtp:SenderEmail"] ?? "no-reply@scamazon.com";
        _senderName  = configuration["Smtp:SenderName"] ?? "Scamazon";
    }

    public async Task SendOtpEmailAsync(string toEmail, string otp)
    {
        var htmlBody = $@"
<!DOCTYPE html>
<html>
<body style=""font-family: Arial, sans-serif; max-width: 500px; margin: 0 auto; padding: 20px;"">
  <h2 style=""color: #FF6B35;"">Scamazon</h2>
  <p>Bạn đã yêu cầu đặt lại mật khẩu.</p>
  <p>Mã OTP của bạn là:</p>
  <div style=""background: #f4f4f4; border-radius: 8px; padding: 20px; text-align: center; margin: 20px 0;"">
    <span style=""font-size: 36px; font-weight: bold; letter-spacing: 8px; color: #FF6B35;"">{otp}</span>
  </div>
  <p>Mã có hiệu lực trong <strong>15 phút</strong>.</p>
  <p>Nếu bạn không yêu cầu đặt lại mật khẩu, hãy bỏ qua email này.</p>
  <hr />
  <p style=""color: #888; font-size: 12px;"">© 2025 Scamazon. Không trả lời email này.</p>
</body>
</html>";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_senderName, _senderEmail));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "Mã OTP đặt lại mật khẩu Scamazon";
        message.Body = new BodyBuilder
        {
            HtmlBody = htmlBody,
            TextBody = $"Mã OTP của bạn là: {otp}. Mã có hiệu lực trong 15 phút."
        }.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_host, _port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_username, _password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
