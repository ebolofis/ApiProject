using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models {
    public class OrderDetailInvoicesModel {
        public Nullable<long> Id { get; set; }
        public Nullable<long> OrderDetailId { get; set; }
        public Nullable<long> StaffId { get; set; }
        public Nullable<long> PosInfoDetailId { get; set; }
        public Nullable<long> Counter { get; set; }

        //xroniki stigmi dimourgias eggrafis
        public Nullable<System.DateTime> CreationTS { get; set; }
        public Nullable<bool> IsPrinted { get; set; }
        public string CustomerId { get; set; }
        public Nullable<long> InvoicesId { get; set; }
        public Nullable<bool> IsDeleted { get; set; }

        //gia antikimena pou theorounte extra
        public Nullable<long> OrderDetailIgredientsId { get; set; }

        //timi antikimenou xoris tin ekptosi (qty * price)
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> Net { get; set; }
        public Nullable<decimal> VatRate { get; set; }
        public Nullable<decimal> VatAmount { get; set; }
        public Nullable<long> VatId { get; set; }
        public Nullable<long> TaxId { get; set; }
        public Nullable<int> VatCode { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }
        public Nullable<double> Qty { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<long> PricelistId { get; set; }

        //egrafi proiontos
        public Nullable<long> ProductId { get; set; }
        public string Description { get; set; }

        //kodikos pou xaraktirizei to product
        public string ItemCode { get; set; }

        //sxolia
        public string ItemRemark { get; set; }

        //Receipt: 1, Order: 2 (deltio paragelias), Void: 3 (akirosi apodijis), Complementary: 4, Allowance: 5, Timologio : 7, VoidOrder : 8, PaymentReceipt : 12, RefundReceipt : 11.
        public Nullable<int> InvoiceType { get; set; }
        public Nullable<long> TableId { get; set; }
        public string TableCode { get; set; }

        //aithousa
        public Nullable<long> RegionId { get; set; }
        public Nullable<long> OrderNo { get; set; }
        public Nullable<long> OrderId { get; set; }

        //0 = main item, 1 = extra
        public bool IsExtra { get; set; }
        public Nullable<long> PosInfoId { get; set; }
        public Nullable<long> EndOfDayId { get; set; }

        //sintomografia parastatikou
        public string Abbreviation { get; set; }
        public Nullable<long> DiscountId { get; set; }
        public Nullable<long> SalesTypeId { get; set; }

        //deutereuousa katigoria proionton
        public Nullable<long> ProductCategoryId { get; set; }

        //kiria katigoria proionton
        public Nullable<long> CategoryId { get; set; }
        public int ItemPosition { get; set; }

        //i thesi tou antikimenou se mia paragelia
        public int ItemSort { get; set; }

        //xaraktirismos antikimenon(Starters, Main Dishes, Beverages κλπ)
        public string ItemRegion { get; set; }

        //thesi ektiposis antikimenon
        public Nullable<int> RegionPosition { get; set; }
        public int ItemBarcode { get; set; }

        //poso xoris ekptosi
        public Nullable<decimal> TotalBeforeDiscount { get; set; }

        //poso ekptosis pou antistixei sto antikimeno apo tin sinoliki ekptosi
        public Nullable<decimal> TotalAfterDiscount { get; set; }

        //poso meta tin ekptosi
        public Nullable<decimal> ReceiptSplitedDiscount { get; set; }

        /// <summary>
        /// for new version of post order.
        /// if product is extra then the id of ingrendient
        /// </summary>
        public Nullable<long> IngredientId { get; set; }

        /// <summary>
        /// Ομαδοποίηση αντικειμένων στο τραπέζι
        /// </summary>
        public string TableLabel { get; set; }

    }
}
