using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class RegionLockersModel
    {
        public long Id { get; set; }

        public Nullable<long> ProductId { get; set; }

        public Nullable<long> PriceListId { get; set; }

        public Nullable<long> Price { get; set; }

        public Nullable<long> Discount { get; set; }

        public string SalesDescription { get; set; }

        public Nullable<long> ReturnPaymentpId { get; set; }

        public Nullable<long> PaymentId { get; set; }

        public Nullable<long> SaleId { get; set; }

        public Nullable<long> PosInfoId { get; set; }
    }
}
