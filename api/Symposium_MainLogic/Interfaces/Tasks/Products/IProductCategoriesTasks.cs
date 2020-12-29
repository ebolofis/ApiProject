using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface IProductCategoriesTasks
    {


        /// <summary>
        /// Return the list with all Product Categories including disabled (status=0)
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="status">status=true : return only enabled Product Categories,  status=false : return all </param>
        /// <returns></returns>
        List<ProductCategoriesModel> GetAll(DBInfoModel Store, bool status);

        List<ProductsCategoriesComboList> GetComboList(DBInfoModel dbInfo);

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<ProductCategoriesSched_Model> model);

        //Update Vat for the selected Product Categories
        int UpdateProductCategoriesVat(DBInfoModel DbInfo, ChangeProdCatModel Obj);
        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateModel(DBInfoModel Store, ProductCategoriesModel item);

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateModelList(DBInfoModel Store, List<ProductCategoriesModel> item);

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long InsertModel(DBInfoModel Store, ProductCategoriesModel item);

        /// <summary>
        /// Return's Product Categories from Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        ProductCategoriesModel GetModelById(DBInfoModel Store, long Id, IDbConnection dbTran = null, IDbTransaction dbTransact = null);
        
        /// <summary>
        /// get list of product category ids and descriptions based on given product category ids
        /// </summary>
        /// <param name="productCategoryIdList">List<long></param>
        /// <returns>List<ProductsCategoriesComboList></returns>
        List<ProductsCategoriesComboList> GetProductCategoryDescriptions(DBInfoModel Store, ProductsCategoriesIdList productCategoryIdList);
    }
}
