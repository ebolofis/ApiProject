using Symposium.Models.Models;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DAO
{
    public interface IDeliveryCustomersDAO
    {
        DeliveryCustomerLookupModel GetLookups(IDbConnection db);
        /// <summary>
        /// Select Customer By Provided Id and Connection
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Id">Delivery_Customer Id</param>
        /// <returns> Single DTO of Delivery Customer </returns>
        Delivery_CustomersDTO SelectById(IDbConnection db, long Id);
        Delivery_CustomersDTO SelectExternalIdType(IDbConnection db, string ExtId, int? ExType);

        void UpdateCustomerPhones(IDbConnection db, List<DeliveryCustomersPhonesModel> modelPhones, long CustId);
        void UpdateBillindAddresses(IDbConnection db, List<DeliveryCustomersBillingAddressModel> addresses, long CustId);

        void UpdateShippingAddresses(IDbConnection db, List<DeliveryCustomersShippingAddressModel> addresses, long CustId);


        void UpsertSearchCustomerPhones(IDbConnection db, List<DeliveryCustomersPhonesModel> modelPhones, long CustId);
        void UpsertSearchBillindAddresses(IDbConnection db, List<DeliveryCustomersBillingAddressModel> addresses, long CustId);
        void UpsertSearchShippingAddresses(IDbConnection db, List<DeliveryCustomersShippingAddressModel> addresses, long CustId);
    }
}
