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
    public class PosInfoDetailTasks : IPosInfoDetailTasks
    {
        IPosInfoDetailDT dt;

        public PosInfoDetailTasks(IPosInfoDetailDT dt)
        {
            this.dt = dt;
        }

        /// <summary>
        /// Return new counter for specific PosIfo and PosInfoDetail
        /// Update's PosInfoDetail all record's for same GroupId and posinfo id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="PosInfoId"></param>
        /// <param name="PosInfoDetailId"></param>
        /// <returns></returns>
        public long GetNextCounter(DBInfoModel Store, long PosInfoId, long PosInfoDetailId)
        {
            return dt.GetNextCounter(Store, PosInfoId, PosInfoDetailId);
        }

        /// <summary>
        /// Return new counter for specific PosIfo and PosInfoDetail
        /// Update's PosInfoDetail all record's for same GroupId and posinfo id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="PosInfoId"></param>
        /// <param name="PosInfoDetailId"></param>
        /// <param name="InvoiceTypeType"></param>
        /// <returns></returns>
        public long GetNextCounter(DBInfoModel Store, long PosInfoId, long PosInfoDetailId, int InvoiceTypeType)
        {
            return dt.GetNextCounter(Store, PosInfoId, PosInfoDetailId, InvoiceTypeType);
        }

        /// <summary>
        /// Get pos info detail according to selected Id
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="PosInfoDetailId"> PosInfoDetail </param>
        /// <returns> Pos info detail model. See: <seealso cref="Symposium.Models.Models.PosInfoDetailModel"/> </returns>
        public PosInfoDetailModel GetSinglePosInfoDetailByposId(DBInfoModel Store, long PosInfoId)
        {
            return dt.GetSinglePosInfoDetailByposId(Store, PosInfoId);
        }

        /// <summary>
        /// Get pos info detail according to PosInfo Id and GroupId
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="PosInfoDetailId"> PosInfoDetail </param>
        /// <param name="GroupId"></param>
        /// <returns> Pos info detail model. See: <seealso cref="Symposium.Models.Models.PosInfoDetailModel"/> </returns>
        public PosInfoDetailModel GetSinglePosInfoDetail(DBInfoModel Store, long PosInfoId, long GroupId)
        {
            return dt.GetSinglePosInfoDetail(Store, PosInfoId, GroupId);
        }

        /// <summary>
        /// Get pos info detail according to selected Id
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="PosInfoDetailId"> PosInfoDetail </param>
        /// <returns> Pos info detail model. See: <seealso cref="Symposium.Models.Models.PosInfoDetailModel"/> </returns>
        public PosInfoDetailModel GetSinglePosInfoDetail(DBInfoModel Store, long PosInfoDetailId, IDbConnection dbTran = null, IDbTransaction dbTransact = null)
        {
            return dt.GetSinglePosInfoDetail(Store, PosInfoDetailId, dbTran, dbTransact);
        }
    }
}
