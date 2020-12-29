using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    public interface IOrderDetailIngredientsDT
    {
        /// <summary>
        /// Add's new OrderDetailIngredientsModel to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddOrderDetailIngredints(DBInfoModel Store, OrderDetailIngredientsModel item);

        /// <summary>
        /// Add's new OrderDetailIngredientsModel to db
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <param name="dbTransact"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        long AddOrderDetailIngredints(IDbConnection db, OrderDetailIngredientsModel item, IDbTransaction dbTransact, out string Error);

        /// <summary>
        /// Return an OrderDetailIngredients using Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        OrderDetailIngredientsModel GetOrderDetailIngredientsById(DBInfoModel Store, long Id);
    }
}
