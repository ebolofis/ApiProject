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
    public class ProductFlows : IProductFlows
    {
        IProductTasks tasks;

        public ProductFlows(IProductTasks tasks)
        {
            this.tasks = tasks;
        }

        /// <summary>
        /// Get Products(DAId, Description) For Dropdown List 
        /// </summary>
        /// <returns></returns>
        public List<ProductsComboList> GetComboListDA(DBInfoModel dbInfo)
        {
            return tasks.GetComboListDA(dbInfo);
        }
        public List<ProductsComboList> GetComboList(DBInfoModel dbInfo)
        {
            return tasks.GetComboList(dbInfo);
        }

        /// <summary>
        /// Return the extended list of products (active only). Every product contains the list of (active) Extras.
        /// </summary>
        /// <returns></returns>
        public List<ProductExtModel> GetExtentedList(DBInfoModel dbInfo)
        {
            return tasks.GetExtentedList(dbInfo);
        }


        /// <summary>
        /// Return the extended list of products filtered by department (active only). Every product contains the list of (active) Extras.
        /// </summary>
        /// <returns></returns>
        public List<ProductExtModel> GetDepartmentExtentedList(DBInfoModel dbInfo,long pmsDepartmentId, long pricelistId)
        {
            return tasks.GetDepartmentExtentedList(dbInfo, pmsDepartmentId, pricelistId);
        }

        /// <summary>
        /// Return List of Pruducts Categories (acrive only). 
        /// Every List of Pruducts Categories includes the extended list of products (active only). 
        /// Every product contains the list of (active) Ingredient Categories. 
        /// Every Ingredient Category includes the extended list of Ingredients (active only).
        /// </summary>
        /// <returns></returns>
        public List<ProductCategoriesSkroutzExtModel> GetSkroutzProducts(DBInfoModel dbInfo, long PricelistId)
        {
            return tasks.GetSkroutzProducts(dbInfo, PricelistId);
        }

        /// <summary>
        /// Return's list of actions after upsert, using as search field DAId
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel dbInfo, List<ProductSched_Model> model)
        {
            foreach (ProductSched_Model item in model)
            {
                ProductModel tmpModel = tasks.GetModelByDAIs(dbInfo, item.DAId ?? 0);
                if(tmpModel != null)
                {
                    item.KdsId = tmpModel.KdsId;
                    item.KitchenId = tmpModel.KitchenId;
                    item.KitchenRegionId = tmpModel.KitchenRegionId;
                }
            }


            return tasks.InformTablesFromDAServer(dbInfo, model);
        }

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModel(DBInfoModel dbInfo, ProductModel item)
        {
            return tasks.UpdateModel(dbInfo, item);
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel dbInfo, List<ProductModel> item)
        {
            return tasks.UpdateModelList(dbInfo, item);
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel dbInfo, ProductModel item)
        {
            return tasks.InsertModel(dbInfo, item);
        }

        /// <summary>
        /// get list of product ids and descriptions based on given product ids
        /// </summary>
        /// <param name="productIdList">List<long></param>
        /// <returns>List<ProductDescription></returns>
        public List<ProductDescription> GetProductDescriptions(DBInfoModel DBInfo, ProductIdsList productIdList)
        {
            return tasks.GetProductDescriptions(DBInfo, productIdList);
        }
    }
}
