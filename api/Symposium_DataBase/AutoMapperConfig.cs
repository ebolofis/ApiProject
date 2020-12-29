using AutoMapper;
using Pos_WebApi.Models;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.Delivery;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.ExternalSystems.Efood;
using Symposium.Models.Models.Hotel;
using Symposium.Models.Models.Hotelizer;
using Symposium.Models.Models.Infrastructure;
using Symposium.Models.Models.MealBoards;
using Symposium.Models.Models.Orders;
using Symposium.Models.Models.Payroll;
using Symposium.Models.Models.Products;
using Symposium.Models.Models.Promos;
using Symposium.Models.Models.TableReservations;
using Symposium_DTOs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(Symposium.WebApi.DataAccess.AutoMapperConfig), "RegisterMappings")]
namespace Symposium.WebApi.DataAccess
{
    /// <summary>
    /// Class Registering AutoMapper classes
    /// </summary>
    public class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<PosInfoModel, PosInfoDTO>();
                cfg.CreateMap<PosInfoDTO, PosInfoModel>();

                cfg.CreateMap<PosInfoDetailModel, PosInfoDetailDTO>();
                cfg.CreateMap<PosInfoDetailDTO, PosInfoDetailModel>();

                cfg.CreateMap<AccountModel, AccountDTO>();
                cfg.CreateMap<AccountDTO, AccountModel>();

                cfg.CreateMap<SalesTypesModels, SalesTypeDTO>();
                cfg.CreateMap<SalesTypeDTO, SalesTypesModels>();

                cfg.CreateMap<PayrollNewModel, PayrollNewDTO>();
                cfg.CreateMap<PayrollNewDTO, PayrollNewModel>();

                cfg.CreateMap<StaffModels, StaffDTO>();
                cfg.CreateMap<StaffDTO, StaffModels>();

                cfg.CreateMap<VatModel, VatDTO>();
                cfg.CreateMap<VatDTO, VatModel>();

                cfg.CreateMap<TablesModel, TableDTO>();
                cfg.CreateMap<TableDTO, TablesModel>();

                cfg.CreateMap<InvoiceTypeModel, InvoiceTypesDTO>();
                cfg.CreateMap<InvoiceTypesDTO, InvoiceTypeModel>();

                cfg.CreateMap<InvoiceModel, InvoicesDTO>();
                cfg.CreateMap<InvoicesDTO, InvoiceModel>();
                               
                cfg.CreateMap<InvoiceQRModel, InvoiceQRDTO>();
                cfg.CreateMap<InvoiceQRDTO, InvoiceQRModel>();

                cfg.CreateMap<OrderDetailInvoicesModel, OrderDetailInvoicesDTO>();
                cfg.CreateMap<OrderDetailInvoicesDTO, OrderDetailInvoicesModel>();

                cfg.CreateMap<OrderDetailModel, OrderDetailDTO>();
                cfg.CreateMap<OrderDetailDTO, OrderDetailModel>();
                cfg.CreateMap<OrderDetailWithExtrasModel, OrderDetailModel>();
                cfg.CreateMap<OrderDetailModel, OrderDetailWithExtrasModel>();


                cfg.CreateMap<OrderModel, OrderDTO>();
                cfg.CreateMap<OrderDTO, OrderModel>();

                cfg.CreateMap<FullOrderWithTablesModel, OrderModel>();
                cfg.CreateMap<OrderModel, FullOrderWithTablesModel>();

                cfg.CreateMap<OrderStatusModel, OrderStatusDTO>();
                cfg.CreateMap<OrderStatusDTO, OrderStatusModel>();

                cfg.CreateMap<NFCcardModel, NFCcardDTO>();
                cfg.CreateMap<NFCcardDTO, NFCcardModel>();

                cfg.CreateMap<LockersModel, LockersDTO>();
                cfg.CreateMap<LockersDTO, LockersModel>();

                cfg.CreateMap<RegionLockersModel, RegionLockerProductDTO>();
                cfg.CreateMap<RegionLockerProductDTO, RegionLockersModel>();

                cfg.CreateMap<LockersStatisticsModel, LockersDTO>();
                cfg.CreateMap<LockersDTO, LockersStatisticsModel>();

