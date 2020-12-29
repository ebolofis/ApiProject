using Symposium.WebApi.DataAccess.Interfaces.DT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System.Transactions;
using System.Data.SqlClient;
using System.Data;
using Symposium.Helpers.Interfaces;
using Dapper;
using log4net;
using Symposium.WebApi.DataAccess.Interfaces.DAO.Delivery;

namespace Symposium.WebApi.DataAccess.DT
{
    public class DeliveryCustomersDT : IDeliveryCustomersDT
    {
        string connectionString;
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        IGenericDAO<Delivery_AddressTypesDTO> genAddressTypes;
        IGenericDAO<Delivery_PhoneTypesDTO> genPhoneTypes;
       

        IGenericDAO<Delivery_CustomersDTO> genCustomer;
        IGenericDAO<Delivery_CustomersBillingAddressDTO> genBillAddress;
        IGenericDAO<Delivery_CustomersShippingAddressDTO> genShipAddress;
        IGenericDAO<Delivery_CustomersPhonesDTO> genPhone;
        IGenericDAO<Delivery_CustomersPhonesAndAddressDTO> genAssocs;

        IGenericDAO<DeliveryCustomerSearchModel> genSearch;

        IDeliveryCustomersDAO dcDAO;
        IDeliveryCustomersBillingAddressDAO dcbaDAO; IDeliveryCustomersShippingAddressDAO dcsaDAO;
        IDeliveryCustomersPhonesDAO dcphDAO;

        IGuestDAO guestDAO;

        IUsersToDatabasesXML usersToDatabases;

        public DeliveryCustomersDT(
            IUsersToDatabasesXML _usersToDatabases,
            IGenericDAO<Delivery_AddressTypesDTO> _genAddressTypes,
            IGenericDAO<Delivery_PhoneTypesDTO> _genPhoneTypes,
            IGenericDAO<Delivery_CustomersDTO> _genCustomer,
            IGenericDAO<Delivery_CustomersBillingAddressDTO> _genBillAddress,
            IGenericDAO<Delivery_CustomersShippingAddressDTO> _genShipAddress,
            IGenericDAO<Delivery_CustomersPhonesDTO> _genPhone,
            IGenericDAO<Delivery_CustomersPhonesAndAddressDTO> _genAssocs,
            IGenericDAO<DeliveryCustomerSearchModel> _genSearch,
            IDeliveryCustomersDAO _dcDAO,
            IDeliveryCustomersBillingAddressDAO _dcbaDAO, IDeliveryCustomersShippingAddressDAO _dcsaDAO,
            IDeliveryCustomersPhonesDAO _dcphDAO,
            IGuestDAO _guestDAO
            )
        {
            this.usersToDatabases = _usersToDatabases;
            this.genPhoneTypes = _genPhoneTypes;
            this.genAddressTypes = _genAddressTypes;
            this.genCustomer = _genCustomer;
            this.genBillAddress = _genBillAddress;
            this.genShipAddress = _genShipAddress;
            this.genPhone = _genPhone;
            this.genAssocs = _genAssocs;
            this.genSearch = _genSearch;

            this.dcDAO = _dcDAO;
            //DC addresses
            this.dcbaDAO = _dcbaDAO; this.dcsaDAO = _dcsaDAO;
            //DC Phone
            this.dcphDAO = _dcphDAO;

            this.guestDAO = _guestDAO;
        }

