using log4net;
using Symposium.Helpers;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Models;
using Symposium.Models.Models.Scheduler;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.DeliveryAgent
{
    public class DA_HangFireJobsMakeCustomerAnonymousFlows : IDA_HangFireJobsMakeCustomerAnonymousFlows
    {
        IDA_CustomerTasks customerTasks;
        IStoreIdsPropertiesHelper allStores;
        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DA_HangFireJobsMakeCustomerAnonymousFlows(IDA_CustomerTasks _customerTasks, IStoreIdsPropertiesHelper _allStores)
        {
            this.customerTasks = _customerTasks;
            this.allStores = _allStores;
        }

        public void Start(ParametersSchedulerModel StartParams = null)
        {
            DA_MakeCustomerAnonymous();
        }

        /// <summary>
        /// Makes DA_Customers anonymous. Only customers with status DA_CustomerAnonymousTypeEnum.WillBeAnonymous will become anonymous
        /// </summary>
        private void DA_MakeCustomerAnonymous()
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
                    dbInfo = allStores.GetStoreById(StoreId);
                    customerTasks.MakeCustomerAnonymous(dbInfo);
                }
                else
                {
                    string Error = "Wrong Web.config Parameters for:\n";
                    if (!isDeliveryAgent)
                        Error += "- IsDeliveryAgent must be true\n";
                    if (isDeliveryStore)
                        Error += "- DA_IsClient must be false\n";
                    logger.Error(Error);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }

    }

}
