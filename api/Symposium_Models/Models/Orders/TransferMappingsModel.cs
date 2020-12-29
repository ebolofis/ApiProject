using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    /// <summary>
    /// Transfer Mapping Model
    /// </summary>
    public class TransferMappingsModel
    {
        public Nullable<long> Id { get; set; }

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
}
