using Autofac;
using Symposium.Helpers;
using Symposium.WebApi.DataAccess;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Tasks;
using Symposium.WebApi.MainLogic.Flows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symposium.WebApi.MainLogic.Flows.TableReservations;
using Symposium.WebApi.MainLogic.Interfaces.Flows.TableReservations;
using Symposium.WebApi.MainLogic.Tasks.TableReservations;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.TableReservations;
using Symposium.WebApi.MainLogic.Flows.ExternalDelivery;
using Symposium.WebApi.MainLogic.Interfaces.Flows.ExternalDelivery;
using Symposium.WebApi.MainLogic.Tasks.ExternalDelivery;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.ExternalDelivery;
using Symposium.WebApi.MainLogic.Tasks.ExternalSystems;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.ExternalSystems;
using Symposium.WebApi.MainLogic.Interfaces.Flows.ExternalSystems;
using Symposium.WebApi.MainLogic.Flows.ExternalSystems;
using Symposium.WebApi.MainLogic.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Tasks.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Promos;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Promos;
using Symposium.WebApi.MainLogic.Flows.Promos;
using Symposium.WebApi.MainLogic.Tasks.Promos;
using Symposium.WebApi.MainLogic.Tasks.Products;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Products;
using Symposium.WebApi.MainLogic.Flows.Products;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Products;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Goodys;
using Symposium.WebApi.MainLogic.Tasks.Goodys;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Goodys;
using Symposium.WebApi.MainLogic.Flows.HotelInfo;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.HotelInfo;
using Symposium.WebApi.MainLogic.Interfaces.Flows.HotelInfo;
using Symposium.WebApi.MainLogic.Tasks.HotelInfo;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Payroll;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Payroll;
using Symposium.WebApi.MainLogic.Flows.Payroll;
using Symposium.WebApi.MainLogic.Tasks.Payroll;
using Symposium.WebApi.MainLogic.Tasks.Infrastructure;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Infrastructure;
using Symposium.WebApi.MainLogic.Flows.Infrastructure;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Infrastructure;
using Symposium.WebApi.MainLogic.Tasks.Configuration;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Configuration;
using Symposium.WebApi.MainLogic.Flows.Configuration;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Configuration;
using Symposium.WebApi.MainLogic.Flows.DahuaRecorder;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DahuaRecorder;
using Symposium.WebApi.MainLogic.Tasks.DahuaRecorder;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DahuaRecorder;
using Symposium.WebApi.MainLogic.Flows.Hotel;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Hotel;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Hotel;
using Symposium.WebApi.MainLogic.Tasks.Hotel;
using Symposium.Plugins;
using Symposium.WebApi.MainLogic.Flows.Goodys;
using Symposium.WebApi.MainLogic.Tasks.Orders;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Orders;
using Symposium.WebApi.MainLogic.Flows.Orders;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Orders;
using Symposium.WebApi.MainLogic.Flows.Delivery;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Delivery;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Delivery;
using Symposium.WebApi.MainLogic.Tasks.Delivery;
using Symposium.WebApi.MainLogic.Flows.Hotelizer;
using Symposium.WebApi.MainLogic.Flows.Kds;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Kds;
using Symposium.WebApi.MainLogic.Tasks.Kds;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Kds;

namespace Symposium.WebApi.MainLogic
{
    public class MainLogicDIModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            //Register DataBase's Autofac module 
            builder.RegisterModule<HelperDIModule>();
            builder.RegisterModule<PluginsDIModule>();
            builder.RegisterModule<DataAccessDIModule>();

