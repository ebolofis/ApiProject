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
    public class OrderDetailIngredientsFlows : IOrderDetailIngredientsFlows
    {
        IOrderDetailIngredientsTasks task;

        public OrderDetailIngredientsFlows(IOrderDetailIngredientsTasks task)
        {
            this.task = task;
        }

        /// <summary>
        /// Add's new OrderDetailIngredientsModel to db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddOrderDetailIngredints(DBInfoModel dbInfo, OrderDetailIngredientsModel item)
        {
            return task.AddOrderDetailIngredints(dbInfo, item);
        }
    }
}
