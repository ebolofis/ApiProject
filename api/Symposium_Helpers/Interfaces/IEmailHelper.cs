using Symposium.Models.Models.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Interfaces
{
    public interface IEmailHelper
    {
        /// <summary>
        /// set smtp server and user to access it
        /// </summary>
        /// <param name="smtp"></param>
        /// <param name="port"></param>
        /// <param name="ssl"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        void Init(string smtp, int port, bool ssl, string username, string password);


        /// <summary>
        /// Send an email
        /// </summary>
        /// <param name="EmailSendModel"></param>
        /// On EmailSendModel exists all the settings for From, To, subject and body
        void Send(EmailSendModel emailModel);
    }
}
