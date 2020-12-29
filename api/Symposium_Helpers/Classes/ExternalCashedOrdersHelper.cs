using log4net;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Classes
{
    /// <summary>
    /// Class that cashes the list of inserted to DB orders from External Delivery Systems (like e-food)
    /// </summary>
   public class ExternalCashedOrdersHelper: IExternalCashedOrdersHelper
    {
        private ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        DateTime threashold;
        int lastMin = 0;
        int lastHour = 0;
        int count;

        public ExternalCashedOrdersHelper()
        {
          // int count= Convert.ToInt32(MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "CachList"));
            Orders = new List<DA_OrderIdsModel>(180);
        }

        /// <summary>
        /// the list of inserted to DB orders from External Delivery Systems (like e-food)
        /// </summary>
        public List<DA_OrderIdsModel> Orders { get; set; }

        /// <summary>
        /// Add an order to list
        /// </summary>
        /// <param name="Order"></param>
        public void AddOrder(DA_OrderIdsModel Order)
        {
            try
            {
                Order.LastSearch = DateTime.Now;
                Orders.Add(Order);
                AutoClear();
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }

        /// <summary>
        /// return true if an order exists to the cashed order list
        /// </summary>
        /// <param name="Order"></param>
        /// <returns></returns>
        public bool OrderExists(DA_OrderIdsModel Order)
        {
            if (string.IsNullOrWhiteSpace(Order.ExtId1)) return false;
            try
            {
                int pos = Orders.FindIndex(x => x.ExtId1 == Order.ExtId1 || x.ExtId2 == Order.ExtId1);
                if (pos >= 0)
                {
                    Orders[pos].LastSearch = DateTime.Now;
                    return true;
                }
                else
                    return false;
            }catch(Exception ex)
            {
                logger.Error(ex.ToString());
                return false;
            }
            
        }

        /// <summary>
        /// Remove orders from list that have not be searched for the last 20 min. 
        /// Perform this check every half hour.
        /// </summary>
        private void AutoClear()
        {
            threashold = DateTime.Now.AddMinutes(-20);
            if (threashold.Minute % 5==0 && lastMin!= threashold.Minute)//(threashold.Minute == 41 || threashold.Minute == 20)
            {
                lastMin = threashold.Minute;
                lastHour = threashold.Hour;
                count = Orders.Count();
                Orders.RemoveAll(x => DateTime.Compare(x.LastSearch, threashold) < 0);
                logger.Info($"CACHED ORDERS {Orders.Count()}. (Before clean: {count})");
            }
        }
    }
}
