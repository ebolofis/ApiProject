using log4net;
using Symposium.Helpers.Interfaces;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Globalization;
using System.Threading;
using Symposium.Models.Models.Scheduler;
using Symposium.Helpers;

namespace Symposium.WebApi.MainLogic.Flows.DeliveryAgent
{
    public class DA_HangFireJobsCheckIsDelayOnStoresFlows : IDA_HangFireJobsCheckIsDelayOnStoresFlows
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IStoreIdsPropertiesHelper AllStores;
        IWebApiClientHelper webHlp;

        public DA_HangFireJobsCheckIsDelayOnStoresFlows(IStoreIdsPropertiesHelper AllStores, IWebApiClientHelper webHlp)
        {
            this.AllStores = AllStores;
            this.webHlp = webHlp;
        }

        /// <summary>
        /// Get's all IsDelayed Orders and send them to kitchen if are available to send
        /// </summary>
        public void StoreDelayOrders_SetToKitchen()
        {
            try
            {
                DBInfoModel dbInfo = null;
                string storeIDRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_StoreId");
                Guid StoreId = new Guid(storeIDRaw);
                bool isDeliveryAgent = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "IsDeliveryAgent");
                bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");
                long DA_SendToKitchenTimeRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_SendToKitchenTime");
                int DA_SendToKitchenTime = Convert.ToInt32(DA_SendToKitchenTimeRaw);


                //this property is for base url etc http://192.168.5.8:5080.
                //get's value after first call no matter of the call.
                //If it is empty then return and wait until filled from first call.

                if (string.IsNullOrEmpty(AllStores.BaseUrl))
                    return;

                if (!isDeliveryAgent && isDeliveryStore)
                {
                    dbInfo = AllStores.GetStoreById(StoreId);

                    int returnCode;
                    string ErrorMess;

                    string url = AllStores.BaseUrl + "api/v3/Orders/SendDelayedOrdersToKitchen";
                    /*To pass parameter's using PostRequest must be as object. So create dummy object ResultsAfterDA_OrderActionsModel and save 
                        DA_SendToKitchenTime to property DA_Order_Id
                     */
                    ResultsAfterDA_OrderActionsModel sendModel = new ResultsAfterDA_OrderActionsModel();
                    sendModel.DA_Order_Id = DA_SendToKitchenTime;
                    string res = webHlp.PostRequest(sendModel, url, dbInfo.Username + ":" + dbInfo.Password, null, out returnCode, out ErrorMess, "application/json", "Basic");
                    if (returnCode != 200)
                        logger.Error("Error on StoreDelayOrders_SetToKitchen. Error Code : " + returnCode.ToString() + ", Message : " + ErrorMess);
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
        }

        public void Start(ParametersSchedulerModel StartParams = null)
        {
            string cultureInfoRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "CultureInfo");
            string cultureInfo = cultureInfoRaw.Trim();
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureInfo);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureInfo);

            StoreDelayOrders_SetToKitchen();
        }
    }
}
