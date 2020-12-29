using Autofac;
using Symposium.WebApi.DataAccess.DAOs;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium.WebApi.DataAccess.DT;
using Symposium.WebApi.DataAccess.XMLs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symposium.WebApi.DataAccess.DT.TableReservations;
using Symposium.WebApi.DataAccess.Interfaces.DT.TableReservations;
using Symposium.WebApi.DataAccess.DT.ExternalDelivery;
using Symposium.WebApi.DataAccess.Interfaces.DT.ExternalDelivery;
using Symposium.WebApi.DataAccess.DAOs.Delivery;
using Symposium.WebApi.DataAccess.Interfaces.DAO.Delivery;
using Symposium.WebApi.DataAccess.DT.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent;
using Symposium.WebApi.DataAccess.DT.ExternalSystems;
using Symposium.WebApi.DataAccess.Interfaces.DT.ExternalSystems;
using Symposium.WebApi.DataAccess.DT.Promos;
using Symposium.WebApi.DataAccess.Interfaces.DT.Promos;
using Symposium.WebApi.DataAccess.DT.Products;
using Symposium.WebApi.DataAccess.Interfaces.DT.Products;
using Symposium.WebApi.DataAccess.Interfaces.DT.Goodys;
using Symposium.WebApi.DataAccess.DT.Goodys;
using Symposium.WebApi.DataAccess.Interfaces.DAO.Goodys;
using Symposium.WebApi.DataAccess.DAOs.Goodys;
using Symposium.WebApi.DataAccess.DT.HotelInfo;
using Symposium.WebApi.DataAccess.Interfaces.DT.HotelInfo;
using Symposium.WebApi.DataAccess.DAOs.HotelInfo;
using Symposium.WebApi.DataAccess.Interfaces.DAO.HotelInfo;
using Symposium.WebApi.DataAccess.DT.Infrastructure;
using Symposium.WebApi.DataAccess.Interfaces.DT.Infrastructure;
using Symposium.WebApi.DataAccess.DT.Payroll;
using Symposium.WebApi.DataAccess.Interfaces.DT.Payroll;
using Symposium.WebApi.DataAccess.DT.CashedObjects;
using Symposium_DTOs.PosModel_Info;
using Symposium.WebApi.DataAccess.Interfaces.DT.CashedObjects;
using Symposium.WebApi.DataAccess.DT.Hotel;
using Symposium.WebApi.DataAccess.Interfaces.DT.Hotel;
using Symposium.WebApi.DataAccess.DT.Orders;
using Symposium.WebApi.DataAccess.Interfaces.DT.Orders;
using Symposium.WebApi.DataAccess.DT.Delivery;
using Symposium.WebApi.DataAccess.Interfaces.DT.Delivery;
using Symposium.WebApi.DataAccess.DT.Kds;
using Symposium.WebApi.DataAccess.Interfaces.DT.Kds;

namespace Symposium.WebApi.DataAccess
{
   public class DataAccessDIModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
    
