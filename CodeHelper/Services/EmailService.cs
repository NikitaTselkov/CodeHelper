using FluentEmail.Core;
using FluentEmail.Smtp;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace CodeHelper.Services
{
    public class EmailService
    {
        private readonly IFluentEmail _fluentEmail;

        public EmailService(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
        }

        public async Task Send(string emailAddress, string content)
        {
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("codehelperemail@gmail.com", "hHR4JJn54htdFoes3");

            var sender = new SmtpSender(client);

            MailMessage mm = new MailMessage("codehelperemail@gmail.com", emailAddress, "woo nuget", content);
            mm.BodyEncoding = Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            client.Send(mm);

            //Email.DefaultSender = sender;

            //var email = await Email
            //    .From("codehelperemail@gmail.com")
            //    .To(emailAddress, "sue")
            //    .Subject("woo nuget")
            //    .Body(content)
            //    .SendAsync();
        }
    }
}
