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
    public class VatFlows : IVatFlows
    {
        IVatTasks Task;

        public VatFlows(IVatTasks Task)
        {
            this.Task = Task;
        }

        /// <summary>
        /// Return a list with all vats Included deleted for Delivery Agent;
        /// If One of them is deleted and on DA is not then we have to update to not deleted
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        public List<VatModel> GetAllVats(DBInfoModel dbInfo)
        {
            return Task.GetAllVats(dbInfo);
        }


        /// <summary>
        /// Return's list of actions after upsert, using as search field DAId
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel dbInfo, List<VatSched_Model> model)
        {
            return Task.InformTablesFromDAServer(dbInfo, model);
        }

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModel(DBInfoModel dbInfo, VatModel item)
        {
            return Task.UpdateModel(dbInfo, item);
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel dbInfo, List<VatModel> item)
        {
            return Task.UpdateModelList(dbInfo, item);
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel dbInfo, VatModel item)
        {
            return Task.InsertModel(dbInfo, item);
        }
    }
}
