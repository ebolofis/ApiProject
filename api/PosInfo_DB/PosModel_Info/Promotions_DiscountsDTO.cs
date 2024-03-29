﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [Table("Promotions_Discounts")]
    [DisplayName("Promotions_Discounts")]
    public class Promotions_DiscountsDTO
    {
        /// <summary>
        /// Promotion Discounts Id
        /// </summary>
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        public long Id { get; set; }

        /// <summary>
        /// Promotion Header Id
        /// </summary>
        [Column("HeaderId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_Promotions_Discounts_Promotions_Headers")]
        [Association("Promotions_Headers", "HeaderId", "Id")]
        public Nullable<long> HeaderId { get; set; }

        /// <summary>
        /// Promotion Id αντικειμένου
        /// </summary>
        [Column("ItemId", Order = 3, TypeName = "BIGINT")]
        public Nullable<long> ItemId { get; set; }


        /// <summary>
        ///  αριθμός αντικειμένων στα οποία θα εφαρμοστεί η έκπτωση (πχ: 1)
        /// </summary>
        [Column("ItemQuantity", Order = 4, TypeName = "DECIMAL(19,2)")]
        public Nullable<decimal> ItemQuantity { get; set; }


        /// <summary>
        /// true: το αντικειμένο είναι product
        /// </summary>
        [Column("ItemIsProduct", Order = 5, TypeName = "BIT")]
        public Nullable<bool> ItemIsProduct { get; set; }

        /// <summary>
        /// Agent Id saved to store
        /// </summary>
        [Column("DAId", Order = 6, TypeName = "BIGINT")]
        public Nullable<long> DAId { get; set; }
    }
}
