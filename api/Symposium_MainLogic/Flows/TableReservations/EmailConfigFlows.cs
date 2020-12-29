using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using Symposium.WebApi.MainLogic.Interfaces.Flows.TableReservations;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.TableReservations
{
    public class EmailConfigFlows : IEmailConfigFlows
    {
        IEmailConfigTasks EmailConfigTasks;
        public EmailConfigFlows(IEmailConfigTasks emailConTasks)
        {
            this.EmailConfigTasks = emailConTasks;
        }

        /// <summary>
        /// Returns the Email Config
        /// </summary>
        /// <returns></returns>
        public EmailConfigModel GetEmailConfig(DBInfoModel Store)
        {
            // get the results
            EmailConfigModel emailConfigDetails = EmailConfigTasks.GetEmailConfig(Store);

            return emailConfigDetails;
        }

        /// <summary>
        /// Insert new Email Config
        /// </summary>
        /// <returns></returns>
        public EmailConfigModel InsertEmailConfig(DBInfoModel Store, EmailConfigModel model)
        {
            return EmailConfigTasks.InsertEmailConfig(Store, model);
        }

        /// <summary>
        /// Update an Email Config
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public EmailConfigModel UpdateEmailConfig(DBInfoModel Store, EmailConfigModel Model)
        {
            return EmailConfigTasks.UpdateEmailConfig(Store, Model);
        }

        /// <summary>
        /// Delete an Email Config
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteEmailConfig(DBInfoModel Store, long Id)
        {
            return EmailConfigTasks.DeleteEmailConfig(Store, Id);
        }
    }
}
