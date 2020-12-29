using log4net;
using Symposium.Helpers;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Models;
using Symposium.Models.Models.Scheduler;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Symposium.WebApi.MainLogic.Flows.DeliveryAgent
{
    public class DA_LoyaltyExecutionsFlow : IDA_LoyaltyExecutionsFlow
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        IStoreIdsPropertiesHelper AllStores;
        IDA_LoyaltyFlows loyaltyFlow;

        public DA_LoyaltyExecutionsFlow(IStoreIdsPropertiesHelper AllStores, IDA_LoyaltyFlows loyaltyFlow)
        {
            this.AllStores = AllStores;
            this.loyaltyFlow = loyaltyFlow;
        }

        /// <summary>
        /// Διαγράφει τους πόντους βάση παλαιότητας
        /// </summary>
        private void DA_LoyaltyDelete()
        {
            try
            {
                DBInfoModel dbInfo = null;
                string storeIDRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_StoreId");
                Guid StoreId = new Guid(storeIDRaw);
                bool isDeliveryAgent = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "IsDeliveryAgent");
                bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");


                if (isDeliveryAgent && !isDeliveryStore)
                {
                    dbInfo = AllStores.GetStoreById(StoreId);
                    loyaltyFlow.DeletePoints(dbInfo);
                }
                else
                {
                    string Error = "Wrong Web.config Parameters for ";
                    if (!isDeliveryAgent)
                        Error += "IsDeliveryAgent must be true \n";
                    if (isDeliveryStore)
                        Error += "DA_IsClient must be false";
                    logger.Error(Error);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }

        public void Start(ParametersSchedulerModel StartParams = null)
        {
            DA_LoyaltyDelete();
        }
    }
}
