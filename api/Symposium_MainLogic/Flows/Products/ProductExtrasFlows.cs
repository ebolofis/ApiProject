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
    public class ProductExtrasFlows : IProductExtrasFlows
    {
        IProductExtrasTasks tasks;

        public ProductExtrasFlows(IProductExtrasTasks tasks)
        {
            this.tasks = tasks;
        }

        /// <summary>
        /// Return's list of actions after upsert, using as search field DAId
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel dbInfo, List<ProductExtrasSched_Model> model)
        {
            return tasks.InformTablesFromDAServer(dbInfo, model);
        }

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModel(DBInfoModel dbInfo, ProductExtrasModel item)
        {
            return tasks.UpdateModel(dbInfo, item);
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel dbInfo, List<ProductExtrasModel> item)
        {
            return tasks.UpdateModelList(dbInfo, item);
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel dbInfo, ProductExtrasModel item)
        {
            return tasks.InsertModel(dbInfo, item);
        }
    }
}
