using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{
    public class ForexServiceTasks : IForexServiceTasks
    {

        IForexServiceDT forexServiceDT;

        public ForexServiceTasks(IForexServiceDT forexServiceDT)
        {
            this.forexServiceDT = forexServiceDT;
        }

        public List<ForexServiceModel> SelectAllForex(DBInfoModel store)
        {
            return forexServiceDT.SelectAllForex(store);
        }

    }
}
