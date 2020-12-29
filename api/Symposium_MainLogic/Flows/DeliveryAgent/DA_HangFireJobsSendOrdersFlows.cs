using log4net;
using Symposium.Helpers;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Models;
using Symposium.Models.Models.Scheduler;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Symposium.WebApi.MainLogic.Flows.DeliveryAgent
{
    public class DA_HangFireJobsSendOrdersFlows : IDA_HangFireJobsSendOrdersFlows
    {

        static object lockvar = 0;


        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IDA_HangFireJobsTasks tasks;
        IStoreIdsPropertiesHelper AllStores;

        public DA_HangFireJobsSendOrdersFlows(IDA_HangFireJobsTasks tasks, IStoreIdsPropertiesHelper AllStores)
        {
            this.tasks = tasks;
            this.AllStores = AllStores;
        }

        /// <summary>
        /// Send's orders from DA Server to Client
        /// </summary>
        public void DA_ServerOrder()
        {
            
            lock (lockvar)
            {
                DBInfoModel dbInfo = null;
                string storeIDRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_StoreId");
                Guid StoreId = new Guid(storeIDRaw);
                bool isDeliveryAgent = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "IsDeliveryAgent");
                bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");
                long delMinutesRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_DeleteOnHoldMinutes");
                int delMinutes = Convert.ToInt32(delMinutesRaw);



                if (isDeliveryAgent && !isDeliveryStore)
                {
                    dbInfo = AllStores.GetStoreById(StoreId);
                    tasks.DA_ServerOrder(dbInfo, delMinutes);
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
        }

        /// <summary>
        /// void For Scheduler to execute routines
        /// </summary>
        public void Start(ParametersSchedulerModel StartParams = null)
        {
            string cultureInfoRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "CultureInfo");
            string cultureInfo = cultureInfoRaw.Trim();
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureInfo);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureInfo);

            DA_ServerOrder();
        }
    }
}
