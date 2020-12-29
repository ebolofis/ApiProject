using log4net;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using Symposium.WebApi.DataAccess.Interfaces.DT.TableReservations;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{
    public class EmailTasks : IEmailTasks
    {
        IEmailConfigDT DT;
        IEmailHelper emailHlp;
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public EmailTasks(IEmailConfigDT DT, IEmailHelper emailHlp)
        {
            this.DT = DT;
            this.emailHlp = emailHlp;
        }

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="email">the email to send</param>
        /// <param name="throwException">if true and there is no SMTP server configured then then throw exception, else return silently</param>
        public void SendEmail(EmailSendModel email, DBInfoModel Store, bool throwException = true)
        {

            if (email == null || email.To.Count == 0)
            {
                logger.Warn("Email model is null or there is no 'TO' email address");
                return;
            }

            //get SMTP Server 
            EmailConfigModel smtp = this.DT.GetEmailConfig(Store);


            //If there is no SMTP server then throw exception OR return silently
            if (smtp == null)
            {
                logger.Warn("There is no SMTP server");
                if (throwException)
                    throw new Exception("There is no SMTP server");
                else if (!throwException)
                    return;
            }

            //if smtp is not active return;
            if (!smtp.IsActive)
            {
                logger.Warn("SMTP server is NOT Active.");
                return;
            }


            //Send email
            logger.Info(String.Format("Sending email from smtp:{0}, port:{1}, ssl:{2}, username:{3} --> To:{4}", smtp.Smtp, smtp.Port, smtp.Ssl, smtp.Username, String.Join(", ", email.To.ToArray())));
            emailHlp.Init(smtp.Smtp, smtp.Port, smtp.Ssl, smtp.Username, smtp.Password);
            email.From = smtp.Sender;
            emailHlp.Send(email);
        }
    }
}
