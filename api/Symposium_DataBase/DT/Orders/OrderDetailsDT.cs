using Symposium.Models.Enums;
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
    public class OrderDetailsDT : IOrderDetailsDT
    {
        IGenericDAO<OrderDetailDTO> dt;
        IUsersToDatabasesXML usersToDatabases;

        string connectionString;

        public OrderDetailsDT(IGenericDAO<OrderDetailDTO> dt, IUsersToDatabasesXML usersToDatabases)
        {
            this.dt = dt;
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Return's a List Of OrderDetail's using list of Ids
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public List<OrderDetailModel> GetOrderDetailsByIds(DBInfoModel Store, List<long> Ids)
        {
            List<OrderDetailModel> ret = new List<OrderDetailModel>();
            try
            {
                string sKeys = string.Join(", ", Ids.ToString());
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    ret = AutoMapper.Mapper.Map<List<OrderDetailModel>>(dt.Select(db, "WHERE Id IN (@Id)", new { Id = sKeys }));
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return ret;
        }

        /// <summary>
        /// Add's new order Detail
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddOrderDetail(DBInfoModel Store, OrderDetailModel item)
        {
            string Error;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return dt.Upsert(db, AutoMapper.Mapper.Map<OrderDetailDTO>(item), out Error);
            }
        }

        /// <summary>
        /// Add's new order Detail
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddOrderDetail(IDbConnection db, OrderDetailModel item, IDbTransaction dbTransact, out string Error)
        {
            Error = "";
            return dt.Upsert(db, AutoMapper.Mapper.Map<OrderDetailDTO>(item), dbTransact, out Error);
        }

        /// <summary>
        /// Returns an OrderDetail using Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public OrderDetailModel GetOrderDetailById(DBInfoModel Store, long Id)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return AutoMapper.Mapper.Map<OrderDetailModel>(dt.SelectFirst(db, "WHERE Id = @Id", new { Id = Id }));
            }
        }

        /// <summary>
        /// Update OrderDetail set status for a specific Order Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public bool SetStatusToOrderDetails(DBInfoModel Store, long OrderId, OrderStatusEnum Status)
        {
            bool result = true;
            try
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    string SQL = "UPDATE OrderDetail SET Status = @Status WHERE OrderId = @OrderId";
                    dt.Execute(db, SQL, new { Status = Status, OrderId = OrderId });
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }


    }
}
