using MailKit.Net.Smtp;
using MimeKit;
using ResearchManagementSystem.Areas.CreSys.Data;
using ResearchManagementSystem.Areas.CreSys.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace ResearchManagementSystem.Areas.CreSys.Services
{
    public class EthicsEmailService : IEthicsEmailService
    {
        private readonly CreDbContext _context;
        private readonly string _fromEmail;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _appPassword;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EthicsEmailService(CreDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;

            _fromEmail = Environment.GetEnvironmentVariable("SMTP_FROM_EMAIL") ?? "cre.rmo4@gmail.com";
            _smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? "smtp.gmail.com";
            _smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
            _appPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? "njmg ofeo xzlo snbz";
        }

        public async Task SendEmailAsync(string toEmail, string recipientName, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Center for Research Ethics", _fromEmail));

                // Extract recipient name from email (before @) if not provided
                string displayName = recipientName ?? toEmail.Split('@')[0];
                message.To.Add(new MailboxAddress(displayName, toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder();

                // Build a simple HTML body using passed in 'body' text
                var htmlBody = $@"
                    <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='margin-bottom: 20px;'>
                                <p>Dear {recipientName},</p>
                                <p>{body}</p>
                            </div>
                            <footer style='margin-top: 20px;'>
                                <p>Best regards,</p>
                                <p>Center for Research Ethics</p>
                            </footer>
                        </body>
                    </html>";

                bodyBuilder.HtmlBody = htmlBody;
                message.Body = bodyBuilder.ToMessageBody();

                // Send the email using the SmtpClient from MailKit
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_smtpHost, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_fromEmail, _appPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while sending email: {ex.Message}");
            }
        }

        public async Task SendEmailWithAttachmentsAsync(string toEmail, string recipientName, string subject, string body, List<MimePart> attachments)
        {
            try
            {
                // Check if attachments list is empty
                if (attachments == null || attachments.Count == 0)
                {
                    Console.WriteLine("No attachments found.");
                }
                else
                {
                    Console.WriteLine($"Number of attachments: {attachments.Count}");
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Center for Research Ethics", _fromEmail));
                message.To.Add(new MailboxAddress(recipientName, toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder();

                // Build the HTML body for email
                var htmlBody = $@"
            <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='margin-bottom: 20px;'>
                        <p>Dear {recipientName},</p>
                        <p>{body}</p> <!-- Insert the body text here -->
                    </div>
                    <footer style='margin-top: 20px;'>
                        <p>Best regards,</p>
                        <p>Center for Research Ethics</p>
                    </footer>
                </body>
            </html>";

                bodyBuilder.HtmlBody = htmlBody;

                // Add attachments if provided
                if (attachments != null && attachments.Count > 0)
                {
                    foreach (var attachment in attachments)
                    {
                        // Add the attachment to the email
                        bodyBuilder.Attachments.Add(attachment);
                    }
                }

                message.Body = bodyBuilder.ToMessageBody();

                // Send email
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_smtpHost, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_fromEmail, _appPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                Console.WriteLine("Email with attachments sent successfully!");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while sending email with attachments: {ex.Message}");
            }
        }
    }
}
