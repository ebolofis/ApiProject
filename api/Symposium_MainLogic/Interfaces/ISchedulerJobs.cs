using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symposium.Models.Models.Scheduler;

namespace Symposium.WebApi.MainLogic.Interfaces
{
    public interface ISchedulerJobs
    {
        void Start(ParametersSchedulerModel StartParams = null);
    }
}
