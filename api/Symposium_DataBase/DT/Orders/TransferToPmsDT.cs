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
    public class TransferToPmsDT : ITransferToPmsDT
    {
        IGenericDAO<TransferToPmsDTO> dt;
        IUsersToDatabasesXML users;
        string connectionString;

        public TransferToPmsDT(IGenericDAO<TransferToPmsDTO> dt, IUsersToDatabasesXML users)
        {
            this.dt = dt;
            this.users = users;
        }

        /// <summary>
        /// Add's new record to TransferToPms
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewTransferToPms(DBInfoModel Store, TransferToPmsModel item)
        {
            string Error;
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return dt.Upsert(db, AutoMapper.Mapper.Map<TransferToPmsDTO>(item), out Error);
            }
        }

        /// <summary>
        /// Add's new record to TransferToPms
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <param name="dbTransact"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public long AddNewTransferToPms(IDbConnection db, TransferToPmsModel item, IDbTransaction dbTransact, out string Error)
        {
            return dt.Upsert(db, AutoMapper.Mapper.Map<TransferToPmsDTO>(item), dbTransact, out Error);
        }

        /// <summary>
        /// Return's a record for Transfer To Pms by Transaction Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="TransactionId"></param>
        /// <returns></returns>
        public TransferToPmsModel GetModelByTransactionId(DBInfoModel Store, long TransactionId)
        {
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return AutoMapper.Mapper.Map<TransferToPmsModel>(dt.SelectFirst(db, "WHERE TransactionId = @TransactionId", new { TransactionId = TransactionId }));
            }
        }

        /// <summary>
        /// Return's a record for Transfer To Pms by Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public TransferToPmsModel GetModelById(DBInfoModel Store, long Id)
        {
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return AutoMapper.Mapper.Map<TransferToPmsModel>(dt.Select(db,Id));
            }
        }

        public List<TransferToPmsModel> GetTransfersToPmsByTransactionIds(DBInfoModel Store, long TransactionId)
        {
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return AutoMapper.Mapper.Map<List<TransferToPmsModel>>(dt.Select(db, "WHERE TransactionId = @TransactionId", new { TransactionId = TransactionId }));
            }
        }

        public int UpdateTransferToPms(DBInfoModel Store, TransferToPmsModel transferToPms)
        {
            int rowsAffected;
            TransferToPmsDTO dbTransferToPms = AutoMapper.Mapper.Map<TransferToPmsDTO>(transferToPms);
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                rowsAffected = dt.Update(db, dbTransferToPms);
            }
            return rowsAffected;
        }
    }
}
