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
    public interface IDA_CustomerDT
    {
        /// <summary>
        /// Authenticate User.On failure return 0.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>CustomerId</returns>
        long LoginUser(DBInfoModel Store, DALoginModel loginModel);

        /// <summary>
        /// Authenticate User with given authToken. On failure return 0.
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="authToken">authToken</param>
        /// <returns></returns>
        long LoginUser(DBInfoModel Store, string authToken);

        /// <summary>
        /// return the number of appearances of an email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        int getEmailCount(DBInfoModel Store, string email);


        /// <summary>
        /// return true if mobile exists into an active customer (isDeleted=0)
        /// </summary>
        /// <param name="mobile">mobile</param>
        /// <returns></returns>
        bool mobileExists(DBInfoModel Store, string mobile);

        /// <summary>
        /// return true if email exists into an active customer (isDeleted=0)
        /// </summary>
        /// <param name="email">email</param>
        /// <returns></returns>
        bool emailExists(DBInfoModel Store, string email);


        /// <summary>
        /// Search Customers 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="search"></param>
        /// <returns>List of customers + addresses </returns>
        List<DASearchCustomerModel> SearchCustomers(DBInfoModel Store, DA_CustomerSearchTypeEnum type, string search);

        /// <summary>
        /// get Customer by id
        /// </summary>
        /// <param name="id">Da_Customers.Id</param>
        /// <returns></returns>
        DACustomerModel GetCustomer(DBInfoModel Store, long id);


        /// <summary>
        /// Gets customers with given email and mobile
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="email"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        List<DACustomerModel> GetCustomersByEmailMobile(DBInfoModel Store, string email, string mobile);

        /// <summary>
        /// Get Customer by ExtId1 or ExtId2. If no customer found return null
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="ExtId">ExtId1 or ExtId2</param>
        /// <param name="type">1 : ExtId1, 2 : ExtId2</param>
        /// <returns></returns>
        DACustomerModel GetCustomer(DBInfoModel Store, string ExtId, int type = 1);


        /// <summary>
        /// Search for a UNIQUE match into customers based on (lastName+firstName+Phone1), then based on (lastName+firstName+Phone2) and finally based on (lastName+firstName+Mobile).
        /// If the results are NOT UNIQUE (more than 1 records found) return null;
        /// 
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="lastName">lastName</param>
        /// <param name="firstName">firstName</param>
        /// <param name="tel">tel</param>
        /// <returns>one DACustomerModel or null</returns>
        DACustomerModel GetCustomerUnique(DBInfoModel Store, string lastName, string firstName, string tel);

        /// <summary>
        /// Add Customer 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        long AddCustomer(DBInfoModel Store, DACustomerModel Model);

        /// <summary>
        /// Update Customer 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        long UpdateCustomer(DBInfoModel Store, DACustomerModel Model);

        /// <summary>
        /// Delete Customer 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        long DeleteCustomer(DBInfoModel Store, long Id);

        /// <summary>
        /// Change SessionKey 
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="Id">CustomerId</param>
        ///  <param name="SessionKey">the new SessionKey</param>
        /// <returns></returns>
        void UpdateSessionKey(DBInfoModel Store, long Id, string SessionKey);

        /// <summary>
        /// Authenticate User based on email and SessionKey 
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="email"></param>
        /// <param name="SessionKey">SessionKey</param>
        /// <returns>CustomerId</returns>
        long LoginUserSessionKey(DBInfoModel Store, DALoginSessionKeyModel loginModel);


        /// <summary>
        /// Change Password (also change SessionKey="") 
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="Id">CustomerId</param>
        ///  <param name="Password">the new Password</param>
        /// <returns></returns>
        void UpdatePassword(DBInfoModel Store, long Id, string Password);


        /// <summary>
        /// Reset password of customer with Id = customerId and clear email of other customers
        /// </summary>
        /// <param name="customers"></param>
        /// <param name="customerId"></param>
        /// <param name="encryptedPassword"></param>
        void UpdateOnePasswordClearOtherEmails(DBInfoModel Store, List<DACustomerModel> customers, long customerId, string encryptedPassword);

        /// <summary>
        /// Get external id 2 of customer with given email
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        string GetExternalId2(DBInfoModel Store, string email);

        /// <summary>
        /// check if password of customer with given email exists
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        bool hasPassword(DBInfoModel Store, string email);

        /// <summary>
        /// Return first customerId for a customer with mobile and empty email. Otherwise return 0.
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        long ExistMobile(DBInfoModel Store, string mobile);


        /// <summary>
        /// Return first customerId for a customer with email and empty mobile. Otherwise return 0.
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        long ExistEmail(DBInfoModel Store, string email);


        /// <summary>
        /// Return first customerId for a Customer with email and mobile. Otherwise return 0.
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="existModel"></param>
        /// <returns></returns>
        long ExistMobileEmail(DBInfoModel Store, DACustomerIdentifyModel existModel);

        /// <summary>
        /// Updates customer with last order note
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="customerId"></param>
        /// <param name="lastOrderNote"></param>
        void UpdateLastOrderNote(DBInfoModel dbInfo, long customerId, string lastOrderNote);

        /// <summary>
        /// Returns customers with status DA_CustomerAnonymousTypeEnum.WillBeAnonymous
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        List<DACustomerModel> GetToBeAnonymousCustomers(DBInfoModel dbInfo);

    }
}