            //Register MainLogic classes 
            builder.RegisterType<EndOfDayFlows>().As<IEndOfDayFlows>();
            builder.RegisterType<EndOfDayTasks>().As<IEndOfDayTasks>();
            builder.RegisterType<RegionLockersFlows>().As<IRegionLockersFlows>();
            builder.RegisterType<RegionLockersTasks>().As<IRegionLockersTasks>();
            builder.RegisterType<AccountTasks>().As<IAccountTasks>();
            builder.RegisterType<AccountFlows>().As<IAccountFlows>();
            builder.RegisterType<ReportFlows>().As<IReportFlows>();
            builder.RegisterType<ReportTasks>().As<IReportTasks>();
            builder.RegisterType<InvoiceTasks>().As<IInvoiceTasks>();
            builder.RegisterType<TableForDisplayFlow>().As<ITableForDisplayFlows>();
            builder.RegisterType<TableForDisplayTasks>().As<ITableForDisplayTasks>();
            builder.RegisterType<PageButtonFlows>().As<IPageButtonFlows>();
            builder.RegisterType<PageButtonTasks>().As<IPageButtonTasks>();
            builder.RegisterType<CustomerFlows>().As<ICustomerFlows>();
            builder.RegisterType<CustomerTasks>().As<ICustomerTasks>();
            builder.RegisterType<StoreFlows>().As<IStoreFlows>();
            builder.RegisterType<StoreTasks>().As<IStoreTasks>();
            builder.RegisterType<StoreMessagesFlows>().As<IStoreMessagesFlows>();
            builder.RegisterType<StoreMessagesTasks>().As<IStoreMessagesTasks>();
            builder.RegisterType<RegionFlows>().As<IRegionFlows>();
            builder.RegisterType<RegionTasks>().As<IRegionTasks>();
            builder.RegisterType<PagesFlows>().As<IPagesFlows>();
            builder.RegisterType<PagesTasks>().As<IPagesTasks>();
            builder.RegisterType<PredefinedCreditsFlows>().As<IPredefinedCreditsFlows>();
            builder.RegisterType<PredefinedCreditsTasks>().As<IPredefinedCreditsTasks>();
            builder.RegisterType<StaffFlows>().As<IStaffFlows>();
            builder.RegisterType<StaffTasks>().As<IStaffTasks>();
            builder.RegisterType<SalesPosLookupsFlows>().As<ISalesPosLookupsFlows>();
            builder.RegisterType<SalesPosLookupsTasks>().As<ISalesPosLookupsTasks>();
            builder.RegisterType<ReceiptDetailsForExtecrFlows>().As<IReceiptDetailsForExtecrFlows>();
            builder.RegisterType<ReceiptDetailsForExtecrTasks>().As<IReceiptDetailsForExtecrTasks>();
            builder.RegisterType<GdprFlows>().As<IGdprFlows>();
            builder.RegisterType<GdprTasks>().As<IGdprTasks>();
            builder.RegisterType<PageSetFlows>().As<IPageSetFlows>();
            builder.RegisterType<PageSetTasks>().As<IPageSetTasks>();
            builder.RegisterType<PosInfoDetailFlows>().As<IPosInfoDetailFlows>();
            builder.RegisterType<PosInfoDetailTasks>().As<IPosInfoDetailTasks>();
            builder.RegisterType<PosInfoFlows>().As<IPosInfoFlows>();
            builder.RegisterType<PosInfoTasks>().As<IPosInfoTasks>();

            builder.RegisterType<OrderDetailInvoicesTasks>().As<IOrderDetailInvoicesTasks>();
            builder.RegisterType<OrderDetailInvoicesFlows>().As<IOrderDetailInvoicesFlows>();

