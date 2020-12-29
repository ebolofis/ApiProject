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
    public class ProductCategoriesFlows : IProductCategoriesFlows
    {
        IProductCategoriesTasks Tasks;

        public ProductCategoriesFlows(IProductCategoriesTasks Tasks)
        {
            this.Tasks = Tasks;
        }

        /// <summary>
        /// Return the list with all Product Categories including disabled (status=0)
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="status">status=true : return only enabled Product Categories,  status=false : return all </param>
        /// <returns></returns>
        public List<ProductCategoriesModel> GetAll(DBInfoModel dbInfo, bool status)
        {
            return Tasks.GetAll(dbInfo, status);
        }

        public List<ProductsCategoriesComboList> GetComboList(DBInfoModel dbInfo)
        {
            return Tasks.GetComboList(dbInfo);
        }


        /// <summary>
        /// Return's list of actions after upsert, using as search field DAId
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel dbInfo, List<ProductCategoriesSched_Model> model)
        {
            return Tasks.InformTablesFromDAServer(dbInfo, model);
        }
        // Update Selected Product Categories Vat
       public int UpdateProductCategoriesVat(DBInfoModel DbInfo,ChangeProdCatModel Obj)
        {
            return Tasks.UpdateProductCategoriesVat(DbInfo,Obj);
            
        }
        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModel(DBInfoModel dbInfo, ProductCategoriesModel item)
        {
            return Tasks.UpdateModel(dbInfo, item);
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel dbInfo, List<ProductCategoriesModel> item)
        {
            return Tasks.UpdateModelList(dbInfo, item);
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel dbInfo, ProductCategoriesModel item)
        {
            return Tasks.InsertModel(dbInfo, item);
        }

        /// <summary>
        /// get list of product category ids and descriptions based on given product category ids
        /// </summary>
        /// <param name="productCategoryIdList">List<long></param>
        /// <returns>List<ProductsCategoriesComboList></returns>
        public List<ProductsCategoriesComboList> GetProductCategoryDescriptions(DBInfoModel DBInfo, ProductsCategoriesIdList productCategoryIdList)
        {
            return Tasks.GetProductCategoryDescriptions(DBInfo, productCategoryIdList);
        }
    }
}