        /// <summary>
        /// Return object with 3 List lookups for DeliveryCustomer Entities Types
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public DeliveryCustomerLookupModel LookupTypes(DBInfoModel Store)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    return dcDAO.GetLookups(db);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    throw new Exception(Symposium.Resources.Errors.DELIVERYCUSTOMERLOOKUPS);
                }
            }
        }

        /// <summary>
        /// Provide filters to get paged flat searchModel of delivery customer
        /// Creates sql query with filters provided also a where predicate to filter results 
        /// then calls generic dao to create selection of obj asked and parse it into PaginationModel.PageList
        /// </summary>
        /// <param name="store"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageLength"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public PaginationModel<DeliveryCustomerSearchModel> SearchPagedCustomers(DBInfoModel store, int pageNumber, int pageLength, DeliveryCustomerFilterModel filters)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                PaginationModel<DeliveryCustomerSearchModel> pres = new PaginationModel<DeliveryCustomerSearchModel>();

                object parameters = new { };

                string wq = " where isnull(dc.Isdeleted , 0)=0 and isnull(ba.Isdeleted , 0)=0 and isnull(sa.Isdeleted , 0)=0 "; string orderStr = " dc.LastName ";
                if (filters != null)
                {
                    if (!string.IsNullOrEmpty(filters.Name))
                        wq += " and ((isnull (dc.LastName , '' ) +' '+ isnull(dc.FirstName, '' ) like '%" + filters.Name + "%') or  (isnull (dc.FirstName , '' ) +' '+ isnull(dc.LastName, '' ) like '%" + filters.Name + "%'))";

                    if (!string.IsNullOrEmpty(filters.VatNo))
                        wq += " and (dc.VatNo like '%" + filters.VatNo + "%' or dc.BillingVatNo like '%" + filters.VatNo + "%') ";

                    if (!string.IsNullOrEmpty(filters.PhoneNumber))
                        wq += " and PhoneNumber like '%" + filters.PhoneNumber + "%'";
                    if (!string.IsNullOrEmpty(filters.Address))
                        wq += " and (ba.AddressStreet like '%" + filters.Address + "%' or sa.AddressStreet like '%" + filters.Address + "%') ";

                    if (!string.IsNullOrEmpty(filters.Name)) orderStr = "dc.LastName";
                    else if (!string.IsNullOrEmpty(filters.VatNo)) orderStr = "dc.VatNo";
                    else if (!string.IsNullOrEmpty(filters.PhoneNumber)) orderStr = "ph.PhoneNumber";
                    else if (!string.IsNullOrEmpty(filters.Address)) orderStr = "sa.AddressStreet";

                }
                string query = "select Distinct ROW_NUMBER() OVER(ORDER BY " + orderStr + ") as AA, isnull (dc.LastName,'') + ' ' + isnull(dc.FirstName, '' ) as Name, dc.*, ph.ID as PhoneId, ph.PhoneNumber, sa.Id as ShippingAddressId, sa.AddressStreet as ShippingAddress, sa.AddressNo as ShippingNo, sa.City as ShippingCity, ba.Id as BillingAddressId, ba.AddressStreet as BillingAddress, ba.AddressNo as BillingNo, ba.City as BillingCity ";
                string qfrom = " from Delivery_Customers as dc ";
                string qjoinph = " left outer join Delivery_CustomersPhones as ph on dc.ID = ph.CustomerID ";
                string qjoinbil = " left outer join Delivery_CustomersBillingAddress as ba on dc.ID = ba.CustomerID and isnull(ba.IsDeleted , 0)=0 ";
                string qjoinship = " left outer join Delivery_CustomersShippingAddress as sa on dc.ID = sa.CustomerID and isnull(sa.IsDeleted , 0)=0 ";
                query += qfrom + qjoinph + qjoinbil + qjoinship + wq;

                string sqlData = @"SELECT * FROM(" + query + ") fin";
                string sWhere = "WHERE fin.AA BETWEEN @StartRow AND @EndRow order by fin.AA asc ";
                string sqlCount = @"SELECT COUNT(*)  FROM(" + query + ") fin";
                try
                {
                    pres = genSearch.GetPaginationQueryResult(db, sqlData, sWhere, sqlCount, pageNumber, pageLength);
                    return pres;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    throw new Exception(Symposium.Resources.Errors.DELIVERYCUSTOMERSEARCHPAGED);
                }
            }
        }

        /// <summary>
        /// Provide Id to get customer with details arrays 
        /// Assocs, Phones , Addresses billing and shipping 
        /// If phoneId , SAddID , BAddID  provided not != 0 or -1 makes them selected or it just returns Customer
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id">Unique identifier of LocalDB Delivery_Customer table</param>
        /// <param name="PhoneId"></param>
        /// <param name="SAddressId"></param>
        /// <param name="BAddressId"></param>
        /// <returns>DeliveryCustomer with details arrays</returns>
        public DeliveryCustomerModel GetCustomerById(DBInfoModel Store, long Id, long PhoneId, long SAddressId, long BAddressId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    DeliveryCustomerModel res = new DeliveryCustomerModel();

                    res = AutoMapper.Mapper.Map<DeliveryCustomerModel>(dcDAO.SelectById(db, Id));
                    res.BillingAddresses = AutoMapper.Mapper.Map<List<DeliveryCustomersBillingAddressModel>>(dcbaDAO.SelectDCustomerBillingAddressByCustomerId(db, Id));
                    res.ShippingAddresses = AutoMapper.Mapper.Map<List<DeliveryCustomersShippingAddressModel>>(dcsaDAO.SelectDCustomerShippingAddressByCustomerId(db, Id));
                    res.Phones = AutoMapper.Mapper.Map<List<DeliveryCustomersPhonesModel>>(dcphDAO.SelectDCustomerPhoneByCustomerId(db, Id));

                    res.Assocs = AutoMapper.Mapper.Map<List<DeliveryCustomersPhonesAndAddressModel>>(genAssocs.Select(db, "where CustomerId = @CustomerId ", new { CustomerId = Id }));

                    if (BAddressId > 0)
                    {
                        foreach (var item in res.BillingAddresses)
                        {
                            if (item.ID.Equals(BAddressId))
                                item.IsSelected = true;
                            else item.IsSelected = false;
                        }
                    }

                    if (SAddressId > 0)
                    {
                        foreach (var item in res.ShippingAddresses)
                        {
                            if (item.ID.Equals(SAddressId))
                                item.IsSelected = true;
                            else item.IsSelected = false;
                        }
                    }
                    if (PhoneId > 0)
                    {
                        foreach (var item in res.Phones)
                        {
                            if (item.ID.Equals(PhoneId))
                                item.IsSelected = true;
                            else item.IsSelected = false;
                        }
                    }
                    return res;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    throw new Exception(Symposium.Resources.Errors.DELIVERYCUSTOMERDETBYIDS);
                }
            }
        }

        /// <summary>
        /// Get's Id From Delivery_Customers for specific ExtCustId and ExtType
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="ExtId"></param>
        /// <param name="ExtType"></param>
        /// <returns></returns>
        public long? GetCustomerIdByExtId(DBInfoModel Store, string ExtId, ExternalSystemOrderEnum ExtType)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<long?>("SELECT Id FROM Delivery_Customers WHERE ExtCustId = @ExtCustId AND ExtType = @ExtType", new { ExtCustId = ExtId, ExtType = ExtType }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Provide ExtCUSTID and or TYPE to get customer with details arrays 
        /// Assocs, Phones , Addresses billing and shipping
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="ExtCUSTID"> External Key identification of system stored customer </param>
        /// <param name="ExtTYPE"> EXt Type of System</param>
        /// <returns>DeliveryCustomer with details arrays</returns>
        public DeliveryCustomerModel GetCustomerByExtKeyId(DBInfoModel Store, string ExtCUSTID, int? ExtTYPE)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    DeliveryCustomerModel res = new DeliveryCustomerModel();
                    res = AutoMapper.Mapper.Map<DeliveryCustomerModel>( dcDAO.SelectExternalIdType(db, ExtCUSTID, ExtTYPE) );

                    if (res != null)
                    {
                        res.BillingAddresses = AutoMapper.Mapper.Map<List<DeliveryCustomersBillingAddressModel>>( dcbaDAO.SelectDCustomerBillingAddressByCustomerId(db, res.ID) );
                        res.ShippingAddresses = AutoMapper.Mapper.Map<List<DeliveryCustomersShippingAddressModel>>( dcsaDAO.SelectDCustomerShippingAddressByCustomerId(db, res.ID) );
                        res.Phones = AutoMapper.Mapper.Map<List<DeliveryCustomersPhonesModel>>( dcphDAO.SelectDCustomerPhoneByCustomerId(db, res.ID) );

                        res.Assocs = AutoMapper.Mapper.Map<List<DeliveryCustomersPhonesAndAddressModel>>( genAssocs.Select(db, "where CustomerId = @CustomerId ", new { CustomerId = res.ID }) );
                    }
                    return res;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    throw new Exception(Symposium.Resources.Errors.DELIVERYCUSTOMERDETBYIDS);
                }
            }
        }
        
        /// <summary>
        /// Provide ExtCUSTID and or TYPE to get customer with details arrays 
        /// Assocs, Phones , Addresses billing and shipping
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="ExtCUSTID"> External Key identification of system stored customer </param>
        /// <param name="ExtTYPE"> EXt Type of System</param>
        /// <returns>DeliveryCustomer with details arrays</returns>
        public List<DeliveryCustomersShippingAddressModel> GetCustomerShippingAddressByExtKeyId(DBInfoModel Store, string ExtKey, int? ExtType)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    List<DeliveryCustomersShippingAddressModel> res = new List<DeliveryCustomersShippingAddressModel>();
                    //res = AutoMapper.Mapper.Map<List<DeliveryCustomersShippingAddressModel>>(genShipAddress.Select(db, "where CustomerId = @CustomerId and isnull(IsDeleted , 0)=0 ", new { CustomerId = res.ID }));
                    return res;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    throw new Exception(Symposium.Resources.Errors.DELIVERYCUSTOMERDETBYIDS);
                }
            }
        }



        /// <summary>
        /// Provide a local Customer with addresses and phones in primary model
        /// If slave entities are id deleted then they will be removed or marked as deleted if an invoice has been performed over them
        /// If they have ID = 0 then they will be inserted Else they will be updated 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="model"></param>
        /// <returns> User added via search after transaction is completed with model Id and all slave entities </returns>
        public DeliveryCustomerModel AddCustomer(DBInfoModel store, DeliveryCustomerModel model)
        {
            long newID = -1;
            Delivery_CustomersDTO ret = AutoMapper.Mapper.Map<Delivery_CustomersDTO>(model);
            connectionString = usersToDatabases.ConfigureConnectionString(store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        //Insert Delivery Customer move to DAO 
                        newID = genCustomer.Insert(db, ret);
                        dcDAO.UpdateCustomerPhones(db, model.Phones, newID);
                        dcDAO.UpdateBillindAddresses(db, model.BillingAddresses, newID);
                        dcDAO.UpdateShippingAddresses(db, model.ShippingAddresses, newID);
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        throw new Exception(Symposium.Resources.Errors.DELIVERYCUSTOMERINSERT);
                    }
                }
            };
            return GetCustomerById(store, newID, -1, -1, -1);
        }

        /// <summary>
        /// Provide a local Customer with addresses and phones in primary model
        /// If slave entities are id deleted then they will be removed or marked as deleted if an invoice has been performed over them
        /// If they have ID = 0 then they will be inserted Else they will be updated 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="model"></param>
        /// <returns> User updated via search after transaction is completed with model Id and all slave entities </returns>
        public DeliveryCustomerModel UpdateCustomer(DBInfoModel store, DeliveryCustomerModel model)
        {
            Delivery_CustomersDTO ret = AutoMapper.Mapper.Map<Delivery_CustomersDTO>(model);
            connectionString = usersToDatabases.ConfigureConnectionString(store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        //Update Delivery Customer move to DAO 
                        //r is on 
                        int r = genCustomer.Update(db, ret);
                        dcDAO.UpdateCustomerPhones(db, model.Phones, model.ID);
                        dcDAO.UpdateBillindAddresses(db, model.BillingAddresses, model.ID);
                        dcDAO.UpdateShippingAddresses(db, model.ShippingAddresses, model.ID);
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        throw new Exception(Symposium.Resources.Errors.DELIVERYCUSTOMERUPDATE + " " + model.ID);
                    }
                }
            };
            return GetCustomerById(store, model.ID, -1, -1, -1);
        }


        public DeliveryCustomerModelDS UpsertCustomerWithExtCustId(DBInfoModel store, DeliveryCustomerModelDS model)
        {
            long newID = -1;
            connectionString = usersToDatabases.ConfigureConnectionString(store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        //Getcustomer  with EXt ID 
                        Delivery_CustomersDTO m = genCustomer.Select(db, "where ExtCustId = @ExtId ", new { ExtId = model.ExtCustId }).FirstOrDefault();
                        DeliveryCustomerModel updatedc = new DeliveryCustomerModel();
                        //Insert on null 
                        if (m == null)
                        {
                            Delivery_CustomersDTO ret = AutoMapper.Mapper.Map<Delivery_CustomersDTO>(model);
                            newID = genCustomer.Insert(db, ret);
                            dcDAO.UpsertSearchCustomerPhones(db, model.Phones, newID);
                            dcDAO.UpsertSearchBillindAddresses(db, model.BillingAddresses, newID);
                            dcDAO.UpsertSearchShippingAddresses(db, model.ShippingAddresses, newID);
                            updatedc = GetCustomerById(store, newID, -1, -1, -1);
                        }
                        //update on  exist
                        else
                        {
                            Delivery_CustomersDTO ret = AutoMapper.Mapper.Map<Delivery_CustomersDTO>(model);
                            ret.ID = m.ID;
                            //use nested properties DAO 
                            int r = genCustomer.Update(db, ret);

                            dcDAO.UpsertSearchCustomerPhones(db, model.Phones, ret.ID);
                            dcDAO.UpsertSearchBillindAddresses(db, model.BillingAddresses, ret.ID);
                            dcDAO.UpsertSearchShippingAddresses(db, model.ShippingAddresses, ret.ID);
                            updatedc = GetCustomerById(store, ret.ID, -1, -1, -1);
                        }
                        //Use updated GUest DAOS 
                        long guest_id = guestDAO.upsertGuestFromDeliveryCustomer(db, updatedc);
                        //return new  Extend GDelivery C DS model with GuestID updated
                        DeliveryCustomerModelDS dsret = AutoMapper.Mapper.Map<DeliveryCustomerModelDS>(updatedc);
                        dsret.GuestId = guest_id;
                        scope.Complete();
                        return dsret;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        throw new Exception(Symposium.Resources.Errors.DELIVERYCUSTOMERUPSERTCUSTOMEREXTCUSTID + " " + model.ExtCustId);
                    }
                }
            };
        }


        /// <summary>
        /// Provide id of customer to delete
        /// Shippping addresses Billing Addresses , Phone and assocs 
        /// Then tryies to delete Customer with this id and returns this id else throws 
        /// exception raised 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="id">id  of customer to delete and filter its deps</param>
        /// <returns>Id of Delivery Customer Deleted</returns>
        public long DeleteCustomer(DBInfoModel store, long id)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        DeliveryCustomerModel model = GetCustomerById(store, id, -1, -1, -1);
                        model.Phones.ForEach(x => x.IsDeleted = true);
                        model.ShippingAddresses.ForEach(x => x.IsDeleted = true);
                        model.BillingAddresses.ForEach(x => x.IsDeleted = true);

                        List<long> shipids = model.ShippingAddresses.Select(q => q.ID).ToList();
                        List<long> billids = model.BillingAddresses.Select(q => q.ID).ToList();
                        int billacnt = db.RecordCount<InvoiceShippingDetailsDTO>(" where CustomerID = @cid  ", new { cid = model.ID });
                        int billacnth = db.RecordCount<InvoiceShippingDetails_HistDTO>(" where CustomerID = @cid  ", new { cid = model.ID });
                        dcDAO.UpdateCustomerPhones(db, model.Phones, model.ID);
                        dcDAO.UpdateBillindAddresses(db, model.BillingAddresses, model.ID);
                        dcDAO.UpdateShippingAddresses(db, model.ShippingAddresses, model.ID);

                        if (billacnt == 0 && billacnth == 0)
                        {
                            genCustomer.DeleteList(db, "where Id=@Id", new { Id = id });
                        }
                        else
                        {
                            model.IsDeleted = true;
                            Delivery_CustomersDTO ret = AutoMapper.Mapper.Map<Delivery_CustomersDTO>(model);
                            int r = genCustomer.Update(db, ret);
                        }
                        scope.Complete();

                        return id;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        throw new Exception(Symposium.Resources.Errors.DELIVERYCUSTOMERDELETE + " " + id);
                    };
                }

            }
        }
    }
}
