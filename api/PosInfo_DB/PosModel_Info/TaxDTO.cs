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
    [Table("Tax")]
    [DisplayName("Tax")]
    public class TaxDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Tax")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(50)")]
        public string Description { get; set; }

        [Column("Percentage", Order = 3, TypeName = "DECIMAL(8,4)")]
        public Nullable<decimal> Percentage { get; set; }

        [Column("DAId", Order = 4, TypeName = "BIGINT")]
        public Nullable<long> DAId { get; set; }
    }
}
