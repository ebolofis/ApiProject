using log4net;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.ExternalSystems.Efood;
using Symposium.Plugins;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DT.ExternalSystems;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using Symposium.WebApi.MainLogic.Tasks.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Symposium.WebApi.MainLogic.Tasks.ExternalSystems
{
    /// <summary>
    /// Tasks for External Delivery Systems like e-food
    /// </summary>
    public class ExtDeliverySystemsTasks : IExtDeliverySystemsTasks
    {

        private ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
  
        IDA_OrdersDT daOrdersDT;
        IDA_CustomerDT daCustomerDT;
        IDA_AddressesDT daAddressDT;
        IEfoodDT efoodDT;
        IWebApiClientHelper webApiClientHelper;
        IDA_GeoPolygonsDT da_GeoPolygonsDT;
        IDA_StoresDT storesDT;
        IProductDT productDT;
        IVatTasks vatTasks;
        IPricelistDT pricelistDT;
        IFilesHelper filesHelper;
        IIngredientsDT ingredientsDT;
        IDA_AddressesTasks daAddressTasks;
        IExternalCashedOrdersHelper externalOrdersHelper;
        IDA_OrdersFlows daOdersFlows;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Helpers.Hubs.DA_Hub>();
        PluginHelper pluginHelper;
        /// <summary>
        ///  get list of vats
        /// </summary>
        List<VatModel> vats;
        ProductExtModel notMachedProd;



        public ExtDeliverySystemsTasks(
            //IEFoodHelper efoodHelper,
            IWebApiClientHelper webApiClientHelper,
            IDA_OrdersDT daOrdersDT,
            IEfoodDT efoodDT,
            IDA_CustomerDT daCustomerDT,
            IDA_AddressesDT daAddressDT,
            IDA_GeoPolygonsDT da_GeoPolygonsDT,
            IDA_StoresDT storesDT,
            IProductDT productDT,
            IVatTasks vatTasks,
            IPricelistDT pricelistDT,
            IFilesHelper filesHelper,
            IIngredientsDT ingredientsDT,
            IDA_AddressesTasks daAddressTasks,
             IDA_OrdersFlows daOdersFlows,
           IExternalCashedOrdersHelper externalOrdersHelper
            )
        {
           // this.efoodHelper = efoodHelper;
            this.webApiClientHelper = webApiClientHelper;
            this.daOrdersDT = daOrdersDT;
            this.efoodDT = efoodDT;
            this.daCustomerDT = daCustomerDT;
            this.daAddressDT = daAddressDT;
            this.da_GeoPolygonsDT = da_GeoPolygonsDT;
            this.storesDT = storesDT;
            this.productDT = productDT;
            this.vatTasks = vatTasks;
            this.pricelistDT = pricelistDT;
            this.filesHelper = filesHelper;
            this.ingredientsDT = ingredientsDT;
            this.daAddressTasks = daAddressTasks;
            this.externalOrdersHelper = externalOrdersHelper;
            this.daOdersFlows = daOdersFlows;

           
        }

        /// <summary>
        /// Return the list of instances of plugins 'ExternalDelivery'
        /// </summary>
        /// <returns></returns>
        public List<object> InstansiateExternalDeliveryPlugins()
        {
            pluginHelper = new PluginHelper();
            lock (pluginHelper)
            {
                return pluginHelper.InstanciatePluginList(typeof(Symposium.Plugins.ExternalDelivery));
            }
        }

        /// <summary>
        /// Confirm order back to External Delivery system
        /// </summary>
        /// <param name="plugins">plugins instances</param>
        /// <param name="order">order to confirm</param>
        public void ConfirmOrder(List<object> plugins, DA_ExtDeliveryModel order)
        {
            try
            {
                if (plugins == null)
                    plugins= InstansiateExternalDeliveryPlugins();
                if(plugins == null)
                {
                    logger.Error($"ExternalDeliveryPlugins Not Found . Unable to ConfirmOrder {order.Order.Id}");
                    return;
                }
                foreach (var plugin in plugins)
                {
                    bool res = pluginHelper.InvokePluginMethod<bool>(plugin, "ConfirmOrder", new Type[] { typeof(DA_ExtDeliveryModel), typeof(ILog) }, new object[] { order, logger });
                }
            }
            catch(Exception ex)
            {
                logger.Error($"Error on Confirming Order {order.Order.Id} : "+ex.ToString());
            }
          
        }

        /// <summary>
        ///  Get the list of orders from external delivery web-apis by calling plugins 'ExternalDelivery'.
        /// </summary>
        ///  <param name="extDeliveryPlugins">list of instances of plugins 'ExternalDelivery'</param>
        /// <returns>the  list of orders </returns>
        public List<DA_ExtDeliveryModel> GetfromDeliveryApis(DBInfoModel dbInfo, List<object> extDeliveryPlugins)
        {
            List<DA_ExtDeliveryModel> daOrders = new List<DA_ExtDeliveryModel>();
            int c = 0;
            object x = 0;
            foreach (var plugin in extDeliveryPlugins)
            {
                c++;
                try
                {
                    lock (x){
                       
                        daOrders.AddRange(
                              pluginHelper.InvokePluginMethod<List<DA_ExtDeliveryModel>>(
                                  plugin, 
                                  "GetOrders", 
                                  new Type[] { typeof(DBInfoModel), typeof(ILog), typeof(Dictionary<string, Dictionary<string, dynamic>>) }, 
                                  new object[] { dbInfo, logger, MainConfigurationHelper.MainConfigurationDictionary }
                                  )
                            );
                       // System.Threading.Thread.Sleep(150);
                    }
                   
                  //if(c<extDeliveryPlugins.Count) Task.Run(() => Task.Delay(120)).Wait();
                }
                catch (Exception) {}//error has already logged. So on error continue with the next external delivery system...
            }

            return daOrders;

            ////1. Get orders asynchronously
            //List<Task<List<DA_ExtDeliveryModel>>> tasks = new List<Task<List<DA_ExtDeliveryModel>>>();
            //lock (pluginHelper)
            //{
            //    foreach (var plugin in extDeliveryPlugins)
            //    {
            //        tasks.Add(pluginHelper.InvokePluginMethodAsync<List<DA_ExtDeliveryModel>>(plugin, "GetOrders", new Type[] { typeof(ILog),typeof(Dictionary<string, Dictionary<string, dynamic>>) }, new object[] { logger, MainConfigurationHelper.MainConfigurationDictionary }));
            //    }
            //}
            //List<DA_ExtDeliveryModel>[] results = null;
            //try
            //{
            //    //2. wait for all tasks to complete
            //    results = await Task.WhenAll(tasks);
            //}
            //catch (Exception ex)
            //{
            //    //show the main error
            //    logger.Error("MAIN ERROR: " + ex.ToString() + Environment.NewLine);
            //    //show exception from each task with error
            //    foreach (var task in tasks.Where(t => t.Exception != null))
            //    {
            //        string str = task.Exception.ToString();
            //        logger.Error("ERROR In TASK: " + str);
            //    }
            //    //get results from tasks WITHOUT error
            //    results = (from t in tasks where t.Exception == null select t.Result).ToArray<List<DA_ExtDeliveryModel>>();
            //}

            //List<DA_ExtDeliveryModel> daOrders = new List<DA_ExtDeliveryModel>();
            //foreach (List<DA_ExtDeliveryModel> r in results)
            //{
            //    daOrders.AddRange(r);
            //}

            //return daOrders;

        }

     
        /// <summary>
        /// From the list of e-food orders remove them that already exist into cashed list, DA_Orders or EFoodBucket tables
        /// 
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="orders">OrdersEfood</param>
        public void RemoveExistingOrders(DBInfoModel dbInfo, List<DA_ExtDeliveryModel> orders)
        {
           int c = orders.Count();
            foreach (DA_ExtDeliveryModel order in orders)
            {
                //1. find order into cashed list of inserted orders (see externalOrdersHelper)
                if (externalOrdersHelper.OrderExists(order.Order))
                    order.Order.Flag = true;

                //2. find order to DA_Orders
                if (!order.Order.Flag)
                {
                    long id = daOrdersDT.GetOrderByExtId1(dbInfo, order.Order.ExtId1);  //If ExtId1 not found return 0;
                    if (id != 0)
                    {
                        externalOrdersHelper.AddOrder(order.Order); //add order into cashed list of inserted orders
                        order.Order.Flag = true;
                    }
                }

                //3. find order to Backet
                if (!order.Order.Flag)
                {
                    ExtDeliveryBucketModel m = efoodDT.GetOrder(dbInfo, order.Order.ExtId1);
                    if (m != null)
                    {
                        externalOrdersHelper.AddOrder(order.Order); //add order into cashed list of inserted orders
                        order.Order.Flag = true;
                    }
                }
               
            }

            //4. remove existing orders
            orders.RemoveAll(x => x.Order.Flag);

            if (logger.IsDebugEnabled)
            {
                string ids = String.Join<string>(", ", orders.Select(x => (x.Order.ExtId1 ?? "<null>")));
                logger.Debug("> " + c + " orders got from External Delivery Apis. " + orders.Count.ToString() + " of them are new orders : " + ids);
            }
            
        }

        /// <summary>
        /// Match a customer from DB, or insert new customer
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool MatchCustomer(DBInfoModel dbInfo, DA_ExtDeliveryModel order)
        {
            //1. try matching customer against DB Customer's ExtId1
            DACustomerModel cust = daCustomerDT.GetCustomer(dbInfo, order.Customer.ExtId1, 1);

            //2. try matching customer against DB Customer's ExtId2
            if (cust == null)
            {
                cust = daCustomerDT.GetCustomer(dbInfo, order.Customer.ExtId1, 2);
            }
           
            //3. try matching customer against DB  Customer's lastName + firstName + Phones
            if (cust == null)
            {
                cust = daCustomerDT.GetCustomerUnique(dbInfo, order.Customer.LastName, order.Customer.FirstName, order.Customer.Phone1 ?? order.Customer.Mobile);
                if (cust != null && !string.IsNullOrWhiteSpace(cust.PhoneComp) && string.IsNullOrWhiteSpace(cust.VatNo)) cust.PhoneComp = null; //correct bugs from the past...
            }
            //4. try matching customer against DB Customer's ExtId3
            if (cust == null)
            {
                cust = daCustomerDT.GetCustomer(dbInfo, order.Customer.ExtId1, 3);
            }
            //5. try matching customer against DB Customer's ExtId4
            if (cust == null)
            {
                cust = daCustomerDT.GetCustomer(dbInfo, order.Customer.ExtId1, 4);
            }

            //6. customer found into DB
            if (cust != null)
                doCustomerMatched(dbInfo, cust, order.Customer);
            else
            //7. if customer did not find into DB, insert him to DB
            {
                try
                {
                    order.Customer.Id = daCustomerDT.AddCustomer(dbInfo, order.Customer);
                }
                catch (Exception ex)
                {
                    constructError(order.Order, "New customer not inserted to DB:  " + ex.Message);
                    logger.Error(order.Order.Origin + ": New customer not inserted to DB:  " + ex.ToString());
                    return false;
                }
            }
            //8. inform OrderModel and AddressModels with Customer Id 
            order.Order.CustomerId = order.Customer.Id;
            order.ShippingAddress.OwnerId= order.Customer.Id;
            if(order.BillingAddress!=null) order.BillingAddress.OwnerId = order.Customer.Id;
            return true;
        }

        /// <summary>
        /// Work to do when customer is matched against DB
        /// </summary>
        /// <param name="dbInfo">dbInfo</param>
        /// <param name="custDB">customer model from DB</param>
        /// <param name="custExt">customer model from external delivery</param>
        private void doCustomerMatched(DBInfoModel dbInfo, DACustomerModel custDB, DACustomerModel custExt)
        {
            custExt.Id = custDB.Id;
            bool changes = false;
            
            if (string.IsNullOrWhiteSpace(custDB.ExtId1))
            {
                custDB.ExtId1 = custExt.ExtId1.Trim();
                changes = true;
            }
            if (!string.IsNullOrWhiteSpace(custDB.ExtId1) && (string.IsNullOrWhiteSpace(custDB.ExtId2) || (custDB.ExtId2??"").StartsWith(",")) && custDB.ExtId1 != custExt.ExtId1)
            {
                changes = true;
                custDB.ExtId2 = custExt.ExtId1;
            }
            if (!string.IsNullOrWhiteSpace(custDB.ExtId1) && 
                !string.IsNullOrWhiteSpace(custDB.ExtId2) && 
                custDB.ExtId1 != custExt.ExtId1 && 
                custDB.ExtId2 != custExt.ExtId1 )
            {
                changes = true;
                custDB.ExtId3 = custExt.ExtId1;
            }
            if (!string.IsNullOrWhiteSpace(custDB.ExtId1) &&
               !string.IsNullOrWhiteSpace(custDB.ExtId2) &&
               !string.IsNullOrWhiteSpace(custDB.ExtId3) &&
               custDB.ExtId1 != custExt.ExtId1 &&
               custDB.ExtId2 != custExt.ExtId1 &&
               custDB.ExtId3 != custExt.ExtId1)
            {
                changes = true;
                custDB.ExtId4 = custExt.ExtId1;
            }
            if (changes)
                daCustomerDT.UpdateCustomer(dbInfo, custDB);
        }

        /// <summary>
        /// Match e-food Shipping address based on ExtId1 against the existing Addresses from DB
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="order">e-food order</param>
        public bool MatchShippingDetailsFromDB(DBInfoModel dbInfo, DA_ExtDeliveryModel order)
        {
            //1. order should contain Address
            if (order.ShippingAddress == null )
            {
                constructError(order.Order, $" Order {order.Order.ExtId1} from {order.Order.ExtDeliveryOrigin} does not contain Address !!!");
                return false;
            }

            //2. try matching addresses based on ExtId1
            List<DA_AddressModel> addrsDB = daAddressDT.getCustomerAddresses(dbInfo, order.Customer.Id);
            foreach (DA_AddressModel addrDB in addrsDB)
            {
                    //3. Address Matched 
                    if (addrDB.ExtId1 == order.ShippingAddress.ExtId1 || addrDB.ExtId2 == order.ShippingAddress.ExtId1)
                    {
                        DoMatchedAddresses(order, order.ShippingAddress, addrDB, dbInfo);
                        return true;
                    } 
            }
            return false;
        }

        /// <summary>
        /// Match e-food Shipping address based on Proximity against the existing Addresses from DB
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="order">DA_ExtDeliveryModel</param>
        /// <returns></returns>
        public bool MatchShippingDetailsProximity(DBInfoModel dbInfo, DA_ExtDeliveryModel order)
        {
            List<DA_AddressModel> addrsDB = daAddressDT.getCustomerAddresses(dbInfo, order.Customer.Id);
            
                DA_AddressModel addrDB = daAddressDT.proximityCustomerAddress(dbInfo, order.Customer.Id, order.ShippingAddress.Latitude ?? 0, order.ShippingAddress.Longtitude ?? 0);
                if (addrDB != null)
                {
                    DoMatchedAddresses(order, order.ShippingAddress, addrDB, dbInfo);
                    return true;
                }
            return false;
        }

        /// <summary>
        /// Find Longitude and Latitude for a DA_AddressModel only if Longitude & Latitude do not exist
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="addrExt">DA_AddressModel</param>
        public void GeocodeAddress(DBInfoModel dbInfo, DA_AddressModel addrExt)
        {
            if (!AddressIsGeolocated(addrExt))
            {
                DA_AddressModel addrTmp = daAddressTasks.GeoLocationMaps(dbInfo, addrExt);
                if (addrTmp != null)
                {
                    addrExt.Longtitude = addrTmp.Longtitude;
                    addrExt.Latitude = addrTmp.Latitude;
                    addrExt.AddressProximity = addrTmp.AddressProximity;
                }
            }
        }

        /// <summary>
        /// Work to do when an address from the External Delivery System is matched with an existing address from DB
        /// </summary>
        /// <param name="order">DA_ExtDeliveryModel</param>
        /// <param name="addrExt">address from the External Delivery System</param>
        /// <param name="addrDB">existing address from DB</param>
        private void DoMatchedAddresses(DA_ExtDeliveryModel order, DA_AddressModel addrExt, DA_AddressModel addrDB, DBInfoModel dbInfo)
        {
            order.Order.ShippingAddressId = addrDB.Id;
            addrExt.Id = addrDB.Id;
            if (string.IsNullOrWhiteSpace(addrDB.ExtId1) && !string.IsNullOrWhiteSpace(addrExt.ExtId1))
                addrDB.ExtId1 = addrExt.ExtId1;
            if (!string.IsNullOrWhiteSpace(addrDB.ExtId1) && 
                !string.IsNullOrWhiteSpace(addrExt.ExtId1) && 
                string.IsNullOrWhiteSpace(addrDB.ExtId2) && 
                addrDB.ExtId1!= addrExt.ExtId1 &&
                addrDB.ExtId1.Substring(addrDB.ExtId1.Length - 2)!= addrDB.ExtId1.Substring(addrDB.ExtId1.Length - 2))
                  addrDB.ExtId2 = addrExt.ExtId1;

            //3a. fix Geolocation
            if (addrDB.Zipcode == addrExt.Zipcode)
            {
                if (!AddressIsGeolocated(addrExt) && AddressIsGeolocated(addrDB))
                {
                    addrExt.Longtitude = addrDB.Longtitude;
                    addrExt.Latitude = addrDB.Latitude;
                }
                if (AddressIsGeolocated(addrExt) && !AddressIsGeolocated(addrDB))
                {
                    addrDB.Longtitude = addrExt.Longtitude;
                    addrDB.Latitude = addrExt.Latitude;
                    addrDB.AddressProximity = AddressProximityEnum.ROOFTOP; // consider that external delivery system has accurate geolocation!!!
                    daAddressDT.UpdateAddress(dbInfo, addrDB);              // UPDATE the address to DB
                }
                if (!AddressIsGeolocated(addrExt) && !AddressIsGeolocated(addrDB))
                {

                    DA_AddressModel addrTmp = daAddressTasks.GeoLocationMaps(dbInfo, addrExt);
                    if (addrTmp != null)
                    {
                        addrDB.Longtitude = addrTmp.Longtitude;
                        addrDB.Latitude = addrTmp.Latitude;
                        addrDB.AddressProximity = addrTmp.AddressProximity;
                        addrExt.Longtitude = addrTmp.Longtitude;
                        addrExt.Latitude = addrTmp.Latitude;
                        addrExt.AddressProximity = addrTmp.AddressProximity;
                        daAddressDT.UpdateAddress(dbInfo, addrDB);  // UPDATE the address to DB
                    }
                }
            }
            else //OOps :  addrDB.Zipcode <> addrExt.Zipcode 
            {
                if (!AddressIsGeolocated(addrExt))
                {
                    GeocodeAddress(dbInfo,addrExt);
                }
                else
                    addrExt.AddressProximity = AddressProximityEnum.ROOFTOP; //consider that external delivery system has accurate geolocation!!!

                //Save Address's newer data from External Delivery to DB 
                addrDB.Longtitude = addrExt.Longtitude;
                addrDB.Latitude = addrExt.Latitude;
                addrDB.AddressProximity = addrExt.AddressProximity;

                if(!string.IsNullOrWhiteSpace(addrExt.AddressNo))
                    addrDB.AddressNo = addrExt.AddressNo;
                if ( !string.IsNullOrWhiteSpace(addrExt.AddressStreet))
                    addrDB.AddressStreet = addrExt.AddressStreet;
                if (!string.IsNullOrWhiteSpace(addrExt.Area))
                    addrDB.Area = addrExt.Area;
                if (!string.IsNullOrWhiteSpace(addrExt.Bell))
                    addrDB.Bell = addrExt.Bell;
                if (!string.IsNullOrWhiteSpace(addrExt.City))
                    addrDB.City = addrExt.City;
                if (!string.IsNullOrWhiteSpace(addrExt.Floor))
                    addrDB.Floor = addrExt.Floor;
                if (!string.IsNullOrWhiteSpace(addrExt.Notes))
                    addrDB.Notes = addrExt.Notes;
                if (!string.IsNullOrWhiteSpace(addrExt.VerticalStreet))
                    addrDB.VerticalStreet = addrExt.VerticalStreet;
                if (!string.IsNullOrWhiteSpace(addrExt.Zipcode))
                    addrDB.Zipcode = addrExt.Zipcode;

                daAddressDT.UpdateAddress(dbInfo, addrDB);    // UPDATE the address to DB
            }
        }

        /// <summary>
        /// Return true id an address has values for Longtitude and Latitude
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        private bool AddressIsGeolocated(DA_AddressModel addr)
        {
            return (addr.Longtitude != 0 || addr.Latitude != 0 || addr.Longtitude != null || addr.Latitude != null);
        }


        /// <summary>
        /// Match e-food order with  timologio details and billing address
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="order">e-food order</param>
        public void MatchBillingDetails(DBInfoModel dbInfo, DA_ExtDeliveryModel  order)
        {
            if (order.BillingAddress == null) return;

            //1. check timologio integrity data
            if (string.IsNullOrWhiteSpace(order.Customer.JobName) || order.Customer.JobName.Length < 2) constructError(order.Order, "Company is required for invoice (τιμολόγιο)");
            if (order.BillingAddress==null || order.BillingAddress.AddressStreet.Length < 7) constructError(order.Order, "Company address is required for invoice (τιμολόγιο)");
            if (string.IsNullOrWhiteSpace(order.Customer.Proffesion) || order.Customer.Proffesion.Length < 2) constructError(order.Order, "Profession is required for invoice (τιμολόγιο)");
            if (string.IsNullOrWhiteSpace(order.Customer.VatNo) || order.Customer.VatNo.Length < 9) constructError(order.Order, "Tax number (ΑΦΜ) is required for invoice (τιμολόγιο)");
            if (string.IsNullOrWhiteSpace(order.Customer.Doy) || order.Customer.Doy.Length < 2) constructError(order.Order, "ΔΟΥ is required for invoice (τιμολόγιο)");
            if (order.BillingAddress.OwnerId == 0) constructError(order.Order, "A Known (matched) Customer is required for invoice (τιμολόγιο)");
            

            //2. match billing address
            if (order.Order.ExtDeliveryErrors != "") return;
           
            //2a. get customer and existing billing address from DB
           
            List<DA_AddressModel> addrs = daAddressDT.getCustomerAddresses(dbInfo, order.Customer.Id);
            DA_AddressModel billingAddr = addrs.FirstOrDefault(x => x.AddressType == 1);

            //3. if there is no billing address from DB then Create one...
            if (billingAddr == null)
            {
                //3c. add new Address and Update customer
                DACustomerModel cust = daCustomerDT.GetCustomer(dbInfo, order.Customer.Id);
                order.BillingAddress.Id = daAddressDT.AddAddress(dbInfo, order.BillingAddress);
                order.Customer.BillingAddressesId = order.BillingAddress.Id;
                cust.BillingAddressesId = billingAddr.Id;
                if (string.IsNullOrWhiteSpace(cust.PhoneComp)) cust.PhoneComp = order.Customer.Phone1?? order.Customer.Mobile;
                daCustomerDT.UpdateCustomer(dbInfo, cust);
            }
            else //4. There IS billing address from DB then Update Billing Details with those from e-food
            {
                order.BillingAddress.Notes = "Address changed by " + order.Order.Origin+" in order :"+ order.Order.ExtId1;
                daAddressDT.UpdateAddress(dbInfo, order.BillingAddress);
            }
        }
    

        /// <summary>
        /// Match e-food products/extras  with existing products/extras in DB. Also fill in the products/extras descriptions. 
        /// If matching fails then add error messages for every mismatch... 
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="order">e-food order</param>
        public void MatchProductsExtras(DBInfoModel dbInfo, DA_ExtDeliveryModel order)
        {
            long efoodPriceList = order.PriceListId;
            if (efoodPriceList <= 0)
            {
                logger.Error("ExternalDelivery plugin (like efood,...) did not set PriceListId");
                throw new Exception("ExternalDelivery plugin (like efood,...) did not set PriceListId");
            }
            //get default e-food product  and extras
            notMachedProd = productDT.GetProductExt(dbInfo, order.DefaultProductId);

            //1. try matching product
            foreach (DA_OrderDetails efoodProd in order.Order.Details)
            {
                ProductExtModel productDb = productDT.GetProductExt(dbInfo, efoodProd.ProductCode);
                if (productDb == null)
                {
                    //1b. product did not matched. replace with the default product and add problem to ErrorLogic
                    order.Order.LogicErrors = order.Order.LogicErrors + string.Format(Symposium.Resources.Errors.PRODUCTNOTFOUNDRICELIST, efoodProd.ProductId, notMachedProd.Description) + "|";// Product with Code: " + efoodProd.productCode + " not found. Replaced with default product '"+ notMachedProd.Description + "' |";
                    productDb = notMachedProd;
                   // efoodProd.productCode = notMachedProd.Code;
                    efoodProd.ProductId = notMachedProd.Id;
                    efoodProd.Description = notMachedProd.Description;
                    efoodProd.RateVat = vatTasks.GetProductVatFromPricelist(dbInfo, notMachedProd.Id, efoodPriceList).Percentage ?? 0;

                    efoodProd.NetAmount = efoodProd.Total / (1 + efoodProd.RateVat / 100);  //NetAmount = Total /(1+ RateVat/100) 
                    efoodProd.TotalVat = efoodProd.Total - efoodProd.NetAmount; //ποσό ΦΠΑ TotalVat = Total - NetAmount 
                    order.Order.TotalVat = order.Order.TotalVat + efoodProd.TotalVat;

                    efoodProd.RateTax = 0;
                    efoodProd.TotalTax = 0;
                    order.Order.TotalTax = order.Order.TotalTax + efoodProd.TotalTax;
                    order.Order.NetAmount = order.Order.NetAmount + efoodProd.NetAmount;
                    continue;
                }
                //2. product matched
                efoodProd.Description = productDb.Description;
                efoodProd.ProductId = productDb.Id;
                efoodProd.Id = 0;/////// efoodProd.ProductId;
                //2b. get vat percentage from price-list
                try
                {
                    efoodProd.RateVat = vatTasks.GetProductVatFromPricelist(dbInfo, productDb.Id, efoodPriceList).Percentage ?? 0;

                    efoodProd.NetAmount = efoodProd.Total / (1 + efoodProd.RateVat / 100);  //NetAmount = Total /(1+ RateVat/100) 
                    efoodProd.TotalVat = efoodProd.Total - efoodProd.NetAmount; //ποσό ΦΠΑ TotalVat = Total - NetAmount 
                    order.Order.TotalVat = order.Order.TotalVat + efoodProd.TotalVat;

                    efoodProd.RateTax = 0;
                    efoodProd.TotalTax = 0;
                    order.Order.TotalTax = order.Order.TotalTax + efoodProd.TotalTax;
                    order.Order.NetAmount = order.Order.NetAmount + efoodProd.NetAmount;
                }
                catch (Exception ex)
                {
                    constructError(order.Order, "Error for '" + (efoodProd.Description ?? "") + "': " + ex.Message);
                }


                //3. try matching extras
                if (efoodProd.Extras != null)
                {
                    foreach (var efoodExtras in efoodProd.Extras)
                    {

                        ProductExtrasIngredientsModel extraDb = productDb.Extras.FirstOrDefault(x => (x.Code ?? "") == efoodExtras.ExtraCode);
                        if (extraDb == null)
                        {
                            //3b. extras did not find under product. Find exrta based on code in general...
                            IngredientsModel ingr = ingredientsDT.GetModelByCode(dbInfo, (efoodExtras.ExtraCode));
                            if (ingr != null)
                            {
                                efoodExtras.Description = ingr.Description ?? "<NULL>";
                                efoodExtras.ExtrasId = ingr.Id ?? 0;
                                efoodExtras.RateVat = vatTasks.GetExtraVatFromPricelist(dbInfo, ingr.Id ?? 0, efoodPriceList).Percentage ?? 0;
                            }
                            else
                            {
                                //3c. extra did not find at all, replace with the default extras and add problem to ErrorLogic
                                if (notMachedProd.Extras == null || notMachedProd.Extras.Count == 0) throw new Exception("E-food Default Product " + notMachedProd.Id.ToString() + " without Extra.");
                                if (notMachedProd.Extras[0] == null) throw new Exception("EfoodProductId (see web config) must contain one extra.");
                                extraDb = notMachedProd.Extras[0];
                                efoodExtras.Description = extraDb.Description ?? "<NULL>";
                                efoodExtras.RateVat = vatTasks.GetExtraVatFromPricelist(dbInfo, notMachedProd.Extras[0].Id ?? 0, efoodPriceList).Percentage ?? 0;
                                //  order.LogicErrors = order.LogicErrors + string.Format(Symposium.Resources.Errors.NOEXTRACODE,  (efoodExtras.materialCode??"<NULL>")) + "|";
                                constructError(order.Order, string.Format(Symposium.Resources.Errors.NOEXTRACODE, (efoodExtras.ExtrasId.ToString())));
                                //continue;
                            }
                            //////3b. extras did not matched. replace with the default extras and add problem to ErrorLogic
                            //// if (notMachedProd.Extras == null || notMachedProd.Extras.Count == 0) throw new Exception("E-food Default Product " + notMachedProd.Id.ToString() + " without Extra.");
                            //// if (notMachedProd.Extras[0] == null) throw new Exception("EfoodProductId (see web config) must contain one extra.");
                            //// extraDb = notMachedProd.Extras[0];
                            //// efoodExtras.PosExtrasDescription = extraDb.Description ?? "<NULL>";
                            //// efoodProd.VatPercentage = vatTasks.GetExtraVatFromPricelist(dbInfo, notMachedProd.Extras[0].Id??0, efoodPriceList).Percentage ?? 0;
                            //// order.LogicErrors = order.LogicErrors + string.Format(Symposium.Resources.Errors.PRODUCTHAVENOEXTRA, productDb.Description, (efoodExtras.PosExtrasDescription ?? ""), efoodExtras.materialCode) + "|";//"Product '" + productDb.Description + "' does not have Extra '"+ (efoodExtras.PosExtrasDescription ?? "")+ "' with Code: " + efoodExtras.materialCode + "|";
                            //// continue;
                        }
                        else
                        {
                            //4. extra matched
                            efoodExtras.Description = extraDb.Description ?? "<NULL>";
                            efoodExtras.ExtrasId = extraDb.Id ?? 0;
                            efoodExtras.Id = 0;

                            if (order.RemoveRecipeExtras && extraDb.isRecipe & efoodExtras.Qnt > 0) efoodExtras.Qnt--; //στη λίστα των extras δε συμπεριλαμβάνονται όσα ανήκουν στη Συνταγή
                            if (order.Order.ReduceRecipeQnt) // for an e-food order ADD prices to extras and REDUCE prices from main products
                            {
                                if (efoodExtras.Qnt > 0)
                                {
                                    efoodExtras.Price = extraDb.Price ?? 0;
                                    efoodProd.Price = efoodProd.Price - (efoodExtras.Price * efoodExtras.Qnt);
                                    efoodProd.Total = efoodProd.Total - (efoodExtras.Price * efoodExtras.Qnt * efoodProd.Qnt);
                                }
                            }
                          
                        }
                        //4b. get vat percentage from price-list
                        if (efoodExtras.Qnt > 0)
                        {
                            try
                            {
                                if (efoodExtras.RateVat == 0) efoodExtras.RateVat = vatTasks.GetExtraVatFromPricelist(dbInfo, efoodExtras.ExtrasId, efoodPriceList).Percentage ?? 0;
                                var ttl = efoodProd.Qnt * efoodExtras.Qnt * efoodExtras.Price;
                                efoodExtras.NetAmount = ttl / (1 + efoodExtras.RateVat / 100);  //NetAmount = ttl /(1+ RateVat/100 ) , Όπου:  ttl = Detail.Qnt * Extra.Qnt * Extra.Price  
                                efoodExtras.OrderDetailId = 0;

                                efoodExtras.RateVat = efoodExtras.RateVat;//
                                efoodExtras.TotalVat = ttl - efoodExtras.NetAmount;//ποσό ΦΠΑ TotalVat = ttl -NetAmount  
                                order.Order.TotalVat = order.Order.TotalVat + efoodExtras.TotalVat;

                                efoodExtras.RateTax = 0;
                                efoodExtras.TotalTax = 0;
                                order.Order.TotalTax = order.Order.TotalTax + efoodExtras.TotalTax;
                                order.Order.NetAmount = order.Order.NetAmount + efoodExtras.NetAmount;
                            }
                            catch (Exception ex)
                            {
                                constructError(order.Order, "Error for '" + (efoodExtras.Description ?? "") + "': " + ex.Message);
                                continue;
                            }
                        }
                        else if (efoodExtras.Qnt < 0) //excluded from recipe
                        {
                            efoodExtras.Qnt = -1;
                            efoodExtras.Price = 0;
                        }

                    }
                    // 4c. remove all extras with Qnt = 0
                    efoodProd.Extras.RemoveAll(x => x.Qnt == 0);
                }
            }
            order.Order.ReduceRecipeQnt = false;
        }




        /// <summary>
        /// match store by pos address id. 
        /// Εάν StoreId = 0 Τότε η διεύθυνση δεν αντιστοιχεί σε κατάστημα
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="order">efood order</param>
        public void SelectPolygonByAddressId(DBInfoModel dbInfo, DA_ExtDeliveryModel order)
        {
            if (order.Order.StoreId > 0) return;
            DA_GeoPolygonsBasicModel polygon;
            try
            {
                if (order.ShippingAddress != null)
                {
                    polygon = da_GeoPolygonsDT.SelectPolygonByAddressId(dbInfo, order.ShippingAddress.Id);
                    order.Order.StoreId = polygon.StoreId ?? 0;
                    order.Order.GeoPolygonId = polygon.Id ;
                    if (order.Order.StoreId == 0)
                    {
                        constructError(order.Order, Symposium.Resources.Errors.STORENOTMATCHED);
                        logger.Warn(order.Order.Origin + ": Store is not matched for order " + order.Order.Id);
                    }
                    else if (order.Order.StoreId < 0)
                    {
                        var store = storesDT.GetStoreById(dbInfo, -order.Order.StoreId);
                        constructError(order.Order, "Store '" + store.Title + "' is not serving address '" + order.ShippingAddress.ToString() + "' for now. Polygon is inactive");
                        logger.Warn(order.Order.Origin + ": Store with id=" + (-order.Order.StoreId).ToString() + " does not serve address " + order.ShippingAddress.ToString() + ". Polygon is inactive.  Order " + order.Order.ExtId1);
                    }
                    else
                    {
                        DA_StoreModel storeModel = storesDT.GetStoreById(dbInfo, order.Order.StoreId);
                        order.Order.EstBillingDate = order.Order.OrderDate.AddMinutes(storeModel.DeliveryTime ?? 0);
                        order.Order.EstTakeoutDate = order.Order.OrderDate.AddMinutes(storeModel.TakeOutTime ?? 0);
                        order.Order.PosId = storeModel.PosId ?? 0;
                    }
                }
                else
                {
                    constructError(order.Order, Symposium.Resources.Errors.STORENOTMATCHED2);
                    logger.Warn(order.Order.Origin + ": Store is not matched for order " + order.Order.ExtId1 + ". Set Address's Latitude and Longitude first");
                }

            }
            catch (Exception ex)
            {
                constructError(order.Order, Symposium.Resources.Errors.STORENOTMATCHED + " : " + ex.Message);
                logger.Error(order.Order.Origin + ": Store is not matched for order " + order.Order.ExtId1 + " : " + ex.ToString());
            }


        }



        /// <summary>
        /// return all e-food Bucket from DB
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <returns></returns>
        public List<DA_ExtDeliveryModel> GetOrders(DBInfoModel dbInfo)
        {
            List<ExtDeliveryBucketModel> bucket = efoodDT.GetOrders(dbInfo);
            List<DA_ExtDeliveryModel> Orders = new List<DA_ExtDeliveryModel>();
            foreach (var item in bucket)
            {
                DA_ExtDeliveryModel order = JsonConvert.DeserializeObject<DA_ExtDeliveryModel>(item.Json);
                //  order.Errors = item.Errors;
                Orders.Add(order);
            }
            return Orders;
        }

        /// <summary>
        /// search an item from efood bucket based on efood order id
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="id">efood order id</param>
        /// <returns>EFoodBucketModel</returns>
        public ExtDeliveryBucketModel GetOrder(DBInfoModel Store, string id)
        {
            return efoodDT.GetOrder(Store, id);
        }


        /// <summary>
        /// upsert order into bucket
        /// </summary>
        /// <param name="dbInfo">db</param>
        ///  <param name="EFoodBucketModel">EFoodBucketModel (model from table EFoodBucket)</param>
        /// <returns></returns>
        public void UpsertBucketOrder(DBInfoModel dbInfo, ExtDeliveryBucketModel model)
        {
            efoodDT.UpsertOrder(dbInfo, model);
        }

        /// <summary>
        /// upsert order into bucket
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="OrderEfood">OrderEfood (model from e-food api)</param>
        /// <returns></returns>
        public void UpsertBucketOrder(DBInfoModel dbInfo, DA_ExtDeliveryModel orderEfood)
        {
            ExtDeliveryBucketModel model = new ExtDeliveryBucketModel();
            model.Id = orderEfood.Order.ExtId1;
            model.CreateDate = DateTime.Now;
            model.Json = JsonConvert.SerializeObject(orderEfood);
            UpsertBucketOrder(dbInfo, model);
            /*Send bucket size signal to hub*/
            int bucketSize = efoodDT.GetBucketSize(dbInfo);
            hub.Clients.Group("Agents").bucketModified(bucketSize);
            logger.Warn($" Order with ExtId1 = {orderEfood.Order.ExtId1} INSERTED to BUCKET. Error: {orderEfood.Order.ExtDeliveryErrors ?? ""}.  XML : {orderEfood.Order.ExtData.Replace(Environment.NewLine," ")} ");
        }

        /// <summary>
        /// Insert order to DA_Orders or to EfoodBucket.  
        /// Return true if order is saved to DA_Orders OR to EfoodBucket...
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool SaveOrder(DBInfoModel DBInfo, DA_ExtDeliveryModel order)
        {
            try
            {
                if (order.Order.ExtDeliveryErrors != "")
                {
                    try
                    {
                        //6a. add order to bucket for manually process
                        UpsertBucketOrder(DBInfo, order);
                        externalOrdersHelper.AddOrder(order.Order); //add order into cashed list of inserted orders
                        return true;
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"Error inserting order {order.Order.ExtId1} to Bucket: " + ex.ToString());
                        return false;
                    }
                }
                else
                {
                    //6b. add order to DA_Orders
                    try
                    {
                        daOdersFlows.InsertOrder(DBInfo, order.Order, 0);
                        externalOrdersHelper.AddOrder(order.Order);//add order into cashed list of inserted orders
                    }
                    catch (BusinessException bex)
                    {
                        try
                        {
                            order.Order.LogicErrors = order.Order.LogicErrors + bex.Message + "|";
                            order.Order.IgnoreShortages = true;
                            daOdersFlows.InsertOrder(DBInfo, order.Order, 0);
                            externalOrdersHelper.AddOrder(order.Order); //add order into cashed list of inserted orders
                            return true;
                        }
                        catch (Exception ex2)
                        {
                            //6c. on error add the order to bucket for manually process
                            constructError(order.Order, ex2.Message);
                            UpsertBucketOrder(DBInfo, order);
                            externalOrdersHelper.AddOrder(order.Order); //add order into cashed list of inserted orders
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        //6c. on error add the order to bucket for manually process
                        constructError(order.Order, ex.Message);
                        UpsertBucketOrder(DBInfo, order);
                        externalOrdersHelper.AddOrder(order.Order);//add order into cashed list of inserted orders
                        return true;
                    }
                }
                return true;
            }
            catch (Exception ext)
            {
                logger.Error($"Error Saving order {order.Order.ExtId1} to DA_Orders or to EfoodBucket: " + ext.ToString());
                return false;
            }
        }
  


        /// <summary>
        /// delete order from bucket
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="Id">e-food Order Id</param>
        /// <returns></returns>
        public void DeleteOrder(DBInfoModel dbInfo, string Id)
        {
            efoodDT.DeleteOrder(dbInfo, Id);
        }

        /// <summary>
        /// mark a bucket order as deleted
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="Id">efood Order Id</param>
        /// <returns></returns>
        public void MarkDeleted(DBInfoModel dbInfo, string Id)
        {
            efoodDT.MarkDeleted(dbInfo, Id);
            int bucketSize = efoodDT.GetBucketSize(dbInfo);
            hub.Clients.Group("Agents").bucketModified(bucketSize);
        }

        /// <summary>
        /// delete old orders marked as deleted from bucket
        /// </summary>
        /// <param name="Store">db</param>
        /// <returns></returns>
        public void DeleteOldOrder(DBInfoModel Store)
        {
            efoodDT.DeleteOldOrder(Store);
        }



        /// <summary>
        /// construct order's errors into the form: 'error1|error2|error3'
        /// </summary>
        /// <param name="order">OrderEfood</param>
        /// <param name="error">the error to add</param>
        public void constructError(DA_OrderExtDeliveryModel order, string error)
        {
            if (string.IsNullOrEmpty(order.ExtDeliveryErrors))
                order.ExtDeliveryErrors = error;
            else
                order.ExtDeliveryErrors = order.ExtDeliveryErrors + "|" + error;
        }
    }
}
