using Dapper;
using log4net;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Symposium.WebApi.DataAccess.DT.DeliveryAgent
{
    public class DA_CustomerDT : IDA_CustomerDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<DA_CustomersDTO> daCustomerDao;
        LocalConfigurationHelper configHlp;
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IGreekVowelsHelper greekVowelsHelper;
        PhoneticAbstHelper phoneticsHlp;
        IDA_PhoneticsDT phoneticsDT;

        public DA_CustomerDT(IUsersToDatabasesXML usersToDatabases, IGenericDAO<DA_CustomersDTO> daCustomerDao, LocalConfigurationHelper configHlp, IGreekVowelsHelper greekVowelsHelper, PhoneticAbstHelper phoneticsHlp, IDA_PhoneticsDT phoneticsDT)
        {
            this.usersToDatabases = usersToDatabases;
            this.daCustomerDao = daCustomerDao;
            this.configHlp = configHlp;
            this.greekVowelsHelper= greekVowelsHelper;
            this.phoneticsHlp = phoneticsHlp;
            this.phoneticsDT = phoneticsDT;
        }

        /// <summary>
        /// Authenticate User based on username and password. On failure return 0.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>CustomerId</returns>
        public long LoginUser(DBInfoModel Store, DALoginModel loginModel)
        {
            List<DACustomerModel> custModel = null;
            long custId = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string custSQL = @"SELECT * FROM DA_Customers AS dc WHERE dc.Email=@email AND dc.password = @password and dc.isDeleted=0";
                custModel = db.Query<DACustomerModel>(custSQL, new { email = loginModel.Email, password = loginModel.Password }).ToList();
                if (custModel != null && custModel.Count > 0)
                {
                    custId = custModel.FirstOrDefault().Id;
                }
            }
            return custId;
        }


        /// <summary>
        /// Gets customers with given authToken. On failure return 0.
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="authToken">authToken</param>
        /// <returns></returns>
        public long LoginUser(DBInfoModel Store, string authToken)
        {
            long custId = 0;
            List<DACustomerModel> custModel =null;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {

                string sqlSearchCustomers = @"SELECT * FROM DA_Customers AS dc
                                            WHERE dc.authToken = @authToken AND dc.IsDeleted = 0";
                custModel = db.Query<DACustomerModel>(sqlSearchCustomers, new { authToken = authToken }).ToList();
            }
            if (custModel!=null && custModel.Count > 0)
            {
                custId = custModel.FirstOrDefault().Id;
            }
            return custId;
        }

        /// <summary>
        /// return the number of appearances of an email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public int getEmailCount(DBInfoModel Store, string email)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
               return daCustomerDao.RecordCount(db,"where email=@email and isDeleted=0", new { email = email });
            }
        }

        /// <summary>
        /// return true if mobile exists into an active customer (isDeleted=0)
        /// </summary>
        /// <param name="mobile">mobile</param>
        /// <returns></returns>
        public bool mobileExists(DBInfoModel Store, string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile)) return false;
            mobile = mobile.Trim();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sql = "where (Mobile = @mobile or Phone1 = @mobile or Phone2 = @mobile) and isDeleted = 0";
                bool res= daCustomerDao.RecordCount(db, sql, new { mobile = mobile }) > 0;
                if (!res)
                {
                    if (mobile.StartsWith(configHlp.PhonePrefix())) { 
                        mobile = mobile.Replace(configHlp.PhonePrefix(), "");
                        mobile = mobile.Trim();
                    }
                    else
                        mobile = configHlp.PhonePrefix() + mobile;

                    res = daCustomerDao.RecordCount(db, sql, new { mobile = mobile }) > 0;
                }
                return res;
               
            }
        }

        /// <summary>
        /// return true if email exists into an active customer (isDeleted=0)
        /// </summary>
        /// <param name="email">email</param>
        /// <returns></returns>
        public bool emailExists(DBInfoModel Store, string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            email = email.Trim();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sql = "where email=@Email and isDeleted = 0";
                bool res = daCustomerDao.RecordCount(db, sql, new { Email = email }) > 0;
               
                return res;
            }
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
        public List<DASearchCustomerModel> SearchCustomers(DBInfoModel Store, DA_CustomerSearchTypeEnum type, string search)
        {
            List<DASearchCustomerModel> custModel = new List<DASearchCustomerModel>();

            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            string sqlSearchCust;
            string sqlSearchAddr;
            List<DACustomerModel> tmpCustList = new List<DACustomerModel>();

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                switch (type)
                {
                    case DA_CustomerSearchTypeEnum.Name:
                        sqlSearchCust = $@"SELECT * 
                                    FROM DA_Customers AS dc
                                    WHERE ({greekVowelsHelper.ReplaceSql("dc.FirstName")} LIKE '%{greekVowelsHelper.RemoveTonoi(search)}%' OR {greekVowelsHelper.ReplaceSql("dc.LastName")} LIKE '%{greekVowelsHelper.RemoveTonoi(search)}%') AND dc.IsDeleted = 0 AND ISNULL(dc.IsAnonymous, 0) != 2";
                        tmpCustList = db.Query<DACustomerModel>(sqlSearchCust).ToList();
                        break;

                    case DA_CustomerSearchTypeEnum.Address:
                        int mode = (int)MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_AddressSearchMode");
                        if (mode == 0)
                        {
                            var parts = search.Split(',');
                            string addr = parts[0].Trim();
                            string addrNoTonos = greekVowelsHelper.RemoveTonoi(addr);

                            if (parts.Count() == 1)
                            {
                                sqlSearchCust = $@"SELECT  dc.* 
                                            FROM DA_Customers AS dc
                                            INNER JOIN DA_Addresses AS da on da.OwnerId = dc.Id AND REPLACE({greekVowelsHelper.ReplaceSql("da.AddressStreet + ' ' + da.AddressNo")}, ' ','') LIKE REPLACE('%{addrNoTonos}%', ' ','') WHERE dc.IsDeleted = 0 AND ISNULL(dc.IsAnonymous, 0) != 2";
                            }
                            else
                            {
                                string area = "";
                                area = parts[1].Trim();
                                string areaNoTonos = greekVowelsHelper.RemoveTonoi(area);

                                sqlSearchCust = $@"  SELECT  dc.* 
                                            FROM DA_Customers AS dc
                                            INNER JOIN DA_Addresses AS da on da.OwnerId = dc.Id AND REPLACE({greekVowelsHelper.ReplaceSql("da.AddressStreet + ' ' + da.AddressNo")}, ' ','') LIKE REPLACE('%{addrNoTonos}%', ' ','') and {greekVowelsHelper.ReplaceSql("Area")} like '%{areaNoTonos}%' WHERE dc.IsDeleted = 0 AND ISNULL(dc.IsAnonymous, 0) != 2";
                            }

                            //sqlSearchCust = @"SELECT DISTINCT dc.* 
                            //                    FROM DA_Customers AS dc
                            //                    INNER JOIN DA_Addresses AS da on da.OwnerId = dc.Id AND REPLACE(da.AddressStreet + ' ' + da.AddressNo, ' ','') LIKE REPLACE('%" + search + "%', ' ','') WHERE dc.IsDeleted = 0";
                            tmpCustList = db.Query<DACustomerModel>(sqlSearchCust).ToList();
                        }
                        else
                        {   //search using Full Text Index
                            sqlSearchCust = phoneticsDT.ConstructCustomersSqlQuery(search);
                            if (string.IsNullOrEmpty(sqlSearchCust)) return custModel;
                            tmpCustList = db.Query<DACustomerModel>(sqlSearchCust).ToList();
                        }
                      
                        break;

                    case DA_CustomerSearchTypeEnum.VatNo:
                        sqlSearchCust = @"SELECT * 
                                    FROM DA_Customers AS dc
                                    WHERE dc.VatNo LIKE '%"+search+ "%' AND dc.IsDeleted = 0 AND ISNULL(dc.IsAnonymous, 0) != 2";
                        tmpCustList = db.Query<DACustomerModel>(sqlSearchCust).ToList();
                        break;

                    case DA_CustomerSearchTypeEnum.Email:
                        sqlSearchCust = @"SELECT * 
                                    FROM DA_Customers AS dc
                                    WHERE dc.Email =@email AND dc.IsDeleted = 0 AND ISNULL(dc.IsAnonymous, 0) != 2";
                        tmpCustList = db.Query<DACustomerModel>(sqlSearchCust, new { email= search }).ToList();
                        break;

                    default:

                        string searchInversePrefix = search;
                        if (search.StartsWith(configHlp.PhonePrefix()))
                            searchInversePrefix = search.Replace(configHlp.PhonePrefix(), "");
                        else
                            searchInversePrefix = configHlp.PhonePrefix() + search;
                        sqlSearchCust = @"SELECT *  FROM DA_Customers AS dc
                                        WHERE (dc.Phone1 =@search OR dc.Phone1 =@searchInversePrefix OR dc.Mobile =@search OR dc.Mobile =@searchInversePrefix OR dc.Phone2 =@search OR dc.Phone2 =@searchInversePrefix ) AND dc.IsDeleted = 0 AND ISNULL(dc.IsAnonymous, 0) != 2";
                        tmpCustList = db.Query<DACustomerModel>(sqlSearchCust, new { search = search, searchInversePrefix = searchInversePrefix }).ToList();
                        break;
                }

                if (tmpCustList == null) return new List<DASearchCustomerModel>();

                foreach (DACustomerModel customer in tmpCustList)
                {
                    DASearchCustomerModel tmp = new DASearchCustomerModel();
                    tmp.daCustomerModel = customer;
                    sqlSearchAddr = @"SELECT * FROM DA_Addresses AS da WHERE da.OwnerId=" + customer.Id + " and da.AddressType<>2  AND da.IsDeleted = 0";
                    tmp.daAddrModel = db.Query<DA_AddressModel>(sqlSearchAddr).ToList();
                    custModel.Add(tmp);
                }
            }
            return custModel;
        }

        /// <summary>
        /// Get Customer by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DACustomerModel GetCustomer(DBInfoModel Store, long id)
        {
            DA_CustomersDTO dto=null;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                dto = daCustomerDao.Select(db, id);
            }
            return AutoMapper.Mapper.Map<DACustomerModel>(dto); 
        }


        /// <summary>
        /// Gets customers with given email and mobile
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="email"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public List<DACustomerModel> GetCustomersByEmailMobile(DBInfoModel Store, string email, string mobile)
        {
            List<DACustomerModel> customers ;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string mobileInversePrefix;
                if (mobile.StartsWith(configHlp.PhonePrefix()))
                    mobileInversePrefix = mobile.Replace(configHlp.PhonePrefix(), "");
                else
                    mobileInversePrefix = configHlp.PhonePrefix() + mobile;
                string sqlSearchCustomers = @"SELECT * FROM DA_Customers AS dc
                                            WHERE dc.Email = @email AND (dc.Mobile = @mobile OR dc.Mobile = @mobileInversePrefix) AND dc.IsDeleted = 0";
                customers = db.Query<DACustomerModel>(sqlSearchCustomers, new { email = email, mobile = mobile, mobileInversePrefix = mobileInversePrefix }).ToList();
            }
            return customers;
        }

      


        /// <summary>
        /// Get Customer by ExtId1 or ExtId2. If no customer found return null
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="ExtId">ExtId1 or ExtId2</param>
        /// <param name="type">1 : ExtId1, 2 : ExtId2</param>
        /// <returns></returns>
        public DACustomerModel GetCustomer(DBInfoModel Store, string ExtId, int type=1)
        {
            string wheresql = " where ExtId" + type.ToString() + "=@ExtId";
            List<DA_CustomersDTO> dtos = null;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                dtos = daCustomerDao.Select(db, wheresql,new { ExtId= ExtId });
            }
            if (dtos == null || dtos.Count == 0) return null;
            return AutoMapper.Mapper.Map<DACustomerModel>(dtos[0]);
        }

        /// <summary>
        /// Search for a UNIQUE match into customers based on (lastName+firstName+Phone1), then based on  (lastName+firstName+Mobile).
        /// If the results are not unique (more than 1 records found) return null;
        /// 
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="lastName">lastName</param>
        /// <param name="firstName">firstName</param>
        /// <param name="tel">tel</param>
        /// <returns>one DACustomerModel or null</returns>
        public DACustomerModel GetCustomerUnique(DBInfoModel Store, string lastName,string firstName,string tel)
        {
            if (string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(tel)) return null;

           
            List<DA_CustomersDTO> dtos = null;
            var obj = new { lastName = lastName, firstName = firstName, tel = tel };
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string wheresql = " where LastName=@lastName and Firstname=@firstName and Phone1=@tel";
                dtos = daCustomerDao.Select(db, wheresql, obj );
                if (dtos == null || dtos.Count == 0)
                {
                    wheresql = " where LastName=@lastName and Firstname=@firstName and Mobile=@tel";
                    dtos = daCustomerDao.Select(db, wheresql, obj);
                }
                //if (dtos == null || dtos.Count == 0)
                //{
                //    wheresql = " where LastName=@lastName and Firstname=@firstName and Phone2=@tel";
                //    dtos = daCustomerDao.Select(db, wheresql, obj);
                //}
                if (dtos == null || dtos.Count == 0)
                {
                    if (tel.StartsWith(configHlp.PhonePrefix()))
                        tel = tel.Replace(configHlp.PhonePrefix(), "");
                    else
                        tel = configHlp.PhonePrefix() + tel;

                    obj = new { lastName = lastName, firstName = firstName, tel = tel };
                    wheresql = " where LastName=@lastName and Firstname=@firstName and Phone1=@tel";
                    dtos = daCustomerDao.Select(db, wheresql, obj);
                    if (dtos == null || dtos.Count == 0)
                    {
                        wheresql = " where LastName=@lastName and Firstname=@firstName and Mobile=@tel";
                        dtos = daCustomerDao.Select(db, wheresql, obj);
                    }
                    //if (dtos == null || dtos.Count == 0)
                    //{
                    //    wheresql = " where LastName=@lastName and Firstname=@firstName and Phone2=@tel";
                    //    dtos = daCustomerDao.Select(db, wheresql, obj);
                    //}

                }
            }
            if (dtos == null || dtos.Count == 0 || dtos.Count>1)
                return null;
            return AutoMapper.Mapper.Map<DACustomerModel>(dtos[0]);
        }

        /// <summary>
        /// Add Customer 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public long AddCustomer(DBInfoModel Store, DACustomerModel Model)
        {
            configHlp.CheckDeliveryAgent();
            long custId = 0;
            DA_CustomersDTO dto = AutoMapper.Mapper.Map<DA_CustomersDTO>(Model);
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                custId = daCustomerDao.Insert(db, dto);
            }
            logger.Info($"NEW CUSTOMER. Id: {custId}, ExtId1: {Model.ExtId1 ?? ""}");
            return custId;
        }

        /// <summary>
        /// Update Customer 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public long UpdateCustomer(DBInfoModel Store, DACustomerModel Model)
        {
            configHlp.CheckDeliveryAgent();
            long c = 0;
            DA_CustomersDTO dto = AutoMapper.Mapper.Map<DA_CustomersDTO>(Model);
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                c = daCustomerDao.Update(db, dto);
            }
            logger.Info($"UPDATE CUSTOMER. Id: {Model.Id}, ExtId1: {Model.ExtId1 ?? ""}, ExtId2: {Model.ExtId2 ?? ""}, ExtId3: {Model.ExtId3 ?? ""}, ExtId4: {Model.ExtId4 ?? ""}");
            return c;
        }

        /// <summary>
        /// Delete Customer 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteCustomer(DBInfoModel Store, long Id)
        {
            configHlp.CheckDeliveryAgent();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            logger.Info($"Deleting customer {Id}");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                   return daCustomerDao.Delete(db, Id);
                }
                catch (Exception)
                {
                    string custSQL = @"UPDATE DA_Customers SET IsDeleted = 1 WHERE Id =@ID";
                    db.Execute(custSQL, new { ID = Id });
                } 
            }
            return Id;
        }


        /// <summary>
        /// Change SessionKey (customer has forgotten his password)
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="Id">CustomerId</param>
        ///  <param name="SessionKey">the new SessionKey</param>
        /// <returns></returns>
        public void UpdateSessionKey(DBInfoModel Store, long Id, string SessionKey)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                    daCustomerDao.Execute(db,"update [DA_Customers] set SessionKey=@SessionKey where Id=@id", new  { Id = Id, SessionKey = SessionKey });
            }
        }


        /// <summary>
        /// Change Password (also change SessionKey="") 
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="Id">CustomerId</param>
        ///  <param name="Password">the new Password</param>
        /// <returns></returns>
        public void UpdatePassword(DBInfoModel Store, long Id, string Password)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            logger.Info($"Updating password for Customer {Id}...");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                daCustomerDao.Execute(db, "update [DA_Customers] set Password=@Password,SessionKey=''  where Id=@id", new { Id = Id, Password = Password });
            }
        }


        /// <summary>
        /// Reset password of customer with Id = customerId and clear email of other customers
        /// </summary>
        /// <param name="customers"></param>
        /// <param name="customerId"></param>
        /// <param name="newPassword"></param>
        public void UpdateOnePasswordClearOtherEmails(DBInfoModel Store, List<DACustomerModel> customers, long customerId, string encryptedPassword)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    foreach (DACustomerModel c in customers)
                    {
                        if (c.Id == customerId)
                        {
                            logger.Info($"Setting new password for customer {c.Id}...");
                            db.Execute("UPDATE DA_Customers set Password = @password where Id = @id", new { password = encryptedPassword, id = c.Id });
                        }
                        else
                        {
                            logger.Warn($"DELETING EMAIL for customer {c.Id}...");
                            db.Execute("UPDATE DA_Customers set Email = NULL where Id = @id", new { id = c.Id });
                        }
                    }
                    scope.Complete();   
                }
            }
        }

        /// <summary>
        /// Authenticate User based on email and SessionKey 
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="email"></param>
        /// <param name="SessionKey">SessionKey</param>
        /// <returns>CustomerId</returns>
        public long LoginUserSessionKey(DBInfoModel Store, DALoginSessionKeyModel loginModel)
        {
            List<DACustomerModel> custModel = new List<DACustomerModel>();
           // long custId = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string custSQL = @"SELECT * FROM DA_Customers AS dc WHERE dc.Email=@email AND dc.SessionKey = @SessionKey and isDeleted=0";
                custModel = db.Query<DACustomerModel>(custSQL, new { email = loginModel.Email, SessionKey = loginModel.SessionKey }).ToList();
                if (custModel.Count > 0)
                {
                    return custModel[0].Id;
                }
                else
                {
                    throw new BusinessException(Symposium.Resources.Errors.USERLOGINFAILED);
                }
            }
          
        }

        /// <summary>
        /// Get external id 2 of customer with given email
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public string GetExternalId2(DBInfoModel Store, string email)
        {
            string externalId = "";
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                DA_CustomersDTO customer = daCustomerDao.SelectFirst(db, "where Email = @email", new { email = email });
                if (customer != null)
                {
                    externalId = customer.ExtId2;
                }
            }
            return externalId;
        }

        /// <summary>
        /// check if password of customer with given email exists
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool hasPassword(DBInfoModel Store, string email)
        {
            string password = "";
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                DA_CustomersDTO customer = daCustomerDao.SelectFirst(db, "where Email = @email and isDeleted=0", new { email = email });
                if (customer != null)
                {
                    password = customer.Password;
                }
            }
            bool hasPassword = true;
            if (password == null || password == "")
            {
                hasPassword = false;
            }
            return hasPassword;
        }

        /// <summary>
        /// Return first customerId for a customer with mobile and empty email. Otherwise return 0.
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public long ExistMobile(DBInfoModel Store, string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile)) return 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                DA_CustomersDTO customer = daCustomerDao.SelectFirst(db, "where (mobile = @mobile) and (email='' or email is null) and isDeleted=0", new { mobile = mobile });
                if (customer != null)
                    return customer.Id;
                else
                    return 0;
            }
          
        }

        /// <summary>
        /// Return first customerId for a customer with email and empty mobile. Otherwise return 0.
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public long ExistEmail(DBInfoModel Store, string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                DA_CustomersDTO customer = daCustomerDao.SelectFirst(db, "where Email = @email and (mobile is null or mobile='') and isDeleted=0", new { email = email });
                if (customer != null)
                    return customer.Id;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Return first customerId for a Customer with email and mobile. Otherwise return 0.
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="existModel"></param>
        /// <returns></returns>
        public long ExistMobileEmail(DBInfoModel Store, DACustomerIdentifyModel existModel)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                DA_CustomersDTO customer = daCustomerDao.SelectFirst(db, "where mobile = @mobile and email=@email and isDeleted=0", new { mobile = existModel.Mobile, email = existModel.Email });
                if (customer != null)
                    return customer.Id;
                else
                    return 0;
            }

        }

        /// <summary>
        /// Updates customer with last order note
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="customerId"></param>
        /// <param name="lastOrderNote"></param>
        public void UpdateLastOrderNote(DBInfoModel dbInfo, long customerId, string lastOrderNote)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string updateCustSQL = @"UPDATE DA_Customers SET LastOrderNote = @note WHERE Id = @id";
                db.Execute(updateCustSQL, new { note = lastOrderNote, id = customerId });
            }
        }

        /// <summary>
        /// Returns customers with status DA_CustomerAnonymousTypeEnum.WillBeAnonymous
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        public List<DACustomerModel> GetToBeAnonymousCustomers(DBInfoModel dbInfo)
        {
            List<DA_CustomersDTO> customers;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                customers = daCustomerDao.Select(db, "WHERE IsAnonymous = 1", null);
            }
            return AutoMapper.Mapper.Map<List<DACustomerModel>>(customers);
        }

    }
}
