using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{
    public class PosInfoTasks : IPosInfoTasks
    {
        IPosInfoDT dt;
        IDepartmentDT departDt;

        public PosInfoTasks(IPosInfoDT dt, IDepartmentDT departDt)
        {
            this.dt = dt;
            this.departDt = departDt;
        }

        /// <summary>
        /// Return's ExtEcr Name for specific PosInfo Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="PosInfoId"></param>
        /// <returns></returns>
        public string GetExtEcrName(DBInfoModel Store, long PosInfoId)
        {
            return dt.GetExtEcrName(Store, PosInfoId);
        }

        /// <summary>
        /// Get pos info according to selected Id
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="PosInfoId"> PosInfo </param>
        /// <returns> Pos info model. See: <seealso cref="Symposium.Models.Models.PosInfoModel"/> </returns>
        public PosInfoModel GetSinglePosInfo(DBInfoModel Store, long PosInfoId)
        {
            return dt.GetSinglePosInfo(Store, PosInfoId);
        }

        /// <summary>
        /// Return's Pos Department Description
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DepartmentId"></param>
        /// <returns></returns>
        public string GetPosDepartmentDescr(DBInfoModel Store, long DepartmentId)
        {
            return departDt.GetDepartmentDescr(Store, DepartmentId);
        }
    }
}
