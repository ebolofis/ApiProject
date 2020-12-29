using Dapper;
using log4net;
using Symposium.Helpers;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.DAOs.Delivery;
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

    public class DA_ClientJobsDT : IDA_ClientJobsDT
    {
        string connectionString;
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IUsersToDatabasesXML usersToDatabases;
        IDeliveryCustomersDAO idcd;

        public DA_ClientJobsDT(IUsersToDatabasesXML usersToDatabases, IDeliveryCustomersDAO idcd)
        {
            this.usersToDatabases = usersToDatabases;
            this.idcd = idcd;
        }

        /// <summary>
        /// Return's an order model to send to client to make new order
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <param name="customers"></param>
        /// <param name="extType"></param>
        /// <returns></returns>
        public DA_NewOrderModel ReturnOrderDetailExternalList(DBInfoModel Store, DA_OrderModel model, List<DASearchCustomerModel> customers, 
            ExternalSystemOrderEnum extType, out string Error)
        {
            DA_NewOrderModel ret = new DA_NewOrderModel();
            string SQL = "";
            Error = "";
            try
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    db.Open();

                    ret = AutoMapper.Mapper.Map<DA_NewOrderModel>(model);
                    ret.OrderDetails = new List<DA_OrderDetailExtModel>();
                    ret.CustomerData = customers.Where(w => w.CustomerId == model.CustomerId && w.OrderId == model.Id).FirstOrDefault();
                    //ret.CustomerData.OrderType = model.OrderType;

                    foreach (DA_OrderDetails item in model.Details)
                    {
                        DA_OrderDetailExtModel obj = new DA_OrderDetailExtModel();
                        obj = AutoMapper.Mapper.Map<DA_OrderDetailExtModel>(item);

                        SQL = "SELECT p.Id StoreProductId, p.Code ProductCode, p.Description ProductDescription, p.ProductCategoryId ProductCategoryId, \n"
                            + " pc.Description ProductCategory, pc.CategoryId CategoryId, ISNULL(c.Description,'') Category, p.UnitId UnitId, \n"
                            + " ISNULL(u.Description,'') Unit, pl.Id StorePriceListId, pl.Description PriceList, \n"
                            + "	pd.Id PriceListDetailId, p.PreparationTime PreparationTime, ISNULL(p.KdsId, kd.Id) KdsId, ISNULL(p.KitchenId, k.Id) KitchenId \n"
                            + "FROM Product AS p  \n"
                            + "INNER JOIN ProductCategories AS pc ON pc.Id = p.ProductCategoryId \n"
                            + "LEFT OUTER JOIN Categories AS c ON c.Id = pc.CategoryId \n"
                            + "LEFT OUTER JOIN Units AS u ON u.Id = p.UnitId \n"
                            + "INNER JOIN Pricelist AS pl ON pl.DAId = " + item.PriceListId.ToString() + " \n"
                            + "OUTER APPLY ( \n"
                            + "	SELECT TOP 1 * \n"
                            + "	FROM PricelistDetail AS pd \n"
                            + "	WHERE pd.ProductId = p.Id AND pd.PricelistId = pl.Id \n"
                            + ") pd \n"
                            + "OUTER APPLY( \n"
                            + "	SELECT TOP 1 * \n"
                            + "	FROM Kitchen AS k	 \n"
                            + ") k \n"
                            + "OUTER APPLY( \n"
                            + "	SELECT TOP 1 * \n"
                            + "	FROM Kds AS kd	 \n"
                            + ") kd \n"
                            + "WHERE p.DAId = " + item.ProductId.ToString();
                        ClientsIDsAndDescrModel tmpObj = db.Query<ClientsIDsAndDescrModel>(SQL).FirstOrDefault();
                        if (tmpObj == null)
                        {
                            logger.Error("ReturnOrderDetailExternalList [SQL : " + SQL + "] No data found for order id  : " + model.Id.ToString());
                            Error = "No data found for order id  : " + model.Id.ToString();
                            return null;
                        }
                        obj.IsExtra = 0;
                        obj.StoreProductId = tmpObj.StoreProductId;
                        obj.ProductCode = tmpObj.ProductCode;
                        obj.ProductDescription = tmpObj.ProductDescription;
                        obj.ProductCategoryId = tmpObj.ProductCategoryId;
                        obj.ProductCategory = tmpObj.ProductCategory;
                        obj.CategoryId = tmpObj.CategoryId;
                        obj.Category = tmpObj.Category;
                        obj.UnitId = tmpObj.UnitId;
                        obj.Unit = tmpObj.Unit;
                        obj.StorePriceListId = tmpObj.StorePriceListId;
                        obj.PriceList = tmpObj.PriceList;
                        obj.PriceListDetailId = tmpObj.PriceListDetailId;
                        obj.PreparationTime = tmpObj.PreparationTime;
                        obj.KdsId = tmpObj.KdsId;
                        obj.KitchenId = tmpObj.KitchenId;
                        ret.OrderDetails.Add(obj);

                        foreach (DA_OrderDetailsExtrasModel ext in item.Extras)
                        {
                            DA_OrderDetailExtModel extItem = new DA_OrderDetailExtModel();

                            SQL = "SELECT i.Id StoreProductId, i.Description ProductDescription, i.Code ProductCode FROM Ingredients AS i WHERE i.DAId = " + ext.ExtrasId.ToString();
                            tmpObj = db.Query<ClientsIDsAndDescrModel>(SQL).FirstOrDefault();

                            extItem.IsExtra = 1;
                            extItem.StoreProductId = tmpObj.StoreProductId;
                            extItem.ProductCode = tmpObj.ProductCode;
                            extItem.ProductDescription = tmpObj.ProductDescription;
                            extItem.ProductCategoryId = obj.ProductCategoryId;
                            extItem.ProductCategory = obj.ProductCategory;
                            extItem.CategoryId = obj.CategoryId;
                            extItem.Category = obj.Category;
                            extItem.UnitId = obj.UnitId;
                            extItem.Unit = obj.Unit;
                            extItem.StorePriceListId = obj.StorePriceListId;
                            extItem.PriceList = obj.PriceList;
                            extItem.PriceListDetailId = obj.PriceListDetailId;
                            extItem.PreparationTime = obj.PreparationTime;
                            extItem.KdsId = obj.KdsId;
                            extItem.KitchenId = obj.KitchenId;
                            extItem.NetAmount = ext.NetAmount;
                            extItem.DAOrderId = ext.OrderDetailId;
                            extItem.Price = ext.Price;
                            extItem.Qnt = ext.Qnt;
                            extItem.RateTax = ext.RateTax;
                            extItem.RateVat = ext.RateVat;
                            extItem.TotalTax = ext.TotalTax;
                            extItem.TotalVat = ext.TotalVat;
                            extItem.Id = ext.Id;
                            extItem.Description = ext.Description;

                            ret.OrderDetails.Add(extItem);
                        }

                    }
                    SQL = "SELECT * FROM DA_Stores WHERE Id = " + model.StoreId.ToString();
                    ret.StoreModel = db.Query<DA_StoreModel>(SQL).FirstOrDefault();
                    ret.PosId = ret.StoreModel.PosId ?? 0;
                    ret.PosStaffId = ret.StoreModel.PosStaffId ?? 0;
                    ret.ExtType = (int)extType;
                }
            }
            catch (Exception ex)
            {
                logger.Error("ReturnOrderDetailExternalList [SQL : " + SQL + "] : " + ex.ToString());
                Error = "General Error : " + ex.ToString();
                return null;
            }
            return ret;
        }

        /// <summary>
        /// Get's Invoice Shipping for specific Invoice Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        public InvoiceShippingDetailsModel GetInvoiceShippingForSpecificInvoice(DBInfoModel Store, long InvoiceId)
        {
            InvoiceShippingDetailsModel ret = null;
            string SQL = "";
            try
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    db.Open();
                    SQL = "SELECT * FROM InvoiceShippingDetails WHERE InvoicesId = " + InvoiceId.ToString();
                    ret = db.Query<InvoiceShippingDetailsModel>(SQL).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                logger.Error("GetInvoiceShippingForSpecificInvoice [SQL: " + SQL + "] \n" + ex.ToString());
            }
            return ret;
        }

        /// <summary>
        /// Return's Order Status
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public int GetLastStatusForDeliverOrder(DBInfoModel Store, long OrderId)
        {
            int ret = -1;
            string SQL = "";
            try
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    db.Open();
                    SQL = "SELECT MAX(os.Status) Status  \n"
                       + "FROM OrderStatus AS os \n"
                       + "INNER JOIN ( \n"
                       + "	SELECT MAX(ost.TimeChanged) TimeChanged \n"
                       + "	FROM OrderStatus AS ost \n"
                       + "	WHERE ost.OrderId = " + OrderId.ToString() + "	 \n"
                       + ") ost ON ost.TimeChanged = os.TimeChanged \n"
                       + "WHERE os.OrderId = " + OrderId.ToString();
                    ret = db.Query<int>(SQL).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                logger.Error("GetLastStatusForDeliverOrder [SQL: " + SQL + "] \n" + ex.ToString());
            }
            return ret;
        }

        /// <summary>
        /// Return's an order from db for specific extrnal key and type
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="ExtType"></param>
        /// <param name="ExtKey"></param>
        /// <returns></returns>
        public OrderModel GetOrderFromDBUsingExternalKey(DBInfoModel Store, ExternalSystemOrderEnum ExtType, string ExtKey)
        {
            OrderModel ret = null;
            string SQL = "";
            try
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    db.Open();
                    SQL = "SELECT * FROM [Order] WHERE ExtType = " + ExtType.ToString() + " AND LTRIM(RTRIM(ExtKey)) = '" + ExtKey + "'";
                    ret = db.Query<OrderModel>(SQL).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                logger.Error("GetOrderFromDBUsingExternalKey [SQL: " + SQL + "] \n" + ex.ToString());
            }
            return ret;
        }

        /// <summary>
        /// Return's an invoice for specific External type and External Key (Delivery Key)
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="ExternalType"></param>
        /// <param name="ExtKey"></param>
        /// <returns></returns>
        public InvoiceModel GetInvoiceFromDBForDelivery(DBInfoModel Store, ExternalSystemOrderEnum ExternalType, string ExtKey, bool forCancel)
        {
            string SQL = "";
            InvoiceModel ret = null;
            try
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    db.Open();
                    SQL = "SELECT DISTINCT i.* \n"
                       + "FROM [Order] AS o \n"
                       + "INNER JOIN OrderDetail AS od ON od.OrderId = o.Id \n"
                       + "INNER JOIN OrderDetailInvoices AS odi ON odi.OrderDetailId = od.Id \n"
                       + "INNER JOIN Invoices AS i ON i.Id = odi.InvoicesId \n";
                    if (forCancel)
                        SQL += "INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId AND it.[Type] NOT IN (3,8) \n";
                    else //Get's Only ΔΠ
                        SQL += "INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId AND it.[Type] IN (2) \n";
                    SQL += "WHERE o.ExtType = " + ExternalType.ToString() + " AND LTRIM(RTRIM(o.ExtKey)) = '" + ExtKey + "'";
                    ret = db.Query<InvoiceModel>(SQL).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                logger.Error("GetInvoiceFromDBForDelivery [SQL: " + SQL + "] \n" + ex.ToString());
            }
            return ret;
        }

        /// <summary>
        /// Check Customer and Address and Phones if Exists and Insert's Or Update's Data
        /// return's a Guest model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Customer"></param>
        /// <param name="Addresses"></param>
        /// <param name="OrderType"></param>
        /// <param name="Error"></param>
        /// <param name="guest"></param>
        /// <returns></returns>
        public DeliveryCustomerModel UpsertCustomer(DBInfoModel Store, DACustomerModel Customer, List<DA_AddressModel> Addresses, int OrderType, 
            out string Error, ref GuestModel guest)
        {
            string SQL = "";
            Error = "";
            bool IsNew = false;

            if (Addresses == null || Addresses.Count < 1 && OrderType != 21)
            {
                Error = "No address exists for this order";
                return null;
            }

            long CustomerId = 0, ShippindId = 0, BillingId = 0, Phone1Id = 0, Phone2Id = 0, MobileId = 0;
            DeliveryCustomerModel result = new DeliveryCustomerModel();
            result.ShippingAddresses = new List<DeliveryCustomersShippingAddressModel>();
            result.BillingAddresses = new List<DeliveryCustomersBillingAddressModel>();
            result.Phones = new List<DeliveryCustomersPhonesModel>();
            try
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    DA_AddressModel tmpAdd = Addresses.Find(f => f.isShipping == true);
                    if (tmpAdd == null)
                        tmpAdd = Addresses[0];

                    db.Open();
                    SQL = "SELECT * FROM Delivery_Customers WHERE ISNULL(ExtType,0) = " + ((int)Customer.ExtType).ToString() + " AND LTRIM(RTRIM(ISNULL(ExtCustId,''))) = '" + Customer.Id.ToString() + "'";
                    DeliveryCustomer dbCustomer = db.Query<DeliveryCustomer>(SQL).FirstOrDefault();
                    if (dbCustomer == null)
                    {
                        IsNew = true;
                        dbCustomer = new DeliveryCustomer();
                        dbCustomer.ExtCustId = Customer.Id.ToString();
                        dbCustomer.ExtType = (int)Customer.ExtType;

                    }
                    SQL = "SELECT TOP 1 ID FROM Delivery_CustomersTypes";
                    int? customerType = db.Query<int?>(SQL).FirstOrDefault();

                    dbCustomer.LastName = Customer.LastName;
                    dbCustomer.FirstName = Customer.FirstName;
                    dbCustomer.VatNo = Customer.VatNo;
                    dbCustomer.DOY = Customer.Doy;
                    dbCustomer.Floor = tmpAdd.Floor;
                    dbCustomer.email = Customer.Email;
                    dbCustomer.Comments = Customer.Notes;
                    dbCustomer.CustomerType = IsNew ? customerType : dbCustomer.CustomerType;
                    dbCustomer.BillingName = Customer.LastName + " " + Customer.FirstName;
                    dbCustomer.BillingVatNo = Customer.VatNo;
                    dbCustomer.BillingDOY = Customer.Doy;
                    dbCustomer.BillingJob = Customer.JobName;
                    dbCustomer.GDPR_Marketing = Customer.GTPR_Marketing;
                    dbCustomer.GDPR_Returner = Customer.GTPR_Returner;
                    dbCustomer.Proffesion = Customer.Proffesion;
                    dbCustomer.SendEmail = Customer.SendEmail;
                    dbCustomer.SendSms = Customer.SendSms;
                    dbCustomer.PhoneComp = Customer.PhoneComp;
                    dbCustomer.IsDeleted = false;
                    if (IsNew)
                        CustomerId = db.Insert<long>(AutoMapper.Mapper.Map<Delivery_CustomersDTO>(dbCustomer));
                    else
                    {
                        CustomerId = dbCustomer.ID;
                        db.Update(AutoMapper.Mapper.Map<Delivery_CustomersDTO>(dbCustomer));
                    }
                    result.CustomerType = customerType;
                    result.ExtObj = dbCustomer.ExtObj;

                    SQL = "SELECT TOP 1 ID FROM Delivery_AddressTypes";
                    int? addressType = db.Query<int?>(SQL).FirstOrDefault();

                    foreach (DA_AddressModel item in Addresses)
                    {
                        if (item.isShipping??false)
                        {
                            SQL = "SELECT * FROM Delivery_CustomersShippingAddress \n"
                                + "WHERE CustomerID = " + CustomerId.ToString() + " AND LTRIM(RTRIM(ExtKey)) = " + item.Id.ToString() + " AND ExtType = " + ((int)Customer.ExtType).ToString();
                            Delivery_CustomersShippingAddressModel adrShip = db.Query<Delivery_CustomersShippingAddressModel>(SQL).FirstOrDefault();

                            if(adrShip == null)
                            {
                                IsNew = true;
                                adrShip = new Delivery_CustomersShippingAddressModel();
                                adrShip.Type = addressType;
                                adrShip.ExtKey = item.Id.ToString();
                                adrShip.ExtType = (int)Customer.ExtType;
                                adrShip.CustomerID = CustomerId;
                            }
                            adrShip.AddressNo = item.AddressNo;
                            adrShip.AddressStreet = item.AddressStreet;
                            if(!string.IsNullOrWhiteSpace(item.Area))
                                adrShip.City = item.Area; // item.City;
                            else
                                adrShip.City = item.City;
                            adrShip.ExtId1 = item.ExtId1;
                            adrShip.ExtId2 = item.ExtId2;
                            adrShip.Floor = item.Floor;
                            adrShip.IsDeleted = false;
                            adrShip.IsSelected = true;
                            adrShip.Latitude = item.Latitude.ToString();
                            adrShip.Longtitude = item.Longtitude.ToString();
                            adrShip.VerticalStreet = item.VerticalStreet;
                            adrShip.Zipcode = item.Zipcode;
                            adrShip.Notes = item.Notes;
                            if (IsNew)
                                adrShip.ID = db.Insert<long>(AutoMapper.Mapper.Map<Delivery_CustomersShippingAddressDTO>(adrShip));
                            else
                                db.Update(AutoMapper.Mapper.Map<Delivery_CustomersShippingAddressDTO>(adrShip));
                            ShippindId = adrShip.ID ?? 0;
                            SQL = "UPDATE Delivery_CustomersShippingAddress SET IsSelected = 0 \n"
                                + "WHERE ID <> " + ShippindId.ToString() + " AND CustomerID = " + CustomerId.ToString();
                            CommandDefinition cmd = new CommandDefinition(SQL);
                            db.Execute(cmd);
                            result.ShippingAddresses.Add(AutoMapper.Mapper.Map<DeliveryCustomersShippingAddressModel>(adrShip));
                        }
                        else
                        {
                            SQL = "SELECT * FROM Delivery_CustomersBillingAddress \n"
                                + "WHERE CustomerID = " + CustomerId.ToString() + " AND LTRIM(RTRIM(ExtKey)) = " + item.Id.ToString() + " AND ExtType = " + ((int)Customer.ExtType).ToString();
                            Delivery_CustomersBillingAddressModel adrShip = db.Query<Delivery_CustomersBillingAddressModel>(SQL).FirstOrDefault();

                            if (adrShip == null)
                            {
                                IsNew = true;
                                adrShip = new Delivery_CustomersBillingAddressModel();
                                adrShip.Type = addressType;
                                adrShip.ExtKey = item.Id.ToString();
                                adrShip.ExtType = (int)Customer.ExtType;
                                adrShip.CustomerID = CustomerId;
                            }
                            adrShip.AddressNo = item.AddressNo;
                            adrShip.AddressStreet = item.AddressStreet;
                            if (!string.IsNullOrWhiteSpace(item.Area))
                                adrShip.City = item.Area; // item.City;
                            else
                                adrShip.City = item.City;
                            adrShip.ExtId1 = item.ExtId1;
                            adrShip.ExtId2 = item.ExtId2;
                            adrShip.Floor = item.Floor;
                            adrShip.IsDeleted = false;
                            adrShip.IsSelected = true;
                            adrShip.Latitude = item.Latitude.ToString();
                            adrShip.Longtitude = item.Longtitude.ToString();
                            adrShip.VerticalStreet = item.VerticalStreet;
                            adrShip.Zipcode = item.Zipcode;
                            adrShip.Notes = item.Notes;
                            if (IsNew)
                                adrShip.ID = db.Insert<long>(AutoMapper.Mapper.Map<Delivery_CustomersBillingAddressDTO>(adrShip));
                            else
                                db.Update(AutoMapper.Mapper.Map<Delivery_CustomersBillingAddressDTO>(adrShip));
                            BillingId = adrShip.ID ?? 0;
                            SQL = "UPDATE Delivery_CustomersBillingAddress SET IsSelected = 0 \n"
                                + "WHERE ID <> " + BillingId.ToString() + " AND CustomerID = " + CustomerId.ToString();
                            CommandDefinition cmd = new CommandDefinition(SQL);
                            db.Execute(cmd);
                            result.BillingAddresses.Add(AutoMapper.Mapper.Map<DeliveryCustomersBillingAddressModel>(adrShip));

                        }

                    }

                    result.BillingDOY = Customer.Doy;
                    result.BillingJob = Customer.JobName;
                    result.BillingName = Customer.LastName + " " + Customer.FirstName;
                    result.BillingVatNo = Customer.VatNo;
                    result.Comments = Customer.Notes;
                    result.DOY = Customer.Doy;
                    result.email = Customer.Email;
                    result.ExtCustId = Customer.Id.ToString();
                    result.ExtType = (int)Customer.ExtType;//  customerType;
                    result.FirstName = Customer.FirstName;
                    result.Floor = tmpAdd.Floor;
                    result.GDPR_Marketing = Customer.GTPR_Marketing;
                    result.GDPR_Returner = Customer.GTPR_Returner;
                    result.ID = CustomerId;
                    result.IsDeleted = false;
                    result.LastName = Customer.LastName;
                    result.VatNo = Customer.VatNo;
                    result.Proffesion = Customer.Proffesion;
                    result.SendSms = Customer.SendSms;
                    result.SendEmail = Customer.SendEmail;
                    result.PhoneComp = Customer.PhoneComp;
                    

                    SQL = "SELECT TOP 1 ID FROM Delivery_PhoneTypes";
                    int? PhoneType = db.Query<int?>(SQL).FirstOrDefault(); 
                    
                    DeliveryCustomersPhonesModel phone;
                    //Check Phone
                    if (!string.IsNullOrEmpty(Customer.Phone1))
                    {
                        SQL = "SELECT * FROM Delivery_CustomersPhones WHERE CustomerID = " + CustomerId.ToString() + " AND PhoneNumber = '" + Customer.Phone1 + "'";
                        phone = db.Query<DeliveryCustomersPhonesModel>(SQL).FirstOrDefault();
                        if (phone == null)
                        {
                            IsNew = true;
                            phone = new DeliveryCustomersPhonesModel();
                            phone.CustomerID = CustomerId;
                            phone.PhoneType = PhoneType;
                        }
                        phone.PhoneNumber = Customer.Phone1;
                        phone.IsSelected = true;
                        phone.IsDeleted = false;
                        if (IsNew)
                            phone.ID = db.Insert<long>(AutoMapper.Mapper.Map<Delivery_CustomersPhonesDTO>(phone));
                         else
                            db.Update(AutoMapper.Mapper.Map<Delivery_CustomersPhonesDTO>(phone));
                        Phone1Id = phone.ID;
                        result.Phones.Add(phone);
                    }
                    if (!string.IsNullOrEmpty(Customer.Phone2))
                    {
                        SQL = "SELECT * FROM Delivery_CustomersPhones WHERE CustomerID = " + CustomerId.ToString() + " AND PhoneNumber = '" + Customer.Phone2 + "'";
                        phone = db.Query<DeliveryCustomersPhonesModel>(SQL).FirstOrDefault();
                        if (phone == null)
                        {
                            IsNew = true;
                            phone = new DeliveryCustomersPhonesModel();
                            phone.CustomerID = CustomerId;
                            phone.PhoneType = PhoneType;
                        }
                        phone.PhoneNumber = Customer.Phone2;
                        phone.IsSelected = true;
                        phone.IsDeleted = false;
                        if (IsNew)
                            phone.ID = db.Insert<long>(AutoMapper.Mapper.Map<Delivery_CustomersPhonesDTO>(phone));
                        else
                            db.Update(AutoMapper.Mapper.Map<Delivery_CustomersPhonesDTO>(phone));
                        Phone2Id = phone.ID;
                        result.Phones.Add(phone);
                    }
                    if (!string.IsNullOrEmpty(Customer.Mobile))
                    {
                        SQL = "SELECT * FROM Delivery_CustomersPhones WHERE CustomerID = " + CustomerId.ToString() + " AND PhoneNumber = '" + Customer.Mobile + "'";
                        phone = db.Query<DeliveryCustomersPhonesModel>(SQL).FirstOrDefault();
                        if (phone == null)
                        {
                            IsNew = true;
                            phone = new DeliveryCustomersPhonesModel();
                            phone.CustomerID = CustomerId;
                            phone.PhoneType = PhoneType;
                        }
                        phone.PhoneNumber = Customer.Mobile;
                        phone.IsSelected = true;
                        phone.IsDeleted = false;
                        if (IsNew)
                            phone.ID = db.Insert<long>(AutoMapper.Mapper.Map<Delivery_CustomersPhonesDTO>(phone));
                        else
                            db.Update(AutoMapper.Mapper.Map<Delivery_CustomersPhonesDTO>(phone));
                        MobileId = phone.ID;
                        result.Phones.Add(phone);
                    }

                    if (Phone1Id > 0 || Phone2Id > 0 || MobileId > 0)
                    {
                        SQL = "UPDATE Delivery_CustomersPhones SET IsSelected = 0 WHERE CustomerID = " + CustomerId.ToString();
                        if (Phone1Id > 0)
                            SQL += " AND ID <> " + Phone1Id.ToString();
                        else if (Phone2Id > 0)
                            SQL += " AND ID <> " + Phone2Id.ToString();
                        else if (MobileId > 0)
                            SQL += " AND ID <> " + MobileId.ToString();
                        CommandDefinition cmd = new CommandDefinition(SQL);
                        db.Execute(cmd);
                    }
                    //Phones And Addresses Associations
                    if (ShippindId > 0 && OrderType != 21 && (Phone1Id > 0 || Phone2Id > 0 || MobileId > 0))
                    {
                        CommandDefinition cmd = new CommandDefinition();
                        if (Phone1Id > 0)
                        {
                            SQL = "IF NOT EXISTS(SELECT 1 FROM Delivery_CustomersPhonesAndAddress WHERE CustomerID = " + CustomerId.ToString() + " AND PhoneID = " + Phone1Id.ToString() + " AND AddressID = " + ShippindId.ToString() + " AND IsShipping = 1) \n "
                                + "  INSERT INTO Delivery_CustomersPhonesAndAddress(CustomerID, PhoneID, AddressID, IsShipping) VALUES(" + CustomerId.ToString() + "," + Phone1Id.ToString() + "," + ShippindId.ToString() + ",1)";
                            cmd = new CommandDefinition(SQL);
                            db.Execute(cmd);
                        }
                        if (Phone2Id > 0)
                        {
                            SQL = "IF NOT EXISTS(SELECT 1 FROM Delivery_CustomersPhonesAndAddress WHERE CustomerID = " + CustomerId.ToString() + " AND PhoneID = " + Phone2Id.ToString() + " AND AddressID = " + ShippindId.ToString() + " AND IsShipping = 1) \n "
                                + "  INSERT INTO Delivery_CustomersPhonesAndAddress(CustomerID, PhoneID, AddressID, IsShipping) VALUES(" + CustomerId.ToString() + "," + Phone2Id.ToString() + "," + ShippindId.ToString() + ",1)";
                            cmd = new CommandDefinition(SQL);
                            db.Execute(cmd);
                        }
                        if (MobileId > 0)
                        {
                            SQL = "IF NOT EXISTS(SELECT 1 FROM Delivery_CustomersPhonesAndAddress WHERE CustomerID = " + CustomerId.ToString() + " AND PhoneID = " + MobileId.ToString() + " AND AddressID = " + ShippindId.ToString() + " AND IsShipping = 1) \n "
                                + "  INSERT INTO Delivery_CustomersPhonesAndAddress(CustomerID, PhoneID, AddressID, IsShipping) VALUES(" + CustomerId.ToString() + "," + MobileId.ToString() + "," + ShippindId.ToString() + ",1)";
                            cmd = new CommandDefinition(SQL);
                            db.Execute(cmd);
                        }
                    }
                    if (BillingId > 0 && OrderType != 21 && (Phone1Id > 0 || Phone2Id > 0 || MobileId > 0))
                    {
                        CommandDefinition cmd = new CommandDefinition();
                        if (Phone1Id > 0)
                        {
                            SQL = "IF NOT EXISTS(SELECT 1 FROM Delivery_CustomersPhonesAndAddress WHERE CustomerID = " + CustomerId.ToString() + " AND PhoneID = " + Phone1Id.ToString() + " AND AddressID = " + BillingId.ToString() + " AND IsShipping = 0) \n "
                                + "  INSERT INTO Delivery_CustomersPhonesAndAddress(CustomerID, PhoneID, AddressID, IsShipping) VALUES(" + CustomerId.ToString() + "," + Phone1Id.ToString() + "," + BillingId.ToString() + ",0)";
                            cmd = new CommandDefinition(SQL);
                            db.Execute(cmd);
                        }
                        if (Phone2Id > 0)
                        {
                            SQL = "IF NOT EXISTS(SELECT 1 FROM Delivery_CustomersPhonesAndAddress WHERE CustomerID = " + CustomerId.ToString() + " AND PhoneID = " + Phone2Id.ToString() + " AND AddressID = " + BillingId.ToString() + " AND IsShipping = 0) \n "
                                + "  INSERT INTO Delivery_CustomersPhonesAndAddress(CustomerID, PhoneID, AddressID, IsShipping) VALUES(" + CustomerId.ToString() + "," + Phone2Id.ToString() + "," + BillingId.ToString() + ",0)";
                            cmd = new CommandDefinition(SQL);
                            db.Execute(cmd);
                        }
                        if (MobileId > 0)
                        {
                            SQL = "IF NOT EXISTS(SELECT 1 FROM Delivery_CustomersPhonesAndAddress WHERE CustomerID = " + CustomerId.ToString() + " AND PhoneID = " + MobileId.ToString() + " AND AddressID = " + BillingId.ToString() + " AND IsShipping = 0) \n "
                                + "  INSERT INTO Delivery_CustomersPhonesAndAddress(CustomerID, PhoneID, AddressID, IsShipping) VALUES(" + CustomerId.ToString() + "," + MobileId.ToString() + "," + BillingId.ToString() + ",0)";
                            cmd = new CommandDefinition(SQL);
                            db.Execute(cmd);
                        }
                    }

                    DA_AddressModel shipAdr = Addresses.Find(f => f.isShipping == true);
                    DA_AddressModel billAdr = Addresses.Find(f => f.isShipping == false);



                    SQL = "SELECT * FROM Guest WHERE ProfileNo = " + CustomerId.ToString();

                    guest = db.Query<GuestModel>(SQL).FirstOrDefault();
                    if(guest == null)
                    {
                        IsNew = true;
                        guest = new GuestModel();
                        guest.ProfileNo = (int?)CustomerId;
                    }
                    guest.FirstName = Customer.FirstName;
                    guest.LastName = Customer.LastName;
                    guest.Address = shipAdr != null ? shipAdr.AddressStreet : (billAdr != null ? billAdr.AddressStreet : "");
                    guest.City = shipAdr != null ? shipAdr.Area : (billAdr != null ? billAdr.Area : "");
                    guest.PostalCode = shipAdr != null ? shipAdr.Zipcode : (billAdr != null ? billAdr.Zipcode : "");
                    guest.Email = Customer.Email;
                    guest.Telephone = !string.IsNullOrEmpty(Customer.Phone1) ? Customer.Phone1 : (!string.IsNullOrEmpty(Customer.Phone2) ? Customer.Phone2 : Customer.Mobile);
                    guest.Note1 = Customer.Notes;

                    if (IsNew)
                        guest.Id = db.Insert<long>(AutoMapper.Mapper.Map<GuestDTO>(guest));
                    else
                        db.Update(AutoMapper.Mapper.Map<GuestDTO>(guest));
                }
            }
            catch (Exception ex)
            {
                logger.Error("UpsertCustomer [SQL: " + SQL + "] " + ex.ToString());
            }
            return result;
        }

        /// <summary>
        /// Check's if the order from DA exists and returns last order status.
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="headOrder"></param>
        /// <param name="lastStatus"></param>
        /// <returns></returns>
        public bool CheckIfDA_OrderExists(DBInfoModel Store, DA_OrderModel headOrder, DA_StoreModel daStore, out OrderStatusEnum lastStatus)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            lastStatus = 0;
            if (headOrder.StoreOrderId > 0)
            {
                string sql = "SELECT o.status  \n"
                           + "FROM OrderStatus AS o \n"
                           + "CROSS APPLY ( \n"
                           + "	SELECT os.OrderId, MAX(os.TimeChanged) TimeChanged \n"
                           + "	FROM OrderStatus AS os \n"
                           + "	WHERE os.OrderId = " + headOrder.StoreOrderId.ToString() + "  \n"
                           + "	GROUP BY os.OrderId \n"
                           + ") a  \n"
                           + "WHERE o.TimeChanged = a.TimeChanged";
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    db.Open();
                    lastStatus = db.Query<OrderStatusEnum>(sql).FirstOrDefault();
                    if (lastStatus == 0)
                        lastStatus = 0;
                    return true;
                }
            }
            else
            {
                string SQL = "SELECT Id FROM [Order] \n"
                            + "WHERE ISNULL(ExtType,0) = " + headOrder.ExtType.ToString()+ " AND LTRIM(RTRIM(ISNULL(ExtKey,''))) = " + headOrder.Id.ToString() + " \n";
                if (headOrder.PosId > 0)
                    SQL += " AND PosId = " + headOrder.PosId.ToString() + " \n";
                if (daStore.PosStaffId > 0)
                    SQL += " AND StaffId = " + daStore.PosStaffId.ToString() + " \n";

                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    db.Open();
                    int OrderId = db.Query<int>(SQL).FirstOrDefault();

                    if (OrderId < 1)
                        return false;
                    else
                    {
                        string sql = "SELECT o.status  \n"
                           + "FROM OrderStatus AS o \n"
                           + "CROSS APPLY ( \n"
                           + "	SELECT os.OrderId, MAX(os.TimeChanged) TimeChanged \n"
                           + "	FROM OrderStatus AS os \n"
                           + "	WHERE os.OrderId = " + OrderId.ToString() + "  \n"
                           + "	GROUP BY os.OrderId \n"
                           + ") a  \n"
                           + "WHERE o.TimeChanged = a.TimeChanged";
                        lastStatus = db.Query<OrderStatusEnum>(sql).FirstOrDefault();
                        if (lastStatus == 0)
                            lastStatus = 0;
                        return true;
                    }

                }

            }
        }

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
        public List<ReceiptDetails> CreateReceiptDetailsFromProductCodesForDA(DBInfoModel Store, DA_NewOrderModel order, List<VatModel> vmModel,
            PosInfoDetailModel PosInfoDetail, InvoiceTypeModel InvoiceType, out string ErrorMess)
        {
            ErrorMess = "";
            List<ReceiptDetails> retrds = new List<ReceiptDetails>();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                if (order.OrderDetails == null || order.OrderDetails.Count < 1)
                {
                    ErrorMess = Resources.Errors.MISSING_PRODUCTS_FOR_DELIVERYAGENT_ORDER;
                    return null;
                    //throw new Exception(Resources.Errors.MISSING_PRODUCTS_FOR_DELIVERYAGENT_ORDER);
                }
                VatModel chkVat;
                foreach (DA_OrderDetailExtModel item in order.OrderDetails)
                {
                    if (string.IsNullOrEmpty(item.ProductCode))
                    {
                        ErrorMess = string.Format(Resources.Errors.MISSING_PRODUCTCODE_FOR_DA, (order.StoreOrderId > 0 ? order.StoreOrderId : order.Id).ToString(), item.ProductId.ToString());
                        return null;
                    }
                    if (item.StoreProductId < 1)
                    {
                        ErrorMess = string.Format(Resources.Errors.MISSING_PRODUCTID_FOR_DA, (order.StoreOrderId > 0 ? order.StoreOrderId : order.Id).ToString(), item.ProductId.ToString());
                        return null;
                    }
                    if (item.StorePriceListId < 1)
                    {
                        ErrorMess = string.Format(Resources.Errors.MISSING_PRODUCTPRICELIST_FOR_DA, (order.StoreOrderId > 0 ? order.StoreOrderId : order.Id).ToString(), item.ProductId.ToString());
                        return null;
                    }
                    if (item.ProductCategoryId < 1)
                    {
                        ErrorMess = string.Format(Resources.Errors.MISSING_PRODUCTCATEGORY_FOR_DA, (order.StoreOrderId > 0 ? order.StoreOrderId : order.Id).ToString(), item.ProductId.ToString());
                        return null;
                    }
                    chkVat = vmModel.Find(f => f.Percentage == item.RateVat); // fposents.VatList.First(f => f.Percentage.Equals(item.RateVat * 100 - 100));
                    if (chkVat == null)
                    {
                        ErrorMess = string.Format(Resources.Errors.MISSING_PRODUCTVAT_FOR_DA, (order.StoreOrderId > 0 ? order.StoreOrderId : order.Id).ToString(), item.ProductId.ToString());
                        return null;
                    }
                }
                if (order.PosId < 0) logger.Error("         fposents.PosInfo is null");
                if (order.PosStaffId < 0) logger.Error("         fposents.Staff is null");
                if (PosInfoDetail == null) logger.Error("         fposents.PosInfoDetail is null");

                foreach (DA_OrderDetailExtModel item in order.OrderDetails)
                {
                    chkVat = vmModel.Find(f => f.Percentage == item.RateVat);
                    ReceiptDetails rd = new ReceiptDetails
                    {
                        ReceiptsId = null,
                        EndOfDayId = null,
                        PosInfoId = order.PosId,
                        StaffId = order.PosStaffId,
                        Abbreviation = InvoiceType.Abbreviation,
                        InvoiceType = order.InvoiceType,
                        //long? OrderDetailId 
                        PosInfoDetailId = PosInfoDetail.Id,
                        ItemCode = item.ProductCode,
                        ItemDescr = item.ProductDescription, //prod.Description,
                        ItemQty = (double)item.Qnt,
                        Price = item.Price, // Total after discount 
                        ItemGross = item.Total,
                        ItemDiscount = item.Discount,
                        ItemVatRate = item.RateVat,
                        ItemVatValue = item.TotalVat,
                        //long? TaxId 
                        //decimal? ItemTaxAmount 
                        ItemNet = item.NetAmount,
                        VatId = chkVat.Id,
                        VatCode = chkVat.Code,
                        Status = 1,
                        PaidStatus = 0, //(order.payment_method.Equals(DeliveryForkeyPaymentEnum.CASH.ToString()) || order.payment_method.Equals(DeliveryForkeyPaymentEnum.MPOS.ToString())) ? 0 : 1,
                        KitchenId = item.KitchenId,
                        PreparationTime = item.PreparationTime,
                        KdsId = item.KdsId,
                        //Guid? Guid 
                        //string TableCode ='' //long? TableId = null  //long? RegionId = null 
                        //long? OrderNo = 
                        //long? OrderId = 
                        PriceListDetailId = item.PriceListDetailId,
                        PricelistId = item.StorePriceListId, // item.PriceListId,
                        ProductId = item.StoreProductId, // item.Id,
                        IsExtra = item.IsExtra,
                        IsInvoiced = order.InvoiceType == 7,
                        SalesTypeId = (int) order.OrderType,
                        ProductCategoryId = item.ProductCategoryId,
                        CategoryId = item.CategoryId,
                        //long? CategoryId = prod
                        ItemPosition = 0,
                        ItemSort = order.OrderDetails.IndexOf(item),
                        //String ItemRegion 
                        //int? RegionPosition 
                        ItemBarcode = 0,
                        TotalBeforeDiscount = item.Price,
                        ReceiptSplitedDiscount = 0
                    };
                    retrds.Add(rd);

                }
            }
            return retrds;
        }


    }
}
