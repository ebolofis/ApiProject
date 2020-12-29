using log4net;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Symposium.Helpers.Classes
{
    /// <summary>
    /// Class that holds the list of cashed logins
    /// </summary>
    public class CashedLoginsHelper: ICashedLoginsHelper
    {
        private ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        DateTime threashold;
        int lastMin = 0;
        int count,newCount;
       // string loginExistsError, addLoginError, autoCleanError;

        /// <summary>
        /// the list of cashed logins
        /// </summary>
        public List<CashedLoginModel> logins { get; set; }

        public CashedLoginsHelper()
        {
            logins = new List<CashedLoginModel>(180);
        }


        /// <summary>
        /// Add a cashed login
        /// </summary>
        /// <param name="loginModel">DALoginModel</param>
        /// <param name="Id">Customer/Staff Id</param>
        public void AddLogin(DALoginModel loginModel, long Id)
        {
            AddLogin(loginModel.Email + ":" + loginModel.Password,Id);
        }

        /// <summary>
        /// Add a cashed login
        /// </summary>
        /// <param name="authentication">string that contains 'username:password' or 'AuthToken'</param>
        /// <param name="Id">Customer/Staff Id</param>
        public void AddLogin(string authentication, long Id)
        {
            lock (logins)
            {
                if (doLoginExist(authentication) >= 0) return;
                try
                {
                    CashedLoginModel model = new CashedLoginModel();
                    model.LastUpdate = DateTime.Now;
                    model.Authentication = authentication;
                    model.Id = Id;
                    logins.Add(model);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
                AutoClear();
            }
            
        }


        /// <summary>
        /// return the customer/staff Id if the login exists to the cashed logins list
        /// </summary>
        /// <param name="loginModel">DALoginModel</param>
        /// <returns></returns>
        public long LoginExists(DALoginModel loginModel)
        {
            return LoginExists(loginModel.Email.ToLower() + ":" + loginModel.Password);
        }


        /// <summary>
        /// return the customer/staff Id if the login exists to the cashed logins list. On Feature return -1.
        /// </summary>
        /// <param name="authentication">string that contains 'username:password' or 'AuthToken'</param>
        /// <returns></returns>
        public long LoginExists(string authentication)
        {
            lock (logins)
            {
                return doLoginExist(authentication);
            }
        }

        private long doLoginExist(string authentication)
        {
            if (string.IsNullOrWhiteSpace(authentication)) return -1;
            try
            {
                int pos = logins.FindIndex(x => x.Authentication == authentication);
                if (pos >= 0)
                {
                    logins[pos].LastUpdate = DateTime.Now;
                    return logins[pos].Id;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
           return -1;
        }

        /// <summary>
        /// Remove a login based in Id
        /// </summary>
        /// <param name="Id"></param>
        public void RemoveLogin(long Id)
        {
            lock (logins)
            {
                try
                {
                    logins.RemoveAll(x =>x.Id==Id);
                    newCount = logins.Count();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }

        }

        /// <summary>
        /// Remove cashed logins from list that have not be searched for the last 10 min. 
        /// Perform this check every minute.
        /// </summary>
        public void AutoClear()
        {
            threashold = DateTime.Now.AddMinutes(-10);
            if (lastMin != threashold.Minute)
            {
                lastMin = threashold.Minute;
                //lock (logins)
                //{
                    try
                    {
                        count = logins.Count();
                        logins.RemoveAll(x => DateTime.Compare(x.LastUpdate, threashold) < 0);
                        newCount = logins.Count();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                    }
              //  }
                if (threashold.Minute == 45 || threashold.Minute == 15) logger.Info($"CACHED LOGINS {newCount}. (Before clean: {count})");
            }
        }
    }
}
