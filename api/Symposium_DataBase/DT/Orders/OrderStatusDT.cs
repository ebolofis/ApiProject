using Dapper;
using log4net;
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
    public class OrderStatusDT : IOrderStatusDT
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IGenericDAO<OrderStatusDTO> dt;
        IUsersToDatabasesXML users;
        string conncetionString;

        public OrderStatusDT(IGenericDAO<OrderStatusDTO> dt, IUsersToDatabasesXML users)
        {
            this.dt = dt;
            this.users = users;
        }

        /// <summary>
        /// Add new record to db
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <param name="dbTransact"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public long AddOrderStatus(IDbConnection db, OrderStatusModel item,IDbTransaction dbTransact, out string Error)
        {
            return dt.Insert(db, AutoMapper.Mapper.Map<OrderStatusDTO>(item), dbTransact, out Error);
        }

        /// <summary>
        /// Add new record to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddOrderStatus(DBInfoModel Store, OrderStatusModel item)
        {
            OrderStatusModel ordStatusModel = GetLastOrdrStatusForOrderId(Store, item.OrderId ?? 0);
            if (ordStatusModel != null && ordStatusModel.Status == item.Status)
            {
                return ordStatusModel.Id;
            }

            conncetionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(conncetionString))
            {
                return dt.Insert(db, AutoMapper.Mapper.Map<OrderStatusDTO>(item));
            }
        }

        /// <summary>
        /// Return's all not send order status to inform DA
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public List<OrderStatusModel> GetNotSendStatus(DBInfoModel Store, ExternalSystemOrderEnum extType)
        {
            conncetionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(conncetionString))
            {
                List<OrderStatusDTO> dtoModel = dt.Select(db, "WHERE ISNULL(IsSend,0) = 0 AND ExtState = @ExtState", new { ExtState = extType });

                return AutoMapper.Mapper.Map<List<OrderStatusModel>>(dtoModel);
            }
        }

        /// <summary>
        /// Update's a model to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateOrderStatus(DBInfoModel Store, OrderStatusModel item)
        {
            conncetionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(conncetionString))
            {
                return dt.Update(db, AutoMapper.Mapper.Map<OrderStatusDTO>(item));
            }
        }

        /// <summary>
        /// Get's an OrderStatus Model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public OrderStatusModel GetOrderStatusById(DBInfoModel Store, long Id)
        {
            conncetionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(conncetionString))
            {
                return AutoMapper.Mapper.Map<OrderStatusModel>(dt.SelectFirst(db, "WHERE Id = @Id", new { Id = Id }));
            }
        }

        /// <summary>
        /// Return's Last Order Status for an Order Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public OrderStatusModel GetLastOrdrStatusForOrderId(DBInfoModel Store, long OrderId)
        {
            conncetionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(conncetionString))
            {
                string SQL = "SELECT TOP 1 * FROM OrderStatus WHERE OrderId = @Id ORDER BY TimeChanged DESC";
                return db.Query<OrderStatusModel>(SQL, new { Id = OrderId }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Update a list of OrderStatus to IsSend = Parameter IsSend using List Of Order Status Ids
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Ids"></param>
        /// <param name="IsSend"></param>
        public void UpdateListOfOrderStatusToIsSendById(DBInfoModel Store, List<long> Ids, bool IsSend)
        {
            conncetionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(conncetionString))
            {
                foreach (long item in Ids)
                {
                    OrderStatusDTO sendModel = dt.SelectFirst(db, "WHERE Id = @Id", new { Id = item });
                    sendModel.IsSend = IsSend;
                    dt.Update(db, sendModel);
                }
            }
        }

    }
}