                cfg.CreateMap<EndOfDayModel, EndOfDayDTO>();
                cfg.CreateMap<EndOfDayDTO, EndOfDayModel>();

                cfg.CreateMap<EndOfDayDetailModel, EndOfDayDetailDTO>();
                cfg.CreateMap<EndOfDayDetailDTO, EndOfDayDetailModel>();

                cfg.CreateMap<EndOfDayPaymentAnalysisModel, EndOfDayPaymentAnalysisDTO>();
                cfg.CreateMap<EndOfDayPaymentAnalysisDTO, EndOfDayPaymentAnalysisModel>();

                cfg.CreateMap<EndOfDayVoidsAnalysisModel, EndOfDayVoidsAnalysisDTO>();
                cfg.CreateMap<EndOfDayVoidsAnalysisDTO, EndOfDayVoidsAnalysisModel>();

                cfg.CreateMap<EndOfDayTransferToPmsModel, TransferToPmsDTO>();
                cfg.CreateMap<TransferToPmsDTO, EndOfDayTransferToPmsModel>();

                cfg.CreateMap<PricelistDTO, PricelistModel>();
                cfg.CreateMap<PricelistModel, PricelistDTO>();

                cfg.CreateMap<ProductDTO, ProductModel>();
                cfg.CreateMap<ProductModel, ProductDTO>();

                cfg.CreateMap<PricelistDetailDTO, PricelistDetailModel>();
                cfg.CreateMap<PricelistDetailModel, PricelistDetailDTO>();

                //Delivery_Customers - agent Adds
                cfg.CreateMap<Delivery_CustomersDTO, DeliveryCustomer>();
                cfg.CreateMap<DeliveryCustomer, Delivery_CustomersDTO>();

                cfg.CreateMap<Delivery_CustomersDTO, DeliveryCustomerModel>();
                cfg.CreateMap<DeliveryCustomerModel, Delivery_CustomersDTO>();

                cfg.CreateMap<Delivery_CustomersDTO, DeliveryCustomerModelDS>();
                cfg.CreateMap<DeliveryCustomerModelDS, Delivery_CustomersDTO>();

                cfg.CreateMap<DeliveryCustomerModel, DeliveryCustomerModelDS>();
                cfg.CreateMap<DeliveryCustomerModelDS, DeliveryCustomerModel>();

                cfg.CreateMap<DeliveryCustomer, DeliveryCustomerModel>();
                cfg.CreateMap<DeliveryCustomerModel, DeliveryCustomer>();

                cfg.CreateMap<DeliveryCustomer, Delivery_CustomersDTO>();
                cfg.CreateMap<Delivery_CustomersDTO, DeliveryCustomer>();

                cfg.CreateMap<Delivery_CustomersPhonesDTO, DeliveryCustomersPhonesModel>();
                cfg.CreateMap<DeliveryCustomersPhonesModel, Delivery_CustomersPhonesDTO>();
                //Addresses
                cfg.CreateMap<Delivery_CustomersBillingAddressDTO, DeliveryCustomersBillingAddressModel>();
                cfg.CreateMap<DeliveryCustomersBillingAddressModel, Delivery_CustomersBillingAddressDTO>();

                cfg.CreateMap<Delivery_CustomersShippingAddressDTO, DeliveryCustomersShippingAddressModel>();
                cfg.CreateMap<DeliveryCustomersShippingAddressModel, Delivery_CustomersShippingAddressDTO>();

                //Assocs
                cfg.CreateMap<Delivery_CustomersPhonesAndAddressDTO, DeliveryCustomersPhonesAndAddressModel>();
                cfg.CreateMap<DeliveryCustomersPhonesAndAddressModel, Delivery_CustomersPhonesAndAddressDTO>();
                //Types
                cfg.CreateMap<Delivery_AddressTypesDTO, DeliveryAddressTypesModel>();
                cfg.CreateMap<DeliveryAddressTypesModel, Delivery_AddressTypesDTO>();

                cfg.CreateMap<Delivery_PhoneTypesDTO, DeliveryPhoneTypesModel>();
                cfg.CreateMap<DeliveryPhoneTypesModel, Delivery_PhoneTypesDTO>();

