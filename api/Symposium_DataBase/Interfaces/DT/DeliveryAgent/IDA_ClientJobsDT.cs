using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent
{
    public interface IDA_ClientJobsDT
    {
        /// <summary>
        /// Check's if the order from DA exists and returns last order status.
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="headOrder"></param>
        /// <param name="lastStatus"></param>
        /// <returns></returns>
        bool CheckIfDA_OrderExists(DBInfoModel Store, DA_OrderModel headOrder, DA_StoreModel daStore, out OrderStatusEnum lastStatus);

        /// <summary>
        /// Based on Delivery Agents order details 
        /// on WebPOS DB creates a receipt detail list and applies items based on recipe code
        /// Method also checks if vat is a valid pos ent, product and price list else it returns errors depented on missing property
        /// 
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="order"></param>
        /// <param name="vmModel"></param>
        /// <param name="PosInfoDetail"></param>
        /// <param name="InvoiceType"></param>
        /// <returns></returns>
        List<ReceiptDetails> CreateReceiptDetailsFromProductCodesForDA(DBInfoModel Store, DA_NewOrderModel order, List<VatModel> vmModel,
            PosInfoDetailModel PosInfoDetail, InvoiceTypeModel InvoiceType, out string ErrorMess);

        /// <summary>
        /// Return's an invoice for specific Rxternal type and External Key (Delivery Key)
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="ExternalType"></param>
        /// <param name="ExtKey"></param>
        /// <returns></returns>
        InvoiceModel GetInvoiceFromDBForDelivery(DBInfoModel Store, ExternalSystemOrderEnum ExternalType, string ExtKey, bool forCancel);

        /// <summary>
        /// Return's an order from db for specific extrnal key and type
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="ExtType"></param>
        /// <param name="ExtKey"></param>
        /// <returns></returns>
        OrderModel GetOrderFromDBUsingExternalKey(DBInfoModel Store, ExternalSystemOrderEnum ExtType, string ExtKey);

        /// <summary>
        /// Return's Order Status
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        int GetLastStatusForDeliverOrder(DBInfoModel Store, long OrderId);

        /// <summary>
        /// Get's Invoice Shipping for specific Invoice Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        InvoiceShippingDetailsModel GetInvoiceShippingForSpecificInvoice(DBInfoModel Store, long InvoiceId);

        /// <summary>
        /// Check Customer and Address and Phones if Exists and Insert's Or Update's Data
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Customer"></param>
        /// <param name="Addresses"></param>
        /// <param name="OrderType"></param>
        /// <param name="Error"></param>
        /// <param name="guest"></param>
        /// <returns></returns>
        DeliveryCustomerModel UpsertCustomer(DBInfoModel Store, DACustomerModel Customer, List<DA_AddressModel> Addresses, int OrderType,
            out string Error, ref GuestModel guest);


        /// <summary>
        /// Return's an order model to send to client to make new order
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <param name="customers"></param>
        /// <param name="extType"></param>
        /// <returns></returns>
        DA_NewOrderModel ReturnOrderDetailExternalList(DBInfoModel Store, DA_OrderModel model, List<DASearchCustomerModel> customers, ExternalSystemOrderEnum extType, out string Error);


    }
}
