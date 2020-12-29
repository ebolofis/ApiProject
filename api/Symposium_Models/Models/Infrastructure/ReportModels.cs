using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models {
    public class ReportPreviewForPosModel {
        public List<ReportByAccountTotalsModel> Totals { get; set; }
        public long PosInfoId { get; set; }
        public long? StaffId { get; set; }
        public long? DepartmentId { get; set; }
        public string PosInfoDescription { get; set; }

    }

    public class ReportByAccountTotalsModel {
        /// <summary>
        /// the list of Receipts with a specific Account
        /// </summary>
        public List<ReportAnalysisReceiptModel> List { get; set; }
        /// <summary>
        /// this list of vat Details are items of Totals by VatId of ReportAnalysisReceiptModel Items
        /// </summary>
        public List<VatDetailsAcountTotalModel> VatDetailsTotal { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Number of receipts
        /// </summary>
        public int ReceiptCount { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// AccountId
        /// </summary>
        public long AccountId { get; set; }
    }

    /// <summary>
    /// Model that describes a receipt for End-of-Day analysis
    /// </summary>
    public class ReportAnalysisReceiptModel {
        public long Id { get; set; }
        public DateTime Day { get; set; }
        public string Description { get; set; }
        public string DepartmentDescription { get; set; }
        public long PosInfoId { get; set; }
        /// <summary>
        /// Receipt Abbreviation
        /// </summary>
        public string Abbreviation { get; set; }
        public int Couver { get; set; }
        public string StaffName { get; set; }
        public long StaffId { get; set; }
        public decimal Total { get; set; }
        public decimal Discount { get; set; }
        public decimal PaidTotal { get; set; }
        public decimal Amount { get; set; }
        public int ItemsCount { get; set; }
        /// <summary>
        /// one or more Order numbers (in case a receipt comes from 2 or more orders), OrderNo is comma separeted values
        /// </summary>
        public string OrderNo { get; set; }
        public long ReceiptNo { get; set; }
        public int InvoiceType { get; set; }
        public bool IsInvoiced { get; set; }
        public bool IsVoided { get; set; }
        public int IsPaid { get; set; }
        public string Rooms { get; set; }
        public string TableCode { get; set; }
        public long TableId { get; set; }
        /// <summary>
        /// this list are objects of Totals sum by VatId of OrderDetailInvoices of current invoice
        /// </summary>
        public List<VatDetailsReceiptModel> VatDetails { get; set; }

    }
    public class VatDetailsReceiptModel {
        /// <summary>
        /// Refered to Vat Id on database
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Description of VatId
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// A sum of by vatId Prices of Items
        /// </summary>
        public decimal? TotalAmount { get; set; }

    }
    public class VatDetailsAcountTotalModel {
        /// <summary>
        /// Refered to Vat Id on database
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Description of VatId
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// A sum of by vatId Prices of Items
        /// </summary>
        public decimal? TotalAmountByVat { get; set; }

    }
}
