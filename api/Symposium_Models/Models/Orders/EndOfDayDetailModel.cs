using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class EndOfDayDetailModel
    {
        public long Id { get; set; }
        public Nullable<long> EndOfdayId { get; set; }

        //ΦΠΑ
        public int VatId { get; set; }

        //pososto ΦΠΑ
        public decimal VatRate { get; set; }
        public decimal VatAmount { get; set; }
        public Nullable<int> TaxId { get; set; }

        //poso forou
        public decimal TaxAmount { get; set; }

        //meikta kerdh
        public decimal Gross { get; set; }

        //kathara kerdh
        public decimal Net { get; set; }
        public decimal Discount { get; set; }
    } 
}
