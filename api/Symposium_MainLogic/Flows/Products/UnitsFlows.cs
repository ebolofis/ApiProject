using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows
{
    public class UnitsFlows : IUnitsFlows
    {
        IUnitsTasks Task;

        public UnitsFlows(IUnitsTasks Task)
        {
            this.Task = Task;
        }


        /// <summary>
        /// Return a list with all units
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        public List<UnitsModel> GetAll(DBInfoModel dbInfo)
        {
            return Task.GetAll(dbInfo);
        }

        /// <summary>
        /// Return's list of actions after upsert, using as search field DAId
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel dbInfo, List<UnitsSched_Model> model)
        {
            return Task.InformTablesFromDAServer(dbInfo, model);
        }

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModel(DBInfoModel dbInfo, UnitsModel item)
        {
            return Task.UpdateModel(dbInfo, item);
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel dbInfo, List<UnitsModel> item)
        {
            return Task.UpdateModelList(dbInfo, item);
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel dbInfo, UnitsModel item)
        {
            return Task.InsertModel(dbInfo, item);
        }
    }
}
