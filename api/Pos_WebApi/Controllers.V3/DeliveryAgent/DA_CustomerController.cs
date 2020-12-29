using Pos_WebApi.Modules;
using Symposium.Helpers;
using Symposium.Models.Enums;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.DeliveryAgent
{
    [RoutePrefix("api/v3/da/Customers")]
    public class DA_CustomerController : BasicV3Controller
    {
        IDA_CustomerFlows customerFlow;

        public DA_CustomerController(IDA_CustomerFlows _customerFlow)
        {
            this.customerFlow = _customerFlow;
        }

        /// <summary>
        /// Authenticate User 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>CustomerId</returns>
        [AllowAnonymous]
        [HttpPost, Route("Login")]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage LoginUser(DALoginModel loginModel)
        {
            long res = customerFlow.LoginUser(DBInfo, loginModel);
            logger.Info("Successful login for customer : " + (loginModel.Email ?? "<NULL>"));
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Get a Customer (and his addresses) based on Id. 
        /// For DA only.
        /// </summary>
        /// <param name="id">customer id</param>
        /// <returns></returns>
        [HttpGet, Route("get/{Id}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetCustomer(long Id)
        {
            //allow only calls with Staff as Authorization header. This means that Web is not allowed to use this call, Web should call GetCurrentCustomer instead.
            if (CustomerId != 0 || StaffId == 0) throw new BusinessException("Invalid Call.");

            DACustomerExtModel res = customerFlow.getCustomer(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }



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
        [HttpPost, Route("identify")]
        [AllowAnonymous]
        [IsDA]
        public HttpResponseMessage IdentifyCustomer(DACustomerIdentifyModel model)
        {
            Dictionary<int, long> res = customerFlow.IdentifyCustomer(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Create Authorization Token for a customer based on email and mobile. Return the new AuthToken
        /// 
        /// </summary>
        [HttpPost, Route("CreateAuthToken")]
        [AllowAnonymous]
        [IsDA]
        [ValidateModelState]
        [CheckModelForNull]
        public HttpResponseMessage CreateAuthToken(DACustomerIdentifyModel model)
        {
            string res = customerFlow.CreateAuthToken(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        /// <summary>
        /// return true if mobile exists into an active customer (isDeleted=0)
        /// For DA only. ex: http://localhost:5420/api/v3/da/Customers/mobileExists/?mobile=+306971234567
        /// </summary>
        /// <param name="mobile">mobile</param>
        /// <returns></returns>
        [HttpGet, Route("mobileExists/")]
        [AllowAnonymous]
        [IsDA]
        public HttpResponseMessage mobileExists(string mobile)
        {
            bool res = customerFlow.mobileExists(DBInfo, mobile);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        /// <summary>
        /// return true if email exists into an active customer (isDeleted=0)
        /// For DA only. ex: http://localhost:5420/api/v3/da/Customers/emailExists/?email=nntounias@hit.com.gr
        /// </summary>
        /// <param name="email">email</param>
        /// <returns></returns>
        [HttpGet, Route("emailExists/")]
        [AllowAnonymous]
        [IsDA]
        public HttpResponseMessage emailExists(string email)
        {
            bool res = customerFlow.emailExists(DBInfo, email);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Get Current Customer (and his addresses) based on Authorization Header. 
        /// For WEB site only
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("getcurrent")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetCurrentCustomer()
        {
            DACustomerExtModel res = customerFlow.getCustomer(DBInfo, CustomerId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Search Customers
        /// type:
        /// 0: search by firstname or lastname
        /// 1: search by Address and AddressNo
        /// 2: ΑΦΜ
        /// 3: Phone1 ή Phone2 ή Mobile
        /// search: Λεκτικό αναζήτησης
        /// </summary>
        /// <param name="type"></param>
        /// <param name="search"></param>
        /// <returns>List of customers + addresses </returns>
        [HttpPost, Route("SearchCustomers")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage SearchCustomers(DACustomerSearchFilters filter)
        {
            //allow only calls with Staff or Customer as Authorization header.
            if (CustomerId == 0 && StaffId == 0) throw new BusinessException("Invalid Call");

            List<DASearchCustomerModel> res = customerFlow.SearchCustomers(DBInfo, filter.Type, filter.Value);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Delete Customer 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet, Route("Delete/Id/{Id}")]
        [Authorize]
        public HttpResponseMessage DeleteCustomer(long Id)
        {
            //allow only calls with Staff as Authorization header.
            if (CustomerId != 0 || StaffId == 0) throw new BusinessException("Invalid Call");

            long res = customerFlow.DeleteCustomer(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Delete Current Customer based on Authentication Header. 
        /// Only from WEB
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet, Route("delete/current")]
        [Authorize]
        public HttpResponseMessage DeleteCurrentCustomer()
        {
            long res = customerFlow.DeleteCustomer(DBInfo, CustomerId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Add Customer (with out addresses). 
        /// Only from DA
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("Add")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage AddCustomer(DACustomerModel Model)
        {
            //allow only calls with Staff as Authorization header.
            if (CustomerId != 0 || StaffId == 0) throw new BusinessException("Invalid Call");

            long res = customerFlow.AddCustomer(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        /// <summary>
        /// Add Customer from web (with  addresses) [NOT Authorized] 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns>user Model with Ids (customer and addresses)</returns>
        [HttpPost, Route("insert")]
        [ValidateModelState]
        [CheckModelForNull]
        [AllowAnonymous]
        public HttpResponseMessage InsertCustomer(DACustomerExtModel Model)
        {
            DACustomerExtModel res = customerFlow.AddCustomer(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        /// <summary>
        /// Update Customer (from Agent). 
        /// Note: we keep the existing  value for password!!!!
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdateCustomer")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        public HttpResponseMessage UpdateCustomer(DACustomerModel Model)
        {
            long res = customerFlow.UpdateCustomer(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        /// <summary>
        /// Update Customer (from Web). 
        ///  Note: we keep the existing  value for password!!!!
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("Update")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        public HttpResponseMessage UpdateCustomerWeb(DACustomerModel Model)
        {
            long res = customerFlow.UpdateCustomer(DBInfo, Model,true,CustomerId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Change password based on email and old password
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("loginchange")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        public HttpResponseMessage ChangePassword(DAchangePasswordModel model)
        {
             customerFlow.ChangePassword(DBInfo, model, CustomerId);
            return Request.CreateResponse(HttpStatusCode.OK);
        }


        /// <summary>
        /// User has forgot his password. Application create and return a session key
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("loginchangereq")]
        [ValidateModelState]
        [CheckModelForNull]
        [AllowAnonymous]
        public HttpResponseMessage ForgotPassword(ForgotPasswordModel model )
        {
          string sessionKey=  customerFlow.ForgotPassword(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK, sessionKey);
        }

        /// <summary>
        /// Change password based on email and Session key (see ForgotPassword)
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("loginchangesl")]
        [AllowAnonymous]
        [ValidateModelState]
        [CheckModelForNull]
        public HttpResponseMessage ChangePasswordSessionKey(DALoginSessionKeyModel model)
        {
            customerFlow.ChangePasswordSessionKey(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Resets password of FIRST customer with given email and mobile and clears email of other customers with given email and mobile
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("resetpassword")]
        [AllowAnonymous]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage ResetPassword(DAresetPasswordModel model)
        {
            customerFlow.ResetPassword(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Get external id 2 of customer and if there is password with given email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost, Route("GetExternalId2ByEmail")]
        [AllowAnonymous]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage GetExternalId2ByEmail(GetExternalIdModel model)
        {
            ExternalId2PasswordModel externalIdPassword = customerFlow.GetExternalId2(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK, externalIdPassword);
        }

        /// <summary>
        /// Get customer info from 3rd party source
        /// For DA only. ex: http://localhost:5420/api/v3/da/Customers/GetCustomerInfoExternal/?phone=+306971234567
        /// </summary>
        /// <param name="phone">phone</param>
        /// <returns></returns>
        [HttpGet, Route("GetCustomerInfoExternal/")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetCustomerInfoExternal(string phone)
        {
            DACustomerModel customer = customerFlow.GetCustomerInfoExternal(DBInfo, phone);
            return Request.CreateResponse(HttpStatusCode.OK, customer);
        }

    }
}