using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using log4net;
using Newtonsoft.Json;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.ExternalSystems.Efood;
using Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows.ExternalSystems;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;

namespace Symposium.WebApi.MainLogic.Flows.ExternalSystems
{
    /// <summary>
    /// Flows for External Delivery Systems like e-food
    /// </summary>
    public class ExtDeliverySystemsFlows: IExtDeliverySystemsFlows
    {
        IDA_OrdersFlows daOdersFlows;
        IStoreIdsPropertiesHelper storesHelper;
        IExtDeliverySystemsTasks efoodTasks;
        IDA_AddressesTasks daAddressesTasks;
        IDA_OrdersTasks da_OrdersTasks;
        IDA_EfoodTasks extEfoodTasks;

        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        List<object> extDeliveryPlugins;

       static object lockObj = 0;
        static object lock2Obj = 0;


        public ExtDeliverySystemsFlows(IDA_OrdersFlows daOdersFlows, IStoreIdsPropertiesHelper storesHelper, IExtDeliverySystemsTasks efoodTasks, IDA_AddressesTasks daAddressesTasks, IDA_OrdersTasks da_OrdersTasks, IDA_EfoodTasks extEfoodTasks)
        {
            this.daOdersFlows = daOdersFlows;
            this.storesHelper = storesHelper;
            this.efoodTasks = efoodTasks;
            this.daAddressesTasks = daAddressesTasks;
            this.da_OrdersTasks = da_OrdersTasks;
            this.extEfoodTasks = extEfoodTasks;
        }

        /// <summary>
        /// Get the list of orders from external delivery webapi (like efood) and add them to DA_Orders or to EfoodBucket
        /// </summary>
        public void GetEFoodOrders()
        {

            string cultureInfoRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "CultureInfo");
            string cultureInfo = cultureInfoRaw.Trim();
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureInfo);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureInfo);

          //  OrdersEfood orders = new OrdersEfood();
          //  orders.OrdersList = new List<OrderEfood>();

            //1. get Store (db) from web config
            DBInfoModel DBInfo = storesHelper.GetDAStore();

            //2. instantiate plugins
            extDeliveryPlugins = efoodTasks.InstansiateExternalDeliveryPlugins();

            //3. get the list of orders from external apis.
            List<DA_ExtDeliveryModel> daOrders =  efoodTasks.GetfromDeliveryApis(DBInfo,extDeliveryPlugins);
            lock (lockObj)
            {
                //4. check if orders already exist into  DA_Orders or into EfoodBucket. If so then REMOVE them from list
                efoodTasks.RemoveExistingOrders(DBInfo, daOrders);

                foreach (DA_ExtDeliveryModel order in daOrders)
                {
                    InsertEFoodOrder(DBInfo, order);
                }

                //11. delete old bucket orders marked as deleted
                if (DateTime.Now.Minute <= 1) efoodTasks.DeleteOldOrder(DBInfo);

            }

        }


        /// <summary>
        /// Insert an E-food Order to DA_Orders or to EfoodBucket. 
        ///  Return true for inserting into DA_Orders, 
        ///        false for inserting into EfoodBucket.
        /// </summary>
        /// <param name="DBInfo"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public DA_ExtDeliveryModel InsertEFoodOrder(DBInfoModel DBInfo, DA_ExtDeliveryModel order)
        {
            order.Order.ExtDeliveryErrors = "";

            //5. Match customer and Addresses for each new order
            //5a. Match a customer from DB, or insert new customer 
            efoodTasks.MatchCustomer(DBInfo, order);
            //5b. Match e-food Shipping address based on ExtId1 against the existing Addresses from DB
            if (!efoodTasks.MatchShippingDetailsFromDB(DBInfo, order))
                //5c. Match e-food Shipping address based on Proximity against the existing Addresses from DB
                if (!efoodTasks.MatchShippingDetailsProximity(DBInfo, order))
                {
                    //5d. Add new address for the customer
                    efoodTasks.GeocodeAddress(DBInfo,order.ShippingAddress);
                    order.Order.ShippingAddressId = daAddressesTasks.AddAddress(DBInfo, order.ShippingAddress);
                    order.ShippingAddress.Id = (order.Order.ShippingAddressId??0);
                }

            //6. match billing details (if timologio=true)
            efoodTasks.MatchBillingDetails(DBInfo, order);

            //7. match Store and Polygon
            efoodTasks.SelectPolygonByAddressId(DBInfo, order);

            //8. match products and extras
            efoodTasks.MatchProductsExtras(DBInfo, order);

            //9. Insert order to DA_Orders or to EfoodBucket. Return true if order is saved to DA_Orders OR to EfoodBucket...
            efoodTasks.SaveOrder(DBInfo, order);
                         
            return order;
        }

        /// <summary>
        /// get the list of orders from efood bucket
        /// </summary>
        /// <returns></returns>
        public List<DA_ExtDeliveryModel> getBucket(DBInfoModel DBInfo)
        {
            return efoodTasks.GetOrders(DBInfo);
        }

        /// <summary>
        /// delete an item from bucket
        /// </summary>
        /// <param name="id">efood order id</param>
        public void DeleteBucketItem(DBInfoModel DBInfo, string id)
        {
            efoodTasks.DeleteOrder(DBInfo, id);
        }

        /// <summary>
        /// mark a bucket order as deleted
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="Id">efood Order Id</param>
        /// <returns></returns>
        public void MarkDeleted(DBInfoModel dbInfo, string Id)
        {
            efoodTasks.MarkDeleted(dbInfo, Id);
        }

        /// <summary>
        /// insert an item from bucket
        /// </summary>
        /// <param name="model">EFoodBucketModel</param>
        public void InsertBucketItem(DBInfoModel DBInfo, ExtDeliveryBucketModel model)
        {
            efoodTasks.UpsertBucketOrder(DBInfo, model);
        }


        /// <summary>
        /// check e-food order again and insert order to DA_Orders or EfoodBucket 
        /// This action is used when OrderEfood had been MANUALLY repaired .
        /// Return the same OrderEfood with updated error messages. 
        ///   If error='' then order is inserted into DA_Orders and deleted from EfoodBucket.
        /// </summary>
        /// <param name="DBInfo">db </param>
        /// <param name="order">OrderEfood</param>
        public DA_ExtDeliveryModel ResendDAOrder(DBInfoModel DBInfo, DA_ExtDeliveryModel order)
        {
            lock (lock2Obj)
            {
                //1. check if order is already marked as deleted into bucket 
                ExtDeliveryBucketModel dborder = efoodTasks.GetOrder(DBInfo, order.Order.ExtId1);
                if (dborder != null && dborder.IsDeleted == true) return null;

                //2. check if order already exist into Da_Orders
                long id = da_OrdersTasks.GetOrderByExtId1(DBInfo, order.Order.ExtId1);
                if (id>0) return null;

                if (order != null && order.BillingAddress != null && order.BillingAddress.Id == 0)
                    order.BillingAddress = null;

                //3. check e-food order again and insert order to DA_Orders or EfoodBucket
                DA_ExtDeliveryModel res = InsertEFoodOrder(DBInfo, order);

                //3. delete from bucket if e-food order has inserted to DA_Orders
                if (string.IsNullOrWhiteSpace(res.Order.ExtDeliveryErrors)) MarkDeleted(DBInfo, order.Order.ExtId1);

                return res;
            }
        }

      
    }
}
