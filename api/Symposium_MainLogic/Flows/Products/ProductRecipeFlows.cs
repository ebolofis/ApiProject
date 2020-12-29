using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Products;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.Products
{
    public class ProductRecipeFlows : IProductRecipeFlows
    {
        IProductRecipeTasks task;

        public ProductRecipeFlows(IProductRecipeTasks task)
        {
            this.task = task;
        }

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<ProductRecipeSched_Model> model)
        {
            return task.InformTablesFromDAServer(Store, model);
        }
    }
}
