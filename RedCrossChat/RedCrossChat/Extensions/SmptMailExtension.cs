using Microsoft.Bot.Schema;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RedCrossChat.Extensions
{
    public class SmptMailExtension
    {

        public static async Task<Boolean> SendEmailAsync(string toEmail, string subject, string body)
        {

            var smtpClient = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("noreply@redcross.or.ke", "Sok01405"),
                EnableSsl = true,
            };


            var mailMessage = new MailMessage
            {
                From = new MailAddress("noreply@redcross.or.ke"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);

                return true;
            }
            catch (Exception ex)
            {
                 
                Console.WriteLine("Error sending email: " + ex.Message);
            }

            return false;

        }
    }
}
