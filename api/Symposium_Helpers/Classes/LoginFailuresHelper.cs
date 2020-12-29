using log4net;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Classes
{

    /// <summary>
    /// Keep track of login failures to prevent hackers attacks
    /// </summary>
   public class LoginFailuresHelper
    {
        /// <summary>
        /// List of failured logins
        /// </summary>
        private List<DALoginModelExt> failures;

      //  private readonly object obj = new object();

        private int thresholdSec=-30;

        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public LoginFailuresHelper()
        {
            failures = new List<DALoginModelExt>();
        }

        /// <summary>
        /// Add a login failure to the list
        /// </summary>
        /// <param name="login"></param>
       public void AddFailure(DALoginModel login)
        {
            lock (failures)
            {
                DALoginModelExt model = new DALoginModelExt() { Email = login.Email, LoginTime = DateTime.Now,Password=login.Password };
                failures.Add(model);
            }
            Task.Delay(2400).Wait();
        }

        /// <summary>
        /// throw exception if the failure logins for the specific email overcome the threshold within 40 sec
        /// </summary>
        /// <param name="login"></param>
        /// <param name="threashold"></param>
        /// <returns></returns>
        public void ManyFailures(DALoginModel login, int threshold = 5)
        {
            deleteOlds();
            int c;
            lock (failures)
            {
                c = failures.Count(x => x.Email.ToLower() == login.Email.ToLower());
            }
            if (c > threshold)
            {
                Task.Delay(7000+ threshold*1000).Wait();
                throw new BusinessException($"The maximum number of failures has exceeded for user {login.Email ?? "<NULL>"}. Try again later.");
            }
        }


        /// <summary>
        /// throw exception if the login has already failured
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public void HistoricFailure(DALoginModel login)
        {
            int c;
            lock (failures)
            {
                c = failures.Count(x => x.Email.ToLower() == login.Email.ToLower() && x.Password == login.Password);
            }
            if (c > 1)
            {
                deleteOlds();
                logger.Warn("Historic Login Failed for Customer : " + (login.Email ?? "<NULL>"));
                Task.Delay(6000).Wait();
                throw new BusinessException(Symposium.Resources.Errors.USERLOGINFAILED);
            }
               
        }


        private void deleteOlds()
        {
            
            lock (failures)
            {
                var olds = failures.Where(x => x.LoginTime < DateTime.Now.AddSeconds(thresholdSec));
                failures = failures.Except(olds).ToList();
            }
           
        }
    }
}
