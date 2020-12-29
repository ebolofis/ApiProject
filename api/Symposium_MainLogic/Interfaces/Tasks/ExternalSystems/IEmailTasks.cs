using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface IEmailTasks
    {
        /// <summary>
        /// Sends a email
        /// </summary>
        /// <param name="model">the email to send</param>
        /// <param name="throwException">if true and there is no SMTP server configured then then throw exception, else return silently</param>
        void SendEmail(EmailSendModel model, DBInfoModel Store, bool throwException = true);
    }
}
