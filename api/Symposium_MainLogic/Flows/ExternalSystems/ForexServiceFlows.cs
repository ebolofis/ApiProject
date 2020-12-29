using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows
{
    public class ForexServiceFlows : IForexServiceFlows
    {

        IForexServiceTasks forexServiceTasks;

        public ForexServiceFlows(IForexServiceTasks forexServiceTasks)
        {
            this.forexServiceTasks = forexServiceTasks;
        }

        public List<ForexServiceModel> SelectAllForex(DBInfoModel store)
        {
            return forexServiceTasks.SelectAllForex(store);
        }
    }
}
