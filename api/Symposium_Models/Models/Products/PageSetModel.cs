using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class PageSetModel
    {
        public Nullable<long> Id { get; set; }

        public string Description { get; set; }

        public Nullable<System.DateTime> ActivationDate { get; set; }

        public Nullable<System.DateTime> DeactivationDate { get; set; }

        public Nullable<long> DAId { get; set; }

    }
}
