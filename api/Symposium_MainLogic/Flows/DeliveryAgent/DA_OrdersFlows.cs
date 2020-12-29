using log4net;
using Newtonsoft.Json;
using Symposium.Helpers;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.WebGoodysOrders;
using Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Goodys;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.DeliveryAgent
{
    public class DA_OrdersFlows : IDA_OrdersFlows
    {
        IDA_OrdersTasks ordersTasks;
        IDA_StoresTasks storesTasks;
        IDA_LoyaltyTasks loyaltyTasks;
        IDA_GeoPolygonsTasks geoPolygonsTasks;
        IDA_AddressesTasks addressesTasks;
        IDA_CustomerMessagesTasks customerMessagesTasks;
        IGoodysFlow goodysFlows;
        IDA_CustomerTasks custTasks;
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DA_OrdersFlows(IDA_OrdersTasks _ordersTasks, IDA_LoyaltyTasks loyaltyTasks, IDA_GeoPolygonsTasks geoPolygonsTasks,
            IDA_AddressesTasks addressesTasks, IDA_CustomerMessagesTasks customerMessagesTasks, IGoodysFlow goodysFlows, IDA_StoresTasks storesTasks, IDA_CustomerTasks _custTasks)
        {
            this.ordersTasks = _ordersTasks;
            this.loyaltyTasks = loyaltyTasks;
            this.geoPolygonsTasks = geoPolygonsTasks;
            this.addressesTasks = addressesTasks;
            this.customerMessagesTasks = customerMessagesTasks;
            this.goodysFlows = goodysFlows;
            this.storesTasks = storesTasks;
            this.custTasks = _custTasks;
        }

        /// <summary>
        /// Return the status of an Order
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="Id">Order Id</param>
        /// <returns></returns>
        public OrderStatusEnum GetStatus(DBInfoModel dbInfo, long Id)
        {
            return ordersTasks.GetStatus(dbInfo, Id);
        }

        /// <summary>
        /// Get All Orders
        /// </summary>
        /// <returns></returns>
        public List<DA_OrderModelExt> GetAllOrders(DBInfoModel dbInfo)
        {
            return ordersTasks.GetAllOrders(dbInfo);
        }

        /// <summary>
        /// Get Orders By Date
        /// </summary>
        /// <returns></returns>
        public List<DA_OrderModelExt> GetOrdersByDate(DBInfoModel dbInfo, string SelectedDate)
        {
            return ordersTasks.GetOrdersByDate(dbInfo, SelectedDate);
        }

        /// <summary>
        /// Get Customer Recent Orders
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <param name="top"></param>
        /// <param name="filter">define filter for returning orders</param>
        /// <returns>Επιστρέφει τις τελευταίες παραγγελίες ενός πελάτη</returns>
        public List<DA_OrderModel> GetOrders(DBInfoModel dbInfo, long id, int top, GetOrdersFilterEnum filter = GetOrdersFilterEnum.All)
        {
            return ordersTasks.GetOrders(dbInfo, id, top, filter);
        }

        /// <summary>
        /// Get A Specific Order
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>Order + details + ShippingAddress</returns>
        public DA_ExtOrderModel GetOrderById(DBInfoModel dbInfo, long Id)
        {
            return ordersTasks.GetOrderById(dbInfo, Id);
        }

        /// <summary>
        /// Search for Orders
        /// </summary>
        /// <param name="Model">Filter Model</param>
        /// <returns>Επιστρέφει τις παραγγελίες βάση κριτηρίων</returns>
        public List<DA_OrderModel> SearchOrders(DBInfoModel dbInfo, DA_SearchOrdersModel Model)
        {
            return ordersTasks.SearchOrders(dbInfo, Model);
        }

        /// <summary>
        /// Mεταβάλλει το DA_orders. StatusChange και εισάγει νέα εγγραφή στον DA_OrderStatus
        /// </summary>
        /// <param name="Id">Order Id</param>
        /// <returns></returns>
        public long UpdateStatus(DBInfoModel dbInfo, long Id, OrderStatusEnum Status)
        {
            return ordersTasks.UpdateStatus(dbInfo, Id, Status);
        }

        /// <summary>
        /// Get Customer Recent Remarks
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <param name="top"></param>
        /// <returns>Επιστρέφει τις τελευταίες παραγγελίες ενός πελάτη Remarks != null</returns>
        public List<DA_OrderModel> GetRemarks(DBInfoModel dbInfo, long Id, int top)
        {
            return ordersTasks.GetRemarks(dbInfo, Id, top);
        }

        /// <summary>
        /// Get Order Status For Specific Order by OrderId
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public List<DA_OrderStatusModel> GetOrderStatusTimeChanges(DBInfoModel dbInfo, long OrderId)
        {
            return ordersTasks.GetOrderStatusTimeChanges(dbInfo, OrderId);
        }

        /// <summary>
        /// Update Customer Remarks
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public long UpdateRemarks(DBInfoModel dbInfo, UpdateRemarksModel Model)
        {
            return ordersTasks.UpdateRemarks(dbInfo, Model);
        }

        /// <summary>
        /// Add new Order 
        /// </summary>
        ///  <param name="dbInfo">db</param>
        /// <param name="Model">order to insert</param>
        /// <param name="CustomerId">CustomerId from Auth Header or CustomerId=0</param>
        /// <returns></returns>
        public long InsertOrder(DBInfoModel dbInfo, DA_OrderModel Model, long CustomerId)
        {
            long anonymousCustomerId = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_AnonymousCustomerId");
            long anonymousCustIdNew = 0;
            if (Model.CustomerId == anonymousCustomerId)
            {
                //Insert Anonymous Customer
                if (Model.AnonymousCustomer != null)
                {
                    Model.AnonymousCustomer.IsAnonymous = DA_CustomerAnonymousTypeEnum.WillBeAnonymous;
                    Model.AnonymousCustomer.Id = 0;
                    if (string.IsNullOrWhiteSpace(Model.AnonymousCustomer.LastName))
                    {
                        Model.AnonymousCustomer.LastName = "AnonymousCustomer";
                    }
                    Model.AnonymousCustomer.SendSms = false;
                    Model.AnonymousCustomer.SendEmail = false;
                    Model.AnonymousCustomer.Email = null;
                    anonymousCustIdNew = custTasks.AddCustomer(dbInfo, Model.AnonymousCustomer);
                }
                else
                {
                    logger.Warn($">>--> Anonymous Customer with Id {Model.CustomerId} NOT Found");
                    throw new BusinessException(Symposium.Resources.Errors.ANONYMOUSCUSTOMERNULLMODEL);
                }
            }
            else
            {
                // check if CustomerId from Auth Header is the same with the CustomerId from DA_OrderModel (calls from Web)
                if (CustomerId > 0)
                {
                    if (Model.CustomerId != CustomerId) throw new BusinessException(Symposium.Resources.Errors.WRONGCUSTOMERIDS);
                }
            }

            //1. sanitize order model
            ordersTasks.SanitizeOrder(Model);

            //2. from product codes get ids
            ordersTasks.MatchIdFromCode(dbInfo, Model);

            //2b. from store code get id
            if (Model.StoreId <= 0 && !string.IsNullOrEmpty(Model.StoreCode))
            {
                Model.StoreId = storesTasks.GetStoreIdFromCode(dbInfo, Model.StoreCode);
            }

            //3. fill extras codes
            ordersTasks.FillExtrasCodes(dbInfo, Model.Details);

            //4. check selected store, set polygon
            if (Model.ShippingAddressId <= 0) Model.ShippingAddressId = null;
            ordersTasks.CheckStorePolygon(dbInfo, Model);
            //5. check prices
            ordersTasks.CheckPrices(Model);
            //6. check Store Status and set the proper value to isDelay (χρονοκαθηστέριση)
            ordersTasks.CheckStoreAvailabilityDelay(dbInfo, Model);

            //7.Check if the DA_Order is within valid DA_OpeningHours
            if (MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_OpeningHours") == true)
            {
                DateTime checkDate;
                if (Model.OrderType == OrderTypeStatus.TakeOut) checkDate = Convert.ToDateTime(Model.EstTakeoutDate);
                else
                    checkDate = Convert.ToDateTime(Model.EstBillingDate);

                ordersTasks.CheckDA_OpeningHours(dbInfo, Model.StoreId, checkDate);
            }
            //8. check if the correct price-lists are chosen
            ordersTasks.CheckPricelist(dbInfo, Model);
            //9.other validations
            ordersTasks.OrderValidations(dbInfo, Model);
            //10. check shortages
            ordersTasks.CheckShoratges(dbInfo, Model);

            //11. check customer address 
            if (Model.ShippingAddressId != null && Model.OrderType == OrderTypeStatus.Delivery)
            {
                List<DA_AddressModel> addrs = addressesTasks.getCustomerAddresses(dbInfo, Model.CustomerId);
                var ad = addrs.Where(c => c.Id == Model.ShippingAddressId).FirstOrDefault();
                if (ad == null)
                {
                    if (Model.CustomerId == anonymousCustomerId)
                    {
                        logger.Error($">>--> Anonymous Customer with Id {Model.CustomerId} has no Shipping Address with Id {Model.ShippingAddressId}");
                        throw new BusinessException($"No Address found with id {Model.ShippingAddressId} for the Customer.");
                    }
                    else
                    {
                        logger.Error($">>--> Customer with Id {Model.CustomerId} has no Shipping Address with Id {Model.ShippingAddressId}");
                        throw new BusinessException($"No Address found with id {Model.ShippingAddressId} for the Customer.");
                    }
                }
                else
                {
                    if (Model.CustomerId == anonymousCustomerId)
                    {
                        ad.OwnerId = anonymousCustIdNew;
                        addressesTasks.ChangeOwnerId(dbInfo, ad.Id, ad.OwnerId);
                        Model.CustomerId = anonymousCustIdNew;
                    }
                }
            }
            else
            {
                if (Model.AnonymousCustomer != null && Model.CustomerId == anonymousCustomerId)
                {
                    Model.CustomerId = anonymousCustIdNew;
                }
            }
            if (Model.ShippingAddressId == null && Model.OrderType == OrderTypeStatus.Delivery)
            {
                logger.Error($">>--> Delivery Order with ShippingAddressId = 0 or null");
                throw new BusinessException($"A Delivery Order should have Shipping Address.");
            }


            //12. set init values
            Model.Id = 0;
            Model.BillingDate = null;
            Model.TakeoutDate = null;
            Model.FinishDate = null;
            Model.Duration = null;
            Model.Duration = null;
            if (Model.Status != OrderStatusEnum.Received && Model.Status != OrderStatusEnum.OnHold)
            {
                logger.Warn("Order status changed from " + Model.Status + " to " + OrderStatusEnum.Received);
                Model.Status = OrderStatusEnum.Received;
            }
            Model.StatusChange = DateTime.Now;
            Model.IsSend = 1;
            Model.StoreOrderNo = null;
            Model.StoreOrderId = 0;
            Model.PointsGain = 0;


            //13. get active loyalty
            long activeLoyaltyRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.agentConfiguration, "ActiveLoyalty");
            DA_LoyaltyTypeEnum activeLoyalty = (DA_LoyaltyTypeEnum)activeLoyaltyRaw;

            //14. check Redeem Loyalty Points
            if (activeLoyalty == DA_LoyaltyTypeEnum.Hit)
            {
                if (Model.PointsRedeem > 0)
                {
                    loyaltyTasks.CheckRedeemPoints(dbInfo, Model);
                }
            }

            //15. calc loyalty gained points (No gained points for orders from efood, delivaras and clickdelivery)
            if (activeLoyalty == DA_LoyaltyTypeEnum.Hit)
            {
                if (Model.AddLoyaltyPoints)
                    Model.PointsGain = loyaltyTasks.CalcPointsFromOrder(dbInfo, Model);
                else
                    Model.PointsGain = 0;
            }

            //16. insert order to DB
            Model.Id = ordersTasks.InsertOrder(dbInfo, Model);
            string loggstr = $"NEW ORDER.  Id : {Model.Id}, ExtId1:{Model.ExtId1 ?? "<null>"}, Customer: {Model.CustomerId }, ShipAdd: {Model.ShippingAddressId ?? 0},  Origin: {Model.Origin}, Type: {Model.OrderType}, Status: {Model.Status}, Store: {Model.StoreId}, IsDelay: {Model.IsDelay}, PointsRedeem: {Model.PointsRedeem}, PointsGain: {Model.PointsGain}, Total: {Model.Total}, Items: {Model.Details.Count}.";
            if (!Model.AddLoyaltyPoints) loggstr = loggstr + " Client forbidden to Add LoyaltyPoints for Customer.";
            logger.Info(loggstr);
            // Task.Run(() => logger.Info($"NEW ORDER.  {JsonConvert.SerializeObject(Model).Replace("\\r\\n", " ").Replace("\n", " ")}"));

            //17. set loyalty points (gained/redeemed)
            if (activeLoyalty == DA_LoyaltyTypeEnum.Hit)
            {
                try
                {
                    loyaltyTasks.AddPoints(dbInfo, Model.Id, Model.CustomerId, Model.PointsGain, Model.OrderDate, 1, 0);
                    loyaltyTasks.AddPoints(dbInfo, Model.Id, Model.CustomerId, -Model.PointsRedeem, Model.OrderDate, 2, 0);
                }
                catch (Exception ex)
                {
                    logger.Error("Error Setting Loyalty points : " + ex.ToString());
                }
            }
            else if (activeLoyalty == DA_LoyaltyTypeEnum.Goodys)
            {
                try
                {
                    goodysFlows.RedeemGoodysLoyalty(dbInfo, Model);
                }
                catch (Exception ex)
                {
                    logger.Error("Error redeeming goodys loyalty: " + ex.ToString());
                }
            }

            //18. create customer message
            try
            {
                customerMessagesTasks.CreateMessageOnOrderCreate(dbInfo, Model.Id, Model.CustomerId, Model.Staffid ?? 0, Model.StoreId);
            }
            catch (Exception ex)
            {
                logger.Error("Error creating customer message on create: " + ex.ToString());
            }

            return Model.Id;
        }


        /// <summary>
        /// Update an Order (from DA or WEB only)
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="CustomerId">CustomerId from Auth Header</param>
        /// <returns></returns>
        public long UpdateOrder(DBInfoModel dbInfo, DA_OrderModel Model, long CustomerId)
        {
            // int points = 0;
            bool hasChanges = false;

            //1. check if CustomerId from Auth Header is the same with the CustomerId from DA_OrderModel
            if (CustomerId > 0)
            {
                if (Model.CustomerId != CustomerId) throw new BusinessException(Symposium.Resources.Errors.WRONGCUSTOMERIDS);
            }
            if (Model.ShippingAddressId <= 0) Model.ShippingAddressId = null;

            //2. get the existing order from DB
            DA_ExtOrderModel dbOrder = GetOrderById(dbInfo, Model.Id);

            //3. check if existing order (from DB) has the proper status to change.
            if (dbOrder.OrderModel.Status == OrderStatusEnum.Ready ||
                dbOrder.OrderModel.Status == OrderStatusEnum.Onroad ||
                dbOrder.OrderModel.Status == OrderStatusEnum.Canceled ||
                dbOrder.OrderModel.Status == OrderStatusEnum.Complete ||
                dbOrder.OrderModel.Status == OrderStatusEnum.Returned)
                throw new BusinessException(Symposium.Resources.Errors.ORDERCANNOTBEUPDATED);

            //4. OrderDate, CustomerId and StoreId must not be changed
            if (DateTime.Compare(dbOrder.OrderModel.OrderDate, Model.OrderDate) != 0 ||
                    dbOrder.OrderModel.CustomerId != Model.CustomerId ||
                    dbOrder.OrderModel.StoreId != Model.StoreId) throw new BusinessException("OrderDate, Customer and Store must not be changed");

            //5. sanitize order model
            ordersTasks.SanitizeOrder(Model);

            //6. check prices
            ordersTasks.CheckPrices(Model);
            //7. check store status and set the proper value to isDelay (χρονοκαθηστέριση)
            ordersTasks.CheckStoreAvailabilityDelay(dbInfo, Model);


            //8.Check if the DA_Order is within valid DA_OpeningHours
            if ( MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_OpeningHours") == true)
            {
                DateTime checkDate;
                if (Model.OrderType == OrderTypeStatus.TakeOut) checkDate = Convert.ToDateTime(Model.EstTakeoutDate);
                else
                    checkDate = Convert.ToDateTime(Model.EstBillingDate);
                ordersTasks.CheckDA_OpeningHours(dbInfo, Model.StoreId, checkDate);
            }

            //9. check if the correct price-lists are chosen
            ordersTasks.CheckPricelist(dbInfo, Model);
            //10.other validations
            ordersTasks.OrderValidations(dbInfo, Model);
            //11. check selected store, set polygon
            ordersTasks.CheckStorePolygon(dbInfo, Model);
            //12. check if items/extras have changed
            hasChanges = ordersTasks.CheckOrderItemsForChanges(dbInfo, Model);

            //13. prevent some existing values
            Model.Origin = dbOrder.OrderModel.Origin;
            Model.StoreOrderId = dbOrder.OrderModel.StoreOrderId;
            Model.StoreOrderNo = dbOrder.OrderModel.StoreOrderNo;
            Model.IsSend = dbOrder.OrderModel.IsSend;
            Model.BillingDate = dbOrder.OrderModel.BillingDate;
            Model.TakeoutDate = dbOrder.OrderModel.TakeoutDate;
            Model.FinishDate = dbOrder.OrderModel.FinishDate;
            Model.Duration = dbOrder.OrderModel.Duration;
            Model.StatusChange = dbOrder.OrderModel.StatusChange;
            Model.OrderDate = dbOrder.OrderModel.OrderDate;
            // Model.ExtObj = dbOrder.OrderModel.ExtObj;
            Model.PointsGain = dbOrder.OrderModel.PointsGain;
            Model.PointsRedeem = dbOrder.OrderModel.PointsRedeem;
            if (dbOrder.OrderModel.ItemsChanged && Model.IsSend > 0)
                Model.ItemsChanged = true;
            else
                Model.ItemsChanged = hasChanges;
            Model.ExtId1 = dbOrder.OrderModel.ExtId1;
            Model.ExtId2 = dbOrder.OrderModel.ExtId2;
            Model.ExtData = dbOrder.OrderModel.ExtData;
            if (Model.Metadata == "***")
                Model.Metadata = null;
            else if (string.IsNullOrWhiteSpace(Model.Metadata))
                Model.Metadata = dbOrder.OrderModel.Metadata;
            Model.NetAmount = Model.Total - Model.TotalVat - Model.TotalTax;
            Model.IsSend = ++dbOrder.OrderModel.IsSend;

            //14. set correct orderIds
            foreach (var item in Model.Details)
            {
                item.DAOrderId = Model.Id;
                if (item.Extras != null)
                {
                    foreach (var extra in item.Extras)
                        extra.OrderDetailId = item.Id;
                }
            }


            //15. check shortages
            ordersTasks.CheckShoratges(dbInfo, Model);

            //16. get active loyalty
            long activeLoyaltyRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.agentConfiguration, "ActiveLoyalty");
            DA_LoyaltyTypeEnum activeLoyalty = (DA_LoyaltyTypeEnum)activeLoyaltyRaw;

            if (hasChanges == true)
            {
                //17. set loyalty points (gained/redeemed)
                if (activeLoyalty == DA_LoyaltyTypeEnum.Hit)
                {
                    loyaltyTasks.DeleteGainPoints(dbInfo, Model.Id, 0);
                    //points = loyaltyTasks.CalcPointsFromOrder(dbInfo, Model);
                    if (Model.AddLoyaltyPoints)
                        Model.PointsGain = loyaltyTasks.CalcPointsFromOrder(dbInfo, Model);
                    else
                        Model.PointsGain = 0;
                    loyaltyTasks.CheckRedeemPoints(dbInfo, Model);
                }
            }

            if (dbOrder.OrderModel.IsDelay != Model.IsDelay)
            {
                logger.Warn("  >> Order with Id=" + Model.Id.ToString() + ": IsDelay has changed from " + dbOrder.OrderModel.IsDelay.ToString() + " to " + Model.IsDelay.ToString());
            }

            //18.  Update an Order to DB
            var c = ordersTasks.UpdateOrder(dbInfo, Model, hasChanges);

            string loggstr = $"ORDER UPDATED.  Id : {Model.Id}, ExtId1:{Model.ExtId1 ?? "<null>"}, Customer: {Model.CustomerId }, ShipAdd: {Model.ShippingAddressId},  Origin: {Model.Origin}, Type: {Model.OrderType}, Status: {Model.Status}, Store: {Model.StoreId}, IsDelay: {Model.IsDelay}, PointsRedeem: {Model.PointsRedeem}, PointsGain: {Model.PointsGain},  Total: {Model.Total}, Items: {Model.Details.Count}.";
            if (!Model.AddLoyaltyPoints) loggstr = loggstr + " Client forbidden to Add LoyaltyPoints for Customer.";
            logger.Info(loggstr);

            //19. update loyalty points (gained/redeemed) to DA_Orders and DA_LoyaltyPoints
            if (hasChanges == true)
            {
                if (activeLoyalty == DA_LoyaltyTypeEnum.Hit)
                {
                    loyaltyTasks.AddPoints(dbInfo, Model.Id, Model.CustomerId, Model.PointsGain, Model.OrderDate, 1, 0);
                    loyaltyTasks.AddPoints(dbInfo, Model.Id, Model.CustomerId, -Model.PointsRedeem, Model.OrderDate, 2, 0);
                }
            }

            //20. create customer message
            try
            {
                customerMessagesTasks.CreateMessageOnOrderUpdate(dbInfo, Model.Id, Model.CustomerId, Model.Staffid ?? 0, Model.StoreId);
            }
            catch (Exception ex)
            {
                logger.Error("Error creating customer message on update: " + ex.ToString());
            }

            return c;
        }

        /// <summary>
        /// Ακύρωση παραγγελίας από όλους εκτός από το κατάστημα. 
        /// </summary>
        /// <param name="Id">Order Id</param>
        /// <returns></returns>
        public long CancelOrder(DBInfoModel dbInfo, long Id, OrderStatusEnum[] cancelStasus)
        {
            long activeLoyaltyRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.agentConfiguration, "ActiveLoyalty");
            DA_LoyaltyTypeEnum activeLoyalty = (DA_LoyaltyTypeEnum)activeLoyaltyRaw;
            if (activeLoyalty == DA_LoyaltyTypeEnum.Hit)
            {
                loyaltyTasks.DeleteGainPoints(dbInfo, Id, 0);
            }
            else if (activeLoyalty == DA_LoyaltyTypeEnum.Goodys)
            {
                goodysFlows.CancelGoodysLoyalty(dbInfo, Id);
            }
            logger.Info($"ORDER CANCELED.  OrderId :{Id}...");
            return ordersTasks.CancelOrder(dbInfo, Id, cancelStasus);
        }

        /// <summary>
        /// Ακύρωση παραγγελίας από το κατάστημα MONO.  
        /// </summary>
        /// <param name="Id">Order Id</param>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        public long StoreCancelOrder(DBInfoModel dbInfo, long Id, long StoreId, OrderStatusEnum[] cancelStasus)
        {
            long k = ordersTasks.StoreCancelOrder(dbInfo, Id, StoreId, cancelStasus);
            logger.Info($"ORDER CANCELED.  OrderId :{Id},Store {StoreId}");
            return k;
        }

        /// <summary>
        /// Επιλογή Των Order Status που επιτρέπεται η ακύρωση Παραγγελίας.
        /// </summary>
        /// <returns>List of Status</returns>
        public List<int> StatusForCancel(DBInfoModel dbInfo, int[] cancelStasus)
        {
            return ordersTasks.StatusForCancel(dbInfo, cancelStasus);
        }

        /// <summary>
        /// Update order with external payment id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns>order id</returns>
        public long UpdateExternalPayment(DBInfoModel dbInfo, ExternalPaymentModel model)
        {
            long idUpdated = 0;
            long orderId = model.OrderId;
            DA_OrderModel order = ordersTasks.GetSingleOrderById(dbInfo, orderId);
            if (order != null)
            {
                idUpdated = ordersTasks.UpdatePaymentIdAndStatus(dbInfo, order, model);
            }
            return idUpdated;
        }

        /// <summary>
        /// Post Web Goodys Orders model from ATCOM/OMNIREST
        /// </summary>
        /// <returns></returns>
        public bool PostWebGoodysOrder(DBInfoModel dbInfo, WebGoodysOrdersModel Model)
        {
            return ordersTasks.PostWebGoodysOrder(dbInfo, Model);
        }

        /// <summary>
        /// Returns an order from orderno
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DAOrderNo"></param>
        /// <returns></returns>
        public DA_OrderModel GetOrderByOrderNo(DBInfoModel dbInfo, long DAOrderNo)
        {
            return ordersTasks.GetOrderByOrderNo(dbInfo, DAOrderNo);
        }

        /// <summary>
        /// Update's ExtId1 with Omnirest External system OrderNo 
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <returns></returns>
        public bool UpdateDA_OrderExtId1(DBInfoModel dbInfo, long DA_OrderId, long OrderNo)
        {
            return ordersTasks.UpdateDA_OrderExtId1(dbInfo, DA_OrderId, OrderNo);
        }

        /// <summary>
        /// Returns a list of order status for Goodys Omnirest and a list of open orders more than 24 hours to close them
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="completedOrders"></param>
        /// <returns></returns>
        public List<DA_OrdersForGoodysOmnirestStatus> GetOrdersForGoodysOmnirest(DBInfoModel dbInfo, out List<DA_OrderStatusModel> completedOrders)
        {
            string error;
            completedOrders = new List<DA_OrderStatusModel>();
            //1. Get all orders with open status
            List<DA_OrdersForGoodysOmnirestStatus> result = null;
            try
            {
                result = ordersTasks.GetOrdersForGoodysOmnirest(dbInfo, out error);
                if (result == null)
                {
                    logger.Error(error);
                    return result;
                }

                //2. If different between orderdate and getdate bigger than 24 then close order
                foreach (DA_OrdersForGoodysOmnirestStatus item in result)
                {
                    if (item.StayHours > 24)
                    {
                        DA_OrderStatusModel tmp = new DA_OrderStatusModel();
                        tmp.Id = 0;
                        tmp.KeepStoreTimeStatus = false;
                        tmp.OrderDAId = item.Id;
                        tmp.Status = (int)OrderStatusEnum.Complete;
                        tmp.StatusDate = DateTime.Now;
                        completedOrders.Add(tmp);
                    }
                }

                result.RemoveAll(r => r.StayHours > 24);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            return result;

        }

        /// <summary>
        /// Converts a list of Omnirest Orders status to da_order status
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<DA_OrderStatusModel> ConvertOmniRestStatusToDA_OrderStatus(List<DA_OrdersForGoodysOmnirestStatus> model)
        {
            List<DA_OrderStatusModel> result = new List<DA_OrderStatusModel>();
            foreach (DA_OrdersForGoodysOmnirestStatus item in model)
            {
                if (item.StayHours < 25)
                {
                    DA_OrderStatusModel tmp = new DA_OrderStatusModel();
                    tmp.Id = 0;
                    tmp.KeepStoreTimeStatus = false;
                    tmp.OrderDAId = item.Id;
                    tmp.Status = (short)item.Status;
                    tmp.StatusDate = DateTime.Now;
                    result.Add(tmp);
                }
            }
            return result;
        }
    }
}
