using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows
{
    public interface IPosInfoDetailFlows
    {

        /// <summary>
        /// Return new counter for specific PosIfo and PosInfoDetail
        /// Update's PosInfoDetail all record's for same GroupId and posinfo id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="PosInfoId"></param>
        /// <param name="PosInfoDetailId"></param>
        /// <returns></returns>
        long GetNextCounter(DBInfoModel dbInfo, long PosInfoId, long PosInfoDetailId);

        /// <summary>
        /// Return new counter for specific PosIfo and PosInfoDetail
        /// Update's PosInfoDetail all record's for same GroupId and posinfo id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="PosInfoId"></param>
        /// <param name="PosInfoDetailId"></param>
        /// <param name="InvoiceTypeType"></param>
        /// <returns></returns>
        long GetNextCounter(DBInfoModel dbInfo, long PosInfoId, long PosInfoDetailId, int InvoiceTypeType);

        /// <summary>
        /// Get POS info detail according to selected Id
        /// </summary>
        /// <param name="dbInfo"> dbInfo </param>
        /// <param name="PosInfoDetailId"> PosInfoDetail </param>
        /// <returns> POS info detail model. See: <seealso cref="Symposium.Models.Models.PosInfoDetailModel"/> </returns>
        PosInfoDetailModel GetSinglePosInfoDetailByposId(DBInfoModel dbInfo, long PosInfoId);

        /// <summary>
        /// Get POS info detail according to PosInfo Id and GroupId
        /// </summary>
        /// <param name="dbInfo"> dbInfo </param>
        /// <param name="PosInfoDetailId"> PosInfoDetail </param>
        /// <param name="GroupId"></param>
        /// <returns> POS info detail model. See: <seealso cref="Symposium.Models.Models.PosInfoDetailModel"/> </returns>
        PosInfoDetailModel GetSinglePosInfoDetail(DBInfoModel dbInfo, long PosInfoId, long GroupId);
    }
}
