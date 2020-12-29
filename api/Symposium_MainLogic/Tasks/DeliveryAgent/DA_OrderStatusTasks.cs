using log4net;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Plugins;
using Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks.DeliveryAgent
{
    public class DA_OrderStatusTasks : IDA_OrderStatusTasks
    {
        IDA_OrderStatusDT dt;
        IUsersToDatabasesXML users;
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DA_OrderStatusTasks(IDA_OrderStatusDT dt, IUsersToDatabasesXML users)
        {
            this.dt = dt;
            this.users = users;
        }

        /// <summary>
        /// Insert's a New Model To DB
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewModel(DBInfoModel dbInfo, DA_OrderStatusModel item)
        {
            long daOrderStatusId = 0;
            daOrderStatusId = dt.AddNewModel(dbInfo, item);

            //################ Hook for DA_StatusChange Plugins #####################//
            //######################## Create List of Plugins #######################//
            //#######################################################################//
            PluginHelper pluginHelper = new PluginHelper();
            List<object> ImplementedClassInstance = pluginHelper.InstanciatePluginList(typeof(DA_StatusChange));

            //######################## Plugin for Send Orders To External Systemt ###################//
            //#######################################################################################//
            object[] InvokedMethodParameters = { dbInfo, users, logger, item };
            foreach (object pluginClassInstance in ImplementedClassInstance)
            {
                Task.Run(() => pluginHelper.InvokePluginMethod<bool>(pluginClassInstance, "InvokeDaStatusChange", new[] { typeof(DBInfoModel), typeof(IUsersToDatabasesXML), typeof(ILog), typeof(DA_OrderStatusModel) }, InvokedMethodParameters));
            }

            return daOrderStatusId;
        }

        /// <summary>
        /// Update statuses for DA_Order
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public void UpdateDa_OrderStatus(DBInfoModel dbInfo, DA_OrderStatusModel item)
        {
            dt.UpdateDa_OrderStatus(dbInfo, item);
            return;
        }

        /// <summary>
        /// Get's a List of orders with max status onhold (based on statusdate) and hour different bwtween now and statusdate bigger than 2
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public List<long> GetOnHoldOrdersForDelete(DBInfoModel Store, int delMinutes)
        {
            return dt.GetOnHoldOrdersForDelete(Store, delMinutes);
        }
    }
}