            //Table Reservations
            builder.RegisterType<ReservationTypesFlows>().As<IReservationTypesFlows>();
            builder.RegisterType<ReservationTypesTasks>().As<IReservationTypesTasks>();
            builder.RegisterType<RestaurantsFlows>().As<IRestaurantsFlows>();
            builder.RegisterType<RestaurantsTasks>().As<IRestaurantsTasks>();
            builder.RegisterType<RestrictionsRestaurantsAssocFlows>().As<IRestrictionsRestaurantsAssocFlows>();
            builder.RegisterType<RestrictionsRestaurantsAssocTasks>().As<IRestrictionsRestaurantsAssocTasks>();
            builder.RegisterType<ExcludeDaysFlows>().As<IExcludeDaysFlows>();
            builder.RegisterType<ExcludeDaysTasks>().As<IExcludeDaysTasks>();
            builder.RegisterType<ExcludeRestrictionsFlows>().As<IExcludeRestrictionsFlows>();
            builder.RegisterType<ExcludeRestrictionsTasks>().As<IExcludeRestrictionsTasks>();
            builder.RegisterType<CapacitiesFlows>().As<ICapacitiesFlows>();
            builder.RegisterType<CapacitiesTasks>().As<ICapacitiesTasks>();
            builder.RegisterType<RestrictionsFlows>().As<IRestrictionFlows>();
            builder.RegisterType<RestrictionsTasks>().As<IRestrictionTasks>();
            builder.RegisterType<OverwrittenCapacitiesFlows>().As<IOverwrittenCapacitiesFlows>();
            builder.RegisterType<OverwrittenCapacitiesTasks>().As<IOverwrittenCapacitiesTasks>();
            builder.RegisterType<ReservationsFlows>().As<IReservationsFlows>();
            builder.RegisterType<ReservationsTasks>().As<IReservationsTasks>();
            builder.RegisterType<ReservationCustomersFlows>().As<IReservationCustomersFlows>();
            builder.RegisterType<ReservationCustomersTasks>().As<IReservationCustomersTasks>();
            builder.RegisterType<ConfigFlows>().As<IConfigFlows>();
            builder.RegisterType<ConfigTasks>().As<IConfigTasks>();
            builder.RegisterType<EmailConfigFlows>().As<IEmailConfigFlows>();
            builder.RegisterType<EmailConfigTasks>().As<IEmailConfigTasks>();
            builder.RegisterType<EmailTasks>().As<IEmailTasks>();
            builder.RegisterType<TransactionsTasks>().As<ITransactionsTasks>();
            builder.RegisterType<TransactionsFlows>().As<ITransactionsFlows>();
            builder.RegisterType<SuppliersTasks>().As<ISuppliersTasks>();
            builder.RegisterType<SuppliersFlows>().As<ISuppliersFlows>();
            builder.RegisterType<InvoiceShippingDetailsFlows>().As<IInvoiceShippingDetailsFlows>();
            builder.RegisterType<InvoiceShippingDetailsTasks>().As<IInvoiceShippingDetailsTasks>();
            builder.RegisterType<OrderStatusTasks>().As<IOrderStatusTasks>();
            builder.RegisterType<OrderStatusFlows>().As<IOrderStatusFlows>();
            builder.RegisterType<Delivery_CustomersShippingAddressTasks>().As<IDelivery_CustomersShippingAddressTasks>();
            builder.RegisterType<Delivery_CustomersShippingAddressFlows>().As<IDelivery_CustomersShippingAddressFlows>();
            builder.RegisterType<Delivery_CustomersBillingAddressTasks>().As<IDelivery_CustomersBillingAddressTasks>();
            builder.RegisterType<Delivery_CustomersBillingAddressFlows>().As<IDelivery_CustomersBillingAddressFlows>();
            builder.RegisterType<DA_OrderStatusTasks>().As<IDA_OrderStatusTasks>();
            builder.RegisterType<DA_OrderStatusFlows>().As<IDA_OrderStatusFlows>();

            builder.RegisterType<InvoiceTypeTasks>().As<IInvoiceTypeTasks>();

            //DeliveryAgent
            builder.RegisterType<DA_CustomerFlows>().As<IDA_CustomerFlows>();
            builder.RegisterType<DA_CustomerTasks>().As<IDA_CustomerTasks>();
            builder.RegisterType<DA_StaffFlows>().As<IDA_StaffFlows>();
            builder.RegisterType<DA_StaffTasks>().As<IDA_StaffTasks>();
            builder.RegisterType<DA_OrdersFlows>().As<IDA_OrdersFlows>();
            builder.RegisterType<DA_OrdersTasks>().As<IDA_OrdersTasks>();
            builder.RegisterType<DA_GeoPolygonsFlows>().As<IDA_GeoPolygonsFlows>();
            builder.RegisterType<DA_GeoPolygonsTasks>().As<IDA_GeoPolygonsTasks>();
            builder.RegisterType<DA_Store_PriceListAssocFlows>().As<IDA_Store_PriceListAssocFlows>();
            builder.RegisterType<DA_Store_PriceListAssocTasks>().As<IDA_Store_PriceListAssocTasks>();
            builder.RegisterType<DA_StoresFlows>().As<IDA_StoresFlows>();
            builder.RegisterType<DA_StoresTasks>().As<IDA_StoresTasks>();
            builder.RegisterType<DA_LoyaltyFlows>().As<IDA_LoyaltyFlows>();
            builder.RegisterType<DA_LoyaltyTasks>().As<IDA_LoyaltyTasks>();
            builder.RegisterType<DA_ShortagesFlows>().As<IDA_ShortagesFlows>();
            builder.RegisterType<DA_ShortagesTasks>().As<IDA_ShortagesTasks>();
            builder.RegisterType<DA_ConfigFlows>().As<IDA_ConfigFlows>();
            builder.RegisterType<DA_ConfigTasks>().As<IDA_ConfigTasks>();
            builder.RegisterType<DA_AddressesFlows>().As<IDA_AddressesFlows>();
            builder.RegisterType<DA_AddressesTasks>().As<IDA_AddressesTasks>();
            builder.RegisterType<DA_CustomerTokenFlows>().As<IDA_CustomerTokenFlows>();
            builder.RegisterType<DA_CustomerTokenTasks>().As<IDA_CustomerTokenTasks>();

