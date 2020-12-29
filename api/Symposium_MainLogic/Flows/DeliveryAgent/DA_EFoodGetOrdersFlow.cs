using log4net;
using Symposium.Helpers;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Models;
using Symposium.Models.Models.Scheduler;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows.ExternalSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Symposium.WebApi.MainLogic.Flows.DeliveryAgent
{
    public class DA_EFoodGetOrdersFlow : IDA_EFoodGetOrdersFlow
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IStoreIdsPropertiesHelper AllStores;
        IExtDeliverySystemsFlows efoodFlows;
        IDA_EfoodFlows extDeliveryFlows;

        public DA_EFoodGetOrdersFlow(IStoreIdsPropertiesHelper AllStores, IExtDeliverySystemsFlows efoodFlows, IDA_EfoodFlows extDeliveryFlows)
        {
            this.AllStores = AllStores;
            this.efoodFlows = efoodFlows;
            this.extDeliveryFlows = extDeliveryFlows;
        }

        public void GetEFoodOrders()
        {
            try
            {
                DBInfoModel dbInfo = null;
                string storeIDRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_StoreId");
                bool isDeliveryAgent = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "IsDeliveryAgent");
                bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");
                Guid StoreId = new Guid(storeIDRaw);

                if (isDeliveryAgent && !isDeliveryStore)
                {
                    dbInfo = AllStores.GetStoreById(StoreId);
                    efoodFlows.GetEFoodOrders();
                }
                else
                {
                    string Error = "Wrong Configuration for ";
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
            GetEFoodOrders();
        }
    }
}
