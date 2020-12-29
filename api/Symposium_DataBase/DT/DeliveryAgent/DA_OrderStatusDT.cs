using Dapper;
using Symposium.Helpers.Classes;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT.DeliveryAgent
{
    public class DA_OrderStatusDT : IDA_OrderStatusDT
    {
        IGenericDAO<DA_OrderStatusDTO> dt;
        IUsersToDatabasesXML users;
        string connectionString;
        LocalConfigurationHelper configHlp;

        public DA_OrderStatusDT(IGenericDAO<DA_OrderStatusDTO> dt, IUsersToDatabasesXML users, LocalConfigurationHelper configHlp)
        {
            this.dt = dt;
            this.users = users;
            this.configHlp = configHlp;
        }

        /// <summary>
        /// Insert's a New Model To DB
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewModel(DBInfoModel Store, DA_OrderStatusModel item)
        {
            configHlp.CheckDeliveryAgent();
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return dt.Insert(db, AutoMapper.Mapper.Map<DA_OrderStatusDTO>(item));
            }
        }

        /// <summary>
        /// Update statuses for DA_Order
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public void UpdateDa_OrderStatus(DBInfoModel Store, DA_OrderStatusModel item)
        {
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string SQL = "UPDATE DA_Orders SET Status = CASE WHEN Status = 5 THEN Status ELSE " + item.Status.ToString() + " END, StatusChange = GETDATE() \n";

                switch (item.Status)
                {
                    case (short)OrderStatusEnum.Ready:
                        SQL += " , TakeoutDate = GETDATE() \n";
                        break;
                    case (short)OrderStatusEnum.Onroad:
                    case (short)OrderStatusEnum.Canceled:
                    case (short)OrderStatusEnum.Complete:
                        SQL += " , FinishDate = GETDATE(), Duration = DATEDIFF(minute, OrderDate, GETDATE()) \n";
                        break;

                }
                SQL += "WHERE Id = @Id";
                dt.Execute(db, SQL, new { Id = item.OrderDAId });
            }
            return;
        }

        /// <summary>
        /// Delete's a list of DA_OrdrStatuses
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <param name="ErrorMess"></param>
        /// <returns></returns>
        public bool DeleteDA_OrderStatusList(DBInfoModel Store, List<DA_OrderStatusModel> model, out string ErrorMess)
        {
            bool res = true;
            ErrorMess = "";
            try
            {
                connectionString = users.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    foreach (DA_OrderStatusModel item in model)
                        dt.Delete(db, AutoMapper.Mapper.Map<DA_OrderStatusDTO>(item));
                }
            }
            catch (Exception ex)
            {
                res = false;
                ErrorMess = ex.ToString();
            }
            return res;

        }

        /// <summary>
        /// Get's a list of DAOrderStatuses
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DAOrderId"></param>
        /// <returns></returns>
        public List<DA_OrderStatusModel> GetDA_OrderStatusesByOrderId(DBInfoModel Store, long DAOrderId)
        {
            List<DA_OrderStatusModel> res = new List<DA_OrderStatusModel>();

            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
                res = AutoMapper.Mapper.Map<List<DA_OrderStatusModel>>(dt.Select(db, "WHERE OrderDAId = @OrderDAId", new { OrderDAId = DAOrderId }));

            return res;
        }

        /// <summary>
        /// Get's a List of orders with max status onhold (based on statusdate) and hour different bwtween now and statusdate bigger than 2
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public List<long> GetOnHoldOrdersForDelete(DBInfoModel Store, int delMinutes)
        {
            List<long> res = new List<long>();
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sql = "SELECT DISTINCT do.OrderDAId  \n"
                           + "FROM DA_OrderStatus AS dos  \n"
                           + "CROSS APPLY (  \n"
                           + "	SELECT fin.[Status], fin.OrderDAId, fin.StatusDate  \n"
                           + "	FROM (  \n"
                           + "		SELECT TOP 1 do.[Status], do.StatusDate, do.OrderDAId  \n"
                           + "		FROM DA_OrderStatus AS do   \n"
                           + "		WHERE do.OrderDAId = dos.OrderDAId  \n"
                           + "		ORDER BY do.StatusDate DESC  \n"
                           + "	) fin   \n"
                           + "	WHERE CASE WHEN fin.[Status] = @OnHold THEN 1 ELSE 0 END = 1	  \n"
                           + ") do  \n"
                           + "WHERE dos.[Status] = @OnHold AND DATEDIFF(minute,do.StatusDate,GETDATE()) > @delMinutes \n"
                           + "UNION ALL \n"
                           + "SELECT DISTINCT do.Id OrderDAId \n"
                           + "FROM DA_Orders do  \n"
                           + "LEFT OUTER JOIN DA_OrderStatus da ON da.OrderDAId = do.Id \n"
                           + "WHERE do.Status = @OnHold and DATEDIFF(minute,do.OrderDate,GETDATE()) > @delMinutes and da.Id IS NULL";
                res = db.Query<long>(sql, new { OnHold = (int)OrderStatusEnum.OnHold, delMinutes = delMinutes }).ToList();
            }
            return res;
        }
    }
}
