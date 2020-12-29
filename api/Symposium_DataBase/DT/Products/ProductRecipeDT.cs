using AutoMapper;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT.Products;
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

namespace Symposium.WebApi.DataAccess.DT.Products
{
    public class ProductRecipeDT : IProductRecipeDT
    {
        IGenericDAO<ProductRecipeDTO> dt;
        IIngredientsDT ingrDT;
        IProductDT prodDT;
        IUnitsDT unitDT;
        IUsersToDatabasesXML users;
        string connectionString;

        public ProductRecipeDT(IGenericDAO<ProductRecipeDTO> dt, IUsersToDatabasesXML users,
            IProductDT prodDT, IIngredientsDT ingrDT, IUnitsDT unitDT)
        {
            this.dt = dt;
            this.users = users;
            this.prodDT = prodDT;
            this.ingrDT = ingrDT;
            this.unitDT = unitDT;
        }

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<ProductRecipeSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (ProductRecipeSched_Model item in model)
                {
                    if (item.IngredientId != null)
                        item.IngredientId = ingrDT.GetIdByDAIs(Store, item.IngredientId ?? 0);
                    if (item.ProductId != null)
                        item.ProductId = prodDT.GetIdByDAIs(Store, item.ProductId ?? 0);
                    if (item.UnitId != null)
                        item.UnitId = unitDT.GetIdByDAIs(Store, item.UnitId ?? 0);
                }


                results = dt.Upsert(db, Mapper.Map<List<ProductRecipeDTO>>(model));
                //{TODO Make It Better }
                SchedulerHelper schHlp = new SchedulerHelper();
                results.Results = schHlp.ReturnMasterIds(results.Results, model);
            }
            return results;
        }

        /// <summary>
        /// Returns a ProductRecipeDTO model form DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DAId"></param>
        /// <returns></returns>
        public ProductRecipeDTO GetModelByDAId(DBInfoModel Store, long DAId)
        {
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return dt.Select(db, "WHERE DAId = @DAId", new { DAId = DAId }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Delete's Or Set's the Field IsDeleted if Exists to true for a list
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel DeleteRecordsSendedFromDAServer(DBInfoModel Store, List<ProductRecipeSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (ProductRecipeSched_Model item in model)
                {
                    item.DAId = item.TableId;
                    ProductRecipeDTO tmp = GetModelByDAId(Store, item.TableId ?? 0);
                    if (tmp != null)
                        item.Id = tmp.Id;
                }

                results = dt.DeleteOrSetIsDeletedList(db, Mapper.Map<List<ProductRecipeDTO>>(model));
                //{TODO Make It Better }
                SchedulerHelper schHlp = new SchedulerHelper();
                results.Results = schHlp.ReturnMasterIds(results.Results, model);
            }
            return results;
        }
    }
}
