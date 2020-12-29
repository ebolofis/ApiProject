using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows
{
    public interface ITransferToPmsFlows
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
        /// Return's a record for Transfer To Pms by Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        TransferToPmsModel GetModelById(DBInfoModel Store, long Id);
    }
}
