using log4net;
using Symposium.Helpers.Classes;
using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using Symposium.WebApi.DataAccess.Interfaces.DT.TableReservations;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks.TableReservations
{
    public class EmailConfigTasks : IEmailConfigTasks
    {
        IEmailConfigDT emailConfigDT;
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public EmailConfigTasks(IEmailConfigDT rDT)
        {
            this.emailConfigDT = rDT;
        }

        /// <summary>
        /// Returns the Email Config
        /// </summary>
        /// <returns></returns>
        public EmailConfigModel GetEmailConfig(DBInfoModel Store)
        {
            // get the results
            EmailConfigModel emailConfigDetails = emailConfigDT.GetEmailConfig(Store);

            return emailConfigDetails;
        }

        /// <summary>
        /// Insert new Email Config
        /// </summary>
        /// <returns></returns>
        public EmailConfigModel InsertEmailConfig(DBInfoModel Store, EmailConfigModel model)
        {
            return emailConfigDT.InsertEmailConfig(Store, model);
        }

        /// <summary>
        /// Update an Email Config
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public EmailConfigModel UpdateEmailConfig(DBInfoModel Store, EmailConfigModel Model)
        {
            return emailConfigDT.UpdateEmailConfig(Store, Model);
        }

        /// <summary>
        /// Delete an Email Config
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteEmailConfig(DBInfoModel Store, long Id)
        {
            return emailConfigDT.DeleteEmailConfig(Store, Id);
        }

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="email">the email to send</param>
        public void SendEmail(DBInfoModel Store, EmailSendModel email)
        {
            // 1. Check email
            if (email == null || email.To.Count == 0)
            {
                logger.Warn("Email model is null or there are no email recipients!");
                return;
            }

            // 2. Get SMTP server 
            EmailConfigModel smtpServer = emailConfigDT.GetEmailConfig(Store);
            if (smtpServer == null)
            {
                logger.Warn("There is no SMTP server!");
                return;
            }
            if (!smtpServer.IsActive)
            {
                logger.Warn("SMTP server is not active!");
                return;
            }

            // 3. Configure sender
            email.From = smtpServer.Sender;

            // 4. Send email
            logger.Info(String.Format("Sending email From smtp:{0}, port:{1}, ssl:{2}, username:{3} --> To:{4}", smtpServer.Smtp, smtpServer.Port, smtpServer.Ssl, smtpServer.Username, String.Join(", ", email.To.ToArray())));
            EmailHelper emailHelper = new EmailHelper();
            emailHelper.Init(smtpServer.Smtp, smtpServer.Port, smtpServer.Ssl, smtpServer.Username, smtpServer.Password);
            emailHelper.Send(email);

            return;
        }
    }
}
