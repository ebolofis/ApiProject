using Dapper;
using log4net;
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
    public class TransactionsDT : ITransactionsDT
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        IGenericDAO<TransactionsDTO> dt;
        IUsersToDatabasesXML usersToDatabases;
        string connectionString;

        public TransactionsDT(IGenericDAO<TransactionsDTO> dt, IUsersToDatabasesXML usersToDatabases)
        {
            this.dt = dt;
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Return's a list of transaction based on invoiceId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        public List<TransactionsModel> GetTransactionsByInvoiceId(DBInfoModel Store, long InvoiceId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return AutoMapper.Mapper.Map<List<TransactionsModel>>(dt.Select(db, "WHERE InvoicesId = @InvoicesId", new { InvoicesId = InvoiceId }));
            }
        }

        /// <summary>
        /// Return's PMSRoom fo TransferToPms and for cash, CC, etc..
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="AccountId"></param>
        /// <param name="PosInfoId"></param>
        /// <returns></returns>
        public PmsRoomModel GetPmsRoomForCashForTransferToPMS(DBInfoModel Store, long AccountId, long PosInfoId)
        {
            PmsRoomModel ret = null;
            StringBuilder SQL = new StringBuilder();
            try
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    SQL.Clear();
                    SQL.Append("SELECT etpt.PmsRoom, a.SendsTransfer \r"
                            + "FROM Accounts AS a \r"
                            + "INNER JOIN EODAccountToPmsTransfer AS etpt ON etpt.AccountId = a.Id AND etpt.PosInfoId = " + PosInfoId.ToString() + " AND etpt.AccountId = " + AccountId.ToString());
                    ret = db.Query<PmsRoomModel>(SQL.ToString()).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                logger.Error("GetPmsRoomForCashForTransferToPMS [SQL:" + SQL.ToString() + "] \r\n" + ex.ToString());
            }
            return ret;
        }

        /// <summary>
        /// add's new transacion to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewTransaction(DBInfoModel Store, TransactionsModel item)
        {
            string Error;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return dt.Upsert(db, AutoMapper.Mapper.Map<TransactionsDTO>(item), out Error);
            }
        }

        /// <summary>
        /// add's new transacion to db
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <param name="dbTransact"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public long AddNewTransaction(IDbConnection db, TransactionsModel item, IDbTransaction dbTransact, out string Error)
        {
            Error = "";
            return dt.Upsert(db, AutoMapper.Mapper.Map<TransactionsDTO>(item), dbTransact, out Error);
        }
    }
}
