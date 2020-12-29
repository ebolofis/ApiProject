using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{
    public class TransferToPmsTasks : ITransferToPmsTasks
    {
        ITransferToPmsDT dt;

        public TransferToPmsTasks(ITransferToPmsDT dt)
        {
            this.dt = dt;
        }

        /// <summary>
        /// Add's new record to TransferToPms
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewTransferToPms(DBInfoModel Store, TransferToPmsModel item)
        {
            return dt.AddNewTransferToPms(Store, item);
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
            return dt.AddNewTransferToPms(db, item, dbTransact, out Error);
        }

        /// <summary>
        /// Return's a list of TransferToPms for list Of Transactions
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="TransactionId"></param>
        /// <returns></returns>
        public List<TransferToPmsModel> GetModelByTransactionId(DBInfoModel Store, List<long> TransactionId)
        {
            List<TransferToPmsModel> results = new List<TransferToPmsModel>();
            foreach (long item in TransactionId)
                results.Add(dt.GetModelByTransactionId(Store, item));
            return results;
        }

        /// <summary>
        /// Return's a record for Transfer To Pms by Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public TransferToPmsModel GetModelById(DBInfoModel Store, long Id)
        {
            return dt.GetModelById(Store, Id);
        }

        public List<TransferToPmsModel> GetTransfersToPmsByTransactionIds(DBInfoModel Store, List<long> TransactionId)
        {
            List<TransferToPmsModel> results = new List<TransferToPmsModel>();
            if (TransactionId != null)
            {
                foreach (long id in TransactionId)
                {
                    List<TransferToPmsModel> transfersToPms = dt.GetTransfersToPmsByTransactionIds(Store, id);
                    if (transfersToPms != null)
                    {
                        foreach (TransferToPmsModel transfer in transfersToPms)
                        {
                            results.Add(transfer);
                        }
                    }
                }
            }
            return results;
        }

        public void UpdateTransfersToPms(DBInfoModel Store, List<TransferToPmsModel> transfersToPMS)
        {
            foreach(TransferToPmsModel transferToPms in transfersToPMS)
            {
                int rowsAffected = dt.UpdateTransferToPms(Store, transferToPms);
            }
            return;
        }
    }
}