            builder.RegisterType<DA_ClientJobsTasks>().As<IDA_ClientJobsTasks>();
            builder.RegisterType<DA_ClientJobsFlows>().As<IDA_ClientJobsFlows>();
            builder.RegisterType<DA_HangFireJobsTasks>().As<IDA_HangFireJobsTasks>();
            builder.RegisterType<DA_HangFireJobsSendOrdersFlows>().As<IDA_HangFireJobsSendOrdersFlows>();
            builder.RegisterType<DA_HangFireJobsUpdateClientTableFlows>().As<IDA_HangFireJobsUpdateClientTableFlows>();
            builder.RegisterType<DA_HangFireJobsUpdateStatusFlows>().As<IDA_HangFireJobsUpdateStatusFlows>();
            builder.RegisterType<DA_HangFireJobsCheckIsDelayOnStoresFlows>().As<IDA_HangFireJobsCheckIsDelayOnStoresFlows>();
            builder.RegisterType<DA_HangFireJobsMakeCustomerAnonymousFlows>().As<IDA_HangFireJobsMakeCustomerAnonymousFlows>();
            builder.RegisterType<DA_LoyaltyExecutionsFlow>().As<IDA_LoyaltyExecutionsFlow>();
            builder.RegisterType<DA_EFoodGetOrdersFlow>().As<IDA_EFoodGetOrdersFlow>();
            builder.RegisterType<DA_UpdateStoresTablesTasks>().As<IDA_UpdateStoresTablesTasks>();

            builder.RegisterType<ProductRecipeTasks>().As<IProductRecipeTasks>();
            builder.RegisterType<ProductRecipeFlows>().As<IProductRecipeFlows>();

            builder.RegisterType<NFCcardFlows>().As<INFCcardFlows>();
            builder.RegisterType<NFCcardTasks>().As<INFCcardTasks>();

            builder.RegisterType<PricelistDetailFlows>().As<IPricelistDetailFlows>();
            builder.RegisterType<PricelistDetailTasks>().As<IPricelistDetailTasks>();

            builder.RegisterType<HotelInfoFlows>().As<IHotelInfoFlows>();
            builder.RegisterType<HotelInfoTasks>().As<IHotelInfoTasks>();


            builder.RegisterType<InvoicesFlows>().As<IInvoicesFlows>();
            builder.RegisterType<CancelReceiptTasks>().As<ICancelReceiptTasks>();
            builder.RegisterType<PaymentsFlows>().As<IPaymentsFlows>();
            builder.RegisterType<PaymentsTasks>().As<IPaymentsTasks>();

            builder.RegisterType<UnitsFlows>().As<IUnitsFlows>();
            builder.RegisterType<UnitsTasks>().As<IUnitsTasks>();

            builder.RegisterType<VatFlows>().As<IVatFlows>();
            builder.RegisterType<VatTasks>().As<IVatTasks>();

            builder.RegisterType<TaxFlows>().As<ITaxFlows>();
            builder.RegisterType<TaxTasks>().As<ITaxTasks>();

            builder.RegisterType<CategoriesFlows>().As<ICategoriesFlows>();
            builder.RegisterType<CategoriesTasks>().As<ICategoriesTasks>();

