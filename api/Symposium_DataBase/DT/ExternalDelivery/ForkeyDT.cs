

using Dapper;
using log4net;
using Symposium.Helpers;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.ExternalDelivery;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.DT.ExternalDelivery;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Transactions;

namespace Symposium.WebApi.DataAccess.DT.ExternalDelivery
{
    public class ForkeyDT : IForkeyDT
    {
        string connectionString;
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<Delivery_CustomersDTO> genCustomer;
        IGenericDAO<Delivery_CustomersBillingAddressDTO> genBillAddress;
        IGenericDAO<Delivery_CustomersShippingAddressDTO> genShipAddress;
        IGenericDAO<Delivery_CustomersPhonesDTO> genPhone;
        IGenericDAO<OrderDTO> genorder;
        IPricelistDetailDT priceListDetails;

        IDeliveryCustomersDAO idcd;
        IPosInfoDAO posinfod;
        IPosInfoDetailDAO posinfodd;
        IPricelistDAO priceld;
        ISalesTypeDAO salestd;
        IStaffDAO staffd;
        IInvoiceTypeDAO invtd;
        IAccountDAO accd;
        IVatDAO vatd;
        IProductDAO prodd;
        IOrderDAO ordd;
        IInvoicesDAO invd;
        IOrderStatusDAO ostd;

        ICustomJsonSerializers cjson;

        public ForkeyDT(
            IUsersToDatabasesXML _usersToDatabases,
            IGenericDAO<Delivery_CustomersDTO> _genCustomer,
            IGenericDAO<Delivery_CustomersBillingAddressDTO> _genBillAddress,
            IGenericDAO<Delivery_CustomersShippingAddressDTO> _genShipAddress,
            IGenericDAO<Delivery_CustomersPhonesDTO> _genPhone,
            IGenericDAO<OrderDTO> _genorder,

            IDeliveryCustomersDAO _idcd,
            IPosInfoDAO _posinfod, IPosInfoDetailDAO _posinfodd, IPricelistDAO _priceld, ISalesTypeDAO _salestd, IStaffDAO _staffd, IInvoiceTypeDAO _invtd, IAccountDAO _accd, IVatDAO _vatd,
            IProductDAO _prodd, IOrderDAO _ordd, IInvoicesDAO _invd, IOrderStatusDAO _ostd,

            ICustomJsonSerializers _cjson,
            IPricelistDetailDT _priceListDetails
            )
        {
            idcd = _idcd;
            usersToDatabases = _usersToDatabases;
            genCustomer = _genCustomer;
            genBillAddress = _genBillAddress;
            genShipAddress = _genShipAddress;
            genPhone = _genPhone;
            genorder = _genorder;

            accd = _accd;
            posinfod = _posinfod; posinfodd = _posinfodd; priceld = _priceld; salestd = _salestd; staffd = _staffd; invtd = _invtd; vatd = _vatd;
            prodd = _prodd; ordd = _ordd; invd = _invd; ostd = _ostd;

            cjson = _cjson;
            priceListDetails = _priceListDetails;
        }

