using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface ITransferToPmsTasks
    {

        /// <summary>
        /// Add's new record to TransferToPms
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddNewTransferToPms(DBInfoModel Store, TransferToPmsModel item);

        /// <summary>
        /// Add's new record to TransferToPms
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <param name="dbTransact"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        long AddNewTransferToPms(IDbConnection db, TransferToPmsModel item, IDbTransaction dbTransact, out string Error);

        /// <summary>
        /// Return's a list of TransferToPms for list Of Transactions
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="TransactionId"></param>
        /// <returns></returns>
        List<TransferToPmsModel> GetModelByTransactionId(DBInfoModel Store, List<long> TransactionId);

        /// <summary>
        /// Return's a record for Transfer To Pms by Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        TransferToPmsModel GetModelById(DBInfoModel Store, long Id);

        List<TransferToPmsModel> GetTransfersToPmsByTransactionIds(DBInfoModel Store, List<long> TransactionId);

        void UpdateTransfersToPms(DBInfoModel Store, List<TransferToPmsModel> transfersToPMS);
    }
}
