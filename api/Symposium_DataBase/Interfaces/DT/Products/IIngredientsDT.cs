using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    public interface IIngredientsDT
    {
        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<IngredientsSched_Model> model);


        /// <summary>
        /// check if an ingredient belongs to the product's recipe
        /// </summary>
        ///  <param name="Store">db</param>
        /// <param name="ProductId">Product Id</param>
        /// <param name="IngredientId">Ingredient Id</param>
        /// <returns></returns>
        bool IsProductRecipe(DBInfoModel Store, long ProductId, long IngredientId);

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateModel(DBInfoModel Store, IngredientsModel item);

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateModelList(DBInfoModel Store, List<IngredientsModel> item);

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long InsertModel(DBInfoModel Store, IngredientsModel item);


        /// <summary>
        /// Delete's Or Set's the Field IsDeleted if Exists to true for a list
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel DeleteRecordsSendedFromDAServer(DBInfoModel Store, List<IngredientsSched_Model> model);

        /// <summary>
        /// Return Table Id from field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="dAId"></param>
        /// <returns></returns>
        long? GetIdByDAIs(DBInfoModel Store, long dAId);

        /// <summary>
        /// Return Table Id from field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="dAId"></param>
        /// <returns></returns>
        IngredientsModel GetModelByDAIs(DBInfoModel Store, long dAId, IDbConnection dbTran = null, IDbTransaction dbTransact = null);

        /// <summary>
        /// Return an Ingredient based on code
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="code">code</param>
        /// <returns></returns>
        IngredientsModel GetModelByCode(DBInfoModel Store, string code);

        /// <summary>
        /// Return an Ingredient based on id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        IngredientsModel GetModelById(DBInfoModel Store, long id);
    }
}
