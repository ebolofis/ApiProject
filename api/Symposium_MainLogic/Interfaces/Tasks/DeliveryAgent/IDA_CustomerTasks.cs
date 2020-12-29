using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent
{
    public interface IDA_CustomerTasks
    {
        /// <summary>
        /// Authenticate User. On failure return 0. 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>CustomerId</returns>
        long LoginUser(DBInfoModel dbInfo, DALoginModel loginModel);

        /// <summary>
        /// Authenticate User with given authToken. On failure return 0.
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="authToken">authToken</param>
        /// <returns></returns>
        long LoginUser(DBInfoModel dbInfo, string authToken);

        /// <summary>
        /// check if the email exists in DB. If so then throw exception
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        void CheckUniqueEmail(DBInfoModel dbInfo, string email);

        /// <summary>
        /// check if at least one phone exists to customer model
        /// </summary>
        /// <param name="customer"></param>
        void CheckPhoneExistanse(IDA_CustomerModel customer);

        /// <summary>
        /// return true if mobile exists into an active customer (isDeleted=0)
        /// </summary>
        /// <param name="mobile">mobile</param>
        /// <returns></returns>
        bool mobileExists(DBInfoModel dbInfo, string mobile);

        /// <summary>
        /// return true if email exists into an active customer (isDeleted=0)
        /// </summary>
        /// <param name="email">email</param>
        /// <returns></returns>
        bool emailExists(DBInfoModel dbInfo, string email);

        /// <summary>
        /// Search Customers 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="search"></param>
        /// <returns>List of customers + addresses </returns>
        List<DASearchCustomerModel> SearchCustomers(DBInfoModel dbInfo, DA_CustomerSearchTypeEnum type, string search);


        /// <summary>
        /// get Customer by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DACustomerModel GetCustomer(DBInfoModel dbInfo, long id);


        /// <summary>
        /// Gets customers with given email and mobile
        /// </summary>
        /// <param name="dbInf"></param>
        /// <param name="email"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        List<DACustomerModel> GetCustomersByEmailMobile(DBInfoModel dbInfo, string email, string mobile);

        /// <summary>
        /// Create Authorization Token
        /// </summary>
        /// <returns></returns>
        string CreateAuthToken();

        

        /// <summary>
        /// Add Customer 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        long AddCustomer(DBInfoModel dbInfo, DACustomerModel Model);

        /// <summary>
        /// Update Customer 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        long UpdateCustomer(DBInfoModel dbInfo, DACustomerModel Model);

        /// <summary>
        /// Delete Customer 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        long DeleteCustomer(DBInfoModel dbInfo, long Id);

        /// <summary>
        /// sanitize customer for insert
        /// </summary>
        /// <param name="customer"></param>
        void SanitizeInsertCustomer(IDA_CustomerModel customer);

        /// <summary>
        /// sanitize customer for update
        /// </summary>
        /// <param name="customer"></param>
        void SanitizeUpdateCustomer(IDA_CustomerModel customer);

        /// <summary>
        /// Change SessionKey 
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="Id">CustomerId</param>
        ///  <param name="SessionKey">the new SessionKey</param>
        /// <returns></returns>
        void UpdateSessionKey(DBInfoModel dbInfo, long Id, string SessionKey);

        /// <summary>
        /// Create SessionKey 
        /// </summary>
        /// <returns></returns>
        string CreateSessionKey();

        /// <summary>
        /// Authenticate User based on email and SessionKey 
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="email"></param>
        /// <param name="SessionKey">SessionKey</param>
        /// <returns>CustomerId</returns>
        long LoginUserSessionKey(DBInfoModel dbInfo, DALoginSessionKeyModel loginModel);


        /// <summary>
        /// Change Password (also change SessionKey="") 
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="Id">CustomerId</param>
        ///  <param name="Password">the new Password</param>
        /// <returns></returns>
        void UpdatePassword(DBInfoModel dbInfo, long Id, string Password);


        /// <summary>
        /// Reset password of customer with Id = customerId and clear email of other customers
        /// </summary>
        /// <param name="customers"></param>
        /// <param name="customerId"></param>
        /// <param name="newPassword"></param>
        void UpdateOnePasswordClearOtherEmails(DBInfoModel Store, List<DACustomerModel> customers, long customerId, string newPassword);

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
        /// Get customer info from 3rd party source by phone number
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="phone"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        DACustomerModel GetCustomerInfoExternalByPhone(DBInfoModel dbInfo, string phone, Dictionary<string, dynamic> configuration);

        /// <summary>
        /// Updates customer with last order note
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="customerId"></param>
        /// <param name="lastOrderNote"></param>
        void UpdateLastOrderNote(DBInfoModel dbInfo, long customerId, string lastOrderNote);

        /// <summary>
        /// Makes DA_Customers anonymous. Only customers with status DA_CustomerAnonymousTypeEnum.WillBeAnonymous will become anonymous
        /// </summary>
        /// <param name="dbInfo"></param>
        void MakeCustomerAnonymous(DBInfoModel dbInfo);

        /// <summary>
        /// Sanitizes customer properties
        /// </summary>
        /// <param name="customer"></param>
        void SanitizeCustomer(DACustomerModel customer);

    }
}
