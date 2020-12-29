using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent
{
    public interface IDA_CustomerFlows
    {
        /// <summary>
        /// Authenticate User 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>CustomerId</returns>
        long LoginUser(DBInfoModel dbInfo, DALoginModel loginModel);

        /// <summary>
        /// Authenticate User based on authtoken
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="authtoken"></param>
        /// <returns>CustomerId</returns>
        long LoginUser(DBInfoModel dbInfo, string authtoken);

        /// <summary>
        /// Create Authorization Token for a customer based on email and mobile. Return the new AuthToken
        /// </summary>
        /// <returns></returns>
        string CreateAuthToken(DBInfoModel dbInfo, DACustomerIdentifyModel model);

        /// <summary>
        /// get customer (and his addresses) by Id
        /// </summary>
        /// <param name="Id">customer Id</param>
        /// <returns></returns>
        DACustomerExtModel getCustomer(DBInfoModel dbInfo, long Id);

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
        /// Add Customer  (without addresses)
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        long AddCustomer(DBInfoModel dbInfo, DACustomerModel Model);

        /// <summary>
        /// Add Customer (with addresses), return the same model with  Ids (customer and addresses)
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        DACustomerExtModel AddCustomer(DBInfoModel dbInfo, DACustomerExtModel Model);

        /// <summary>
        /// Update Customer 
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="fromWeb">true: during update keep the existing value for SecretNotes AND ensure that email is required </param>
        /// <param name="CustomerId">if fromWeb=true then CustomerId is Customer Id from authorization header</param>
        /// <returns></returns>
        long UpdateCustomer(DBInfoModel dbInfo, DACustomerModel Model, bool fromWeb = false, long CustomerId = 0);

        /// <summary>
        /// Delete Customer 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        long DeleteCustomer(DBInfoModel dbInfo, long Id);


        /// <summary>
        /// User has forgot his password. Application create and return a session key
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="model">model containing the customer's email</param>
        /// <returns></returns>
        string ForgotPassword(DBInfoModel dbInfo, ForgotPasswordModel model);




        /// <summary>
        /// Change password based on email and Session key
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="model"></param>
        void ChangePasswordSessionKey(DBInfoModel dbInfo, DALoginSessionKeyModel model);



        /// <summary>
        /// Change password based on email and old password
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="model">DAchangePasswordModel</param>
        /// <param name="CustomerId">Customer Id from Auth Model</param>
        void ChangePassword(DBInfoModel dbInfo, DAchangePasswordModel model, long CustomerId);


        /// <summary>
        /// Resets password of FIRST customer with given email and mobile and clears email of other customers with given email and mobile
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        void ResetPassword(DBInfoModel dbInfo, DAresetPasswordModel model);

        /// <summary>
        /// Get external id 2 of customer and if there is password with given email
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        ExternalId2PasswordModel GetExternalId2(DBInfoModel dbInfo, GetExternalIdModel model);


        /// <summary>
        /// Identify customer based on mobile and/or email.  
        /// Return Dictionary of int, long: 
        ///   
        ///   Key:
        ///          3 = customer found with specific/not null values for email and mobile. 
        ///          2 = customer found with specific email and empty mobile. 
        ///          1 = customer found with specific mobile and empty email. 
        ///          0 = NO customer found with specific/not null values for email and mobile. 
        ///          
        ///  Value:  The Customer Id. In case of NO customer found return CustomerId=0.
        /// 
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="existModel"></param>
        /// <returns></returns>
        Dictionary<int, long> IdentifyCustomer(DBInfoModel Store, DACustomerIdentifyModel existModel);


        /// <summary>
        /// Get customer info from 3rd party source
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        DACustomerModel GetCustomerInfoExternal(DBInfoModel Store, string phone);
    }
}
