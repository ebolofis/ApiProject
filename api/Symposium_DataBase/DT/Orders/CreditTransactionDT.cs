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
    public class CreditTransactionDT : ICreditTransactionDT
    {
        IGenericDAO<CreditTransactionsDTO> dt;
        IUsersToDatabasesXML users;
        string connectionString;

        public CreditTransactionDT(IGenericDAO<CreditTransactionsDTO> dt, IUsersToDatabasesXML users)
        {
            this.dt = dt;
            this.users = users;
        }

        /// <summary>
        /// Add new Credit Transaction to db
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dbTranaction"></param>
        /// <param name="item"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public long AddNewCreditTransaction(DBInfoModel Store, CreditTransactionsModel item)
        {
            connectionString = users.ConfigureConnectionString(Store);
            string Error = "";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return dt.Upsert(db, AutoMapper.Mapper.Map<CreditTransactionsDTO>(item), out Error);                
            }
        }

        /// <summary>
        /// Add new Credit Transaction to db
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <param name="dbTransact"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public long AddNewCreditTransaction(IDbConnection db, CreditTransactionsModel item, IDbTransaction dbTransact, out string Error)
        {
            Error = "";
            return dt.Upsert(db, AutoMapper.Mapper.Map<CreditTransactionsDTO>(item), dbTransact, out Error);
        }

        /// <summary>
        /// Return's a List of records for specific Ijvoice Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        public List<CreditTransactionsModel> GetListModelByInvoiceId(DBInfoModel Store, long InvoiceId)
        {
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return AutoMapper.Mapper.Map<List<CreditTransactionsModel>>(dt.Select(db, "WHERE InvoiceId = @InvoiceId", new { InvoiceId = InvoiceId }));
            }
        }

    }
}