                cfg.CreateMap<Delivery_CustomersTypesDTO, DeliveryCustomerTypeModel>();
                cfg.CreateMap<DeliveryCustomerTypeModel, Delivery_CustomersTypesDTO>();

                cfg.CreateMap<ComboDTO, ComboModel>();
                cfg.CreateMap<ComboModel, ComboDTO>();
                cfg.CreateMap<ComboDetailDTO, ComboDetailModel>();
                cfg.CreateMap<ComboDetailModel, ComboDetailDTO>();

                cfg.CreateMap<ForexServiceDTO, ForexServiceModel>();
                cfg.CreateMap<ForexServiceModel, ForexServiceDTO>();

                cfg.CreateMap<HotelInfoDTO, HotelsInfoModel>();
                cfg.CreateMap<HotelsInfoModel, HotelInfoDTO>();
                cfg.CreateMap<HotelInfoDTO, HotelInfoBaseModel>();
                cfg.CreateMap<HotelInfoBaseModel, HotelInfoDTO>();

                //Guest
                cfg.CreateMap<GuestDTO, GuestModel>();
                cfg.CreateMap<GuestModel, GuestDTO>();
                cfg.CreateMap<GuestFutureDTO, GuestFutureModel>();
                cfg.CreateMap<GuestFutureModel, GuestFutureDTO>();

                //Delivery Agent
                cfg.CreateMap<DA_CustomersDTO, DACustomerModel>();
                cfg.CreateMap<DACustomerModel, DA_CustomersDTO>();

                cfg.CreateMap<DA_CustomersTokensDTO, DA_CustomerTokenModel>();
                cfg.CreateMap<DA_CustomerTokenModel, DA_CustomersTokensDTO>();

                cfg.CreateMap<DACustomerExtModel, DACustomerModel>();
                cfg.CreateMap<DACustomerModel, DACustomerExtModel>();

                cfg.CreateMap<StaffDTO, DA_StaffModel>();
                cfg.CreateMap<DA_StaffModel, StaffDTO>();

                cfg.CreateMap<DA_OrdersDTO, DA_OrderModel>();
                cfg.CreateMap<DA_OrderModel, DA_OrdersDTO>(); 
                 cfg.CreateMap<DA_LoyaltyStoreSetPointsModel, DA_OrderModel>();
                 cfg.CreateMap<DA_OrderModel, DA_LoyaltyStoreSetPointsModel>();

                cfg.CreateMap<DA_OrderDetailsDTO, DA_OrderDetails>();
                cfg.CreateMap<DA_OrderDetails, DA_OrderDetailsDTO>();
                cfg.CreateMap<DA_OrderDetailsExtrasDTO, DA_OrderDetailsExtrasModel>();
                cfg.CreateMap<DA_OrderDetailsExtrasModel, DA_OrderDetailsExtrasDTO>();

                cfg.CreateMap<DA_OrderDetailExtModel, DA_OrderDetails>();
                cfg.CreateMap<DA_OrderDetails, DA_OrderDetailExtModel>();

                cfg.CreateMap<DA_NewOrderModel, DA_OrderModel>();
                cfg.CreateMap<DA_OrderModel, DA_NewOrderModel>();

                cfg.CreateMap<Vodafone11Model, Vodafone11HeadersDTO>();
                cfg.CreateMap<Vodafone11HeadersDTO,Vodafone11Model>();

                cfg.CreateMap<Vodafone11DetailsDTO,Vodafone11ProdCategoriesModel>();
                cfg.CreateMap<Vodafone11ProdCategoriesModel,Vodafone11DetailsDTO>();

                cfg.CreateMap<PromotionsModels, Promotions_HeadersDTO>();
                cfg.CreateMap<Promotions_HeadersDTO, PromotionsModels>();

                cfg.CreateMap<PromotionsCombos, Promotions_CombosDTO>();
                cfg.CreateMap<Promotions_CombosDTO, PromotionsCombos>();

                cfg.CreateMap<PromotionsDiscounts, Promotions_DiscountsDTO>();
                cfg.CreateMap<Promotions_DiscountsDTO, PromotionsDiscounts>();

