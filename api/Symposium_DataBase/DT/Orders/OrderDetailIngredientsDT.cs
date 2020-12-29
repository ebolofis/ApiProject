using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT
{
    public class OrderDetailIngredientsDT : IOrderDetailIngredientsDT
    {
        IGenericDAO<OrderDetailIgredientsDTO> dt;
        IUsersToDatabasesXML users;
        string connectionString;

        public OrderDetailIngredientsDT(IGenericDAO<OrderDetailIgredientsDTO> dt, IUsersToDatabasesXML users)
        {
            this.dt = dt;
            this.users = users;
        }

        /// <summary>
        /// Add's new OrderDetailIngredientsModel to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddOrderDetailIngredints(DBInfoModel Store, OrderDetailIngredientsModel item)
        {
            string Error;
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return dt.Upsert(db, AutoMapper.Mapper.Map<OrderDetailIgredientsDTO>(item), out Error);
            }
        }

        /// <summary>
        /// Add's new OrderDetailIngredientsModel to db
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <param name="dbTransact"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public long AddOrderDetailIngredints(IDbConnection db, OrderDetailIngredientsModel item, IDbTransaction dbTransact, out string Error)
        {
            Error = "";
            return dt.Upsert(db, AutoMapper.Mapper.Map<OrderDetailIgredientsDTO>(item), dbTransact, out Error);
        }

        /// <summary>
        /// Return an OrderDetailIngredients using Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public OrderDetailIngredientsModel GetOrderDetailIngredientsById(DBInfoModel Store, long Id)
        {
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return AutoMapper.Mapper.Map<OrderDetailIngredientsModel>(dt.SelectFirst(db, "WHERE Id = @Id", new { Id = Id }));
            }
        }
    }
}
