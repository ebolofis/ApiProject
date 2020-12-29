using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using System;
using Symposium.Models.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Symposium.Helpers.Interfaces;
using System.Web.Configuration;
using log4net;
using Symposium.Models.Enums;
using System.Globalization;
using Symposium.Models.Models.Scheduler;
using Symposium.Helpers;

namespace Symposium.WebApi.MainLogic.Flows.DeliveryAgent
{
    public class DA_HangFireJobsUpdateStatusFlows : IDA_HangFireJobsUpdateStatusFlows
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IDA_HangFireJobsTasks tasks;
        IStoreIdsPropertiesHelper AllStores;

        public DA_HangFireJobsUpdateStatusFlows(IDA_HangFireJobsTasks tasks, IStoreIdsPropertiesHelper AllStores)
        {
            this.tasks = tasks;
            this.AllStores = AllStores;
        }

        /// <summary>
        /// Update's orders from client to DA Server
        /// </summary>
        public void DA_UpdateOrderStatus()
        {
            try
            {
                DBInfoModel dbInfo = null;
                string storeIDRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_StoreId");
                Guid StoreId = new Guid(storeIDRaw);
                bool isDeliveryAgent = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "IsDeliveryAgent");
                bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");
                string DA_BaseURL = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_BaseURL");
                DA_BaseURL = DA_BaseURL.Trim();
                string DA_Staff_Username = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_Staff_Username");
                DA_Staff_Username = DA_Staff_Username.Trim();
                string DA_Staff_Password = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_Staff_Password");
                DA_Staff_Password = DA_Staff_Password.Trim();

                if (!isDeliveryAgent && isDeliveryStore)
                {
                    dbInfo = AllStores.GetStoreById(StoreId);// stHelp.GetStoreFromStoreId(xml, StoreId);
                    tasks.DA_UpdateOrderStatus(dbInfo, DA_BaseURL, DA_Staff_Username, DA_Staff_Password, ExternalSystemOrderEnum.DeliveryAgent);
                }
                else
                {
                    string Error = "Wrong Web.config Parameters for ";
                    if (isDeliveryAgent)
                        Error += "IsDeliveryAgent must be false \n";
                    if (!isDeliveryStore)
                        Error += "DA_IsClient must be true";
                    logger.Error(Error);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

            //tasks.DA_UpdateOrderStatus();
        }

        public void Start(ParametersSchedulerModel StartParams = null)
        {
            string cultureInfoRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "CultureInfo");
            string cultureInfo = cultureInfoRaw.Trim();
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureInfo);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureInfo);

            DA_UpdateOrderStatus();
        }
    }
}