                //cfg.CreateMap<NFCcardModel, NFCcardDTO>().ForMember(
                //    dest => dest.Type,
                //    opt => opt.MapFrom(source => Enum.GetName(typeof(Type), source.Types)
                //));
                //cfg.CreateMap<NFCcardDTO, NFCcardModel>().ForMember(
                //    destination => destination,
                //    opt => opt.MapFrom(source => Enum.GetName(typeof(NFCEnums), source.Type)));//Enum.GetName(typeof(Type), source.Type)));
                cfg.CreateMap<DA_GeoPolygonsDTO, DA_GeoPolygonsModel>();
                cfg.CreateMap<DA_GeoPolygonsModel, DA_GeoPolygonsDTO>();

                cfg.CreateMap<DAStore_PriceListAssocDTO, DAStore_PriceListAssocModel>();
                cfg.CreateMap<DAStore_PriceListAssocModel, DAStore_PriceListAssocDTO>();

                cfg.CreateMap<DA_StoresDTO, DA_StoreModel>();
                cfg.CreateMap<DA_StoreModel, DA_StoresDTO>();

                cfg.CreateMap<DA_LoyalPointsDTO, DA_LoyalPointsModels>();
                cfg.CreateMap<DA_LoyalPointsModels, DA_LoyalPointsDTO>();

                cfg.CreateMap<DA_ShortageProdsDTO, DA_ShortagesExtModel>();
                cfg.CreateMap<DA_ShortagesExtModel, DA_ShortageProdsDTO>();

                cfg.CreateMap<DA_AddressesDTO, DA_AddressModel>();
                cfg.CreateMap<DA_AddressModel, DA_AddressesDTO>();

                cfg.CreateMap<UnitsModel, UnitsDTO>();
                cfg.CreateMap<UnitsDTO, UnitsModel>();

                cfg.CreateMap<TaxModel, TaxDTO>();
                cfg.CreateMap<TaxDTO, TaxModel>();

                cfg.CreateMap<CategoriesModel, CategoriesDTO>();
                cfg.CreateMap<CategoriesDTO, CategoriesModel>();

                cfg.CreateMap<ProductCategoriesModel, ProductCategoriesDTO>();
                cfg.CreateMap<ProductCategoriesDTO, ProductCategoriesModel>();

                cfg.CreateMap<PageButtonDetailModel, PageButtonDetailDTO>();
                cfg.CreateMap<PageButtonDetailDTO, PageButtonDetailModel>();

                cfg.CreateMap<IngredientsModel, IngredientsDTO>();
                cfg.CreateMap<IngredientsDTO, IngredientsModel>();

                cfg.CreateMap<IngredientCategoriesModel, IngredientCategoriesDTO>();
                cfg.CreateMap<IngredientCategoriesDTO, IngredientCategoriesModel>();

                cfg.CreateMap<Ingredient_ProdCategoryAssocModel, Ingredient_ProdCategoryAssocDTO>();
                cfg.CreateMap<Ingredient_ProdCategoryAssocDTO, Ingredient_ProdCategoryAssocModel>();

                cfg.CreateMap<ProductExtrasModel, ProductExtrasDTO>();
                cfg.CreateMap<ProductExtrasDTO, ProductExtrasModel>();
                cfg.CreateMap<PriceList_EffectiveHoursModel, PriceList_EffectiveHoursDTO>();
                cfg.CreateMap<PriceList_EffectiveHoursDTO, PriceList_EffectiveHoursModel>();
                cfg.CreateMap<SalesTypeModel, SalesTypeDTO>();
                cfg.CreateMap<SalesTypeDTO, SalesTypeModel>();
                cfg.CreateMap<PageSetModel, PageSetDTO>();
                cfg.CreateMap<PageSetDTO, PageSetModel>();
                cfg.CreateMap<PricelistMasterModel, PricelistMasterDTO>();
                cfg.CreateMap<PricelistMasterDTO, PricelistMasterModel>();

