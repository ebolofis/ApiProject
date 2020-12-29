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
    public class DA_HangFireJobsUpdateClientTableFlows : IDA_HangFireJobsUpdateClientTableFlows
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IStoreIdsPropertiesHelper stHlp;
        IDA_UpdateStoresTablesTasks updateClientsTask;

        private object lockvar = 0;

        public DA_HangFireJobsUpdateClientTableFlows(IStoreIdsPropertiesHelper stHlp,
            IDA_UpdateStoresTablesTasks updateClientsTask)
        {
            this.stHlp = stHlp;
            this.updateClientsTask = updateClientsTask;
        }


        /// <summary>
        /// Update's client tables such as product, price list from DA Server to client
        /// </summary>
        public void DA_UpdateClientTables(ParametersSchedulerModel Parameters = null)
        {
            lock (lockvar)
            {
                try
                {
                    DBInfoModel dbInfo = null;
                    string storeIDRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_StoreId");
                    Guid StoreId = new Guid(storeIDRaw);
                    bool isDeliveryAgent = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "IsDeliveryAgent");
                    bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");

                    long? ClientId = null;
                    if (Parameters != null)
                        ClientId = Parameters.StoreIdForUpdate;
                    if (isDeliveryAgent && !isDeliveryStore)
                    {
                        long delAfterFaildRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_DeleteAfterFailed");
                        int delAfterFaild = Convert.ToInt32(delAfterFaildRaw);
                        dbInfo = stHlp.GetStoreById(StoreId);
                        updateClientsTask.UpdateTables(dbInfo, delAfterFaild, ClientId);
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
                    logger.Error("DA_UpdateClientTables : " + ex.ToString());
                }
            }
        }

        public void Start(ParametersSchedulerModel StartParams = null)
        {
            string cultureInfoRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "CultureInfo");
            string cultureInfo = cultureInfoRaw.Trim();
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureInfo);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureInfo);
            DA_UpdateClientTables(StartParams);
        }
    }
}
