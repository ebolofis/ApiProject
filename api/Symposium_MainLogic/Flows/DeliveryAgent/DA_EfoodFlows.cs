using log4net;
using Newtonsoft.Json;
using Symposium.Helpers;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.Scheduler;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows.ExternalSystems;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
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
    public class DA_EfoodFlows : IDA_EfoodFlows
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        IDA_EfoodTasks efoodTasks;
        IExtDeliverySystemsFlows extDeliveryFlows;
        IExtDeliverySystemsTasks extDeliveryTasks;
        IStoreIdsPropertiesHelper storesHelper;
        static object lockObj = 0;

        public DA_EfoodFlows(IDA_EfoodTasks _efoodTasks, IExtDeliverySystemsTasks _extDeliveryTasks, IExtDeliverySystemsFlows _extDeliveryFlows, IStoreIdsPropertiesHelper storesHelper)
        {
            this.efoodTasks = _efoodTasks;
            this.extDeliveryTasks = _extDeliveryTasks;
            this.extDeliveryFlows = _extDeliveryFlows;
            this.storesHelper = storesHelper;
        }

        /// <summary>
        /// Get the list of orders from external delivery webapi (like efood) and add them to DA_Orders or to EfoodBucket
        /// </summary>
        public void PostEfoodNewOrders(DA_EfoodModel Model)
        {

            string cultureInfoRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "CultureInfo");
            string cultureInfo = cultureInfoRaw.Trim();
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureInfo);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureInfo);


            //1. Get Store (db) from web config
            DBInfoModel DBInfo = storesHelper.GetDAStore();

            //2. Get the list of orders from external apis.
            List<DA_ExtDeliveryModel> daOrders = efoodTasks.ConvertEfoodModelToDaDeliveryModel(DBInfo, Model);
            lock (lockObj)
            {
                //3. check if orders already exist into  DA_Orders or into EfoodBucket. If so then REMOVE them from list
                extDeliveryTasks.RemoveExistingOrders(DBInfo, daOrders);

                foreach (DA_ExtDeliveryModel order in daOrders)
                {
                    extDeliveryFlows.InsertEFoodOrder(DBInfo, order);
                }

                //4. delete old bucket orders marked as deleted
                if (DateTime.Now.Minute <= 1) extDeliveryTasks.DeleteOldOrder(DBInfo);
            }
        }
    }
}
