using AutoMapper;
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
    public class IngredientsDT : IIngredientsDT
    {
        IGenericDAO<IngredientsDTO> dt;
        IUnitsDT unitDT;
        IIngredientCategoriesDT ingredCategDT;
        IUsersToDatabasesXML usersToDatabases;
        string connectionString;

        public IngredientsDT(IGenericDAO<IngredientsDTO> dt, IUsersToDatabasesXML usersToDatabases,
            IUnitsDT unitDT, IIngredientCategoriesDT ingredCategDT)
        {
            this.dt = dt;
            this.usersToDatabases = usersToDatabases;
            this.unitDT = unitDT;
            this.ingredCategDT = ingredCategDT;
        }

        /// <summary>
        /// Return an Ingredient based on id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public IngredientsModel GetModelById(DBInfoModel Store, long id)
        {
            IngredientsModel ingredient;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                IngredientsDTO item = dt.SelectFirst(db, "WHERE Id = @id", new { id = id });
                ingredient = Mapper.Map<IngredientsModel>(item);
            }
            return ingredient;
        }

        /// <summary>
        /// Return an Ingredient based on code
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="code">code</param>
        /// <returns></returns>
        public IngredientsModel GetModelByCode(DBInfoModel Store, string code)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return AutoMapper.Mapper.Map<IngredientsModel>(dt.SelectFirst(db, "WHERE code = @code", new { code = code }));

            }
        }

        /// <summary>
        /// check if an ingredient belongs to the product's recipe
        /// </summary>
        ///  <param name="Store">db</param>
        /// <param name="ProductId">Product Id</param>
        /// <param name="IngredientId">Ingredient Id</param>
        /// <returns></returns>
        public bool IsProductRecipe(DBInfoModel Store, long ProductId, long IngredientId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                IngredientsDTO dto = dt.SelectFirst("select * from ProductRecipe where productId=@ProductId and IngredientId=@IngredientId", new { ProductId = ProductId, IngredientId = IngredientId }, db);
                if (dto == null)
                {
                    return false;
                    //IngredientsDTO dto1 = dt.SelectFirst("select * from ProductExtras where productId=@ProductId and IngredientId=@IngredientId", new { ProductId = ProductId, IngredientId = IngredientId }, db);
                    //if (dto1 == null)
                    //    return false;
                    //else
                    //    return true;
                }
                else
                    return true;
            }
        }

        /// <summary>
        /// Return Table Id from field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="dAId"></param>
        /// <returns></returns>
        public IngredientsModel GetModelByDAIs(DBInfoModel Store, long dAId, IDbConnection dbTran = null, IDbTransaction dbTransact = null)
        {
            if (dbTran != null)
                return AutoMapper.Mapper.Map<IngredientsModel>(dt.SelectFirst(dbTran, "WHERE DAId = @DAId", new { DAId = dAId }, dbTransact));
            else
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    return AutoMapper.Mapper.Map<IngredientsModel>(dt.SelectFirst(db, "WHERE DAId = @DAId", new { DAId = dAId }));
                }
            }
        }

        /// <summary>
        /// Return's list of actions after upsert, using as search field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<IngredientsSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (IngredientsSched_Model item in model)
                {
                    item.UnitId = unitDT.GetIdByDAIs(Store, item.UnitId ?? 0);
                    item.IngredientCategoryId = ingredCategDT.GetIdByDAIs(Store, item.IngredientCategoryId ?? 0);
                }

                results = this.dt.Upsert(db, Mapper.Map<List<IngredientsDTO>>(model));
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
        public UpsertListResultModel DeleteRecordsSendedFromDAServer(DBInfoModel Store, List<IngredientsSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (IngredientsSched_Model item in model)
                {
                    item.DAId = item.TableId;
                    item.Id = GetIdByDAIs(Store, item.TableId ?? 0) ?? 0;
                }

                results = dt.DeleteOrSetIsDeletedList(db, Mapper.Map<List<IngredientsDTO>>(model));
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
        public int UpdateModel(DBInfoModel Store, IngredientsModel item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.Update(db, Mapper.Map<IngredientsDTO>(item));
            }
            return results;
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel Store, List<IngredientsModel> item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.UpdateList(db, Mapper.Map<List<IngredientsDTO>>(item));
            }
            return results;
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel Store, IngredientsModel item)
        {
            long results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.Insert(db, Mapper.Map<IngredientsDTO>(item));
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
                IngredientsDTO tmp = dt.SelectFirst(db, "WHERE DAId = @DAId", new { DAId = dAId });
                if (tmp == null)
                    return null;
                else
                    return tmp.Id;
            }
        }


    }
}
