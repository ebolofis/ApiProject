using log4net;
using Newtonsoft.Json;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Symposium.WebApi.MainLogic.Tasks.DeliveryAgent
{
    public class DA_EfoodTasks : IDA_EfoodTasks
    {
        LocalConfigurationHelper configHlp;
        IWebApiClientHelper webHlp;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        long selectedEfoodPricelist = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "Pricelist");
        long originId = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "OriginId");
        string phonePrefix = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "PhonePrefix");
        string mobilePrefix = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "MobilePrefix");
        long defaultProductId = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DefaultProductId");
        bool removeRecipeExtras = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "RemoveRecipeExtras");

        public DA_EfoodTasks(IDA_StoresDT _storeDT, LocalConfigurationHelper configHlp, IWebApiClientHelper webHlp)
        {
            this.configHlp = configHlp;
            this.webHlp = webHlp;
        }

        /// <summary>
        ///  Get the list of orders from external delivery web-apis and convert to DA_ExtDeliveryModel.
        /// </summary>
        /// <returns>the  list of orders </returns>
        public List<DA_ExtDeliveryModel> ConvertEfoodModelToDaDeliveryModel(DBInfoModel dbInfo, DA_EfoodModel Model)
        {
            List<DA_ExtDeliveryModel> daOrders = new List<DA_ExtDeliveryModel>();
            try
            {
                if (Model == null || Model.orders == null || Model.orders.Count == 0)
                    return new List<DA_ExtDeliveryModel>();

                //2. sanitize orders
                Model = SanitizeOrders(Model);

                //3. convert OrdersEfood to DA_ExtDeliveryModel
                foreach (Da_EfoodOrderModel eforder in Model.orders)
                {
                    if (CheckNulls(eforder))
                    {
                        DA_ExtDeliveryModel extOrder = new DA_ExtDeliveryModel();

                        //1. set da customer
                        extOrder.Customer = EfoodModelToDaCustomer(eforder);
                        //2. set da shipping address
                        extOrder.ShippingAddress = EfoodOrderToDaAddress(eforder);
                        //3. set billing address
                        extOrder.BillingAddress = null;
                        //4. set pricelistId, removeRecipeExtras and DefaultProductId
                        extOrder.PriceListId = selectedEfoodPricelist;             // <-- To Pricelist Id το οποίο θα χρεισημοποιηθεί για τις παραγγελίες από τη συγκεκριμένη  προέλευση (το χρησιμοποιεί το api EfoodTasks)
                        extOrder.DefaultProductId = defaultProductId;      // <-- Default ProductId (To προϊόν πρέπει να περιέχει ένα ExtrasId).<br> Αν το code του προϊόντος που έρχεται από το external delivery system δε βρίσκεται στην DB, τότε το άγνωστο προϊόν αντιστοιχίζεται με το DefaultProductId (το χρησιμοποιεί το api EfoodTasks)
                        extOrder.RemoveRecipeExtras = removeRecipeExtras;  // <-- true : στη λίστα των extras δε συμπεριλαμβάνονται όσα ανήκουν στη Συνταγή (το χρησιμοποιεί το api EfoodTasks)
                        //5. set da order
                        extOrder.Order = convertOrderToDAOrder(eforder);
                        //6. add to list
                        daOrders.Add(extOrder);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(Environment.NewLine + Environment.NewLine + "         -------  ERROR GETTING ORDERS FROM External Delivery Api ------- " + Environment.NewLine + ex.ToString() + Environment.NewLine + Environment.NewLine);
            }

            return daOrders;
        }

        /// <summary>
        /// sanitize Orders : Fix big length, zip codes, many spaces, replace '&' with 'and',...
        /// </summary>
        private DA_EfoodModel SanitizeOrders(DA_EfoodModel ordersModel)
        {
            //sanitize orders
            foreach (Da_EfoodOrderModel order in ordersModel.orders)
            {
                order.PriceListId = selectedEfoodPricelist;

                if (string.IsNullOrWhiteSpace(order.customer.postal_code)) order.customer.postal_code = "";
                order.customer.postal_code = order.customer.postal_code.Replace(" ", "").Trim();

                if (string.IsNullOrWhiteSpace(order.customer.floor)) order.customer.floor = "";
                if (order.customer.floor.Length > 200) order.customer.floor = order.customer.floor.Substring(0, 200);

                if (string.IsNullOrWhiteSpace(order.customer.address)) order.customer.address = "";
                order.customer.address = order.customer.address.Replace(" &", " and").Trim();
                if (order.customer.address.Length > 200) order.customer.address = order.customer.address.Substring(0, 200);

                if (string.IsNullOrWhiteSpace(order.customer.notes)) order.customer.notes = "";
                if (order.customer.notes.Length > 1500) order.customer.notes = order.customer.notes.Substring(0, 1500);

                if (string.IsNullOrWhiteSpace(order.customer.doorbell)) order.customer.doorbell = "";
                if (order.customer.doorbell.Length > 200) order.customer.doorbell = order.customer.doorbell.Substring(0, 200);
            }
            return ordersModel;
        }

        /// <summary>
        /// check if order has nulls. True if all are OK.
        /// </summary>
        /// <param name="order"></param>
        private bool CheckNulls(Da_EfoodOrderModel order)
        {
            if (order.products == null || order.products.Count == 0)
            {
                logger.Error("External Delivery Web-api: Order " + order.id + " has empty (null) Products.");
                return false;
            }
            if (order.customer == null)
            {
                logger.Error("External Delivery Web-api : Order " + order.id + " has empty (null) Customer.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// convert an OrderEfood to  DA_OrderModel
        /// </summary>
        /// <param name="orderEfood">OrderEfood</param>
        /// <returns></returns>
        private DA_OrderExtDeliveryModel convertOrderToDAOrder(Da_EfoodOrderModel orderef)
        {
            //1. make a new DA_Order
            DA_OrderExtDeliveryModel order = new DA_OrderExtDeliveryModel();
            try
            {
                order.LogicErrors = "";
                order.ExtDeliveryErrors = "";

                //2. set origin
                order.ExtDeliveryOrigin = orderef.brand;
                switch (orderef.brand)
                {
                    case "efood":
                        order.Origin = 3;
                        break;
                    case "ClickDelivery":
                        order.Origin = 4;
                        break;
                    case "Deliveras":
                        order.Origin = 5;
                        break;
                    default:
                        order.Origin = 3;
                        break;
                }

                //3. SET the ExtId1
                switch (orderef.brand)
                {
                    case "efood":
                        order.ExtId1 = orderef.id + "EF";
                        break;
                    case "ClickDelivery":
                        order.ExtId1 = orderef.id + "CD";
                        break;
                    case "Deliveras":
                        order.ExtId1 = orderef.id + "DE";
                        break;
                    default:
                        order.ExtId1 = orderef.id + "EF";
                        break;
                }

                //4.  SET flags 
                order.ReduceRecipeQnt = true;   // <-- ADD prices to extras and REDUCE prices from main products. For an e-food order. see: EffodTasks.MatchProductsExtras. (το χρησιμοποιεί το api EfoodTasks)
                order.AddLoyaltyPoints = false;

                //5. Convert efood order model to DA_OrderExtDeliveryModel
                if (orderef.payment_type == "cash")
                {
                    order.AccountType = 1;
                    order.IsPaid = false;
                }
                else
                {
                    order.AccountType = 4;
                    order.IsPaid = true;
                }
                order.StoreId = orderef.restaurant.id;
                order.StoreCode = orderef.restaurant.vendor_external_id;
                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("E. Europe Standard Time");
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(orderef.timestamp, cstZone);
                order.OrderDate = localTime; //orderef.timestamp;
                order.CustomerId = 0;// orderef.Customer.posCustomerId;
                DateTime localPromiseTime = TimeZoneInfo.ConvertTimeFromUtc(orderef.promised_customer_timestamp, cstZone);
                order.EstBillingDate = localPromiseTime; // orderef.promised_customer_timestamp;
                order.EstTakeoutDate = localPromiseTime; // orderef.promised_customer_timestamp;
                order.TelephoneNumber = orderef.customer.telephone;

                order.ExtType = 5;

                order.IsDelay = false;
                order.IsSend = 1;
                order.ItemsChanged = false;

                order.Remarks = orderef.customer.notes + " TIP: " + orderef.tip;

                if (orderef.type == "delivery")
                    order.OrderType = Symposium.Models.Enums.OrderTypeStatus.Delivery; // delivery=20
                else
                    order.OrderType = Symposium.Models.Enums.OrderTypeStatus.TakeOut; // take-out=21

                order.PointsGain = 0;
                order.PointsRedeem = 0;

                order.ShippingAddressId = 0;// orderef.Customer.posShippingAddressID;
                order.Status = 0;
                order.InvoiceType = 1;
                decimal tmpDiscount = 0;
                string DiscountDescr = "";
                foreach (Da_EfoodDiscountModel discount in orderef.discounts)
                {
                    tmpDiscount = tmpDiscount + discount.amount;
                    DiscountDescr += ", " + discount.type;
                }
                order.Discount = tmpDiscount; // orderef.OrderBody.discount;  ----> NA RWTHSW
                order.DiscountRemark = DiscountDescr;
                order.Total = orderef.price - order.Discount; //Math.Min(orderef.OrderBody.grandTotal, orderef.OrderBody.total);//Συνολικό ποσό μετά την έκπτωση με ΦΠΑ.  Total = Price – Discount
                order.Price = orderef.price; //order.Total + order.Discount; //Συνολικό ποσό πριν την έκπτωση με ΦΠΑ
                order.ExtData = JsonConvert.SerializeObject(orderef);

                //6. add items
                order.Details = new List<DA_OrderDetails>();
                foreach (var item in orderef.products)
                {

                    DA_OrderDetails product = new DA_OrderDetails();
                    if (item.id == "no-valid-code-found")
                    {
                        foreach (Da_EfoodmaterialModel material in item.materials)
                        {
                            if (material.id.StartsWith("#"))
                            {
                                item.id = material.id.Substring(1);
                            }
                        }
                    }
                    product.Id = 0;
                    product.ProductCode = item.id.ToString();
                    //product.ProductId = Convert.ToInt64(item.id);
                    product.DAOrderId = 0;
                    product.Description = item.name;
                    product.PriceListId = selectedEfoodPricelist;
                    product.Qnt = item.quantity;
                    product.ItemRemark = item.notes;
                    if (product.ItemRemark != null && product.ItemRemark.Length > 150) product.ItemRemark = product.ItemRemark.Substring(0, 150);
                    product.Price = item.price;//Συνολικό ποσό μονάδας (πριν την έκπτωση).  Αυτό πρέπει το e-food να το εχει παρει από τον τιμοκατάλογο.
                    product.Discount = 0;
                    product.Total = item.price * item.quantity; //Συνολικό ποσό(μετά την έκπτωση) Total = (Qnt * Price) - Discount

                    //7. add extras
                    product.Extras = new List<DA_OrderDetailsExtrasModel>();
                    if (item.materials != null)
                    {
                        foreach (Da_EfoodmaterialModel material in item.materials)
                        {
                            DA_OrderDetailsExtrasModel extra = new DA_OrderDetailsExtrasModel();
                            if (material.id.StartsWith("#") == false)
                            {
                                extra.Description = "";// material.PosExtrasDescription ?? "<NULL>";
                                                       //extra.ExtrasId = Convert.ToInt64(material.id);
                                extra.ExtraCode = material.id.ToString();
                                extra.ExtrasCode = material.id.ToString();
                                extra.Id = 0;
                                extra.ItemsChanged = false;
                                extra.Qnt = material.quantity; //if flag=-1 then excluded from recipe (see EfoodTasks)

                                product.Extras.Add(extra);
                            }
                        }
                    }

                    order.Details.Add(product);
                }
            }
            catch (Exception ex)
            {
                order.ExtDeliveryErrors = "Error Convert Efood Order To DAOrder : " + ex.ToString();
                logger.Error("Error Convert Efood Order To DAOrder : " + ex.ToString());
                return order;
            }
            return order;
        }

        /// <summary>
        /// Convert EfoodOrder Model To DACustomerModel. 
        /// On error return null DACustomerModel
        /// </summary>
        /// <param name="order">Order Efood Model</param>
        /// <returns></returns>
        public DACustomerModel EfoodModelToDaCustomer(Da_EfoodOrderModel order)
        {
            DACustomerModel cust = new DACustomerModel();
            try
            {
                cust.Email = null;
                //3. SET Order Customer ExtId1
                switch (order.brand)
                {
                    case "efood":
                        cust.ExtId1 = order.customer.id + "EF";
                        break;
                    case "ClickDelivery":
                        cust.ExtId1 = order.customer.id + "CD";
                        break;
                    case "Deliveras":
                        cust.ExtId1 = order.customer.id + "DE";
                        break;
                    default:
                        cust.ExtId1 = order.customer.id + "EF";
                        break;
                }
                cust.ExtType = ExternalSystemOrderEnum.DeliveryAgent;
                cust.FirstName = order.customer.name;
                cust.LastName = order.customer.surname;
                cust.LastOrderNote = "";
                cust.Notes = order.customer.notes;

                if (!string.IsNullOrWhiteSpace(order.customer.telephone) && (order.customer.telephone.StartsWith(mobilePrefix) || order.customer.telephone.StartsWith(phonePrefix + mobilePrefix)))
                {
                    cust.Mobile = order.customer.telephone.Trim();
                    cust.Phone1 = null;
                }
                else
                {
                    cust.Phone1 = order.customer.telephone.Trim();
                    cust.Mobile = null;
                }
                cust.SendEmail = false;
                cust.SendSms = false;
                cust.GTPR_Marketing = false;
                cust.GTPR_Returner = false;
                cust.Id = 0;
                cust.IsDeleted = false;
                cust.Doy = null;
                cust.VatNo = null;
                cust.Proffesion = null;
                cust.JobName = null;
                cust.PhoneComp = null;
                cust.LastAddressId = 0;
                cust.BillingAddressesId = 0;
                cust.Loyalty = false;
            }
            catch (Exception ex)
            {
                logger.Error("Error Convert EfoodModel customer To DaCustomer : " + ex.ToString());
                return cust;
            }
            return cust;
        }

        /// <summary>
        /// Convert EfoodOrder Model To Da_AddressesMolel. 
        /// On error return null Da_AddressModel
        /// </summary>
        /// <param name="order">Order Efood Model</param>
        /// <returns></returns>
        public DA_AddressModel EfoodOrderToDaAddress(Da_EfoodOrderModel order)
        {
            DA_AddressModel addr = new DA_AddressModel();
            if (order == null)
            {
                logger.Warn("OrderEfood order Model is Empty - Cannot Convert to DA_AddressModel");
                return addr;
            }

            try
            {
                addr.OwnerId = order.customer.id;
                addr.AddressStreet = order.customer.address;
                addr.AddressNo = order.customer.street_number;
                addr.Area = order.customer.area;
                addr.Zipcode = order.customer.postal_code;
                addr.Bell = order.customer.doorbell;
                addr.ExtId1 = order.customer.address_id;
                addr.Notes = order.customer.notes;
                addr.Floor = order.customer.floor;
                addr.Longtitude = order.customer.longitude;
                addr.Latitude = order.customer.latitude;
                addr.AddressProximity = AddressProximityEnum.Unown_Proximity;
                addr.AddressType = 0;


                if (order.customer.street_number == "0")
                    addr.AddressNo = "";
                else
                    addr.AddressNo = order.customer.street_number;
                if (string.IsNullOrWhiteSpace(addr.City))
                {
                    if (!string.IsNullOrWhiteSpace(order.customer.area))
                        addr.City = order.customer.area;
                    else
                        addr.City = "";
                }

                if (string.IsNullOrWhiteSpace(addr.AddressStreet)) addr.AddressStreet = "UNKNOWN ADDRESS";
            }
            catch (Exception ex)
            {

                logger.Error("Error create DA_AddressModel from OrderEfood model : " + ex.ToString());
                return addr;
            }

            return addr;
        }

    }
}
