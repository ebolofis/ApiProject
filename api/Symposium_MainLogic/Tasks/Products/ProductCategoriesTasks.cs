using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{
    public class ProductCategoriesTasks : IProductCategoriesTasks
    {
        IProductCategoriesDT dt;

        public ProductCategoriesTasks(IProductCategoriesDT dt)
        {
            this.dt = dt;
        }

        /// <summary>
        /// Return the list with all Product Categories including disabled (status=0)
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="status">status=true : return only enabled Product Categories,  status=false : return all </param>
        /// <returns></returns>
        public List<ProductCategoriesModel> GetAll(DBInfoModel Store,bool status)
        {
            return dt.GetAll(Store,status);
        }

        public List<ProductsCategoriesComboList> GetComboList(DBInfoModel dbInfo)
        {
            return dt.GetComboList(dbInfo);
        }

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<ProductCategoriesSched_Model> model)
        {
            List<ProductCategoriesSched_Model> Upserts = model.Where(w => w.Action != 1).Select(s => s).ToList();
            List<ProductCategoriesSched_Model> Deleted = model.Where(w => w.Action == 1).Select(s => s).ToList();

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

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModel(DBInfoModel Store, ProductCategoriesModel item)
        {
            return dt.UpdateModel(Store, item);
        }

        // updates Selected Product Categories Vat
        public int UpdateProductCategoriesVat(DBInfoModel DbInfo,ChangeProdCatModel Obj)
        {
            return dt.UpdateProductCategoriesVat(DbInfo,Obj);
        }
        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel Store, List<ProductCategoriesModel> item)
        {
            return dt.UpdateModelList(Store, item);
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel Store, ProductCategoriesModel item)
        {
            return dt.InsertModel(Store, item);
        }

        /// <summary>
        /// Return's Product Categories from Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ProductCategoriesModel GetModelById(DBInfoModel Store, long Id, IDbConnection dbTran = null, IDbTransaction dbTransact = null)
        {
            return dt.GetModelById(Store, Id, dbTran, dbTransact);
        }

        /// <summary>
        /// get list of product category ids and descriptions based on given product category ids
        /// </summary>
        /// <param name="productCategoryIdList">List<long></param>
        /// <returns>List<ProductsCategoriesComboList></returns>
        public List<ProductsCategoriesComboList> GetProductCategoryDescriptions(DBInfoModel DBInfo, ProductsCategoriesIdList productCategoryIdList)
        {
            return dt.GetProductCategoryDescriptions(DBInfo, productCategoryIdList);
        }
    }
}
