using Autofac;
using AutoMapper;
using log4net;
using Newtonsoft.Json;
using Pos_WebApi.Models;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.Hotelizer;
using Symposium.Models.Models.MealBoards;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Symposium.WebApi.MainLogic.Flows.Hotelizer
{
    public class HotelizerFlows
    {
        /// <summary>
        /// Instance for Logger
        /// </summary>
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Instance for order flows
        /// </summary>
        private IOrderFlows orderFlow;

        /// <summary>
        /// Instance for Invoices
        /// </summary>
        private IInvoiceTasks invTasks;

        /// <summary>
        /// Instance for Order Detail Invoices
        /// </summary>
        private IOrderDetailInvoicesTasks orderDetInvTask;

        /// <summary>
        /// Instance for OrderDetails
        /// </summary>
        private IOrderDetailTasks orderDetTask;

        /// <summary>
        /// Instance for Transfer to pms Flow
        /// </summary>
        private ITransferToPmsFlows ttpmsFlow;

        /// <summary>
        /// Instance for accounts
        /// </summary>
        private IAccountFlows accountFlow;

        /// <summary>
        /// Apii URL for Hotlizer
        /// </summary>
        private string apiCall;

        /// <summary>
        /// Authentication for api calls username:password
        /// </summary>
        private string apiAuth;

        /// <summary>
        /// List of Mapping method of payments between WebPos and Hotelizer
        /// </summary>
        private List<MappingMethodOfPaymentsModel> mappingPayments;

        /// <summary>
        /// Instance for web api helper
        /// </summary>
        private WebApiClientHelper webHlp;


        /// <summary>
        /// Test project: autofac impl.
        /// </summary>
        //private readonly ContainerBuilder _builder;
        //private readonly IContainer _container;

        public HotelizerFlows()
        {
            var config = System.Web.Http.GlobalConfiguration.Configuration;
            System.Web.Http.Dependencies.IDependencyResolver autofac;
            autofac = config.DependencyResolver;

            orderFlow = (IOrderFlows)autofac.GetService(typeof(IOrderFlows));
            ttpmsFlow = (ITransferToPmsFlows)autofac.GetService(typeof(ITransferToPmsFlows));
            accountFlow = (IAccountFlows)autofac.GetService(typeof(IAccountFlows));
            invTasks = (IInvoiceTasks)autofac.GetService(typeof(IInvoiceTasks));
            orderDetInvTask = (IOrderDetailInvoicesTasks)autofac.GetService(typeof(IOrderDetailInvoicesTasks));
            orderDetTask = (IOrderDetailTasks)autofac.GetService(typeof(IOrderDetailTasks));

            apiCall = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "externalApiURL");
            if (!apiCall.EndsWith("/"))
                apiCall += "/";

            apiAuth = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "externalApiAuthentication");

            webHlp = new WebApiClientHelper();

            // getting mapping values from vconfig and creates a list of them. 
            // Items looks like 9601-1; where first item is PmsRoom from table EODAccountToPmsTransfer and second one is Hotelizer Id
            mappingPayments = new List<MappingMethodOfPaymentsModel>();
            string configVal = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "externalApiMappingMethodOfPayments");
            string[] methods = configVal.Split(';');
            foreach (string item in methods)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    string[] value = item.Split('-');
                    mappingPayments.Add(new MappingMethodOfPaymentsModel { PmsRoom = int.Parse(value[0]), HotelizerId = int.Parse(value[1]) });
                }
            }
        }


        /// <summary>
        /// Returns a list of dynamic with departments from Hotelizer
        /// </summary>
        /// <returns></returns>
        public IEnumerable<HotelizerDepartmentModel> GetHotelizerDepartments()
        {
            var result = new List<HotelizerDepartmentModel>();
            string url = apiCall + "api/integrations/pos/services";
            string res, errorMess = "";
            int errorCode = 0;
            try
            {
                res = webHlp.GetRequest(url, apiAuth, null, out errorCode, out errorMess);
                if (errorCode == 200)
                {
                    HotelizerDepartmentResponceModel hotResponceDep = JsonConvert.DeserializeObject<HotelizerDepartmentResponceModel>(res);
                    if (hotResponceDep.success)
                        result = hotResponceDep.data;
                }
                else
                    logger.Error("[URL : " + url + ", authentication : " + apiAuth + "] \r\n error code : " + errorCode.ToString() + " (" + errorMess + ")");
            }
            catch (Exception ex)
            {
                logger.Error("[URL : " + url + ", authentication : " + apiAuth + "] \r\n error code : " + errorCode.ToString() + " (" + errorMess + ") \r\n" + ex.ToString());
            }
            return result;
        }

        /// <summary>
        /// Returns a list of Taxes model for Hotelizer
        /// </summary>
        /// <returns></returns>
        public List<HotelizerTaxesModel> GetHotelizerTaxes()
        {
            List<HotelizerTaxesModel> result = new List<HotelizerTaxesModel>();
            string url = apiCall + "api/integrations/pos/taxes";
            string res, errorMess = "";
            int errorCode = 0;
            try
            {
                res = webHlp.GetRequest(url, apiAuth, null, out errorCode, out errorMess);
                if (errorCode == 200)
                {
                    HotelizerResponceTaxesModel hotResponce = JsonConvert.DeserializeObject<HotelizerResponceTaxesModel>(res);
                    if (hotResponce.success)
                        result = hotResponce.data;
                }
                else
                    logger.Error("[URL : " + url + ", authentication : " + apiAuth + "] \r\n error code : " + errorCode.ToString() + " (" + errorMess + ")");
            }
            catch (Exception ex)
            {
                logger.Error("[URL : " + url + ", authentication : " + apiAuth + "] \r\n error code : " + errorCode.ToString() + " (" + errorMess + ") \r\n" + ex.ToString());
            }
            return result;
        }

        /// <summary>
        /// Returns a list of Method of payments
        /// </summary>
        /// <returns></returns>
        public List<HotelizerMethodOfPaymentModel> GetHotelizerMethodOfPayments()
        {
            List<HotelizerMethodOfPaymentModel> result = new List<HotelizerMethodOfPaymentModel>();
            string url = apiCall + "api/integrations/pos/payment_methods";
            string res, errorMess = "";
            int errorCode = 0;
            try
            {
                res = webHlp.GetRequest(url, apiAuth, null, out errorCode, out errorMess);
                if (errorCode == 200)
                {
                    HotelizerResponceMethodOfPaymentModel hotResponse = JsonConvert.DeserializeObject<HotelizerResponceMethodOfPaymentModel>(res);
                    if (hotResponse.success)
                        result = hotResponse.data;
                }
                else
                    logger.Error("[URL : " + url + ", authentication : " + apiAuth + "] \r\n error code : " + errorCode.ToString() + " (" + errorMess + ")");
            }
            catch (Exception ex)
            {
                logger.Error("[URL : " + url + ", authentication : " + apiAuth + "] \r\n error code : " + errorCode.ToString() + " (" + errorMess + ") \r\n" + ex.ToString());
            }
            return result;
        }

        /// <summary>
        /// Returns a list of rooms on a Customers from hotelizer 
        /// </summary>
        /// <param name="roomNo"> search for specific room no </param>
        /// <param name="reservId"> search for specigic reservation id</param>
        /// <param name="pageNo"> current page. if less than 0 then 1. first page means pageNo = 0</param>
        /// <param name="pageSize">page size. How many records will be appeared on any page </param>
        /// <returns></returns>
        public List<Customers> GetRoomsAsCustomers(string roomNo, int reservId, int pageNo, int pageSize)
        {
            List<Customers> result = new List<Customers>();
            List<HotelizerCustomerModel> rooms = new List<HotelizerCustomerModel>();
            string url = apiCall + "api/integrations/pos/rooms";
            string res, errorMess = "";
            int errorCode = 0, idx, totRecs;
            try
            {
                // Gets data from apiURL
                res = webHlp.GetRequest(url, apiAuth, null, out errorCode, out errorMess);
                if (errorCode == 200)
                {
                    HotelizerResponceCustomerModel hotRespose = JsonConvert.DeserializeObject<HotelizerResponceCustomerModel>(res);
                    if (!hotRespose.success)
                        return result;

                    rooms = hotRespose.data;

                    //Remove emtpy profiles
                    List<HotelizerCustomerModel> emptyGuest = rooms.FindAll(f => f.guest_id == 0 || f.guest_name == null);
                    rooms.RemoveAll(r => r.guest_id == null || r.guest_name == null);

                    if (emptyGuest != null && emptyGuest.Count > 0)
                        logger.Info("There are empty rpofiles : " + JsonConvert.SerializeObject(emptyGuest));

                    //if room exists filter data based on room no
                    if (!string.IsNullOrWhiteSpace(roomNo))
                        rooms = rooms.Where(w => w.room_name.Contains(roomNo) || w.guest_name.Contains(roomNo)).ToList();

                    //if reservation id exists filter data based on reservation id
                    if (reservId > 0)
                        rooms = rooms.Where(w => w.accommodation_id == reservId).ToList();

                    //Total records for pagination
                    totRecs = rooms.Count();

                    //index for row_id for pagination
                    idx = 1;

                    //Update all records with row id and total records
                    foreach (HotelizerCustomerModel item in rooms)
                    {
                        item.TotalRecs = totRecs;
                        item.Row_ID = idx;
                        idx++;
                    }

                    // find selected page and returns records equals to page size
                    int start, ends;
                    if (pageNo < 1)
                        start = 1;
                    else
                        start = (pageNo * pageSize) + 1;
                    if (pageNo < 1)
                        ends = pageSize;
                    else
                        ends = pageSize * (pageNo + 1);

                    rooms = rooms.Where(w => w.Row_ID >= start && w.Row_ID <= ends).ToList();

                    //returns a list of CustomerModel using automapper
                    result = Mapper.Map<List<Customers>>(rooms);
                }
                else
                    logger.Error("[URL : " + url + ", authentication : " + apiAuth + "] \r\n error code : " + errorCode.ToString() + " (" + errorMess + ")");
            }
            catch (Exception ex)
            {
                logger.Error("[URL : " + url + ", authentication : " + apiAuth + "] \r\n error code : " + errorCode.ToString() + " (" + errorMess + ") \r\n" + ex.ToString());
            }
            return result;
        }

        /// <summary>
        /// Returns a list of rooms on a CustomerModel from hotelizer 
        /// </summary>
        /// <param name="roomNo"> search for specific room no </param>
        /// <param name="reservId"> search for specigic reservation id</param>
        /// <param name="pageNo"> current page. if less than 0 then 1. first page means pageNo = 0</param>
        /// <param name="pageSize">page size. How many records will be appeared on any page </param>
        /// <returns></returns>
        public List<CustomerModel> GetRoomsAsCustomerModel(string roomNo, int reservId, int pageNo, int pageSize, DateTime? fromDt = null, DateTime? toDt = null)
        {
            List<CustomerModel> result = new List<CustomerModel>();
            List<HotelizerCustomerModel> rooms = new List<HotelizerCustomerModel>();
            string url = apiCall + "api/integrations/pos/rooms";
            string res, errorMess = "";
            int errorCode = 0, idx, totRecs;
            try
            {
                // Gets data from apiURL
                res = webHlp.GetRequest(url, apiAuth, null, out errorCode, out errorMess);
                if (errorCode == 200)
                {
                    HotelizerResponceCustomerModel hotRespose = JsonConvert.DeserializeObject<HotelizerResponceCustomerModel>(res);
                    if (!hotRespose.success)
                        return result;

                    rooms = hotRespose.data;

                    //Remove emtpy profiles
                    List<HotelizerCustomerModel> emptyGuest = rooms.FindAll(f => f.guest_id == 0 || f.guest_name == null);
                    rooms.RemoveAll(r => r.guest_id == null || r.guest_name == null);

                    if (emptyGuest != null && emptyGuest.Count > 0)
                        logger.Info("There are empty rpofiles : " + JsonConvert.SerializeObject(emptyGuest));

                    //if room exists filter data based on room no
                    if (!string.IsNullOrWhiteSpace(roomNo))
                        rooms = rooms.Where(w => w.room_name.Contains(roomNo) || w.guest_name.Contains(roomNo)).ToList();

                    //if reservation id exists filter data based on reservation id
                    if (reservId > 0)
                        rooms = rooms.Where(w => w.accommodation_id == reservId).ToList();

                    if (fromDt != null)
                    {
                        DateTime fr = fromDt ?? new DateTime(1900, 1, 1);
                        DateTime to = toDt ?? new DateTime(1900, 1, 1);
                        rooms = rooms.Where(w => w.departure >= fr && w.arrival <= to).ToList();
                    }

                    //Total records for pagination
                    totRecs = rooms.Count();

                    //index for row_id for pagination
                    idx = 1;

                    //Update all records with row id and total records
                    foreach (HotelizerCustomerModel item in rooms)
                    {
                        item.TotalRecs = totRecs;
                        item.Row_ID = idx;
                        idx++;
                    }

                    // find selected page and returns records equals to page size
                    int start, ends;
                    if (pageNo < 1)
                        start = 1;
                    else
                        start = (pageNo * pageSize) + 1;
                    if (pageNo < 1)
                        ends = pageSize;
                    else
                        ends = pageSize * (pageNo + 1);

                    rooms = rooms.Where(w => w.Row_ID >= start && w.Row_ID <= ends).ToList();

                    //returns a list of CustomerModel using automapper
                    result = Mapper.Map<List<CustomerModel>>(rooms);
                }
                else
                    logger.Error("[URL : " + url + ", authentication : " + apiAuth + "] \r\n error code : " + errorCode.ToString() + " (" + errorMess + ")");
            }
            catch (Exception ex)
            {
                logger.Error("[URL : " + url + ", authentication : " + apiAuth + "] \r\n error code : " + errorCode.ToString() + " (" + errorMess + ") \r\n" + ex.ToString());
            }
            return result;
        }

        /// <summary>
        /// Returns a list of rooms on a CustomersDetails from hotelizer 
        /// </summary>
        /// <param name="roomNo"> search for specific room no </param>
        /// <param name="reservId"> search for specigic reservation id</param>
        /// <param name="pageNo"> current page. if less than 0 then 1. first page means pageNo = 0</param>
        /// <param name="pageSize">page size. How many records will be appeared on any page </param>
        /// <returns></returns>
        public List<CustomersDetails> GetRoomsAsCustomersDetails(string roomNo, int reservId, int pageNo, int pageSize)
        {
            List<CustomersDetails> result = new List<CustomersDetails>();
            List<HotelizerCustomerModel> rooms = new List<HotelizerCustomerModel>();
            string url = apiCall + "api/integrations/pos/rooms";
            string res, errorMess = "";
            int errorCode = 0, idx, totRecs;
            try
            {
                // Gets data from apiURL
                res = webHlp.GetRequest(url, apiAuth, null, out errorCode, out errorMess);
                if (errorCode == 200)
                {
                    HotelizerResponceCustomerModel hotRespose = JsonConvert.DeserializeObject<HotelizerResponceCustomerModel>(res);
                    if (!hotRespose.success)
                        return result;

                    rooms = hotRespose.data;

                    //Remove emtpy profiles
                    List<HotelizerCustomerModel> emptyGuest = rooms.FindAll(f => f.guest_id == 0 || f.guest_name == null);
                    rooms.RemoveAll(r => r.guest_id == null || r.guest_name == null);

                    if (emptyGuest != null && emptyGuest.Count > 0)
                        logger.Info("There are empty rpofiles : " + JsonConvert.SerializeObject(emptyGuest));

                    //if room exists filter data based on room no
                    if (!string.IsNullOrWhiteSpace(roomNo))
                        rooms = rooms.Where(w => w.room_name.Contains(roomNo) || w.guest_name.Contains(roomNo)).ToList();

                    //if reservation id exists filter data based on reservation id
                    if (reservId > 0)
                        rooms = rooms.Where(w => w.accommodation_id == reservId).ToList();

                    //Total records for pagination
                    totRecs = rooms.Count();

                    //index for row_id for pagination
                    idx = 1;

                    //Update all records with row id and total records
                    foreach (HotelizerCustomerModel item in rooms)
                    {
                        item.TotalRecs = totRecs;
                        item.Row_ID = idx;
                        idx++;
                    }

                    // find selected page and returns records equals to page size
                    int start, ends;
                    if (pageNo < 1)
                        start = 1;
                    else
                        start = (pageNo * pageSize) + 1;
                    if (pageNo < 1)
                        ends = pageSize;
                    else
                        ends = pageSize * (pageNo + 1);

                    rooms = rooms.Where(w => w.Row_ID >= start && w.Row_ID <= ends).ToList();

                    //returns a list of CustomersDetails using automapper
                    result = Mapper.Map<List<CustomersDetails>>(rooms);
                }
                else
                    logger.Error("[URL : " + url + ", authentication : " + apiAuth + "] \r\n error code : " + errorCode.ToString() + " (" + errorMess + ")");
            }
            catch (Exception ex)
            {
                logger.Error("[URL : " + url + ", authentication : " + apiAuth + "] \r\n error code : " + errorCode.ToString() + " (" + errorMess + ") \r\n" + ex.ToString());
            }
            return result;
        }

        /// <summary>
        /// Post a receipt model to Hotelizer and updates Order and transfettopms tables with returned values
        /// </summary>
        /// <param name="receipt"></param>
        /// <param name="transferToPms"></param>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        public bool PostNewChargeToHotelizer(Receipts receipt, List<TransferToPmsModel> transferToPms, long invoiceId, DBInfoModel dbInfo)
        {
            bool result = true;

            //check if receipt is charge to room
            bool SendPost = false;
            foreach (ReceiptPayments item in receipt.ReceiptPayments)
            {
                if (item.AccountType == 3) //Charge to room
                    SendPost = true;
            }

            if (!SendPost)
                return result;

            //string url = apiCall + "api/integrations/pos/room_charge";
            string url = apiCall + "api/integrations/pos/charge";
            string res, errorMess = "";
            int errorCode = 0;
            List<long> OrderIds = new List<long>();
            List<HotelizerPostOrderModel> postOrder = new List<HotelizerPostOrderModel>();
            try
            {
                List<HotelizerTaxesModel> taxes = GetHotelizerTaxes();

                //list of orders to update with ext keys after post
                if (invoiceId > 0)
                {
                    InvoiceModel inv = invTasks.GetInvoiceById(dbInfo, invoiceId);
                    if (inv != null)
                    {
                        List<OrderDetailInvoicesModel> orderDets = orderDetInvTask.GetOrderDetailInvoicesOfSelectedInvoice(dbInfo, inv.Id ?? 0);
                        if (orderDets != null)
                        {
                            foreach (OrderDetailInvoicesModel item in orderDets)
                            {
                                OrderDetailModel details = orderDetTask.GetOrderDetailById(dbInfo, item.OrderDetailId ?? 0);
                                if (details != null)
                                {
                                    var fldOrder = OrderIds.Find(f => f == (details.OrderId ?? 0));
                                    if (fldOrder == 0)
                                        OrderIds.Add(details.OrderId ?? 0);
                                }
                            }
                        }
                    }
                }

                //Create a model for post
                HotelizerPostOrderModel tmpOrder = new HotelizerPostOrderModel();
                tmpOrder.due_date = receipt.Day ?? DateTime.Now;
                tmpOrder.guid = Guid.NewGuid().ToString();
                tmpOrder.order_no = receipt.OrderNo.ToString();

                //Get Accommodation id from RegNo from table TransferToPms. Passed on Customer Model on property ReservationId
                int tmpAccommodationId = 0;
                int.TryParse(transferToPms[0].RegNo, out tmpAccommodationId);
                tmpOrder.accommodation_id = tmpAccommodationId;


                tmpOrder.products = new List<HotelizerPostOrderProductsModel>();

                TransferMappingsModel mappingDepartment;
                int depId;
                //Products for model 
                foreach (ReceiptDetails item in receipt.ReceiptDetails)
                {
                    HotelizerPostOrderProductsModel tmp = new HotelizerPostOrderProductsModel();

                    // get hotelizer department id from TransferMapping table
                    mappingDepartment = orderFlow.GetTransferMappingForNewTransaction(dbInfo, receipt.DepartmentId ?? 0, item.ProductCategoryId ?? 0, item.PricelistId ?? 0, 1);
                    int.TryParse(mappingDepartment.PmsDepartmentId, out depId);

                    // get hotelizer tax id based on tax percentace
                    var fldTaxes = taxes.Find(f => f.value == (item.ItemVatRate ?? 0));

                    tmp.currency = "EUR";
                    tmp.description = item.ItemDescr;
                    tmp.external_code = item.ItemCode;
                    tmp.gross_value = item.ItemGross ?? 0;
                    tmp.name = item.ItemDescr;
                    tmp.price = item.Price ?? 0;
                    tmp.quantity = (decimal)(item.ItemQty ?? 0);
                    tmp.service_id = depId;
                    tmp.taxes = new int[1];
                    tmp.taxes[0] = fldTaxes == null ? 0 : fldTaxes.id;

                    tmpOrder.products.Add(tmp);
                }
                ///add temporary model to list for send data
                postOrder.Add(tmpOrder);

                //Post model to hotelizer
                res = webHlp.PostRequest(postOrder, url, apiAuth, null, out errorCode, out errorMess);

                //success responce
                if (errorCode == 200)
                {
                    HotelizerResponcePostOrderModel responce = JsonConvert.DeserializeObject<HotelizerResponcePostOrderModel>(res);

                    //post succeded
                    if (responce.success)
                    {
                        //update web pos orders
                        foreach (long item in OrderIds)
                        {
                            OrderModel order = orderFlow.GetOrderById(dbInfo, item);
                            order.ExtKey = responce.data[0].id.ToString();
                            order.ExtType = (int)ExternalSystemOrderEnum.Hotelizer;
                            orderFlow.AddNewOrder(dbInfo, order);
                        }

                        foreach (TransferToPmsModel item in transferToPms)
                        {
                            if (responce.data != null && responce.data.Count > 0)
                            {
                                item.PmsResponseId = responce.data[0].id.ToString();
                                ttpmsFlow.AddNewTransferToPms(dbInfo, item);
                            }
                        }
                    }
                }
                else
                    logger.Error("[URL : " + url + ", authentication : " + apiAuth + "] \r\n error code : " + errorCode.ToString() + " (" + errorMess + ") \r\n Model [" + JsonConvert.SerializeObject(postOrder) + "]");
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("[URL : " + url + ", authentication : " + apiAuth + "] \r\n error code : " + errorCode.ToString() + " (" + errorMess + ") \r\n Model [" + JsonConvert.SerializeObject(postOrder) + "] \r\n" + ex.ToString());
            }
            return result;
        }

        /// <summary>
        /// Updates a list of 
        /// </summary>
        /// <param name="transferToPms"></param>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        public bool PostNewPaymentsToHotelizer(List<EodTransferToPmsModel> transferToPms, DBInfoModel dbInfo)
        {
            bool result = true;
            string url = apiCall + "api/integrations/pos/payments", errorMess = "", res;
            int errorCode = 0;

            HotelizerPostOrderPaymentsModel sendPayments = new HotelizerPostOrderPaymentsModel();
            try
            {

                foreach (EodTransferToPmsModel item in transferToPms)
                {
                    //find the hotelizer payment id from api.json )mapped)
                    var fld = mappingPayments.Find(f => f.PmsRoom.ToString() == item.roomDescription);
                    if (fld != null)
                    {
                        //makes a post model
                        sendPayments = new HotelizerPostOrderPaymentsModel();
                        sendPayments.amount = item.total;
                        sendPayments.due_date = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
                        sendPayments.payment_option_id = fld.HotelizerId;

                        //send request
                        res = webHlp.PostRequest(sendPayments, url, apiAuth, null, out errorCode, out errorMess);

                        //not posted
                        if (errorCode != 200)
                            logger.Error("[URL : " + url + ", authentication : " + apiAuth + "] \r\n error code : " + errorCode.ToString() + " (" + errorMess + ") \r\n Model [" + JsonConvert.SerializeObject(sendPayments) + "]");
                    }
                }


            }
            catch (Exception ex)
            {
                logger.Error("[URL : " + url + ", authentication : " + apiAuth + "] \r\n error code : " + errorCode.ToString() + " (" + errorMess + ") \r\n Model [" + JsonConvert.SerializeObject(sendPayments) + "] \r\n" + ex.ToString());
                result = false;
            }
            return result;
        }

        public bool CancelReceiptToHotelizer(long invoiceId, DBInfoModel dbInfo)
        {
            bool result = true;
            string url = apiCall + "api/integrations/pos/void?order_id=", errorMess = "", res;
            int errorCode = 0;

            ///List of Order to cancelel (gets the ExtKey)
            List<long> orders = new List<long>();

            ///Model to be sended for cancel
            HotelizerCancelReceiptModel canceled = new HotelizerCancelReceiptModel();
            try
            {
                //List of order detail invoices for the inovice id
                List<OrderDetailInvoicesModel> orderDetInv = orderDetInvTask.GetOrderDetailInvoicesOfSelectedInvoice(dbInfo, invoiceId);
                if (orderDetInv == null || orderDetInv.Count < 1)
                {
                    logger.Error("Invoice not found for invoice id : " + invoiceId.ToString());
                    return false;
                }

                //Gets all Order details from Order detail invoices to get the Ordr ids.
                foreach (OrderDetailInvoicesModel item in orderDetInv)
                {
                    OrderDetailModel orderDet = orderDetTask.GetOrderDetailById(dbInfo, item.OrderDetailId ?? 0);
                    if (orderDet != null)
                    {
                        var fld = orders.Find(f => f == (orderDet.OrderId ?? 0));
                        if (fld == 0)
                            orders.Add(orderDet.OrderId ?? 0);
                    }
                }

                foreach (long item in orders)
                {
                    OrderModel orderModel = orderFlow.GetOrderById(dbInfo, item);
                    if (orderModel != null && (ExternalSystemOrderEnum)orderModel.ExtType != null && (ExternalSystemOrderEnum)orderModel.ExtType == ExternalSystemOrderEnum.Hotelizer && !string.IsNullOrWhiteSpace(orderModel.ExtKey))
                    {
                        canceled = new HotelizerCancelReceiptModel();
                        canceled.external_id = orderModel.Id.ToString();
                        canceled.order_id = int.Parse(orderModel.ExtKey);
                        canceled.token = "";

                        url += Convert.ToString(canceled.order_id);

                        //Post canceled model to hotelizer
                        res = webHlp.PostRequest(canceled, url, apiAuth, null, out errorCode, out errorMess);
                        if (errorCode != 200)
                            logger.Error("[URL : " + url + ", authentication : " + apiAuth + "] \r\n error code : " + errorCode.ToString() + " (" + errorMess + ") \r\n Model [" + JsonConvert.SerializeObject(canceled) + "]");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("[URL : " + url + ", authentication : " + apiAuth + "] \r\n error code : " + errorCode.ToString() + " (" + errorMess + ") \r\n Model [" + JsonConvert.SerializeObject(canceled) + "] \r\n" + ex.ToString());
                result = false;
            }
            return result;
        }
    }
}
