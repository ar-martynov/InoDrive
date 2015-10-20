using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Configuration;

using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNet.Identity;

namespace Domain.Helpers
{
    public class EmailHelper : IIdentityMessageService
    {
        private string _mailLogin = "2inodrive@gmail.com";
        private string _mailPassword = "1a1a1a1a";
        private string _smtpAdress = "smtp.gmail.com";
        private int _smtpPort = 587;

        /// <summary>
        /// This method should send the message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendAsync(IdentityMessage message)
        {
            // Init SmtpClient and send
            var smtpClient = new SmtpClient(_smtpAdress, _smtpPort);
            // Create the credentials:
            smtpClient.Credentials = new NetworkCredential(_mailLogin, _mailPassword);

            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;

            //mail message 
            var mailMsg = new MailMessage(_mailLogin, message.Destination);

            // Subject and multipart/alternative Body
            mailMsg.Subject = message.Subject;
            mailMsg.Body = message.Body;
            mailMsg.IsBodyHtml = true;

            await smtpClient.SendMailAsync(mailMsg);
           
        }
    }
}