        /// <summary>
        /// Provides basic lookpus for forkey functionallity
        /// PosInfo, PosInfo Details, Pricelists, SalesTypes , Accounts, Staff, InvoiceTypes
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public ForkeyLookups GetLookups(DBInfoModel Store)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                ForkeyLookups retlookups = new ForkeyLookups
                {
                    PosInfoList = AutoMapper.Mapper.Map<List<PosInfoModel>>(posinfod.SelectAll(db)),
                    PosInfoDetailList = AutoMapper.Mapper.Map<List<PosInfoDetailModel>>(posinfodd.SelectAll(db)),
                    PricelistList = AutoMapper.Mapper.Map<List<PricelistModel>>(priceld.SelectAll(db)),
                    SalesTypeList = AutoMapper.Mapper.Map<List<SalesTypesModels>>(salestd.SelectAll(db)),
                    AccountList = AutoMapper.Mapper.Map<List<AccountModel>>(accd.SelectAll(db)),
                    StaffList = AutoMapper.Mapper.Map<List<StaffModels>>(staffd.SelectAll(db)),
                    InvoiceTypeList = AutoMapper.Mapper.Map<List<InvoiceTypeModel>>(invtd.SelectAll(db)),
                    VatList = AutoMapper.Mapper.Map<List<VatModel>>(vatd.SelectAll(db)),

                    DeliveryCustomerLookups = idcd.GetLookups(db)
                };
                return retlookups;
            }
        }

        /// <summary>
        /// Based on forkey order dishes recipe external_id witch is the mapping entity with the Product.Code 
        /// on WebPOS DB creates a receipt detail list and applies items based on recipe code
        /// Method also checks if forkeyorder vat is a valid pos ent else it returns a Vat missmatch
        /// Then it asks for product with specific code and if it does not exist on DB then returns a MISSING_DISH_ID error
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="order"></param>
        /// <param name="fposents"></param>
        /// <returns></returns>
        public List<ReceiptDetails> CreateReceiptDetailsFromProductCodes(DBInfoModel Store, ForkeyDeliveryOrder order, ForkeyPosEntities fposents)
        {
            List<ReceiptDetails> retrds = new List<ReceiptDetails>();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //VatModel cmv = fposents.VatList.Find(xx => xx.Percentage.Equals(order.vat * 100 - 100));
                //if (cmv == null && fposents.VatList.Count() > 0)
                //{
                //    new ForkeyException(DeliveryForkeyErrorEnum.VAT_MISMATCH.ToString());
                //}
                foreach (ForkeyDishes item in order.dishes)
                {
                    ProductModel prod = AutoMapper.Mapper.Map<ProductModel>(prodd.SelectByCode(db, item.dish.recipe.external_id));
                    if (prod == null)
                    {
                        new ForkeyException(DeliveryForkeyErrorEnum.MISSING_DISH_ID.ToString() + ", missing external_id:" + item.dish.recipe.external_id);
                    }
                    else
                    {
                        if (fposents == null) logger.Error("         fposents is null");
                        else
                        {
                            if (fposents.PosInfo == null) logger.Error("         fposents.PosInfo is null");
                            if (fposents.Staff == null) logger.Error("         fposents.Staff is null");
                            if (fposents.PosInfoDetail == null) logger.Error("         fposents.PosInfoDetail is null");
                        }
                        PricelistDetailModel pricelistDetailModel = new PricelistDetailModel();
                        VatModel vatModel = new VatModel();
                        pricelistDetailModel = priceListDetails.SelectPricelistDetailForProductAndPricelist(Store, prod.Id, fposents.Pricelist.Id);
                        vatModel = db.Query<VatModel>("SELECT * FROM Vat AS v WHERE v.Id =@vatId", new { vatId = pricelistDetailModel.VatId }).FirstOrDefault();

                        ReceiptDetails rd = new ReceiptDetails
                        {
                            ReceiptsId = null,
                            EndOfDayId = null,
                            PosInfoId = fposents.PosInfo.Id,
                            StaffId = fposents.Staff.Id,
                            Abbreviation = fposents.PosInfoDetail.Abbreviation,
                            InvoiceType = (short?)fposents.PosInfoDetail.InvoicesTypeId,
                            //long? OrderDetailId 
                            PosInfoDetailId = fposents.PosInfoDetail.Id,
                            ItemCode = prod.Code,
                            ItemDescr = (!string.IsNullOrEmpty(item.dish.recipe.name)) ? item.dish.recipe.name :
                            ((!string.IsNullOrEmpty(item.dish.recipe.code_name)) ? item.dish.recipe.code_name : item.dish.recipe.description), //prod.Description,
                            ItemQty = item.portions,
                            Price = item.dish.cost, // Total after discount 
                            ItemGross = item.dish.cost,
                            ItemDiscount = 0,
                            //ItemVatRate = (order.vat * 100) - 100,
                            ItemVatRate = vatModel.Percentage,
                            //ItemVatValue = item.dish.cost - (item.dish.cost / order.vat),
                            ItemVatValue = item.dish.cost / (1 + vatModel.Percentage/100),
                            //long? TaxId 
                            //decimal? ItemTaxAmount 
                            //ItemNet = item.dish.cost / order.vat,
                            ItemNet = item.dish.cost - (item.dish.cost / (1 + vatModel.Percentage / 100)),
                            VatId = pricelistDetailModel.VatId,
                            VatCode = vatModel.Code,
                            Status = 1,
                            PaidStatus = 0, //(order.payment_method.Equals(DeliveryForkeyPaymentEnum.CASH.ToString()) || order.payment_method.Equals(DeliveryForkeyPaymentEnum.MPOS.ToString())) ? 0 : 1,
                            KitchenId = prod.KitchenId,
                            PreparationTime = prod.PreparationTime,
                            KdsId = prod.KdsId,
                            //Guid? Guid 
                            //string TableCode ='' //long? TableId = null  //long? RegionId = null 
                            //long? OrderNo = 
                            //long? OrderId = 
                            //PriceListDetailId = null,
                            PricelistId = fposents.Pricelist.Id,
                            ProductId = prod.Id,
                            IsExtra = (byte)0,//(item.dish.is_main != true) ? (byte)1 : (byte)0,
                                              //string ItemRemark 
                                              //long? OrderDetailIgredientsId 
                            IsInvoiced = false,
                            SalesTypeId = fposents.SalesType.Id,
                            ProductCategoryId = prod.ProductCategoryId,
                            //long? CategoryId = prod
                            ItemPosition = 0,
                            ItemSort = order.dishes.IndexOf(item),
                            //String ItemRegion 
                            //int? RegionPosition 
                            ItemBarcode = 0,
                            TotalBeforeDiscount = item.dish.cost,
                            ReceiptSplitedDiscount = 0
                        };
                        retrds.Add(rd);
                    }
                }
            }
            return retrds;
        }

        /// <summary>
        /// Provide a filter model to return LocalEntities of Forkey Order 
        /// Order, Invoices, OrderStatuses
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ForkeyLocalEntities GetForkeyOrderLocalEntities(DBInfoModel Store, ForkeyDeliveryOrder model)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            OrderFilterModel filter = new OrderFilterModel
            {
                PosId = model.dependencies.PosInfoId,
                StaffId = model.dependencies.StaffId,
                EndOfDayId = null,
                ExtKey = model.id.ToString(),
                ExtType = (int)ExternalSystemOrderEnum.Forkey,
                IsDeleted = false,
            };
            return FilterEntities(connectionString, filter);
        }

        /// <summary>
        /// Function providing storeid and orderid  parses Forkey order entities 
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public ForkeyLocalEntities GetForkeyEntities(string storeid, long orderid, int? extType)
        {
            Guid strid = new Guid(storeid);
            connectionString = usersToDatabases.ConfigureConnectionString(strid);

            OrderFilterModel filter = new OrderFilterModel
            {
                Id = orderid,
                ExtType = (extType == 0 ? null : extType),//(int)ExternalSystemOrderEnum.Forkey,
                IsDeleted = false,
            };


            return FilterEntities(connectionString, filter);
        }

        /// <summary>
        /// Providing storeid and orderno returns forkey entities
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="orderno"></param>
        /// <returns></returns>
        public ForkeyLocalEntities GetForkeyEntitiesByOrderNo(string storeid, string orderno)
        {
            Guid strid = new Guid(storeid);
            connectionString = usersToDatabases.ConfigureConnectionString(strid);
            OrderFilterModel filter = new OrderFilterModel
            {
                OrderNo = orderno,
                ExtType = (int)ExternalSystemOrderEnum.Forkey,
                IsDeleted = false,
            };
            return FilterEntities(connectionString, filter);
        }

        /// <summary>
        /// Based on storeid provided and filter for invoice
        /// </summary>
        /// <param name="storeid">Store id to open db Connection</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public ForkeyLocalEntities FilterEntities(string connectionString, OrderFilterModel filter)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                List<OrderDTO> orders = ordd.FilteredOrders(db, filter);
                if (orders.Count > 1)
                {
                    throw new ForkeyException(DeliveryForkeyErrorEnum.MORE_THAN_ONE_SAMEID.ToString());
                }
                else if (orders.Count > 0)
                {
                    OrderDTO single = orders.FirstOrDefault();
                    List<InvoiceModel> invs = AutoMapper.Mapper.Map<List<InvoiceModel>>(invd.InvoicesByOrderNo(db, single.OrderNo.ToString()));
                    List<OrderStatusModel> osl = AutoMapper.Mapper.Map<List<OrderStatusModel>>(ostd.FilteredOrderStatus(db, new OrderStatusFilter { OrderId = single.Id }));
                    return new ForkeyLocalEntities() { Order = AutoMapper.Mapper.Map<OrderModel>(single), InvoiceList = invs, OrderStatusList = osl };
                }
                else
                {
                    throw new Exception("No Forkey local Order to get entities. Possible fail on post invoice.");
                }
            }
        }

        /// <summary>
        /// Based on forkey order creates a dynamic order filter asks order DAO  for entries with external key
        /// and external type having endof day id null and isdeleted false  returnes true if an order found with current creteria 
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool CheckExist(DBInfoModel Store, ForkeyDeliveryOrder model)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);

            OrderFilterModel filter = new OrderFilterModel
            {
                ExtKey = model.id.ToString(),
                ExtType = (int)ExternalSystemOrderEnum.Forkey
            };

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return ordd.OrderExists(db, filter);
            }
        }


        /// <summary>
        /// Based on invoice id joins entities to update Order.ExtObj.Deseriallized.isPrinted
        /// selects invoices, binded to orderdetailinvs, binded to orderdetail , binded to Order
        /// Deserializes obj then updates isPrinted from Obj changes value then updates orders collected
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="printed"></param>
        /// <returns></returns>
        public List<OrderDTO> ChangeForkeyIsPrintedExtObj(DBInfoModel Store, long InvoiceId, bool printed = true)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    string q = @"SELECT * FROM [Order] AS o WHERE o.Id IN (
                                    select Distinct o.Id from invoices as i
                                    inner join OrderDetailInvoices as odi on i.id = odi.InvoicesId
                                    inner join OrderDetail as od on odi.OrderDetailId = od.Id
                                    inner join[Order] as o on o.Id = od.OrderId
                                    where i.EndOfDayId is null and ( o.ExtType = " + (int)ExternalSystemOrderEnum.Forkey + " or o.ExtType = " + (int)ExternalSystemOrderEnum.VivardiaNoKitchen + ") and o.ExtObj is not null and i.Id = @invId)";

                    List<OrderDTO> orders = genorder.Select(q, new { invId = InvoiceId }, db);
                    if (orders != null && orders.Count > 0)
                    {
                        foreach (OrderDTO o in orders)
                        {
                            dynamic e = new ExpandoObject();
                            e = cjson.DynamicDeseriallize(o.ExtObj);
                            e.isPrinted = printed;
                            //if (e != null && e.isPrinted != null && e.isPrinted != true) { } else { e.isPrinted = printed; }
                            o.ExtObj = cjson.DynamicToJson(e);
                        }
                        int rows = genorder.UpdateList(db, orders);
                    }
                    return orders;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString()); throw new Exception(Symposium.Resources.Errors.UPDATEFAILED);
                }
            }

        }

    }
}
