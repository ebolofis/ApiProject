using log4net;
using Newtonsoft.Json;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.OldGoodysModel;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows.ExternalSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static Symposium.Models.Models.OldGoodysModel.OldGoodysLoginResponse;

namespace Pos_WebApi.Controllers.V3.ExternalSystems
{
    public class OldGoodysV3Controller : BasicV3Controller
    {

        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IOldGoodysFlow oldgoodysFlow;
        IDA_CustomerFlows customerFlow;
        IDA_AddressesFlows addressesFlow;
        IDA_GeoPolygonsFlows geopolygonFlow;
        IDA_StoresFlows da_stores;
        

        public OldGoodysV3Controller(IOldGoodysFlow _oldgoodysFlow, IDA_CustomerFlows  _customerFlow, IDA_GeoPolygonsFlows _geopolygonFlow, IDA_AddressesFlows _addressesFlow, IDA_StoresFlows _dastores)
        {
            this.oldgoodysFlow = _oldgoodysFlow;
            this.customerFlow = _customerFlow;
            this.geopolygonFlow = _geopolygonFlow;
            this.addressesFlow = _addressesFlow;
            this.da_stores = _dastores;
    }


        #region CREATEORDERGOODYS
        [HttpPost, Route("api/v3/da/Orders/PostWebGoodysOrder/order/create")]
        [AllowAnonymous]
        public HttpResponseMessage PostNewOrder(object model)
        {
            try
            {
                string ModelToJson = JsonConvert.SerializeObject(model);
                logger.Info("OLD WEB GOODIS, NEW ORDER: " + ModelToJson);
            }
            catch (Exception ex)
            {
                logger.Error("OLD WEB GOODIS, NEW ORDER Json ERROR" + ex.ToString());
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        #endregion CREATEORDERGOODYS

        #region CREATEORDEREFOOD
        [HttpPost, Route("api/v3/da/Orders/PostEfoodOrder/order/create")]
        [AllowAnonymous]
        public HttpResponseMessage PostEfoodNewOrder(object model)
        {
            try
            {
                string ModelToJson = JsonConvert.SerializeObject(model);
                logger.Info("OLD  EFOOD, NEW ORDER: " + ModelToJson);
            }
            catch (Exception ex)
            {
                logger.Error("OLD  EFOOD, NEW ORDER Json ERROR" + ex.ToString());
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        #endregion CREATEORDEREFOOD

        #region LOGIN
        [HttpGet, Route("api/v3/da/Orders/PostWebGoodysOrder/user/login/{Accountid}")]
        [AllowAnonymous]
        public HttpResponseMessage OldLogin(string AccountId)
        {
            long AddressId = 0;
            long customerId = 0;

            logger.Info("OLD WEB GOODYS, TRYING TO LOGIN with AccId: " + AccountId);

            OldGoodysLoginResponse responsemodel = new OldGoodysLoginResponse();
            List<DA_AddressModel> listdaaddress = new List<DA_AddressModel>();
            DA_AddressModel addressmodel = new DA_AddressModel();
       

            DACustomerExtModel customer = new DACustomerExtModel();
            DA_GeoPolygonsBasicModel polygonsmodel = new DA_GeoPolygonsBasicModel();
            DA_AddressModel daaddress = new DA_AddressModel();

            responsemodel.addressList = new List<address>();


            bool isNumeric = long.TryParse(AccountId, out long numericAccountId);

            try { 

            if(isNumeric)
            {
                //DA_Customers.Id
                customer = customerFlow.getCustomer(DBInfo, numericAccountId);
                AddressId = addressesFlow.GetCustomerAddressById(DBInfo, numericAccountId);
            }
            else
            {
                    //DA_Customers.ExtId4
                    customerId = oldgoodysFlow.getCustomerbyExtId(DBInfo, AccountId, 4);

                    if (customerId == -1)
                     {
                        logger.Error("spcMessage:" +"Invalid User AccountId");
                    }
                    else
                    {
                        customer = customerFlow.getCustomer(DBInfo, customerId);
                        //DA_Addresses.ExtId2
                        AddressId = addressesFlow.GetCustomerAddressByExtId(DBInfo, AccountId);
                    }
                    
            }

            polygonsmodel = geopolygonFlow.SelectPolygonByAddressId(DBInfo, AddressId);
             
                long storeid = polygonsmodel.StoreId.GetValueOrDefault();

                DA_StoreModel dastore = new DA_StoreModel();  
                if(storeid != 0)
                {
                    dastore = da_stores.GetStoreById(DBInfo, storeid);
                }
                else
                {
                    string error = "spcMessage:" + "No Matching Store";
                    return Request.CreateResponse(HttpStatusCode.OK, error);
                }
        

                listdaaddress = addressesFlow.GetCustomerAddresses(DBInfo, AddressId);

                responsemodel.id = AccountId;
                responsemodel.accountStatus = "Active";
                responsemodel.location = customer.Phone1;
                responsemodel.name = customer.Email;
                responsemodel.lastName = customer.LastName;
                responsemodel.firstName = customer.FirstName;
                responsemodel.type = "WEB";


                foreach (DA_AddressModel mod in listdaaddress)
                {
                    address addr = new address();

                    addr.addressComment = customer.Notes;
                    addr.addressId = AccountId;
                    addr.addressNAme = customer.Email;
                    addr.addresssFloor = mod.Floor;
                    addr.county = mod.Area;
                    addr.NNHome = mod.AddressNo;
                    addr.phoneNumber = customer.Phone1;
                    addr.postalCode = mod.Zipcode;
                    addr.shop = mod.City;
                    addr.shopId = Convert.ToString(dastore.Code); 
                    addr.specificAddrChar = "";
                    addr.state = mod.Area;
                    addr.streetAddress = mod.AddressStreet;
                    addr.streetAlias = mod.FriendlyName;
                    addr.isPrimary = "N";
                    addr.software = "HIT";

                    responsemodel.addressList.Add(addr);
                }
                return Request.CreateResponse(HttpStatusCode.OK, responsemodel);

            }
            catch (Exception exception)
            {
                logger.Error("spcMessage:" + exception.ToString());
                string error = "spcMessage:" + exception.ToString();
                return Request.CreateResponse(HttpStatusCode.OK, error);

            }

           
        }

        #endregion LOGIN

        #region REGISTER

        [HttpPost, Route("api/v3/da/Orders/PostWebGoodysOrder/user/register/")]
        [AllowAnonymous]
        public HttpResponseMessage OldRegister([FromUri()]string account)
        {
            try {

                //string ModelToJson = JsonConvert.SerializeObject(account);
                logger.Info("- OLD WEB GOODYS, ACCOUNT MODEL ON REGISTER: " + (account??"<NULL>"));
                Account account2 =JsonConvert.DeserializeObject<Account>(account);
                List<DA_AddressModel> listdaaddress = new List<DA_AddressModel>();
                DA_AddressModel addressmodel = new DA_AddressModel();
                OldGoodysLoginResponse responsemodel = new OldGoodysLoginResponse();
                List<DA_AddressModel> shippingaddresslist = new List<DA_AddressModel>();
                 DACustomerExtModel model = new DACustomerExtModel();
                DA_GeoPolygonsBasicModel polygonsmodel = new DA_GeoPolygonsBasicModel();
                responsemodel.addressList = new List<address>();

                model.FirstName = account2.fName;
                model.LastName = account2.lName;
                model.Email = account2.acceMail;
                model.Phone1 = account2.addrPhoneNum;
                model.Phone2 = account2.accountPhone;
                model.Notes = account2.addrComments;
                model.BillingAddress = addressmodel;
              
                addressmodel.Notes = account2.addrComments;
                addressmodel.Zipcode = account2.addrPostalCode;
                addressmodel.City = account2.addrState;
                addressmodel.Area = account2.addrCounty;
                addressmodel.AddressStreet = account2.addrStreetName;
                addressmodel.Floor = account2.addrFloor;
                addressmodel.Longtitude = 0;
                addressmodel.Latitude = 0;

              //  addressmodel.OwnerId = model.Id;      
                
                shippingaddresslist.Add(addressmodel);
                model.ShippingAddresses = shippingaddresslist;

                model = customerFlow.AddCustomer(DBInfo, model);
                long AddressId= addressesFlow.AddAddress(DBInfo, addressmodel, model.Id);  //create the customers address and get its Id


                responsemodel.id = Convert.ToString(model.Id);
                responsemodel.firstName = account2.fName;
                responsemodel.lastName = account2.lName;
                responsemodel.location = account2.accountPhone;
                responsemodel.type = "WEB";
                responsemodel.name = account2.acceMail;
                responsemodel.accountStatus = "New Registered Customer";

      

                listdaaddress = addressesFlow.GetCustomerAddresses(DBInfo, AddressId);      //get the newly created address

            
                    address addr = new address();

                    addr.addressComment = addressmodel.Notes;
                    addr.addressId =Convert.ToString( AddressId);
                    addr.addressNAme = account2.acceMail;
                    addr.addresssFloor = addressmodel.Floor;
                    addr.county = addressmodel.Area;
                    addr.NNHome = addressmodel.AddressNo;
                    addr.phoneNumber = account2.accountPhone;
                    addr.postalCode = addressmodel.Zipcode;
                    addr.shop = "";
                    addr.shopId = "Not ";
                    addr.specificAddrChar = "";
                    addr.state = addressmodel.Area;
                    addr.streetAddress = addressmodel.AddressStreet;
                    addr.streetAlias = addressmodel.FriendlyName;
                    addr.isPrimary = "N";
                    addr.software = "HIT";

                    responsemodel.addressList.Add(addr);
                
             
                return Request.CreateResponse(HttpStatusCode.OK, responsemodel);

            }
            catch (Exception exception)
            {
                logger.Error("spcMessage:" + exception.ToString());
                string error = "spcMessage:" + exception.Message;
                return Request.CreateResponse(HttpStatusCode.OK, error);
            }

           
        }
        #endregion REGISTER

        #region CREATEADDRESS

        [HttpPost, Route("api/v3/da/Orders/PostWebGoodysOrder/address/create")]
        [AllowAnonymous]
        public HttpResponseMessage OldCreateAddress(Address address)
        {

            string ModelToJson = JsonConvert.SerializeObject(address);
            logger.Info("OLD WEB GOODYS, ADDRESS MODEL ON CREATE ADDRESS: " + ModelToJson);
           // Address address = JsonConvert.DeserializeObject<Address>(addressobj);
            long CustomerId = 0;
            DA_AddressModel model = new DA_AddressModel();
            DACustomerExtModel customer = new DACustomerExtModel();
            DA_GeoPolygonsBasicModel polygonsmodel = new DA_GeoPolygonsBasicModel();


            List<DA_AddressModel> listdaaddress = new List<DA_AddressModel>();
            OldGoodysLoginResponse responsemodel = new OldGoodysLoginResponse();
            List<DA_AddressModel> shippingaddresslist = new List<DA_AddressModel>();         
            responsemodel.addressList = new List<address>();

            bool isNumeric = long.TryParse(address.accId, out long numericAccountId);
            try
            {

                if (isNumeric)
            {
                        customer = customerFlow.getCustomer(DBInfo, numericAccountId);
                        CustomerId = customer.Id;
            }
            else
            {
                CustomerId = oldgoodysFlow.getCustomerbyExtId(DBInfo, address.accId, 4);
            }

            model.Notes = address.addressComment;
            model.Floor = address.addresssFloor;
            model.City = address.state;
            model.Zipcode = address.postalCode;
            model.ExtId2 = address.accId;
            model.AddressNo = address.NNHome;
            model.AddressStreet = address.streetAddress;
            model.FriendlyName = address.streetAlias;
            model.Area = address.county;
            model.Longtitude = 0;
            model.Latitude = 0;
            model.OwnerId = CustomerId;


                long AddressId =     addressesFlow.AddAddress(DBInfo,  model, CustomerId);

                polygonsmodel = geopolygonFlow.SelectPolygonByAddressId(DBInfo, AddressId);

                long storeid = polygonsmodel.StoreId.GetValueOrDefault();
                DA_StoreModel dastore = new DA_StoreModel();
                if (storeid != 0)
                {
                    dastore = da_stores.GetStoreById(DBInfo, storeid);
                }
                else
                {
                    string error = "spcMessage:" + "No Matching Store";
                    return Request.CreateResponse(HttpStatusCode.OK, error);
                }


                listdaaddress = addressesFlow.GetCustomerAddresses(DBInfo, AddressId);      //get the newly created address

                foreach (DA_AddressModel mod in listdaaddress)
                {
                    address addr = new address();

                    addr.addressComment = model.Notes;
                    addr.addressId = Convert.ToString(AddressId);
                    addr.addressNAme = customer.Email;
                    addr.addresssFloor = mod.Floor;
                    addr.county = mod.City;
                    addr.NNHome = mod.AddressNo;
                    addr.phoneNumber = address.phoneNumber;
                    addr.postalCode = mod.Zipcode;
                    addr.shop = "";
                    addr.shopId = Convert.ToString(dastore.Code);
                    addr.specificAddrChar = "";
                    addr.state = mod.Area;
                    addr.streetAddress = mod.AddressStreet;
                    addr.streetAlias = mod.FriendlyName;
                    addr.isPrimary = "N";
                    addr.software = "HIT";

                    responsemodel.addressList.Add(addr);
                }

                responsemodel.id = address.accId;
                responsemodel.accountStatus = "Active";
                responsemodel.location = address.phoneNumber;
                responsemodel.name = customer.Email;

                return Request.CreateResponse(HttpStatusCode.OK,responsemodel);

            }
            catch(Exception exception)
            {
                logger.Error("spcMessage:" + exception.ToString());
                string error = "spcMessage:" + exception.Message;
                return Request.CreateResponse(HttpStatusCode.OK, error);
            }

         
        }
        #endregion CREATEADDRESS

        #region DELETEADDRESS

        [HttpGet, Route("api/v3/da/Orders/PostWebGoodysOrder/delete/{addressId}/{accountid}")]
        [AllowAnonymous]
        public HttpResponseMessage OldDeleteAddress(string accountId,string addressId)
        {

            logger.Info("OLD WEB GOODYS, TRYING TO DELETE with AccId: " + accountId + "addressId" + addressId);

            List<DA_AddressModel> listdaaddress = new List<DA_AddressModel>();
            DA_AddressModel addressmodel = new DA_AddressModel();
            OldGoodysLoginResponse responsemodel = new OldGoodysLoginResponse();
            List<DA_AddressModel> shippingaddresslist = new List<DA_AddressModel>();
            DACustomerExtModel customer = new DACustomerExtModel();
            DA_GeoPolygonsBasicModel polygonsmodel = new DA_GeoPolygonsBasicModel();
            responsemodel.addressList = new List<address>();
            long CustomerId = 0;
            long AddressId = 0;
            bool addressIsNumeric = long.TryParse(addressId, out  AddressId);
            bool isNumeric = long.TryParse(accountId, out long numericAccountId);

            try { 

            if (isNumeric)  //if accid is numeric
            {
                CustomerId = numericAccountId;
            }
            else
            {
                CustomerId = oldgoodysFlow.getCustomerbyExtId(DBInfo, accountId, 4);
            }

                customer = customerFlow.getCustomer(DBInfo, CustomerId);
                if (!addressIsNumeric)
                    AddressId = addressesFlow.GetCustomerAddressByExtId(DBInfo, accountId);
                else
                    AddressId = AddressId;


                polygonsmodel = geopolygonFlow.SelectPolygonByAddressId(DBInfo, AddressId);

                long storeid = polygonsmodel.StoreId.GetValueOrDefault();
                DA_StoreModel dastore = new DA_StoreModel();
                if (storeid != 0)
                {
                    dastore = da_stores.GetStoreById(DBInfo, storeid);
                }
                else
                {
                    string error = "spcMessage:" + "No Matching Store";
                    return Request.CreateResponse(HttpStatusCode.OK, error);
                }


                listdaaddress = addressesFlow.GetCustomerAddresses(DBInfo, AddressId);      //get the newly created address

                foreach (DA_AddressModel mod in listdaaddress)
                {
                    address addr = new address();

                    addr.addressComment = customer.Notes;
                    addr.addressId = Convert.ToString(AddressId);
                    addr.addressNAme = customer.Email;
                    addr.addresssFloor = mod.Floor;
                    addr.county = mod.City;
                    addr.NNHome = mod.AddressNo;
                    addr.phoneNumber = customer.Phone2;
                    addr.postalCode = mod.Zipcode;
                    addr.shop = "";
                    addr.shopId = Convert.ToString(dastore.Code);
                    addr.specificAddrChar = "";
                    addr.state = mod.Area;
                    addr.streetAddress = mod.AddressStreet;
                    addr.streetAlias = mod.FriendlyName;
                    addr.isPrimary = "N";
                    addr.software = "HIT";

                    responsemodel.addressList.Add(addr);
                }

                responsemodel.id = accountId;
                responsemodel.accountStatus = "Active";
                responsemodel.location = customer.Phone1;
                responsemodel.name = customer.Email;

            

                //delete the address specified
                addressesFlow.DeleteAddress(DBInfo, AddressId, CustomerId);

                //afterwards delete the rest available addresses of the customer if there are any
                List<DA_AddressModel> listofaddresses = new List<DA_AddressModel>();
                listofaddresses = addressesFlow.GetCustomerAddresses(DBInfo, CustomerId);

                if (listofaddresses.Count >  0)
                {
                           foreach (DA_AddressModel mod in listofaddresses)
                            {
                                addressesFlow.DeleteAddress(DBInfo, mod.Id, CustomerId);
                            }
                }
                return Request.CreateResponse(HttpStatusCode.OK, responsemodel);

            }
            catch(Exception exception)
            {

                string ModelToJson = JsonConvert.SerializeObject(responsemodel);
                logger.Info("OLD WEB GOODYS, Trying to Delete the response model : " + ModelToJson);
                logger.Error("spcMessage:" + exception.ToString());
                string error = "spcMessage:" + exception.Message;
                return Request.CreateResponse(HttpStatusCode.OK, error);

            }
            
        }
        #endregion DELETEADDRESS
    }
}
