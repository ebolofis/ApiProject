using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DT.Products;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks.Products
{
    public class ProductRecipeTasks : IProductRecipeTasks
    {
        IProductRecipeDT dt;

        public ProductRecipeTasks(IProductRecipeDT dt)
        {
            this.dt = dt;
        }

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<ProductRecipeSched_Model> model)
        {
            List<ProductRecipeSched_Model> Upserts = model.Where(w => w.Action != 1).Select(s => s).ToList();
            List<ProductRecipeSched_Model> Deleted = model.Where(w => w.Action == 1).Select(s => s).ToList();

            UpsertListResultModel ups = dt.InformTablesFromDAServer(Store, Upserts);
            UpsertListResultModel del = dt.DeleteRecordsSendedFromDAServer(Store, Deleted);

            ups.TotalDeleted += del.TotalDeleted;
            ups.TotalFailed += del.TotalFailed;
            ups.TotalInserted += del.TotalInserted;
            ups.TotalRecords += del.TotalRecords;
            ups.TotalSucceded += del.TotalSucceded;
            ups.TotalUpdated += del.TotalUpdated;
            ups.TotalUpdated += del.TotalUpdated;
            if (ups.Results != null && ups.Results.Count > 0)
                ups.Results.Union(del.Results);
            else
                ups.Results.AddRange(del.Results);

            return ups;
        }
    }
}