            builder.RegisterType<ProductCategoriesFlows>().As<IProductCategoriesFlows>();
            builder.RegisterType<ProductCategoriesTasks>().As<IProductCategoriesTasks>();

            builder.RegisterType<PageButtonDetailFlows>().As<IPageButtonDetailFlows>();
            builder.RegisterType<PageButtonDetailTasks>().As<IPageButtonDetailTasks>();

            builder.RegisterType<PricelistFlows>().As<IPricelistFlows>();
            builder.RegisterType<PricelistTasks>().As<IPricelistTasks>();

            builder.RegisterType<IngredientsFlows>().As<IIngredientsFlows>();
            builder.RegisterType<IngredientsTasks>().As<IIngredientsTasks>();

            builder.RegisterType<IngredientCategoriesFlows>().As<IIngredientCategoriesFlows>();
            builder.RegisterType<IngredientCategoriesTasks>().As<IIngredientCategoriesTasks>();

            builder.RegisterType<Ingredient_ProdCategoryAssocFlows>().As<IIngredient_ProdCategoryAssocFlows>();
            builder.RegisterType<Ingredient_ProdCategoryAssocTasks>().As<IIngredient_ProdCategoryAssocTasks>();

            builder.RegisterType<ProductExtrasFlows>().As<IProductExtrasFlows>();
            builder.RegisterType<ProductExtrasTasks>().As<IProductExtrasTasks>();

            builder.RegisterType<PriceList_EffectiveHoursFlows>().As<IPriceList_EffectiveHoursFlows>();
            builder.RegisterType<PriceList_EffectiveHoursTasks>().As<IPriceList_EffectiveHoursTasks>();

            builder.RegisterType<SalesTypeFlows>().As<ISalesTypeFlows>();
            builder.RegisterType<SalesTypeTasks>().As<ISalesTypeTasks>();

            builder.RegisterType<PricelistMasterFlows>().As<IPricelistMasterFlows>();
            builder.RegisterType<PricelistMasterTasks>().As<IPricelistMasterTasks>();

            builder.RegisterType<ProductFlows>().As<IProductFlows>();
            builder.RegisterType<ProductTasks>().As<IProductTasks>();

            builder.RegisterType<GuestTasks>().As<IGuestTasks>();
            builder.RegisterType<GuestFutureTasks>().As<IGuestFutureTasks>();
            builder.RegisterType<GuestFutureFlows>().As<IGuestFutureFlows>();

            builder.RegisterType<OrderFlows>().As<IOrderFlows>();
            builder.RegisterType<OrderTask>().As<IOrderTask>();

            builder.RegisterType<OrderDetailTasks>().As<IOrderDetailTasks>();
            builder.RegisterType<OrderDetailFlows>().As<IOrderDetailFlows>();

            builder.RegisterType<OrderDetailIngredientsTasks>().As<IOrderDetailIngredientsTasks>();
            builder.RegisterType<OrderDetailIngredientsFlows>().As<IOrderDetailIngredientsFlows>();

            builder.RegisterType<Invoice_Guest_TransFlows>().As<IInvoice_Guest_TransFlows>();
            builder.RegisterType<Invoice_Guest_TransTasks>().As<IInvoice_Guest_TransTasks>();

            builder.RegisterType<CreditTransactionsFlows>().As<ICreditTransactionsFlows>();
            builder.RegisterType<CreditTransactionsTasks>().As<ICreditTransactionsTasks>();

            builder.RegisterType<TransferToPmsTasks>().As<ITransferToPmsTasks>();
            builder.RegisterType<TransferToPmsFlows>().As<ITransferToPmsFlows>();

            //Agent 
            builder.RegisterType<DeliveryCustomerFlows>().As<IDeliveryCustomerFlows>();
            builder.RegisterType<DeliveryCustomerTasks>().As<IDeliveryCustomerTasks>();
            //Delivery Orders
            builder.RegisterType<DeliveryOrdersFlows>().As<IDeliveryOrdersFlows>();
            builder.RegisterType<DeliveryOrderTasks>().As<IDeliveryOrderTasks>();
            builder.RegisterType<DA_ScheduledTaskesFlows>().As<IDA_ScheduledTaskesFlows>();
            builder.RegisterType<DA_ScheduledTaskesTasks>().As<IDA_ScheduledTaskesTasks>();

