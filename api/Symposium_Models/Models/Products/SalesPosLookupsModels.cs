using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class SalesPosLookupsModelsPreview
    {
        public long storeinfoid { get; set; }
        public PosInfoModel posinfo { get; set; }
        public List<SalesTypesModels> salesTypes { get; set; }
        public List<VatModel> vats { get; set; }
        public List<pricelistModels> pricelist { get; set; }
        public List<StaffModels> staff { get; set; }
        public List<AccountModel> Accounts { get; set; }
        public bool hasCustomers { get; set; }
        public Nullable<byte> CustomerPolicy { get; set; }
        public string CustomerServiceProviderUrl { get; set; }
        public string RedirectToCustomerCard { get; set; }
        public List<CreditCardsModels> CreditCards { get; set; }
        public List<KitchenInstructionsModels> KitchenInstructions { get; set; }
        public List<ItemRegionModels> ItemRegions { get; set; }
        public List<InvoiceTypeModel> InvoiceTypes { get; set; }
        public List<lockerProductsModels> lockerProducts { get; set; }
        public List<allowedboardMealsModels> allowedboardMeals { get; set; }
        public List<transferMappingsModels> transferMappings { get; set; }
        public List<availableHotelsModels> availableHotels { get; set; }
    }

    public class SalesTypesModels
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public string Abbreviation { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<long> DAId { get; set; }
        public List<TransferMappings_Model> TransferMappings { get; set; }
        public List<SalesType_PricelistMaster_Assoc_Model> SalesType_PricelistMaster_Assoc { get; set; }

        public List<OrderDetailModel> OrderDetail { get; set; }
    }

    public class TransferMappings_Model
    {
        public long Id { get; set; }
        public string PmsDepartmentId { get; set; }
        public string PmsDepDescription { get; set; }
        public Nullable<long> ProductId { get; set; }
        public Nullable<long> SalesTypeId { get; set; }
        public Nullable<double> VatPercentage { get; set; }
        public Nullable<long> PosDepartmentId { get; set; }
        public Nullable<long> PriceListId { get; set; }
        public Nullable<int> VatCode { get; set; }
        public Nullable<long> ProductCategoryId { get; set; }
        public Nullable<long> HotelId { get; set; }
    }

    public class SalesType_PricelistMaster_Assoc_Model
    {
        public long Id { get; set; }
        public Nullable<long> PricelistMasterId { get; set; }
        public Nullable<long> SalesTypeId { get; set; }
    }

    public class OrderDetailModel
    {
        public Nullable<long> Id { get; set; }
        public Nullable<long> OrderId { get; set; }
        public Nullable<long> ProductId { get; set; }
        public Nullable<long> KitchenId { get; set; }
        public Nullable<long> KdsId { get; set; }
        public Nullable<int> PreparationTime { get; set; }
        public Nullable<long> TableId { get; set; }
        public Nullable<byte> Status { get; set; }
        public Nullable<System.DateTime> StatusTS { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<long> PriceListDetailId { get; set; }
        public Nullable<double> Qty { get; set; }
        public Nullable<long> SalesTypeId { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<byte> PaidStatus { get; set; }
        public Nullable<long> TransactionId { get; set; }
        public Nullable<decimal> TotalAfterDiscount { get; set; }
        public Nullable<long> GuestId { get; set; }
        public Nullable<int> Couver { get; set; }
        public Nullable<System.Guid> Guid { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<double> PendingQty { get; set; }
        public Nullable<DA_OrderDetail_OtherDiscountEnum> OtherDiscount { get; set; }
        public Nullable<int> KitchenStatus { get; set; }
        public Nullable<long> LoginStaffId { get; set; }
    }

    /// <summary>
    /// List Of Order detail and extras to list of order detail ingrendients
    /// </summary>
    public class OrderDetailWithExtrasModel : OrderDetailModel
    {
        public List<OrderDetailIngredientsModel> OrderDetIngrendients { get; set; }

        public List<OrderDetailInvoicesModel> OrderDetailInvoices { get; set; }
    }

    public class pricelistModels
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public Nullable<long> LookUpPriceListId { get; set; }
        public Nullable<double> Percentage { get; set; }
        public Nullable<byte> Status { get; set; }
        public Nullable<System.DateTime> ActivationDate { get; set; }
        public Nullable<long> SalesTypeId { get; set; }
        public Nullable<System.DateTime> DeactivationDate { get; set; }
        public Nullable<long> PricelistMasterId { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<short> Type { get; set; }
    }
    public class CreditCardsModels
    {
        public AccountModel Account { get; set; }
        public long Room { get; set; }
    }
    public class KitchenInstructionsModels
    {
        public long Id { get; set; }
        public Nullable<long> KitchenId { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }
    }
    public class ItemRegionModels
    {
        public long Id { get; set; }
        public string ItemRegion { get; set; }
        public Nullable<int> RegionPosition { get; set; }
        public string Abbr { get; set; }
    }
    public class lockerProductsModels
    {
        public Nullable<long> PosInfoId { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<long> ReturnPaymentpId { get; set; }
        public Nullable<long> PaymentId { get; set; }
        public Nullable<long> SaleId { get; set; }
        public List<PageButtonModel> PageButton { get; set; }
    }
    public class allowedboardMealsModels
    {
        public long Id { get; set; }
        public string BoardId { get; set; }
        public string BoardDescription { get; set; }
        public Nullable<int> AllowedMeals { get; set; }
        public Nullable<decimal> AllowedDiscountAmount { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<long> PriceListId { get; set; }
        public Nullable<int> AllowedMealsChild { get; set; }
        public Nullable<decimal> AllowedDiscountAmountChild { get; set; }
        public List<AllowedMealsPerBoardDetailsModel> AllowedMealsPerBoardDetails { get; set; }

    }
    public class transferMappingsModels
    {
        public Nullable<long> HotelId { get; set; }
        public Nullable<long> ProductCategoryId { get; set; }
        public List<long> Pricelists { get; set; }
    }
    public class availableHotelsModels
    {
        public Nullable<int> HotelId { get; set; }
        public string Description { get; set; }
        public Nullable<byte> Type { get; set; }
        public Nullable<short> MPEHotel { get; set; }
    }
    public class PageButtonModel
    {
        public Nullable<long> ProductId { get; set; }
        public string Description { get; set; }
        public Nullable<int> PreparationTime { get; set; }
        public Nullable<int> Sort { get; set; }
        public Nullable<long> SetDefaultPriceListId { get; set; }
        public Nullable<int> Type { get; set; }
        public string Code { get; set; }
        public List<PricelistDetailsModel> PricelistDetails { get; set; }
    }
    public class PricelistDetailsModel
    {
        public long Id { get; set; }
        public VatDetailModel Vat { get; set; }
        public Nullable<long> VatId { get; set; }
        public Nullable<long> TaxId { get; set; }
        public Nullable<decimal> Price { get; set; }
    }
    public class AllowedMealsPerBoardDetailsModel
    {
        public long Id { get; set; }
        public Nullable<long> ProductCategoryId { get; set; }
        public Nullable<long> AllowedMealsPerBoardId { get; set; }
    }
}
