using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class PricelistModel: PricelistBasicModel
    {
        public Nullable<byte> Status { get; set; }
        public Nullable<long> LookUpPriceListId { get; set; }
        public Nullable<double> Percentage { get; set; }
        public Nullable<long> DAId { get; set; }
        public Nullable<long> PricelistMasterId { get; set; }
    }

    public class PricelistBasicModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> ActivationDate { get; set; }
        public Nullable<DateTime> DeactivationDate { get; set; }
        public Nullable<long> SalesTypeId { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<short> Type { get; set; }

    }


    /// <summary>
    /// PricelistModel with the list of Details
    /// </summary>
    public class PricelistExtModel : PricelistBasicModel
    {
        /// <summary>
        /// list of Pricelist details
        /// </summary>
        public List<PricelistDetailBasicModel> Details { get; set; } = new List<PricelistDetailBasicModel>();
    }
}
