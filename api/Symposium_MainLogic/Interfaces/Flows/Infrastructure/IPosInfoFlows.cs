using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows
{
    public interface IPosInfoFlows
    {

        /// <summary>
        /// Return's ExtEcr Name for specific PosInfo Id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="PosInfoId"></param>
        /// <returns></returns>
        string GetExtEcrName(DBInfoModel dbInfo, long PosInfoId);

        /// <summary>
        /// Get POS info according to selected Id
        /// </summary>
        /// <param name="dbInfo"> dbInfo </param>
        /// <param name="PosInfoId"> PosInfo </param>
        /// <returns> POS info model. See: <seealso cref="Symposium.Models.Models.PosInfoModel"/> </returns>
        PosInfoModel GetSinglePosInfo(DBInfoModel dbInfo, long PosInfoId);

        /// <summary>
        /// Return's POS Department Description
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="DepartmentId"></param>
        /// <returns></returns>
        string GetPosDepartmentDescr(DBInfoModel dbInfo, long DepartmentId);
    }
}