            //Register DataAccess classes 
            builder.RegisterType<UsersToDatabasesXML>().As<IUsersToDatabasesXML>();
            builder.RegisterType<EndOfDayDT>().As<IEndOfDayDT>();
            builder.RegisterType<RegionLockersDT>().As<IRegionLockersDT>();
            builder.RegisterType<EndOfDayDAO>().As<IEndOfDayDAO>();
            builder.RegisterType<AccountsDT>().As<IAccountsDT>();
            builder.RegisterType<InvoiceTypesDT>().As<IInvoiceTypesDT>();
            builder.RegisterType<TableForDisplayDT>().As<ITableForDisplayDT>();
            builder.RegisterType<PageButtonDT>().As<IPageButtonDT>();
            builder.RegisterType<StoreDT>().As<IStoreDT>();
            builder.RegisterType<StoreMessagesDT>().As<IStoreMessagesDT>();
            builder.RegisterType<CustomerDT>().As<ICustomerDT>();
            builder.RegisterType<RegionDT>().As<IRegionDT>();
            builder.RegisterType<PagesDT>().As<IPagesDT>();
            builder.RegisterType<PredefinedCreditsDT>().As<IPredefinedCreditsDT>();
            builder.RegisterType<StaffDT>().As<IStaffDT>();
            builder.RegisterType<SalesPosLookupsDT>().As<ISalesPosLookupsDT>();
            builder.RegisterType<ReceiptDetailsForExtecrDT>().As<IReceiptDetailsForExtecrDT>();
            builder.RegisterType<GdprDT>().As<IGdprDT>();
            //Table Reservations
            builder.RegisterType<ReservationTypesDT>().As<IReservationTypesDT>();
            builder.RegisterType<RestaurantsDT>().As<IRestaurantsDT>();
            builder.RegisterType<RestrictionsRestaurantsAssocDT>().As<IRestrictionsRestaurantsAssocDT>();
            builder.RegisterType<ExcludeDaysDT>().As<IExcludeDaysDT>();
            builder.RegisterType<ExcludeRestrictionsDT>().As<IExcludeRestrictionsDT>();
            builder.RegisterType<CapacitiesDT>().As<ICapacitiesDT>();
            builder.RegisterType<RestrictionsDT>().As<IRestrictionDT>();
            builder.RegisterType<OverwrittenCapacitiesDT>().As<IOverwrittenCapacitiesDT>();
            builder.RegisterType<ReservationsDT>().As<IReservationsDT>();
            builder.RegisterType<ReservationCustomersDT>().As<IReservationCustomersDT>();
            builder.RegisterType<ConfigDT>().As<IConfigDT>();
            builder.RegisterType<EmailConfigDT>().As<IEmailConfigDT>();
            builder.RegisterType<EmailDT>().As<IEmailDT>();
            builder.RegisterType<DA_ConfigDT>().As<IDA_ConfigDT>();
            builder.RegisterType<DA_AddressesDT>().As<IDA_AddressesDT>();

            //DeliveryAgent
            builder.RegisterType<DA_CustomerDT>().As<IDA_CustomerDT>();
            builder.RegisterType<DA_StaffDT>().As<IDA_StaffDT>();
            builder.RegisterType<DA_OrdersDT>().As<IDA_OrdersDT>();
            builder.RegisterType<DA_GeoPolygonsDT>().As<IDA_GeoPolygonsDT>();
            builder.RegisterType<DA_Store_PriceListAssocDT>().As<IDA_Store_PriceListAssocDT>();
            builder.RegisterType<DA_StoresDT>().As<IDA_StoresDT>();
            builder.RegisterType<DA_LoyaltyDT>().As<IDA_LoyaltyDT>();
            builder.RegisterType<DA_ShortagesDT>().As<IDA_ShortagesDT>();
            builder.RegisterType<DA_ConfigDT>().As<IDA_ConfigDT>();
            builder.RegisterType<DA_OrderNoDT>().As<IDA_OrderNoDT>(); 
            builder.RegisterType<DA_PhoneticsDT>().As<IDA_PhoneticsDT>();

            builder.RegisterType<EfoodDT>().As<IEfoodDT>(); 

            builder.RegisterType<ReportDT>().As<IReportDT>();
            builder.RegisterType<ReportDAO>().As<IReportDAO>();
            builder.RegisterType<InvoicesDT>().As<IInvoicesDT>();
            builder.RegisterType<InvoicesDAO>().As<IInvoicesDAO>();
            builder.RegisterType<OrderDetailInvoicesDT>().As<IOrderDetailInvoicesDT>();
            builder.RegisterType<PosInfoDT>().As<IPosInfoDT>();
            builder.RegisterType<PosInfoDAO>().As<IPosInfoDAO>();

            builder.RegisterType<PosInfoDetailDAO>().As<IPosInfoDetailDAO>();

            builder.RegisterType<PricelistDAO>().As<IPricelistDAO>();
            builder.RegisterType<SalesTypeDAO>().As<ISalesTypeDAO>();
            builder.RegisterType<AccountDAO>().As<IAccountDAO>();
            builder.RegisterType<StaffDAO>().As<IStaffDAO>();
            builder.RegisterType<InvoiceTypeDAO>().As<IInvoiceTypeDAO>();
            builder.RegisterType<VatDAO>().As<IVatDAO>();

