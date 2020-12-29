using Dapper;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DAOs
{
    public class InvoicesDAO : IInvoicesDAO
    {

        IGenericDAO<InvoicesDTO> geninv;
        public InvoicesDAO(IGenericDAO<InvoicesDTO> _geninv)
        {
            geninv = _geninv;
        }
        /// <summary>
        /// Returns id of new invoice that was created during cancelation of selected invoice
        /// </summary>
        /// <param name="db"> DB Connection </param>
        /// <param name="InvoiceId"> Invoice </param>
        /// <param name="PosInfoId"> PosInfo </param>
        /// <param name="StaffId"> Staff </param>
        /// <returns> New invoice id </returns>
        public int cancelReceipt(IDbConnection db, long InvoiceId, long PosInfoId, long StaffId, bool CancelOrder, out long NewInvoiceId)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@invoiceId", InvoiceId);
            queryParameters.Add("@posInfo", PosInfoId);
            queryParameters.Add("@staffId", StaffId);
            queryParameters.Add("@NewInvoiceId", dbType: DbType.Int64, direction: ParameterDirection.Output);
            int returnStoredProcedure = -1;
            if (CancelOrder)
                returnStoredProcedure = db.Query<int>("CancelOrder", queryParameters, commandType: CommandType.StoredProcedure).FirstOrDefault<int>();
            else
                returnStoredProcedure = db.Query<int>("CancelReceipt", queryParameters, commandType: CommandType.StoredProcedure).FirstOrDefault<int>();
            NewInvoiceId = queryParameters.Get<long>("@NewInvoiceId");
            return returnStoredProcedure;
        }

        public long getTicketCount(IDbConnection db, long posInfo)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("PosInfoId", posInfo);

            string query = @"SELECT COUNT(*)
                                FROM Invoices AS i
                                INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId
                                WHERE i.PosInfoId = @PosInfoId AND i.IsVoided = 0 AND i.IsDeleted IS NULL AND it.[Type] NOT IN (2, 3, 8) AND i.EndOfDayId IS NULL";

            return db.Query<long>(query, queryParameters).FirstOrDefault<long>();
        }

        /// <summary>
        /// Get Active invoices by OrderNo
        /// </summary>
        /// <param name="db"></param>
        /// <param name="orderno"></param>
        /// <param name="EndOfDayId"></param>
        /// <returns></returns>
        public List<InvoicesDTO> InvoicesByOrderNo(IDbConnection db, string orderno)
        {
            string query = "select * from Invoices where ltrim(rtrim(OrderNo)) = @ordrn and EndOfDayId is null ";
            return geninv.Select(query, new { ordrn = orderno }, db);
        }
    }
}
