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
    public interface IProductDT
    {
        /// <summary>
        /// Get Products(DAId, Description) For Dropdown List 
        /// </summary>
        /// <returns></returns>
        List<ProductsComboList> GetComboListDA(DBInfoModel Store);
        /// <summary>
        /// Get Products(Id, Description) For Dropdown List 
        /// </summary>
        /// <returns></returns>
        List<ProductsComboList> GetComboList(DBInfoModel Store);
        /// <summary>
        /// Return the extended list of products (active only). Every product contains the list of (active) Extras.
        /// </summary>
        /// <returns></returns>
        List<ProductExtModel> GetExtentedList(DBInfoModel Store);
        List<ProductExtModel> GetDepartmentExtentedList(DBInfoModel dbInfo, long pmsDepartmentId);
        List<ProductExtModel> GetDepartmentExtentedList(DBInfoModel dbInfo, long PosDepartmentId, long pricelistId);
        /// <summary>
        /// Return List of Pruducts Categories (acrive only). 
        /// Every List of Pruducts Categories includes the extended list of products (active only). 
        /// Every product contains the list of (active) Ingredient Categories. 
        /// Every Ingredient Category includes the extended list of Ingredients (active only).
        /// </summary>
        /// <returns></returns>
        List<ProductCategoriesSkroutzExtModel> GetSkroutzProducts(DBInfoModel dbInfo, long PricelistId);

        /// <summary>
        /// Return a product (active only) and the list of (active) Extras. 
        /// If product is not active or it is not exists into DB then return null
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="ProductId">Product Id</param>
        /// <returns></returns>
        ProductExtModel GetProductExt(DBInfoModel Store, long ProductId);

        /// <summary>
        /// Return a product (active only) and the list of (active) Extras. Prices ARE INCLUDED. 
        /// If product is not active or it is not exists into DB or does not belong to price-list then return null
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="Code">Product Code</param>
        /// <param name="PricelistId">Price-list Id</param>
        /// <returns></returns>
        ProductExtModel GetProductExt(DBInfoModel Store, string Code, long PricelistId);

        /// <summary>
        /// Return a product (active only) and the list of (active) Extras. Prices ARE INCLUDED.  
        /// If product is not active or it is not exists into DB or does not belong to price-list then return null
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="ProductId">Product Id</param>
        /// <param name="PricelistId">Pricelist Id</param>
        /// <returns></returns>
        ProductExtModel GetProductExt(DBInfoModel Store, long ProductId, long PricelistId);

        /// <summary>
        /// Return a product (active only) and the list of (active) Extras. 
        /// If product is not active or it is not exists into DB then return null
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="Code">Product Code</param>
        /// <returns></returns>
        ProductExtModel GetProductExt(DBInfoModel Store, string Code);

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<ProductSched_Model> model);

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateModel(DBInfoModel Store, ProductModel item);

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateModelList(DBInfoModel Store, List<ProductModel> item);

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long InsertModel(DBInfoModel Store, ProductModel item);

        /// <summary>
        /// Delete's Or Set's the Field IsDeleted if Exists to true for a list
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel DeleteRecordsSendedFromDAServer(DBInfoModel Store, List<ProductSched_Model> model);

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
        ProductModel GetModelByDAIs(DBInfoModel Store, long dAId, IDbConnection dbTran = null, IDbTransaction dbTransact = null);

        /// <summary>
        /// Return Table Id from field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="dAId"></param>
        /// <returns></returns>
        ProductModel GetModelById(DBInfoModel Store, long Id, IDbConnection dbTran = null, IDbTransaction dbTransact = null);

        /// <summary>
        /// get list of product ids and descriptions based on given product ids
        /// </summary>
        /// <param name="productIdList">List<long></param>
        /// <returns>List<ProductDescription></returns>
        List<ProductDescription> GetProductDescriptions(DBInfoModel Store, ProductIdsList productIdList);
    }
}
