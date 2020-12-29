using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class StoreDetailsModel
    {
      public List<StoreDetails> StoreDetailsPreview { get; set; }
    }

    public class StoreDetails
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public string ExtDescription { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Phone3 { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longtitude { get; set; }
        public string ImageUri { get; set; }
        public string LogoUri { get; set; }
        public Nullable<byte> Status { get; set; }
        public Nullable<System.Guid> StoreMapId { get; set; }
        public Nullable<short> StoreKitchenInstruction { get; set; }
    }
}