            builder.RegisterType<ProductDAO>().As<IProductDAO>();
            builder.RegisterType<OrderDAO>().As<IOrderDAO>();
            builder.RegisterType<OrderStatusDAO>().As<IOrderStatusDAO>();

            builder.RegisterType<PricelistDetailDT>().As<IPricelistDetailDT>();
            builder.RegisterType<PricelistDetailDAO>().As<IPricelistDetailDAO>();


            builder.RegisterType<HotelInfoDT>().As<IHotelInfoDT>();

            builder.RegisterType<PosInfoDetailDT>().As<IPosInfoDetailDT>();
            builder.RegisterType<VatDT>().As<IVatDT>();

            builder.RegisterType<NFCcardDT>().As<INFCcardDT>();
            builder.RegisterType<LockersDT>().As<ILockersDT>();

            builder.RegisterType<UnitsDT>().As<IUnitsDT>();
            builder.RegisterType<TaxDT>().As<ITaxDT>();
            builder.RegisterType<CategoriesDT>().As<ICategoriesDT>();
            builder.RegisterType<ProductCategoriesDT>().As<IProductCategoriesDT>();
            builder.RegisterType<PageButtonDetailDT>().As<IPageButtonDetailDT>();
            builder.RegisterType<PricelistDT>().As<IPricelistDT>();
            builder.RegisterType<IngredientsDT>().As<IIngredientsDT>();
            builder.RegisterType<IngredientCategoriesDT>().As<IIngredientCategoriesDT>();
            builder.RegisterType<Ingredient_ProdCategoryAssocDT>().As<IIngredient_ProdCategoryAssocDT>();
            builder.RegisterType<ProductExtrasDT>().As<IProductExtrasDT>();
            builder.RegisterType<PriceList_EffectiveHoursDT>().As<IPriceList_EffectiveHoursDT>();
            builder.RegisterType<SalesTypeDT>().As<ISalesTypeDT>();
            builder.RegisterType<PageSetDT>().As<IPageSetDT>();
            builder.RegisterType<PricelistMasterDT>().As<IPricelistMasterDT>();
            builder.RegisterType<ProductDT>().As<IProductDT>();
            builder.RegisterType<OrderDetailsDT>().As<IOrderDetailsDT>();
            builder.RegisterType<TransactionsDT>().As<ITransactionsDT>();
            builder.RegisterType<SuppliersDT>().As<ISuppliersDT>();
            builder.RegisterType<OrderDT>().As<IOrderDT>();
            builder.RegisterType<Invoice_Guests_TransDT>().As<IInvoice_Guests_TransDT>();
            builder.RegisterType<TransferMappingsDT>().As<ITransferMappingsDT>();
            builder.RegisterType<DepartmentDT>().As<IDepartmentDT>();
            builder.RegisterType<OrderDetailIngredientsDT>().As<IOrderDetailIngredientsDT>();
            builder.RegisterType<CreditTransactionDT>().As<ICreditTransactionDT>();
            builder.RegisterType<TransferToPmsDT>().As<ITransferToPmsDT>();
            builder.RegisterType<InvoiceShippingDetailsDT>().As<IInvoiceShippingDetailsDT>();
            builder.RegisterType<OrderStatusDT>().As<IOrderStatusDT>();
            builder.RegisterType<Delivery_CustomersShippingAddressDT>().As<IDelivery_CustomersShippingAddressDT>();
            builder.RegisterType<Delivery_CustomersBillingAddressDT>().As<IDelivery_CustomersBillingAddressDT>();
            builder.RegisterType<DA_OrderStatusDT>().As<IDA_OrderStatusDT>();
            builder.RegisterType<TableDT>().As<ITableDT>();
            builder.RegisterType<ProductRecipeDT>().As<IProductRecipeDT>();
            builder.RegisterType<PaymentsDT>().As<IPaymentsDT>();


