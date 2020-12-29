using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows
{
    public interface IOrderDetailIngredientsFlows
    {
        /// <summary>
        /// Add's new OrderDetailIngredientsModel to db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddOrderDetailIngredints(DBInfoModel dbInfo, OrderDetailIngredientsModel item);
    }
}
