using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface IOrderDetailIngredientsTasks
    {
        /// <summary>
        /// Add's new OrderDetailIngredientsModel to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddOrderDetailIngredints(DBInfoModel Store, OrderDetailIngredientsModel item);

        /// <summary>
        /// Return an OrderDetailIngredients using Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        OrderDetailIngredientsModel GetOrderDetailIngredientsById(DBInfoModel Store, long Id);
    }
}
