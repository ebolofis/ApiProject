using Symposium.Helpers.Interfaces;
using Symposium.Models.Models.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Classes
{
    public class EmailHelper : IEmailHelper
    {
        string smtp;
        int port;
        bool ssl;
        string username;
        string password;

        /// <summary>
        /// set smtp server and user to access it
        /// </summary>
        /// <param name="smtp"></param>
        /// <param name="port"></param>
        /// <param name="ssl"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void Init(string smtp, int port, bool ssl, string username, string password)
        {
            this.smtp = smtp;
            this.port = port;
            this.ssl = ssl;
            this.username = username;
            this.password = password;
        }

        /// <summary>
        /// Send an email
        /// </summary>
        /// <param name="EmailSendModel"></param>
        /// On EmailSendModel exists all the settings for From, To, subject and body
        public void Send(EmailSendModel emailModel)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(smtp);

                mail.From = new MailAddress(emailModel.From);
                foreach (string item in emailModel.To)
                {
                    mail.To.Add(item);
                }

                mail.Subject = emailModel.Subject;
                mail.Body = emailModel.Body;
                mail.IsBodyHtml = true;

                SmtpServer.Port = port;
                SmtpServer.Credentials = new System.Net.NetworkCredential(username, password);
                SmtpServer.EnableSsl = ssl;

                if (SmtpServer.EnableSsl)
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                                            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                }

                SmtpServer.Send(mail);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
