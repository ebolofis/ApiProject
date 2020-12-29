using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.ExternalDelivery;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.DeliveryAgent
{
    public class DA_ClientJobsFlows : IDA_ClientJobsFlows
    {
        IDA_ClientJobsTasks task;
        IDA_OrdersTasks daOrderTask;
        IDA_StoresFlows storeFlow;
        IDelivery_CustomersShippingAddressFlows delivShipFlow;
        IDelivery_CustomersBillingAddressFlows deilivBillAFlow;

        public DA_ClientJobsFlows(IDA_ClientJobsTasks task, IDA_OrdersTasks daOrderTask,
            IDA_StoresFlows storeFlow, IDelivery_CustomersShippingAddressFlows delivShipFlow,
            IDelivery_CustomersBillingAddressFlows deilivBillAFlow)
        {
            this.task = task;
            this.daOrderTask = daOrderTask;
            this.storeFlow = storeFlow;
            this.delivShipFlow = delivShipFlow;
            this.deilivBillAFlow = deilivBillAFlow;
        }

        /// <summary>
        /// Check's if the order from DA exists and returns last order status.
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="headOrder"></param>
        /// <param name="daStore"></param>
        /// <param name="lastStatus"></param>
        /// <returns></returns>
        public bool CheckIfDA_OrderExists(DBInfoModel dbInfo, DA_OrderModel headOrder, DA_StoreModel daStore, out OrderStatusEnum lastStatus)
        {
            return task.CheckIfDA_OrderExists(dbInfo, headOrder, daStore, out lastStatus);
        }

        /// <summary>
        /// Check Customer and Address and Phones if Exists and Insert's Or Update's Data
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="Customer"></param>
        /// <param name="Addresses"></param>
        /// <param name="OrderType"></param>
        /// <param name="Error"></param>
        /// <param name="guest"></param>
        /// <returns></returns>
        public DeliveryCustomerModel UpsertCustomer(DBInfoModel dbInfo, DACustomerModel Customer, List<DA_AddressModel> Addresses, int OrderType,
            out string Error, ref GuestModel guest)
        {
            return task.UpsertCustomer(dbInfo, Customer, Addresses, OrderType, out Error, ref guest);
        }


        /// <summary>
        /// Return's an invoice for specific Rxternal type and External Key (Delivery Key)
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="ExternalType"></param>
        /// <param name="ExtKey"></param>
        /// <returns></returns>
        public InvoiceModel GetInvoiceFromDBForDelivery(DBInfoModel dbInfo, ExternalSystemOrderEnum ExternalType, string ExtKey, bool forCancel)
        {
            return task.GetInvoiceFromDBForDelivery(dbInfo, ExternalType, ExtKey, forCancel);
        }


        /// <summary>
        /// Return's an order from db for specific extrnal key and type
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="ExtType"></param>
        /// <param name="ExtKey"></param>
        /// <returns></returns>
        public OrderModel GetOrderFromDBUsingExternalKey(DBInfoModel dbInfo, ExternalSystemOrderEnum ExtType, string ExtKey)
        {
            return task.GetOrderFromDBUsingExternalKey(dbInfo, ExtType, ExtKey);
        }

        /// <summary>
        /// Return's Order Status
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public int GetLastStatusForDeliverOrder(DBInfoModel dbInfo, long OrderId)
        {
            return task.GetLastStatusForDeliverOrder(dbInfo, OrderId);
        }

        /// <summary>
        /// Get's Invoice Shipping for specific Invoice Id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        public InvoiceShippingDetailsModel GetInvoiceShippingForSpecificInvoice(DBInfoModel dbInfo, long InvoiceId)
        {
            return task.GetInvoiceShippingForSpecificInvoice(dbInfo, InvoiceId);
        }

        /// <summary>
        /// Check's table Invoice, InvoiceShippingDetail and Order to find if any field has changed and return's new objects and boolean if need update any of them
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="forder"></param>
        /// <param name="lookups"></param>
        /// <param name="order"></param>
        /// <param name="Customer"></param>
        /// <param name="Addresses"></param>
        /// <param name="chngOrder"></param>
        /// <param name="chOrder"></param>
        /// <param name="inv"></param>
        /// <param name="chngInv"></param>
        /// <param name="chInv"></param>
        /// <param name="invShp"></param>
        /// <param name="chngInvShip"></param>
        /// <param name="chInvShp"></param>
        /// <param name="customer"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public bool CheckAndReturnOrderForUpdate(DBInfoModel dbInfo, DA_NewOrderModel forder, ForkeyLookups lookups,
                OrderModel order, DACustomerModel Customer, List<DA_AddressModel> Addresses, out bool chngOrder, out OrderModel chOrder,
                InvoiceModel inv, out bool chngInv, out InvoiceModel chInv,
                InvoiceShippingDetailsModel invShp, out bool chngInvShip, out InvoiceShippingDetailsModel chInvShp,
                out DeliveryCustomerModel customer, out string Error)
        {
            bool ret = true;
            chngOrder = false;
            chOrder = order;
            chngInv = false;
            chInv = inv;
            chngInvShip = false;
            chInvShp = invShp;
            customer = null;
            Error = "";
            try
            {
                //Update's customer on Delivery tables.....
                GuestModel guestModel = new GuestModel();

                customer = UpsertCustomer(dbInfo, Customer, Addresses, (int) forder.OrderType, out Error, ref guestModel);
                if (customer == null)
                    return false;

                long PosInfoDetailId = lookups.PosInfoDetailList.Find(f => f.PosInfoId == forder.PosId && f.InvoicesTypeId == forder.InvoiceType).Id;


                chngOrder = (order.Total != forder.Total || order.PosId != forder.PosId || order.StaffId != forder.PosStaffId || order.TotalBeforeDiscount != forder.Price);
                if (chngOrder)
                {
                    chOrder.Total = forder.Total;
                    chOrder.TotalBeforeDiscount = forder.Price;
                    chOrder.PosId = forder.PosId;
                    chOrder.StaffId = forder.PosStaffId;
                }
                chngInv = (inv.StaffId != forder.PosStaffId || inv.Total != forder.Total || inv.PosInfoDetailId != PosInfoDetailId ||
                           inv.Vat != forder.TotalVat || inv.Tax != forder.TotalTax || inv.Net != forder.NetAmount);
                if (chngInv)
                {
                    chInv.StaffId = forder.PosStaffId;
                    chInv.Total = forder.Total;
                    chInv.PosInfoDetailId = PosInfoDetailId;
                    chInv.Vat = forder.TotalVat;
                    chInv.Tax = forder.TotalTax;
                    chInv.Net = forder.NetAmount;
                }
                DA_AddressModel shipAdr = Addresses.Find(f => f.isShipping == true);
                DA_AddressModel billAdr = Addresses.Find(f => f.isShipping != true);

                Delivery_CustomersShippingAddressModel delivShipAddr = new Delivery_CustomersShippingAddressModel();
                Delivery_CustomersBillingAddressModel delivBillAddr = new Delivery_CustomersBillingAddressModel();

                if (shipAdr != null)
                    delivShipAddr = delivShipFlow.GetModelByExternalKey(dbInfo, customer.ID, shipAdr.Id.ToString(), (ExternalSystemOrderEnum)forder.ExtType);
                if (billAdr != null)
                    delivBillAddr = deilivBillAFlow.GetModelByExternalKey(dbInfo, customer.ID, billAdr.Id.ToString(), (ExternalSystemOrderEnum)forder.ExtType);

                chngInvShip = ((invShp == null && forder.OrderType != OrderTypeStatus.TakeOut) ||
                                invShp.CustomerName != forder.CustomerData.daCustomerModel.LastName + " " + forder.CustomerData.daCustomerModel.FirstName ||
                                invShp.ShippingAddressId != delivShipAddr.ID ||
                                invShp.ShippingAddress != delivShipAddr.AddressStreet + " " + delivShipAddr.AddressNo ||
                                invShp.ShippingCity != delivShipAddr.City ||
                                invShp.ShippingZipCode != delivShipAddr.Zipcode ||
                                invShp.Floor != delivShipAddr.Floor ||
                                invShp.Latitude != double.Parse(string.IsNullOrEmpty(delivShipAddr.Latitude) ? "0" : delivShipAddr.Latitude) ||
                                invShp.Longtitude != double.Parse(string.IsNullOrEmpty(delivShipAddr.Longtitude) ? "0" : delivShipAddr.Longtitude) ||
                                invShp.BillingAddressId != delivBillAddr.ID ||
                                invShp.BillingAddress != delivBillAddr.AddressStreet + " " + delivBillAddr.AddressNo ||
                                invShp.BillingCity != delivBillAddr.City ||
                                invShp.BillingZipCode != delivBillAddr.Zipcode ||
                                invShp.BillingName != forder.CustomerData.daCustomerModel.LastName + " " + forder.CustomerData.daCustomerModel.FirstName ||
                                invShp.BillingVatNo != forder.CustomerData.daCustomerModel.VatNo ||
                                invShp.BillingDOY != forder.CustomerData.daCustomerModel.Doy ||
                                invShp.BillingJob != forder.CustomerData.daCustomerModel.JobName ||
                                invShp.CustomerRemarks != forder.CustomerData.daCustomerModel.Notes ||
                                ((invShp.Phone == forder.CustomerData.daCustomerModel.Phone1 || invShp.Phone == forder.CustomerData.daCustomerModel.Phone2 || invShp.Phone == forder.CustomerData.daCustomerModel.Mobile) ? false : true)
                                 );
                if (chngInvShip)
                {
                    if (chInvShp == null)
                    {
                        chInvShp = new InvoiceShippingDetailsModel();
                        chInvShp.InvoicesId = inv.Id;
                    }
                    chInvShp.CustomerName = forder.CustomerData.daCustomerModel.LastName + " " + forder.CustomerData.daCustomerModel.FirstName;
                    chInvShp.ShippingAddressId = delivShipAddr.ID;
                    chInvShp.ShippingAddress = delivShipAddr.AddressStreet + " " + delivShipAddr.AddressNo;
                    chInvShp.ShippingCity = delivShipAddr.City;
                    chInvShp.ShippingZipCode = delivShipAddr.Zipcode;
                    chInvShp.Floor = delivShipAddr.Floor;
                    chInvShp.Latitude = double.Parse(string.IsNullOrEmpty(delivShipAddr.Latitude) ? "0" : delivShipAddr.Latitude);
                    chInvShp.Longtitude = double.Parse(string.IsNullOrEmpty(delivShipAddr.Longtitude) ? "0" : delivShipAddr.Longtitude);
                    chInvShp.BillingAddressId = delivBillAddr.ID;
                    chInvShp.BillingAddress = delivBillAddr.AddressStreet + " " + delivBillAddr.AddressNo;
                    chInvShp.BillingCity = delivBillAddr.City;
                    chInvShp.BillingZipCode = delivBillAddr.Zipcode;
                    chInvShp.BillingName = forder.CustomerData.daCustomerModel.LastName + " " + forder.CustomerData.daCustomerModel.FirstName;
                    chInvShp.BillingVatNo = forder.CustomerData.daCustomerModel.VatNo;
                    chInvShp.BillingDOY = forder.CustomerData.daCustomerModel.Doy;
                    chInvShp.BillingJob = forder.CustomerData.daCustomerModel.JobName;
                    chInvShp.CustomerRemarks = forder.CustomerData.daCustomerModel.Notes;
                    if (invShp.Phone != forder.CustomerData.daCustomerModel.Phone1 && invShp.Phone != forder.CustomerData.daCustomerModel.Phone2 && invShp.Phone == forder.CustomerData.daCustomerModel.Mobile)
                    {
                        chInvShp.Phone = forder.CustomerData.daCustomerModel.Phone1;
                        if (string.IsNullOrEmpty(chInvShp.Phone))
                            chInvShp.Phone = forder.CustomerData.daCustomerModel.Phone2;
                        if (string.IsNullOrEmpty(chInvShp.Phone))
                            chInvShp.Phone = forder.CustomerData.daCustomerModel.Mobile;
                    }
                }
            }
            catch (Exception ex)
            {
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// Check's if Order Items has been changed
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="Model"></param>
        /// <returns></returns>
        public bool CheckOrderItemsForChanges(DBInfoModel dbInfo, DA_OrderModel Model)
        {
            return daOrderTask.CheckOrderItemsForChanges(dbInfo, Model);
        }

        /// <summary>
        /// Return's an order model to send to client to make new order
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <param name="customers"></param>
        /// <param name="extType"></param>
        /// <returns></returns>
        public DA_NewOrderModel ReturnOrderDetailExternalList(DBInfoModel dbInfo, DA_OrderModel model, List<DASearchCustomerModel> customers, 
            ExternalSystemOrderEnum extType, out string Error)
        {
            return task.ReturnOrderDetailExternalList(dbInfo, model, customers, extType, out Error);
        }

        /// <summary>
        /// Checks A FullOrderWithTablesModel before post
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="receipt"></param>
        /// <param name="Order"></param>
        /// <param name="CustomerId"></param>
        /// <param name="extType"></param>
        /// <param name="ModifyOrder"></param>
        /// <param name="Error"></param>
        /// <param name="CheckReciptNo"></param>
        /// <returns></returns>
        public bool ValidateFullOrder(DBInfoModel dbInfo, FullOrderWithTablesModel receipt, DA_OrderModel Order, long? CustomerId, ExternalSystemOrderEnum extType,
            ModifyOrderDetailsEnum ModifyOrder, out string Error, bool CheckReciptNo = true)
        {
            return task.ValidateFullOrder(dbInfo, receipt, Order, CustomerId, extType, ModifyOrder, out Error, CheckReciptNo);
        }

        /// <summary>
        /// Check Client Store Status and Order Type Status.
        /// Return true if Store can accept order type status
        /// </summary>
        /// <param name="StoreStatus"></param>
        /// <param name="OrderType"></param>
        /// <returns></returns>
        public bool CheckClientStoreOrderStatus(DBInfoModel dbInfo, long StoreId, OrderTypeStatus OrderType)
        {
            DA_StoreModel store = storeFlow.GetStoreById(dbInfo, StoreId);

            if (store == null)
                return false;

            bool result = true;
            if ((DAStoreStatusEnum)store.StoreStatus == DAStoreStatusEnum.Closed)
                result = false;
            else if ((DAStoreStatusEnum)store.StoreStatus == DAStoreStatusEnum.DeliveryOnly && OrderType != OrderTypeStatus.Delivery)
                result = false;
            else if ((DAStoreStatusEnum)store.StoreStatus == DAStoreStatusEnum.TakeoutOnly && OrderType != OrderTypeStatus.TakeOut)
                result = false;
            else result = (DAStoreStatusEnum)store.StoreStatus == DAStoreStatusEnum.FullOperational;

            return result;
        }
    }
}
