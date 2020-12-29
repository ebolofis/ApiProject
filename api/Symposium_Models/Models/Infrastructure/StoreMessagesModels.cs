using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class StoreMessagesModelsPreview
    {
        public List<StoreMessagesModels> storeMessages { get; set; }
    }
    public class StoreMessagesModels
    {
        public long Id { get; set; }
        public string Message { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public string Title { get; set; }
        public Nullable<long> StoreId { get; set; }
        public Nullable<System.DateTime> ShowFrom { get; set; }
        public Nullable<System.DateTime> ShowTo { get; set; }
        public string ImageUri { get; set; }
        public Nullable<byte> Status { get; set; }

        public virtual DBInfoModel Store { get; set; }
    }
}