                cfg.CreateMap<AccountSched_Model, AccountDTO>();
                cfg.CreateMap<CategoriesSched_Model, CategoriesDTO>();
                cfg.CreateMap<IngedProdCategAssocSched_Model, Ingredient_ProdCategoryAssocDTO>();
                cfg.CreateMap<IngredCategoriesSched_Model, IngredientCategoriesDTO>();
                cfg.CreateMap<PageSetSched_Model, PageSetDTO>();
                cfg.CreateMap<PriceListMasterSched_Model, PricelistMasterDTO>();
                cfg.CreateMap<SalesTypeSched_Model, SalesTypeDTO>();
                cfg.CreateMap<SalesTypeDTO, SalesTypeSched_Model>();

                cfg.CreateMap<TaxSched_Model, TaxDTO>();
                cfg.CreateMap<UnitsSched_Model, UnitsDTO>();
                cfg.CreateMap<VatSched_Model, VatDTO>();
                cfg.CreateMap<IngredientsSched_Model, IngredientsDTO>();
                cfg.CreateMap<PagesSched_Model, PagesDTO>();
                cfg.CreateMap<PriceListSched_Model, PricelistDTO>();
                cfg.CreateMap<PriceList_EffHoursSched_Model, PriceList_EffectiveHoursDTO> ();
                cfg.CreateMap<ProductCategoriesSched_Model, ProductCategoriesDTO> ();
                cfg.CreateMap<ProductSched_Model, ProductDTO>();

                cfg.CreateMap<ProductExtrasSched_Model, ProductExtrasDTO>();

                cfg.CreateMap<PriceListDetailSched_Model, PricelistDetailDTO>();
                cfg.CreateMap<PageButtonSched_Model, PageButtonDTO>();
                cfg.CreateMap<PageButtonDetSched_Model, PageButtonDetailDTO>();

                cfg.CreateMap<PriceListDetailModel, PricelistDetailDTO>();
                cfg.CreateMap<PricelistDetailDTO, PriceListDetailModel>();

                cfg.CreateMap<DA_ShortageProdsModel, DA_ShortageProdsDTO>();
                cfg.CreateMap<DA_ShortageProdsDTO, DA_ShortageProdsModel>();

                cfg.CreateMap<InvoiceShippingDetailsDTO, InvoiceShippingDetailsModel>();
                cfg.CreateMap<InvoiceShippingDetailsModel, InvoiceShippingDetailsDTO>();

                cfg.CreateMap<InvoiceShippingDetailsDTO, InvoiceShippingDetailsExtModel>();
                cfg.CreateMap<InvoiceShippingDetailsExtModel, InvoiceShippingDetailsDTO>();

                cfg.CreateMap<OrderDetailInvoicesModel, ReceiptDetails>();
                cfg.CreateMap<ReceiptDetails, OrderDetailInvoicesModel>();

                cfg.CreateMap<OrderDetailIngredientsModel, OrderDetailIgredientsDTO>();
                cfg.CreateMap<OrderDetailIgredientsDTO, OrderDetailIngredientsModel>();

                cfg.CreateMap<OrderDetailWithExtrasModel, ReceiptDetails>();
                cfg.CreateMap<ReceiptDetails, OrderDetailWithExtrasModel>();

                cfg.CreateMap<Invoice_Guests_TransModel, Invoice_Guests_TransDTO>();
                cfg.CreateMap<Invoice_Guests_TransDTO, Invoice_Guests_TransModel>();

                cfg.CreateMap<TransferMappingsModel, TransferMappingsDTO>();
                cfg.CreateMap<TransferMappingsDTO, TransferMappingsModel>();

                cfg.CreateMap<InvoiceWithTablesModel, InvoiceModel>();
                cfg.CreateMap<InvoiceModel, InvoiceWithTablesModel>();

                cfg.CreateMap<TransactionsExtraModel, TransactionsModel>();
                cfg.CreateMap<TransactionsModel, TransactionsExtraModel>();
                cfg.CreateMap<TransactionsModel, TransactionsDTO>();
                cfg.CreateMap<TransactionsDTO, TransactionsModel>();
                cfg.CreateMap<TransferToPmsModel, TransferToPmsDTO>();
                cfg.CreateMap<TransferToPmsDTO, TransferToPmsModel>();

                cfg.CreateMap<SupplierModel, SuppliersDTO>();
                cfg.CreateMap<SuppliersDTO, SupplierModel>();

