using Symposium.Models.Enums.Promotions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Promos
{
    public class PromotionsModels
    {
        /// <summary>
        /// Promotion Header Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Περιγραφή promo
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// External code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// discount % [0..100]
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// true: ρώτα τον χειριστή pos αν θα εφαρμόσει το promotion
        /// </summary>
        public bool AskOperator { get; set; }

        /// <summary>
        /// σχολια για τον EXTCR
        /// </summary>
        public string ReceiptNote { get; set; }

        /// <summary>
        /// 0: Discount all products, 1:cheepest only, 2: MostExpensive only, 3: UserDecision
        /// </summary>
        public PromoDiscountTypeEnum DiscountType { get; set; }

        /// <summary>
        /// true: το UI ζητά την εισαγωγή κωδικου κουπονιού
        /// </summary>
        public bool AskCode { get; set; }

        public List<PromotionsCombosExt> PromoCombo { get; set; }

        public List<PromotionsDiscountsExt> PromoDiscounts { get; set; }

        /// <summary>
        /// Id From Agent
        /// </summary>
        public Nullable<long> DAId { get; set; }
    }

    public class PromotionsCombos
    {
        /// <summary>
        /// Promotion Combo Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Promotion Header Id
        /// </summary>
        public long HeaderId { get; set; }

        /// <summary>
        /// Promotion Id αντικειμένου
        /// </summary>
        public long ItemId { get; set; }

        /// <summary>
        /// ελλάχιστη ποσότητα αντικειμένου για να εφαρμοστεί το promotion
        /// </summary>
        public decimal ItemQuantity { get; set; }

        /// <summary>
        /// true: το αντικειμένο είναι product
        /// </summary>
        public bool ItemIsProduct { get; set; }

        /// <summary>
        /// Id from Agent 
        /// </summary>
        public Nullable<long> DAId { get; set; }
    }

    public class PromotionsDiscounts
    {
        /// <summary>
        /// Promotion Discounts Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Promotion Header Id
        /// </summary>
        public long HeaderId { get; set; }

        /// <summary>
        /// Promotion Id αντικειμένου
        /// </summary>
        public long ItemId { get; set; }

        /// <summary>
        /// αριθμός αντικειμένων στα οποία θα εφαρμοστεί η έκπτωση (πχ: 1)
        /// </summary>
        public decimal ItemQuantity { get; set; }

        /// <summary>
        /// true: το αντικειμένο είναι product
        /// </summary>
        public bool ItemIsProduct { get; set; }

        /// <summary>
        /// Id From Agent
        /// </summary>
        public Nullable<long> DAId { get; set; }
    }

    public class PromotionsCombosExt : PromotionsCombos
    {
        /// <summary>
        /// Περιγραφή Προϊόντος
        /// </summary>
        public string ProductDescr { get; set; }

        /// <summary>
        /// Περιγραφή Κατηγορίας Προϊόντος
        /// </summary>
        public string ProductCatDescr { get; set; }
    }

    public class PromotionsDiscountsExt : PromotionsDiscounts
    {
        /// <summary>
        /// Περιγραφή Προϊόντος
        /// </summary>
        public string ProductDescr { get; set; }

        /// <summary>
        /// Περιγραφή Κατηγορίας Προϊόντος
        /// </summary>
        public string ProductCatDescr { get; set; }
    }


   

}
