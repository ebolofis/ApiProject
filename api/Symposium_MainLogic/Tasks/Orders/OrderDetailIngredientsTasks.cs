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
    public class OrderDetailIngredientsTasks : IOrderDetailIngredientsTasks
    {
        IOrderDetailIngredientsDT dt;

        public OrderDetailIngredientsTasks(IOrderDetailIngredientsDT dt)
        {
            this.dt = dt;
        }

        /// <summary>
        /// Add's new OrderDetailIngredientsModel to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddOrderDetailIngredints(DBInfoModel Store, OrderDetailIngredientsModel item)
        {
            return dt.AddOrderDetailIngredints(Store, item);
        }

        /// <summary>
        /// Return an OrderDetailIngredients using Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public OrderDetailIngredientsModel GetOrderDetailIngredientsById(DBInfoModel Store, long Id)
        {
            return dt.GetOrderDetailIngredientsById(Store, Id);
        }
    }
}
