using MimeKit;
using MailKit.Net.Smtp;

namespace RentEase.Service.Helper
{
    public interface IEmailHelper
    {
        Task SendVerificationEmailAsync(string email, string verifyCode, string verificationLink);
    }
    public class EmailHelper : IEmailHelper
    {
        private readonly SmtpClient _smtpClient;

        public EmailHelper(SmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
        }

        public async Task SendVerificationEmailAsync(string email, string verifyCode, string verificationLink)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Movie Appication", "Manager"));
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
            await _smtpClient.SendAsync(message);
        }


    }
}
