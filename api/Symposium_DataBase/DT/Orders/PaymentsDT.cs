using log4net;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT.Orders;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT.Orders
{
    public class PaymentsDT : IPaymentsDT
    {
        IGenericDAO<TransferToPmsDTO> transferToPmsDAO;
        IUsersToDatabasesXML usersToDatabases;
        string connectionString;
        protected ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PaymentsDT(IGenericDAO<TransferToPmsDTO> _transferToPmsDAO, IUsersToDatabasesXML _usersToDatabases)
        {
            this.transferToPmsDAO = _transferToPmsDAO;
            this.usersToDatabases = _usersToDatabases;
        }

        public bool InsertPmsCharges(DBInfoModel dbInfo, List<TransferToPmsModel> pmsTransfers)
        {
            bool chargesInserted = false;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Open();
                using (IDbTransaction tr = db.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        foreach (TransferToPmsModel transfer in pmsTransfers)
                        {
                            TransferToPmsDTO transferDTO = AutoMapper.Mapper.Map<TransferToPmsDTO>(transfer);
                            string errorMessage = "";
                            long transferId = transferToPmsDAO.Insert(db, transferDTO, tr, out errorMessage);
                        }
                        tr.Commit();
                        chargesInserted = true;
                    }
                    catch (Exception ex)
                    {
                        tr.Rollback();
                        chargesInserted = false;
                        logger.Error("Error inserting charges for PMS customer: " + ex.ToString());
                    }
                }
                db.Close();
            }
            return chargesInserted;
        }

    }
}
