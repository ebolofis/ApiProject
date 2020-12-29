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
    public class ProductExtrasDT : IProductExtrasDT
    {
        IGenericDAO<ProductExtrasDTO> dt;
        IUsersToDatabasesXML usersToDatabases;
        IProductDT productDT;
        IUnitsDT unitDT;
        IIngredientsDT ingredDT;
        string connectionString;

        public ProductExtrasDT(IGenericDAO<ProductExtrasDTO> dt, IUsersToDatabasesXML usersToDatabases,
            IProductDT productDT, IUnitsDT unitDT,  IIngredientsDT ingredDT)
        {
            this.dt = dt;
            this.usersToDatabases = usersToDatabases;
            this.productDT = productDT;
            this.unitDT = unitDT;
            this.ingredDT = ingredDT;
        }

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<ProductExtrasSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (ProductExtrasSched_Model item in model)
                {
                    item.ProductId = productDT.GetIdByDAIs(Store, item.ProductId ?? 0);
                    item.UnitId = unitDT.GetIdByDAIs(Store, item.UnitId ?? 0);
                    item.IngredientId = ingredDT.GetIdByDAIs(Store, item.IngredientId ?? 0);
                }
                results = this.dt.Upsert(db, Mapper.Map<List<ProductExtrasDTO>>(model));
                //{TODO Make It Better }
                SchedulerHelper schHlp = new SchedulerHelper();
                results.Results = schHlp.ReturnMasterIds(results.Results, model);
            }
            return results;
        }

        public ProductExtrasDTO GetModelByDAId(DBInfoModel Store, long DAId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
                return dt.Select(db, "WHERE DAId = @DAId", new { DAId = DAId }).FirstOrDefault();
        }

        /// <summary>
        /// Delete's Or Set's the Field IsDeleted if Exists to true for a list
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel DeleteRecordsSendedFromDAServer(DBInfoModel Store, List<ProductExtrasSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (ProductExtrasSched_Model item in model)
                {
                    item.DAId = item.TableId;
                    ProductExtrasDTO tmp = GetModelByDAId(Store, item.TableId ?? 0);
                    if (tmp != null)
                        item.Id = tmp.Id;
                }


                results = dt.DeleteOrSetIsDeletedList(db, Mapper.Map<List<ProductExtrasDTO>>(model));
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
        public int UpdateModel(DBInfoModel Store, ProductExtrasModel item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.Update(db, Mapper.Map<ProductExtrasDTO>(item));
            }
            return results;
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel Store, List<ProductExtrasModel> item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.UpdateList(db, Mapper.Map<List<ProductExtrasDTO>>(item));
            }
            return results;
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel Store, ProductExtrasModel item)
        {
            long results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.Insert(db, Mapper.Map<ProductExtrasDTO>(item));
            }
            return results;
        }

    }
}
