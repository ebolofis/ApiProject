using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.DeliveryAgent
{
    public class DA_LoyaltyFullConfigModel
    {
        public DA_LoyalConfigModel LoyalConfigModel { get; set; }

        public List<DA_LoyalGainAmountRangeModel> LoyalGainAmountRangeModel { get; set; }

        public DA_LoyalGainAmountRatioModel LoyalGainAmountRatioModel { get; set; }

        public DA_LoyalRedeemDiscountModel LoyalRedeemDiscountModel { get; set; }

        public List<DA_LoyalRedeemFreeProductModel> LoyalRedeemFreeProductModel { get; set; }
    }
    public class DA_LoyalConfigModel
    {
        /// <summary>
        /// τυπος λήψης πόντων. 0:AmountRange, 1:AmountRatio
        /// </summary>
        public DA_LoyaltyGainPointsTypeEnums GainPointsType { get; set; }

        /// <summary>
        /// τύπος εξαργύρωσης. 0:FreeProduct, 1:Discount, 2: both
        /// </summary>
        public DA_LoyaltyRedeemTypeEnums RedeemType { get; set; }

        /// <summary>
        /// μέγιστος αρ.πόντων. 0 για χωρίς περιορισμό
        /// </summary>
        public int MaxPoints { get; set; }

        /// <summary>
        /// μέγιστη διάρκεια πόντων σε μήνες. 0 για χωρίς περιορισμό
        /// </summary>
        public int ExpDuration { get; set; }

        /// <summary>
        /// ελλάχιστη παραγγελία για λήψη πόντων
        /// </summary>
        public decimal MinAmount { get; set; }

        /// <summary>
        /// αρχικοί πόντοι κατά την εγγραφή του πελάτη
        /// </summary>
        public Nullable<int> InitPoints { get; set; }
    }

    public class DA_LoyalGainAmountRangeModel
    {
        public long Id { get; set; }

        /// <summary>
        /// ποσό παραγγελίας από
        /// </summary>
        public decimal FromAmount { get; set; }

        /// <summary>
        /// ποσό παραγγελίας έως
        /// </summary>
        public decimal ToAmount { get; set; }

        /// <summary>
        /// πόντοι κερδισμένοι
        /// </summary>
        public int Points { get; set; }
    }

    public class DA_LoyalGainAmountRatioModel
    {
        /// <summary>
        /// πόντοι στους οποίους αντιστοιχεί 1 ευρω
        /// </summary>

        public decimal ToPoints { get; set; }
    }

    public class DA_LoyalRedeemDiscountModel
    {
        /// <summary>
        /// ευρω έκπτωσης στα οποία αντιστοιχεί 1 πόντος για εξαργύρωση
        /// </summary>
        public decimal DiscountRatio { get; set; }
    }

    public class DA_LoyalRedeemFreeProductModel
    {
        public long Id { get; set; }

        /// <summary>
        /// πόντοι εξαργύρωσης
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// δωρεάν είδος ή
        /// </summary>
        public Nullable<long> ProductId { get; set; }

        public string ProductName { get; set; }

        /// <summary>
        /// δωρεάν είδος από κατηγορία προϊόντων
        /// </summary>
        public Nullable<long> ProdCategoryId { get; set; }

        public string ProdCategoryName { get; set; }

        public decimal Qnt { get; set; }
    }

    public class DA_LoyalPointsModels
    {
        public long Id { get; set; }

        public long CustomerId { get; set; }

        /// <summary>
        /// πόντοι κερδισμένοι/εξαργύρωσης  (+/-)
        /// </summary>
        public int Points { get; set; }

        public DateTime Date { get; set; }

        /// <summary>
        /// Order Id (αν η κίνηση ΔΕΝ συσχετίζεται με παραγγελία τότε OrderId=0)
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        ///  Id Καταστήματος (αν η κίνηση ΔΕΝ συσχετίζεται με παραγγελία που έγινε σε κατάστημα τότε StoreId=0)
        /// </summary>
        public long StoreId { get; set; }

        public long StaffId { get; set; }

        public string Description { get; set; }

    }


    public class DA_LoyaltyRedeemOptionsModel
    {
        public List<DA_LoyalRedeemFreeProductModel> RedeemFreeProductModel { get; set; }
        public DA_LoytaltyDiscountRedeemModel DiscountRedeemModel { get; set; }
    }

    public class DA_LoytaltyDiscountRedeemModel
    {
        public decimal MaxDiscountAmount { get; set; }
        public int RedeemPoints { get; set; }
    }

    public class DA_LoyaltyRedeemModel {
       public  long Id { get; set; }
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// model που σχετίζεται με τον ορισμό πόντων από παραγγελία που έγινε σε κατάστημα
    /// </summary>
    public class DA_LoyaltyStoreSetPointsModel
    {
        /// <summary>
        /// DA_Customers.Id
        /// </summary>
        [Required]
        [Range(1, long.MaxValue)]
        public long CustomerId { get; set; }

        /// <summary>
        /// Order.Id καταστήματος
        /// </summary>
        [Required]
        [Range(1, long.MaxValue)]
        public long Id { get; set; }

        /// <summary>
        /// Συνολικό ποσό παραγγελίας(μετά την έκπτωση)
        /// </summary>
        [Required]
        [Range(0, Double.MaxValue)]
        public decimal Total { get; set; }

        /// <summary>
        /// Point's redeemed by order (Loyalty)
        /// </summary>
        [Range(0, Int32.MaxValue)]
        public int PointsRedeem { get; set; } = 0;

        /// <summary>
        /// Point's gained by order (Loyalty)
        /// </summary>
        public int PointsGain { get; set; } = 0;

        public long StoreId { get; set; } = 0;

    }

}