            //Delivery DT & DAOS 
            builder.RegisterType<DeliveryCustomersDT>().As<IDeliveryCustomersDT>();
            builder.RegisterType<DeliveryCustomersDTO>().As<IDeliveryCustomersDAO>();
            builder.RegisterType<DeliveryCustomersBillingAddressDAO>().As<IDeliveryCustomersBillingAddressDAO>();
            builder.RegisterType<DeliveryCustomersShippingAddressDAO>().As<IDeliveryCustomersShippingAddressDAO>();
            builder.RegisterType<DeliveryCustomersPhonesDAO>().As<IDeliveryCustomersPhonesDAO>();
            builder.RegisterType<DA_ScheduledTaskesDT>().As<IDA_ScheduledTaskesDT>();

            builder.RegisterType<GuestDT>().As<IGuestDT>();
            builder.RegisterType<GuestDAO>().As<IGuestDAO>();
            builder.RegisterType<GuestFutureDT>().As<IGuestFutureDT>();

            builder.RegisterType<DA_HangFireJobsDT>().As<IDA_HangFireJobsDT>();
            builder.RegisterType<DA_ClientJobsDT>().As<IDA_ClientJobsDT>();
            builder.RegisterType<DA_CustomerTokenDT>().As<IDA_CustomerTokenDT>();

            builder.RegisterType<DeliveryOrdersDT>().As<IDeliveryOrdersDT>();

            builder.RegisterGeneric(typeof(GenericDAO<>)).As(typeof(IGenericDAO<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(GenericITableDAO<>)).As(typeof(IGenericITableDAO<>)).InstancePerLifetimeScope();
            // ... register more services for that layer

            //Forkey Functionality
            builder.RegisterType<ForkeyDT>().As<IForkeyDT>();

            //Combo items DTs and DAOs
            builder.RegisterType<ComboDT>().As<IComboDT>();
            builder.RegisterType<ComboDAO>().As<IComboDAO>();
            builder.RegisterType<ComboDetailDT>().As<IComboDetailDT>();
            builder.RegisterType<ComboDetailDAO>().As<IComboDetailDAO>();

            builder.RegisterType<ForexServiceDT>().As<IForexServiceDT>();
            builder.RegisterType<ForexServiceDAO>().As<IForexServiceDAO>();

            //Promos
            builder.RegisterType<Vodafone11DT>().As<IVodafone11DT>();
            builder.RegisterType<PromotionsDT>().As<IPromotionsDT>();

            //Goodys
            builder.RegisterType<GoodysDT>().As<IGoodysDT>();
            builder.RegisterType<GoodysDao>().As<IGoodysDAO>();

            builder.RegisterType<LoyaltyDT>().As<ILoyaltyDT>();

            //HotelInfo
            builder.RegisterType<HotelInfoV3DT>().As<IHotelInfoV3DT>();
            builder.RegisterType<HotelInfoV3DAO>().As<IHotelInfoV3DAO>();


            builder.RegisterType<DA_CustomerMessagesDT>().As<IDA_CustomerMessagesDT>();

            builder.RegisterType<ProductBarcodesDT>().As<IProductBarcodesDT>();

            builder.RegisterType<PayrollDT>().As<IPayrollDT>();

            builder.RegisterType<KdsDT>().As<IKdsDT>();

            builder.RegisterType<HotelDT>().As<IHotelDT>();
            //Cashed Objects
            builder.RegisterGeneric(typeof(CashManager<>)).As(typeof(CashManager<>)).SingleInstance();
            builder.RegisterGeneric(typeof(CashedDT<,>)).As(typeof(ICashedDT<,>));
            builder.RegisterType<HotelMacroTimezoneDT>().As<IHotelMacroTimezoneDT>();
            builder.RegisterType<HotelMacrosDT>().As<IHotelMacrosDT>();
            builder.RegisterType<HotelCustomMessagesDT>().As<IHotelCustomMessagesDT>();


            //oldGoodys

            builder.RegisterType<oldGoodysDT>().As<IoldGoodysDT>();


            builder.RegisterType<DeliveryRoutingDT>().As<IDeliveryRoutingDT>();
            //DA_OpeningHours
            builder.RegisterType<DA_OpeningHoursDT>().As<IDA_OpeningHoursDT>();
            
        }
    }
}