                cfg.CreateMap<Delivery_CustomersShippingAddressModel, Delivery_CustomersShippingAddressDTO>();
                cfg.CreateMap<Delivery_CustomersShippingAddressDTO, Delivery_CustomersShippingAddressModel>();
                cfg.CreateMap<Delivery_CustomersShippingAddressModel, DeliveryCustomersShippingAddressModel>();
                cfg.CreateMap<DeliveryCustomersShippingAddressModel, Delivery_CustomersShippingAddressModel>();

                cfg.CreateMap<DeliveryCustomersShippingAddressModel, Delivery_CustomersShippingAddressModel>();
                cfg.CreateMap<Delivery_CustomersShippingAddressModel, DeliveryCustomersShippingAddressModel>();

                cfg.CreateMap<DeliveryCustomersBillingAddressModel, Delivery_CustomersBillingAddressModel>();
                cfg.CreateMap<Delivery_CustomersBillingAddressModel, DeliveryCustomersBillingAddressModel>();



                cfg.CreateMap<Delivery_CustomersBillingAddressModel, Delivery_CustomersBillingAddressDTO>();
                cfg.CreateMap<Delivery_CustomersBillingAddressDTO, Delivery_CustomersBillingAddressModel>();
                cfg.CreateMap<Delivery_CustomersBillingAddressModel, DeliveryCustomersBillingAddressModel>();
                cfg.CreateMap<DeliveryCustomersBillingAddressModel, Delivery_CustomersBillingAddressModel>();

                cfg.CreateMap<DA_OrderStatusModel, DA_OrderStatusDTO>();
                cfg.CreateMap<DA_OrderStatusDTO, DA_OrderStatusModel>();
                cfg.CreateMap<DA_OrderStatusModel, DA_OrderStatusExtModel>();
                cfg.CreateMap<DA_OrderStatusExtModel, DA_OrderStatusModel>();

                cfg.CreateMap<ExtDeliveryBucketModel, EFoodBucketDTO>();
                cfg.CreateMap<EFoodBucketDTO, ExtDeliveryBucketModel>();

                cfg.CreateMap<ProductRecipeModel, ProductRecipeDTO>();
                cfg.CreateMap<ProductRecipeDTO, ProductRecipeModel>();
                cfg.CreateMap<ProductRecipeSched_Model, ProductRecipeDTO>();


                cfg.CreateMap<ProductBarcodesModel, ProductBarcodesDTO>();
                cfg.CreateMap<ProductBarcodesDTO, ProductBarcodesModel>();
                cfg.CreateMap<ProductBarcodesSched_Model, ProductBarcodesDTO>();

                cfg.CreateMap<PromotionsHeaderSched_Model, Promotions_HeadersDTO>();
                cfg.CreateMap<PromotionsCombosSched_Model, Promotions_CombosDTO>();
                cfg.CreateMap<PromotionsDiscountsSched_Model, Promotions_DiscountsDTO>();

                cfg.CreateMap<Promotions_PricelistsDTO, PromotionsPricelistModel>();
                cfg.CreateMap<PromotionsPricelistModel, Promotions_PricelistsDTO>();

                cfg.CreateMap<DA_MessagesDTO, DA_MessagesModel>();
                cfg.CreateMap<DA_MessagesModel, DA_MessagesDTO>();

                cfg.CreateMap<DA_MessagesDetailsDTO, DA_MessagesDetailsModel>();
                cfg.CreateMap<DA_MessagesDetailsModel, DA_MessagesDetailsDTO>();

                cfg.CreateMap<DA_CustomerMessagesDTO, DA_CustomerMessagesModel>();
                cfg.CreateMap<DA_CustomerMessagesModel, DA_CustomerMessagesDTO>();

                cfg.CreateMap<DA_MainMessagesDTO, DA_MainMessagesModel>();
                cfg.CreateMap<DA_MainMessagesModel, DA_MainMessagesDTO>();

                cfg.CreateMap<HotelMacrosDTO, MacroDBModel>();
                cfg.CreateMap<MacroDBModel, HotelMacrosDTO>();

                cfg.CreateMap<HotelMacros_HistDTO, MacroDBModel>();
                cfg.CreateMap<MacroDBModel, HotelMacros_HistDTO>();

