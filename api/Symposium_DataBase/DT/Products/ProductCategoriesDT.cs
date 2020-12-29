using AutoMapper;
using Dapper;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT
{
    public class ProductCategoriesDT : IProductCategoriesDT
    {
        IGenericDAO<ProductCategoriesDTO> dt;
        IUsersToDatabasesXML usersToDatabases;
        ICategoriesDT categDT;
        string connectionString;

        public ProductCategoriesDT(IGenericDAO<ProductCategoriesDTO> dt, IUsersToDatabasesXML usersToDatabases,
            ICategoriesDT categDT)
        {
            this.dt = dt;
            this.usersToDatabases = usersToDatabases;
            this.categDT = categDT;
        }


        /// <summary>
        /// Return the list with all Product Categories including disabled (status=0)
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="status">status=true : return only enabled Product Categories,  status=false : return all </param>
        /// <returns></returns>
        public List<ProductCategoriesModel> GetAll(DBInfoModel Store, bool status)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                if(!status)
                       return Mapper.Map<List<ProductCategoriesModel>>(dt.Select(db));
                else
                    return Mapper.Map<List<ProductCategoriesModel>>(dt.Select(db, "where status = 1",new { }));
            }
        }

        public List<ProductsCategoriesComboList> GetComboList(DBInfoModel Store)
        {
            List<ProductsCategoriesComboList> List = new List<ProductsCategoriesComboList>();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string SqlData = @"SELECT pc.Id, pc.[Description] AS Descr FROM ProductCategories AS pc ORDER BY pc.[Description]";
                List = db.Query<ProductsCategoriesComboList>(SqlData).ToList();
            }
            return List;
        }


        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<ProductCategoriesSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (ProductCategoriesSched_Model item in model)
                {
                    item.CategoryId = categDT.GetIdByDAIs(Store, item.CategoryId ?? 0);
                }
                results = this.dt.Upsert(db, Mapper.Map<List<ProductCategoriesDTO>>(model));
                //{TODO Make It Better }
                SchedulerHelper schHlp = new SchedulerHelper();
                results.Results = schHlp.ReturnMasterIds(results.Results, model);
            }
            return results;
        }

        /// <summary>
        /// Delete's Or Set's the Field IsDeleted if Exists to true for a list
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel DeleteRecordsSendedFromDAServer(DBInfoModel Store, List<ProductCategoriesSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (ProductCategoriesSched_Model item in model)
                {
                    item.DAId = item.TableId;
                    item.Id = GetIdByDAIs(Store, item.TableId ?? 0) ?? 0;
                }

                results = dt.DeleteOrSetIsDeletedList(db, Mapper.Map<List<ProductCategoriesDTO>>(model));
                //{TODO Make It Better }
                SchedulerHelper schHlp = new SchedulerHelper();
                results.Results = schHlp.ReturnMasterIds(results.Results, model);
            }
            return results;
        }

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModel(DBInfoModel Store, ProductCategoriesModel item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.Update(db, Mapper.Map<ProductCategoriesDTO>(item));
            }
            return results;
        }

        public int UpdateProductCategoriesVat(DBInfoModel DbInfo, ChangeProdCatModel Obj)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(DbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {

                string SqlData = @" UPDATE PricelistDetail
                                                    SET VatId = @VatId
                                                    WHERE PricelistDetail.Id in
                                                                   (
                                                                    SELECT  o.Id
                                                                    FROM PricelistDetail AS o
                                                                    INNER JOIN Product AS p ON p.Id = o.ProductId
                                                                    where p.ProductCategoryId = @ProductCategoryId
													                )";

                foreach (int  element in Obj.prodcatlist)
                {
                     results = db.Execute(SqlData, new { VatId = Obj.Vat, ProductCategoryId = element});
                } 
                

                }

            return results;
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel Store, List<ProductCategoriesModel> item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.UpdateList(db, Mapper.Map<List<ProductCategoriesDTO>>(item));
            }
            return results;
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel Store, ProductCategoriesModel item)
        {
            long results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.Insert(db, Mapper.Map<ProductCategoriesDTO>(item));
            }
            return results;
        }

        /// <summary>
        /// Return Table Id from field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="dAId"></param>
        /// <returns></returns>
        public long? GetIdByDAIs(DBInfoModel Store, long dAId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                ProductCategoriesDTO tmp = dt.SelectFirst(db, "WHERE DAId = @DAId", new { DAId = dAId });
                if (tmp == null)
                    return null;
                else
                    return tmp.Id;
            }
        }

        /// <summary>
        /// Return's Product Categories from Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ProductCategoriesModel GetModelById(DBInfoModel Store, long Id, IDbConnection dbTran = null, IDbTransaction dbTransact = null)
        {
            if (dbTran != null)
                return AutoMapper.Mapper.Map<ProductCategoriesModel>(dt.SelectFirst(dbTran, "WHERE Id = @Id", new { Id = Id }, dbTransact));
            else
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    return AutoMapper.Mapper.Map<ProductCategoriesModel>(dt.SelectFirst(db, "WHERE Id = @Id", new { Id = Id }));
                }
            }
        }

        /// <summary>
        /// get list of product category ids and descriptions based on given product category ids
        /// </summary>
        /// <param name="productCategoryIdList">List<long></param>
        /// <returns>List<ProductsCategoriesComboList></returns>
        public List<ProductsCategoriesComboList> GetProductCategoryDescriptions(DBInfoModel DBInfo, ProductsCategoriesIdList productCategoryIdList)
        {
            List<ProductsCategoriesComboList> result = new List<ProductsCategoriesComboList>();

            if (productCategoryIdList == null)
            {
                return result;
            }

            if (productCategoryIdList.productCategoryIdList.Count == 0)
            {
                return result;
            }

            string ids = String.Join(",", productCategoryIdList.productCategoryIdList); 

            string SqlData = @"SELECT pc.Id, pc.[Description] AS Descr FROM ProductCategories AS pc WHERE pc.Id IN (" + ids + ")";

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                result = db.Query<ProductsCategoriesComboList>(SqlData).ToList();
            }

            return result;
        }
    }
}
