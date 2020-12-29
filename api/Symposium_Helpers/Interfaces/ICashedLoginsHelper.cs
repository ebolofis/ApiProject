using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Interfaces
{
    /// <summary>
    /// holds the list of cashed logins
    /// </summary>
    public interface ICashedLoginsHelper
    {

        /// <summary>
        /// Add a cashed login
        /// </summary>
        /// <param name="loginModel">DALoginModel</param>
        /// <param name="Id">Customer/Staff Id</param>
        void AddLogin(DALoginModel loginModel, long Id);



        /// <summary>
        /// Add a cashed login
        /// </summary>
        /// <param name="authentication">string that contains 'username:password' or 'AuthToken'</param>
        /// <param name="Id">Customer/Staff Id</param>
        void AddLogin(string authentication, long Id);



        /// <summary>
        /// return the customer/staff Id if the login exists to the cashed logins list. On Feature return -1.
        /// </summary>
        /// <param name="loginModel">DALoginModel</param>
        /// <returns></returns>
        long LoginExists(DALoginModel loginModel);


        /// <summary>
        /// return the customer/staff Id if the login exists to the cashed logins list. On Feature return -1.
        /// </summary>
        /// <param name="authentication">string that contains 'username:password' or 'AuthToken'</param>
        /// <returns></returns>
        long LoginExists(string authentication);

        /// <summary>
        /// Remove a login based in Id
        /// </summary>
        /// <param name="Id"></param>
        void RemoveLogin(long Id);

        /// <summary>
        /// Remove cashed logins from list that have not be searched for the last 10 min. 
        /// Perform this check every minute.
        /// </summary>
        void AutoClear();
    }
}
