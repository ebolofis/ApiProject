using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.ExternalDelivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent
{
    public interface IDA_ClientJobsTasks
    {
        /// <summary>
        /// Check's if the order from DA exists and returns last order status.
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="headOrder"></param>
        /// <param name="daStore"></param>
        /// <param name="lastStatus"></param>
        /// <returns></returns>
        bool CheckIfDA_OrderExists(DBInfoModel dbInfo, DA_OrderModel headOrder, DA_StoreModel daStore, out OrderStatusEnum lastStatus);


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
        DeliveryCustomerModel UpsertCustomer(DBInfoModel dbInfo, DACustomerModel Customer, List<DA_AddressModel> Addresses, int OrderType,
            out string Error, ref GuestModel guest);

        /// <summary>
        /// Return's an invoice for specific Rxternal type and External Key (Delivery Key)
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="ExternalType"></param>
        /// <param name="ExtKey"></param>
        /// <returns></returns>
        InvoiceModel GetInvoiceFromDBForDelivery(DBInfoModel dbInfo, ExternalSystemOrderEnum ExternalType, string ExtKey, bool forCancel);


        /// <summary>
        /// Return's an order from db for specific extrnal key and type
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="ExtType"></param>
        /// <param name="ExtKey"></param>
        /// <returns></returns>
        OrderModel GetOrderFromDBUsingExternalKey(DBInfoModel dbInfo, ExternalSystemOrderEnum ExtType, string ExtKey);

        /// <summary>
        /// Return's Order Status
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        int GetLastStatusForDeliverOrder(DBInfoModel dbInfo, long OrderId);

        /// <summary>
        /// Get's Invoice Shipping for specific Invoice Id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        InvoiceShippingDetailsModel GetInvoiceShippingForSpecificInvoice(DBInfoModel dbInfo, long InvoiceId);

        /// <summary>
        /// Return's an order model to send to client to make new order
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <param name="customers"></param>
        /// <param name="extType"></param>
        /// <returns></returns>
        DA_NewOrderModel ReturnOrderDetailExternalList(DBInfoModel dbInfo, DA_OrderModel model, List<DASearchCustomerModel> customers, ExternalSystemOrderEnum extType, out string Error);


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
        bool ValidateFullOrder(DBInfoModel dbInfo, FullOrderWithTablesModel receipt, DA_OrderModel Order, long? CustomerId, ExternalSystemOrderEnum extType,
            ModifyOrderDetailsEnum ModifyOrder, out string Error, bool CheckReciptNo = true);

    }
}
