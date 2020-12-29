using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows
{
    public class TransferToPmsFlows : ITransferToPmsFlows
    {
        ITransferToPmsTasks task;

        public TransferToPmsFlows(ITransferToPmsTasks task)
        {
            this.task = task;
        }

        /// <summary>
        /// Add's new record to TransferToPms
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewTransferToPms(DBInfoModel dbInfo, TransferToPmsModel item)
        {
            return task.AddNewTransferToPms(dbInfo, item);
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
            return task.AddNewTransferToPms(db, item, dbTransact, out Error);
        }

        /// <summary>
        /// Return's a record for Transfer To Pms by Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public TransferToPmsModel GetModelById(DBInfoModel Store, long Id)
        {
            return task.GetModelById(Store, Id);
        }
    }
}
