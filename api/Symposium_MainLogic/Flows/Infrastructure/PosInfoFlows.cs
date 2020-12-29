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
    public class PosInfoFlows : IPosInfoFlows
    {
        IPosInfoTasks tasks;

        public PosInfoFlows(IPosInfoTasks tasks)
        {
            this.tasks = tasks;
        }

        /// <summary>
        /// Return's ExtEcr Name for specific PosInfo Id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="PosInfoId"></param>
        /// <returns></returns>
        public string GetExtEcrName(DBInfoModel dbInfo, long PosInfoId)
        {
            return tasks.GetExtEcrName(dbInfo, PosInfoId);
        }

        /// <summary>
        /// Get pos info according to selected Id
        /// </summary>
        /// <param name="dbInfo"> Store </param>
        /// <param name="PosInfoId"> PosInfo </param>
        /// <returns> Pos info model. See: <seealso cref="Symposium.Models.Models.PosInfoModel"/> </returns>
        public PosInfoModel GetSinglePosInfo(DBInfoModel dbInfo, long PosInfoId)
        {
            return tasks.GetSinglePosInfo(dbInfo, PosInfoId);
        }

        /// <summary>
        /// Return's Pos Department Description
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="DepartmentId"></param>
        /// <returns></returns>
        public string GetPosDepartmentDescr(DBInfoModel dbInfo, long DepartmentId)
        {
            return tasks.GetPosDepartmentDescr(dbInfo, DepartmentId);
        }
    }
}
