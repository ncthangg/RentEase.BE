using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace RentEase.Service.Helper
{
    public interface IEmailHelper
    {
        Task SendVerificationEmailAsync(string email, string verifyCode, string verificationLink);
    }
    public class EmailHelper : IEmailHelper
    {
        private readonly ISmtpClient _smtpClient;
        private readonly IConfiguration _config;

        public EmailHelper(ISmtpClient smtpClient, IConfiguration config)
        {
            _smtpClient = smtpClient;
            _config = config;
        }


        public async Task SendVerificationEmailAsync(string email, string verifyCode, string verificationLink)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Restease Appication", "Manager"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Verify Code";

            // Email body (plain text)
            message.Body = new TextPart("plain")
            {
                Text = $@"Code: {verifyCode}
==============^-^==============
Please verify your email by clicking on this link: {verificationLink}

If you didn't register for this account, please ignore this email.

Thank you!"
            };

            await _smtpClient.ConnectAsync(_config["SmtpSettings:Host"], int.Parse(_config["SmtpSettings:Port"]!), SecureSocketOptions.StartTls);
            await _smtpClient.AuthenticateAsync(_config["SmtpSettings:Username"], _config["SmtpSettings:Password"]);
            await _smtpClient.SendAsync(message);
            await _smtpClient.DisconnectAsync(true);
        }


    }
}
