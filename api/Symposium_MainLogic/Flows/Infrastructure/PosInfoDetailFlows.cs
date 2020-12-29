using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows
{
    public class PosInfoDetailFlows : IPosInfoDetailFlows
    {
        IPosInfoDetailTasks tasks;

        public PosInfoDetailFlows( IPosInfoDetailTasks tasks)
        {
            this.tasks = tasks;
        }

        /// <summary>
        /// Return new counter for specific PosIfo and PosInfoDetail
        /// Update's PosInfoDetail all record's for same GroupId and posinfo id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="PosInfoId"></param>
        /// <param name="PosInfoDetailId"></param>
        /// <returns></returns>
        public long GetNextCounter(DBInfoModel dbInfo, long PosInfoId, long PosInfoDetailId)
        {
            return tasks.GetNextCounter(dbInfo, PosInfoId, PosInfoDetailId);
        }

        /// <summary>
        /// Return new counter for specific PosIfo and PosInfoDetail
        /// Update's PosInfoDetail all record's for same GroupId and posinfo id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="PosInfoId"></param>
        /// <param name="PosInfoDetailId"></param>
        /// <param name="InvoiceTypeType"></param>
        /// <returns></returns>
        public long GetNextCounter(DBInfoModel dbInfo, long PosInfoId, long PosInfoDetailId, int InvoiceTypeType)
        {
            return tasks.GetNextCounter(dbInfo, PosInfoId, PosInfoDetailId, InvoiceTypeType);
        }

        /// <summary>
        /// Get pos info detail according to selected Id
        /// </summary>
        /// <param name="dbInfo"> Store </param>
        /// <param name="PosInfoDetailId"> PosInfoDetail </param>
        /// <returns> Pos info detail model. See: <seealso cref="Symposium.Models.Models.PosInfoDetailModel"/> </returns>
        public PosInfoDetailModel GetSinglePosInfoDetailByposId(DBInfoModel dbInfo, long PosInfoId)
        {
            return tasks.GetSinglePosInfoDetailByposId(dbInfo, PosInfoId);
        }

        /// <summary>
        /// Get pos info detail according to PosInfo Id and GroupId
        /// </summary>
        /// <param name="dbInfo"> Store </param>
        /// <param name="PosInfoDetailId"> PosInfoDetail </param>
        /// <param name="GroupId"></param>
        /// <returns> Pos info detail model. See: <seealso cref="Symposium.Models.Models.PosInfoDetailModel"/> </returns>
        public PosInfoDetailModel GetSinglePosInfoDetail(DBInfoModel dbInfo, long PosInfoId, long GroupId)
        {
            return tasks.GetSinglePosInfoDetail(dbInfo, PosInfoId, GroupId);
        }
    }
}
