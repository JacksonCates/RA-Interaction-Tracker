using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace RAInteractionTracker
{
    public class Mailer
    {
        private MailMessage mail;
        private SmtpClient SmtpServer;
        private string address, password;

        public Mailer(string emailServer, string address, string password)
        {
            // SMTP server address
            mail = new MailMessage();
            SmtpServer = new SmtpClient(emailServer);
            this.password = password;
            this.address = address;

            // Sets the from
            mail.From = new MailAddress(address);
        }


        public void NewMail(string to, string subject, string body)
        {
            // Sets the address to, the subject and body
            mail.To.Add(to);
            mail.Subject = subject;
            mail.Body = body;
        }

        public void AddAttachement(string path)
        {
            // Adds the file
            Attachment attachment = new Attachment(path);
            mail.Attachments.Add(attachment);            
        }

        public void Send()
        {
            // Adds credentials
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential(address, password);
            SmtpServer.EnableSsl = true;

            // Debugging
            Console.WriteLine("Logged in");
            SmtpServer.Send(mail);
            Console.WriteLine("Sent!");
        }
    }
}