                cfg.CreateMap<MacroTimezoneDBModel, HotelMacroTimezoneDTO>();
                cfg.CreateMap<HotelMacroTimezoneDTO, MacroTimezoneDBModel>();

                cfg.CreateMap<HotelCustomerDataConfigDTO, Hotel__CustomerDataConfigModel>();
                cfg.CreateMap<Hotel__CustomerDataConfigModel,HotelCustomerDataConfigDTO>();

                cfg.CreateMap<HotelCustomMessagesDTO, CustomMessageModel>();
                cfg.CreateMap<CustomMessageModel, HotelCustomMessagesDTO>();

                cfg.CreateMap<LoyaltyModel, LoyaltyDTO>();
                cfg.CreateMap<LoyaltyDTO, LoyaltyModel>();

                cfg.CreateMap<DeliveryRoutingModel, DeliveryRoutingDTO>();
                cfg.CreateMap<DeliveryRoutingDTO, DeliveryRoutingModel>();

                cfg.CreateMap<DeliveryRoutingHistModel, DeliveryRoutingHistDTO>();
                cfg.CreateMap<DeliveryRoutingHistDTO, DeliveryRoutingHistModel>();

                cfg.CreateMap<StaffDeliveryModel, StaffDTO>();
                cfg.CreateMap<StaffDTO, StaffDeliveryModel>();

                // Table Reservations
                cfg.CreateMap<ReservationTypeModel, TR_ReservationTypesDTO>();
                cfg.CreateMap<TR_ReservationTypesDTO, ReservationTypeModel>();



                //Hotelizer
                cfg.CreateMap<HotelizerCustomerModel, CustomerModel>()
                       .ForMember(dest => dest.ReservationId, opt => opt.MapFrom(src => src.accommodation_id))
                       .ForMember(dest => dest.ArrivalDate, opt => opt.MapFrom(src => src.arrival))
                       .ForMember(dest => dest.DepartureDate, opt => opt.MapFrom(src => src.departure))
                       .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.guest_name))
                       .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.room_name))
                       .ForMember(dest => dest.Adults, opt => opt.MapFrom(src => src.adults))
                       .ForMember(dest => dest.Children, opt => opt.MapFrom(src => src.children))
                       .ForMember(dest => dest.RoomType, opt => opt.MapFrom(src => src.room_type_name))
                       .ForMember(dest => dest.ProfileNo, opt => opt.MapFrom(src => src.guest_id));

                cfg.CreateMap<HotelizerCustomerModel, CustomersDetails>()
                       .ForMember(dest => dest.ReservationId, opt => opt.MapFrom(src => src.accommodation_id))
                       .ForMember(dest => dest.arrivalDT, opt => opt.MapFrom(src => src.arrival))
                       .ForMember(dest => dest.departureDT, opt => opt.MapFrom(src => src.departure))
                       .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.guest_name))
                       .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.room_name))
                       .ForMember(dest => dest.Adults, opt => opt.MapFrom(src => src.adults))
                       .ForMember(dest => dest.Children, opt => opt.MapFrom(src => src.children))
                       .ForMember(dest => dest.ProfileNo, opt => opt.MapFrom(src => src.guest_id));

                cfg.CreateMap<HotelizerCustomerModel, Customers>()
                       .ForMember(dest => dest.ReservationId, opt => opt.MapFrom(src => src.accommodation_id))
                       .ForMember(dest => dest.arrivalDT, opt => opt.MapFrom(src => src.arrival))
                       .ForMember(dest => dest.Arrival, opt => opt.MapFrom(src => src.arrival))
                       .ForMember(dest => dest.departureDT, opt => opt.MapFrom(src => src.departure))
                       .ForMember(dest => dest.Departure, opt => opt.MapFrom(src => src.departure))
                       .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.guest_name))
                       .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.room_name))
                       .ForMember(dest => dest.Adults, opt => opt.MapFrom(src => src.adults))
                       .ForMember(dest => dest.Children, opt => opt.MapFrom(src => src.children))
                       .ForMember(dest => dest.ProfileNo, opt => opt.MapFrom(src => src.guest_id));

            });
        }
    }

}
