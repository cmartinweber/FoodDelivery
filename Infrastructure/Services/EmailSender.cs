﻿using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
namespace Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IConfiguration _config)
        {

        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailToSend = new MimeMessage();
            emailToSend.From.Add(MailboxAddress.Parse("cmartinwebersmtp@gmail.com"));
            emailToSend.To.Add(MailboxAddress.Parse(email));
            emailToSend.Subject = subject;
            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };
            ////send email using a GMAIL ACCOUNT
            using (var emailClient = new MailKit.Net.Smtp.SmtpClient())
            {
                emailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                //valid email address, with special application password
                //See https://support.google.com/mail/answer/185833?hl=en for details
                emailClient.Authenticate("cmartinwebersmtp@gmail.com", "qxjhbwgysgpreokb");
                emailClient.Send(emailToSend);
                emailClient.Disconnect(true);
            }
            return Task.CompletedTask;
        }
    }
}
