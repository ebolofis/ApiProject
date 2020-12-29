using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class TableForDisplayPreviewModel
    {
        /// <summary>
        /// List of Items 
        /// </summary>
        public List<TempReceiptDetailsModel> Items { get; set; }

        /// <summary>
        /// Customer to Pay 
        /// </summary>
        public TempGuestPaymentsModel Payments { get; set; }

        public TableCoversModel Covers { get; set; }
    }

    public class TempReceiptDetailsModel
    {
        public Nullable<long> ReceiptId { get; set; }
        public Nullable<long> RegionId { get; set; }
        public Nullable<long> Id { get; set; }
        public long? ReceiptNo { get; set; }
        public Nullable<long> PosInfoId { get; set; }
        public Nullable<long> TableId { get; set; }
        public string TableCode { get; set; }
        public long OrderId { get; set; }
        public Nullable<long> OrderNo { get; set; }
        public int? Cover { get; set; }
        public string ItemCode { get; set; }
        public string Product { get; set; }
        public Nullable<long> ProductId { get; set; }
        public Nullable<decimal> Price { get; set; }
        public long VatId { get; set; }
        public Nullable<long> VatCode { get; set; }
        public Nullable<decimal> VatDesc { get; set; }
        public Nullable<long> PriceListDetailId { get; set; }
        public Nullable<long> PricelistId { get; set; }
        public double? Qty { get; set; }
        public Nullable<long> KitchenId { get; set; }
        public Nullable<System.Guid> Guid { get; set; }
        public Nullable<byte> Status { get; set; }
        public Nullable<DateTime> StatusTS { get; set; }
        public Nullable<byte> PaidStatus { get; set; }
        public decimal? TotalAfterDiscount { get; set; }
        public Nullable<long> Staff { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<long> SalesTypeId { get; set; }
        public long? OrderDetailIgredientsId { get; set; }
        public Nullable<long> ProductCategoryId { get; set; }
        public decimal? ReceiptSplitedDiscount { get; set; }
        public bool IsExtra { get; set; }
        public string ItemRemark { get; set; }
    }

    public class TempGuestPaymentsModel
    {
        public long? GuestId { get; set; }
        public string Room { get; set; }
        public int? RoomId { get; set; }
        public int? ProfileNo { get; set; }
        public string ReservationCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long? InvoicesId { get; set; }
        public long? TransactionId { get; set; }
        public long? HotelId { get; set; }

        //Απαραίτητη πληροφορία loyalty του πελάτη
        public Nullable<int> ClassId { get; set; }
        public string ClassName { get; set; }
        public Nullable<int> AvailablePoints { get; set; }
        public Nullable<int> fnbdiscount { get; set; }
        public Nullable<int> ratebuy { get; set; }
    }

    public class TableCoversModel
    {
        public long? OrderId { get; set; }
        public long? OrderNo { get; set; }
        public int? Cover { get; set; }
    }

    public class KitchenInstructionPreviewModel
    {
        public List<KitchenInstructionModel> KitchenInstrustions { get; set; }
    }

    public class KitchenInstructionModel
    {
        public long Id { get; set; }
        public string Message { get; set; }
        public string StaffCode { get; set; }
        public string Staff { get; set; }
        public Nullable<DateTime>  SendTS { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<long> Origin { get; set; }
    }

    public class TablesPerRegionPreviewModel
    {
        public List<TablesPerRegionModel> TablesPerRegion { get; set; }
    }

    public class TablesPerRegionModel
    {
        public long? TableId { get; set; }
        public string PosInfoDesc { get; set; }
        public string Staff { get; set; }
        public string StaffIds { get; set; }
        public int ColorStatusId { get; set; }

    }

    public class GetAllTablesModelPreview
    {
        public List<GetAllTablesModel> GetAllTable { get; set; }
    }

    public class GetAllTablesModel
    {
        public long Id { get; set; }
        public string Angle { get; set; }
        public string Code { get; set; }

        public string Description { get; set; }
        public Nullable<double> Height { get; set; }
        public string ImageUri { get; set; }
        public Nullable<bool> IsOnline { get; set; }
        public Nullable<int> MaxCapacity { get; set; }
        public Nullable<int> MinCapacity { get; set; }
        public Nullable<long> RegionId { get; set; }
        public Nullable<short> ReservationStatus { get; set; }
        public string SalesDescription { get; set; }
        public Nullable<long> Shape { get; set; }
        public Nullable<byte> Status { get; set; }
        public Nullable<int> TurnoverTime { get; set; }
        public Nullable<double> Width { get; set; }
        public string XPos { get; set; }
        public string YPos { get; set; }
    }


}
