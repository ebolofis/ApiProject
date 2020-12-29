using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows
{
    public interface IProductCategoriesFlows
    {

        /// <summary>
        /// Return the list with all Product Categories including disabled (status=0)
        /// </summary>
        /// <param name="status">status=true : return only enabled Product Categories,  status=false : return all </param>
        /// <returns></returns>
        List<ProductCategoriesModel> GetAll(DBInfoModel Store,bool status);

        List<ProductsCategoriesComboList> GetComboList(DBInfoModel dbInfo);


        /// <summary>
        /// Return's list of Product Categories after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<ProductCategoriesSched_Model> model);

    //Updates the Vat Value of Selected Product Categories
        int  UpdateProductCategoriesVat(DBInfoModel DbInfo, ChangeProdCatModel Obj);

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
        /// get list of product category ids and descriptions based on given product category ids
        /// </summary>
        /// <param name="productCategoryIdList">List<long></param>
        /// <returns>List<ProductsCategoriesComboList></returns>
        List<ProductsCategoriesComboList> GetProductCategoryDescriptions(DBInfoModel Store, ProductsCategoriesIdList productCategoryIdList);
    }
}
