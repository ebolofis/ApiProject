using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.DeliveryAgent
{
    public class DA_MessagesModel
    {
        public long Id { get; set; }
        [MaxLength(1000)]
        public String Description { get; set; }
        public long MainDAMessagesID { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public List<DA_MessagesDetailsModel> DA_MessagesDetailsModel { get; set; }
        public String Email { get; set; }
        public Nullable<bool> OnOrderCreate { get; set; }
        public Nullable<bool> OnOrderUpdate { get; set; }
        public Nullable<bool> OnNewCall { get; set; }
    }
}
