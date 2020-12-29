using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Interfaces
{
    public interface ISchedulerModel
    {

        long MasterId { get; set; }

        string StoreFullURL { get; set; }

        Nullable<long> TableId { get; set; }

        int Action { get; set; }
    }
}