            // Forkey Functionality
            builder.RegisterType<ForkeyFlows>().As<IForkeyFlows>();
            builder.RegisterType<ForkeyTasks>().As<IForkeyTasks>();
            // Icoupon Functionality 
            builder.RegisterType<ICouponsTasks>().As<IiCouponTasks>();
            builder.RegisterType<ICouponFlows>().As<IiCouponFlows>();
            // Combo Items Functionality
            builder.RegisterType<ComboTasks>().As<IComboTasks>();
            builder.RegisterType<ComboFlows>().As<IComboFlows>();

            builder.RegisterType<ForexServiceFlows>().As<IForexServiceFlows>();
            builder.RegisterType<ForexServiceTasks>().As<IForexServiceTasks>();

            builder.RegisterType<ExtDeliverySystemsFlows>().As<IExtDeliverySystemsFlows>();
            builder.RegisterType<ExtDeliverySystemsTasks>().As<IExtDeliverySystemsTasks>();

            builder.RegisterType<DA_EfoodFlows>().As<IDA_EfoodFlows>();
            builder.RegisterType<DA_EfoodTasks>().As<IDA_EfoodTasks>();


            //Promos
            builder.RegisterType<PromosFlows>().As<IPromosFlows>();
            builder.RegisterType<PromosTasks>().As<IPromosTasks>();

            builder.RegisterType<PromotionsFlows>().As<IPromotionsFlows>();
            builder.RegisterType<PromotionsTasks>().As<IPromotionsTasks>();

            //NewPayrollModule

            builder.RegisterType<PayrollFlows>().As<IPayrollFlows>();
            builder.RegisterType<PayrollTasks>().As<IPayrollTasks>();

            //NewKdsModule

            builder.RegisterType<KdsFlows>().As<IKdsFlows>();
            builder.RegisterType<KdsTasks>().As<IKdsTasks>();

            //Promos
            builder.RegisterType<GoodysFlow>().As<IGoodysFlow>();
            builder.RegisterType<GoodysTasks>().As<IGoodysTasks>();

            //HotelInfo
            builder.RegisterType<HotelInfoV3Flow>().As<IHotelInfoV3Flow>();
            builder.RegisterType<HotelInfoV3Tasks>().As<IHotelInfoV3Tasks>();
            // ... register more services for that layer

            builder.RegisterType<DA_CustomerMessagesFlows>().As<IDA_CustomerMessagesFlows>();
            builder.RegisterType<DA_CustomerMessagesTasks>().As<IDA_CustomerMessagesTasks>();

            builder.RegisterType<ProductBarcodesTasks>().As<IProductBarcodesTasks>();
            builder.RegisterType<ProductBarcodesFlows>().As<IProductBarcodesFlows>();

            builder.RegisterType<ConfigurationTasks>().As<IConfigurationTasks>();
            builder.RegisterType<ConfigurationFlows>().As<IConfigurationFlows>();

            builder.RegisterType<PosRecorderFlows>().As<IPosRecorderFlows>();
            builder.RegisterType<PosRecorderTasks>().As<IPosRecorderTasks>();

            builder.RegisterType<HotelFlows>().As<IHotelFlows>();
            builder.RegisterType<HotelTasks>().As<IHotelTasks>();

            //Goodys Old Orders
            builder.RegisterType<GoodysOrdersFlow>().As<IGoodysOrdersFlow>();

            builder.RegisterType<LoyaltyTasks>().As<ILoyaltyTasks>();


            //old Goodys

            builder.RegisterType<OldGoodysFlow>().As<IOldGoodysFlow>();
            builder.RegisterType<OldGoodysTask>().As<IOldGoodysTask>();

            builder.RegisterType<DeliveryRoutingFlows>().As<IDeliveryRoutingFlows>();
            builder.RegisterType<DeliveryRoutingTasks>().As<IDeliveryRoutingTasks>();

            //DA_OpeningHours
            builder.RegisterType<DA_OpeningHoursFlows>().As<IDA_OpeningHoursFlows>();
            builder.RegisterType<DA_OpeningHoursTasks>().As<IDA_OpeningHoursTasks>();

        }
    }
}