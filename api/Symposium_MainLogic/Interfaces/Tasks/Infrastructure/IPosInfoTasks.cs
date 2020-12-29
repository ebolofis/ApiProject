using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface IPosInfoTasks
    {

        /// <summary>
        /// Return's ExtEcr Name for specific PosInfo Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="PosInfoId"></param>
        /// <returns></returns>
        string GetExtEcrName(DBInfoModel Store, long PosInfoId);

        /// <summary>
        /// Get pos info according to selected Id
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="PosInfoId"> PosInfo </param>
        /// <returns> Pos info model. See: <seealso cref="Symposium.Models.Models.PosInfoModel"/> </returns>
        PosInfoModel GetSinglePosInfo(DBInfoModel Store, long PosInfoId);

        /// <summary>
        /// Return's Pos Department Description
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DepartmentId"></param>
        /// <returns></returns>
        string GetPosDepartmentDescr(DBInfoModel Store, long DepartmentId);
        
    }
}
