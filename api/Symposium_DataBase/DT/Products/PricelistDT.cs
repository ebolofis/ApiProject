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
    public class PricelistDT : IPricelistDT
    {
        IGenericDAO<PricelistDTO> dt;
        IUsersToDatabasesXML usersToDatabases;
        ISalesTypeDT salesDT;
        IPricelistMasterDT priceLMasterDT;
        string connectionString;

        public PricelistDT(IGenericDAO<PricelistDTO> dt, IUsersToDatabasesXML usersToDatabases,
            ISalesTypeDT salesDT, IPricelistMasterDT priceLMasterDT)
        {
            this.dt = dt;
            this.usersToDatabases = usersToDatabases;
            this.salesDT = salesDT;
            this.priceLMasterDT = priceLMasterDT;
        }


        /// <summary>
        /// Return the extended pricelists (active only). Every pricelist contains the list of Details.
        /// </summary>
        /// <returns></returns>
        public List<PricelistExtModel> GetExtentedList(DBInfoModel dbInfo)
        {
            string sql = @"
               select 
	                Pricelist.Id,   
	                Pricelist.Code ,  
	                Pricelist.Description ,  
	                Pricelist.ActivationDate ,  
	                Pricelist.DeactivationDate ,  
	                Pricelist.SalesTypeId ,  
	                Pricelist.Type ,  
	                 PricelistDetail.Id,
	                 PricelistDetail.PricelistId,
	                 PricelistDetail.ProductId,
	                 PricelistDetail.IngredientId,
	                 PricelistDetail.Price,
	                 PricelistDetail.VatId,
	                 PricelistDetail.TaxId
                  from Pricelist
	                inner join PricelistDetail on Pricelist.id=PricelistDetail.PricelistId
                  where Pricelist.Status=1 and (IsDeleted=0 or IsDeleted is null) and (Pricelist.LookUpPriceListId is null or Pricelist.LookUpPriceListId =0)

                union all

                select 
	                Pricelist.Id,   
	                Pricelist.Code ,  
	                Pricelist.Description ,  
	                Pricelist.ActivationDate ,  
	                Pricelist.DeactivationDate ,  
	                Pricelist.SalesTypeId ,  
	                Pricelist.Type ,  
	                 PricelistDetail.Id,
	                 PricelistDetail.PricelistId,
	                 PricelistDetail.ProductId,
	                 PricelistDetail.IngredientId,
	                 PricelistDetail.Price* Pricelist.Percentage/100.0 Price,
	                 PricelistDetail.VatId,
	                 PricelistDetail.TaxId
                  from Pricelist
	                inner join PricelistDetail on Pricelist.LookUpPriceListId=PricelistDetail.PricelistId
                where Pricelist.Status=1 and (IsDeleted=0 or IsDeleted is null) and (Pricelist.LookUpPriceListId is not null )
                order by Pricelist.Id";
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            var lookup = new Dictionary<long, PricelistExtModel>();//Create a data structure to store pricelists uniquely

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var result = db.Query<PricelistExtModel, PricelistDetailBasicModel, PricelistExtModel>(// <TFirst, TSecond, TReturn>
                          sql,
                          (pricelist, pricelistDetails) =>
                          {
                              PricelistExtModel prod;
                              if (!lookup.TryGetValue(pricelist.Id, out prod))
                              {
                                  prod = pricelist;//the product does not exit into lookup dictionary
                                  lookup.Add(pricelist.Id, pricelist);
                              }

                              prod.Details.Add(pricelistDetails);
                              return prod;
                          });
                return lookup.Select(x => x.Value).ToList<PricelistExtModel>();
                //or return result.Distinct().ToList<Product>();
            }
        }

        /// <summary>
        /// Return prices info for specific product and price-list (active only)
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="ProductId">Product Id</param>
        /// <param name="PriceListId">PriceList Id</param>
        /// <returns></returns>
        public PricelistExtModel GetProductFromPriceList(DBInfoModel dbInfo, long ProductId, long PriceListId)
        {
            string sql = @"
               select 
	                Pricelist.Id,   
	                Pricelist.Code ,  
	                Pricelist.Description ,  
	                Pricelist.ActivationDate ,  
	                Pricelist.DeactivationDate ,  
	                Pricelist.SalesTypeId ,  
	                Pricelist.Type ,  
	                 PricelistDetail.Id,
	                 PricelistDetail.PricelistId,
	                 PricelistDetail.ProductId,
	                 PricelistDetail.IngredientId,
	                 PricelistDetail.Price,
	                 PricelistDetail.VatId,
	                 PricelistDetail.TaxId
                  from Pricelist
	                inner join PricelistDetail on Pricelist.id=PricelistDetail.PricelistId
                  where Pricelist.Id=@PriceListId and ProductId=@ProductId and Pricelist.Status=1 and (IsDeleted=0 or IsDeleted is null) and (Pricelist.LookUpPriceListId is null or Pricelist.LookUpPriceListId =0)

                union all

                select 
	                Pricelist.Id,   
	                Pricelist.Code ,  
	                Pricelist.Description ,  
	                Pricelist.ActivationDate ,  
	                Pricelist.DeactivationDate ,  
	                Pricelist.SalesTypeId ,  
	                Pricelist.Type ,  
	                 PricelistDetail.Id,
	                 PricelistDetail.PricelistId,
	                 PricelistDetail.ProductId,
	                 PricelistDetail.IngredientId,
	                 PricelistDetail.Price* Pricelist.Percentage/100.0 Price,
	                 PricelistDetail.VatId,
	                 PricelistDetail.TaxId
                  from Pricelist
	                inner join PricelistDetail on Pricelist.LookUpPriceListId=PricelistDetail.PricelistId
                where Pricelist.Id=@PriceListId and ProductId=@ProductId and Pricelist.Status=1 and (IsDeleted=0 or IsDeleted is null) and (Pricelist.LookUpPriceListId is not null )
                order by Pricelist.Id";
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            var lookup = new Dictionary<long, PricelistExtModel>();//Create a data structure to store price-lists uniquely

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var result = db.Query<PricelistExtModel, PricelistDetailBasicModel, PricelistExtModel>(// <TFirst, TSecond, TReturn>
                          sql,
                          (pricelist, pricelistDetails) =>
                          {
                              PricelistExtModel prod;
                              if (!lookup.TryGetValue(pricelist.Id, out prod))
                              {
                                  prod = pricelist;//the product does not exit into lookup dictionary
                                  lookup.Add(pricelist.Id, pricelist);
                              }

                              prod.Details.Add(pricelistDetails);
                              return prod;
                          },new { PriceListId = PriceListId , ProductId = ProductId });
                return lookup.Select(x => x.Value).FirstOrDefault();//.ToList<PricelistExtModel>();

            }
        }

        /// <summary>
        /// Return prices info for specific extra/Ingredient and price-list (active only)
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="IngredientId">Ingredient Id</param>
        /// <param name="PriceListId">PriceList Id</param>
        /// <returns></returns>
        public PricelistExtModel GetExtraFromPriceList(DBInfoModel dbInfo, long IngredientId, long PriceListId)
        {
            string sql = @"
               select 
	                Pricelist.Id,   
	                Pricelist.Code ,  
	                Pricelist.Description ,  
	                Pricelist.ActivationDate ,  
	                Pricelist.DeactivationDate ,  
	                Pricelist.SalesTypeId ,  
	                Pricelist.Type ,  
	                 PricelistDetail.Id,
	                 PricelistDetail.PricelistId,
	                 PricelistDetail.ProductId,
	                 PricelistDetail.IngredientId,
	                 PricelistDetail.Price,
	                 PricelistDetail.VatId,
	                 PricelistDetail.TaxId
                  from Pricelist
	                inner join PricelistDetail on Pricelist.id=PricelistDetail.PricelistId
                  where Pricelist.Id=@PriceListId and IngredientId=@IngredientId and Pricelist.Status=1 and (IsDeleted=0 or IsDeleted is null) and (Pricelist.LookUpPriceListId is null or Pricelist.LookUpPriceListId =0)

                union all

                select 
	                Pricelist.Id,   
	                Pricelist.Code ,  
	                Pricelist.Description ,  
	                Pricelist.ActivationDate ,  
	                Pricelist.DeactivationDate ,  
	                Pricelist.SalesTypeId ,  
	                Pricelist.Type ,  
	                 PricelistDetail.Id,
	                 PricelistDetail.PricelistId,
	                 PricelistDetail.ProductId,
	                 PricelistDetail.IngredientId,
	                 PricelistDetail.Price* Pricelist.Percentage/100.0 Price,
	                 PricelistDetail.VatId,
	                 PricelistDetail.TaxId
                  from Pricelist
	                inner join PricelistDetail on Pricelist.LookUpPriceListId=PricelistDetail.PricelistId
                where Pricelist.Id=@PriceListId and IngredientId=@IngredientId and Pricelist.Status=1 and (IsDeleted=0 or IsDeleted is null) and (Pricelist.LookUpPriceListId is not null )
                order by Pricelist.Id";
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            var lookup = new Dictionary<long, PricelistExtModel>();//Create a data structure to store price-lists uniquely

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var result = db.Query<PricelistExtModel, PricelistDetailBasicModel, PricelistExtModel>(// <TFirst, TSecond, TReturn>
                          sql,
                          (pricelist, pricelistDetails) =>
                          {
                              PricelistExtModel prod;
                              if (!lookup.TryGetValue(pricelist.Id, out prod))
                              {
                                  prod = pricelist;//the product does not exit into lookup dictionary
                                  lookup.Add(pricelist.Id, pricelist);
                              }

                              prod.Details.Add(pricelistDetails);
                              return prod;
                          }, new { PriceListId = PriceListId, IngredientId = IngredientId });
                return lookup.Select(x => x.Value).FirstOrDefault();//.ToList<PricelistExtModel>();

            }
        }

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel dbInfo, List<PriceListSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (PriceListSched_Model item in model)
                {
                    item.SalesTypeId = salesDT.GetIdByDAIs(dbInfo, item.SalesTypeId ?? 0);
                    item.PricelistMasterId = priceLMasterDT.GetIdByDAIs(dbInfo, item.PricelistMasterId ?? 0);
                }

                results = this.dt.Upsert(db, Mapper.Map<List<PricelistDTO>>(model));
                //{TODO Make It Better }
                SchedulerHelper schHlp = new SchedulerHelper();
                results.Results = schHlp.ReturnMasterIds(results.Results, model);
            }
            return results;
        }

        /// <summary>
        /// Delete's Or Set's the Field IsDeleted if Exists to true for a list
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel DeleteRecordsSendedFromDAServer(DBInfoModel dbInfo, List<PriceListSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (PriceListSched_Model item in model)
                {
                    item.DAId = item.TableId;
                    item.Id = GetIdByDAIs(dbInfo, item.TableId ?? 0) ?? 0;
                    item.IsDeleted = true;
                }
                List<PricelistDTO> plLst = Mapper.Map<List<PricelistDTO>>(model);

                results = dt.Upsert(db, plLst);
                //{TODO Make It Better }
                SchedulerHelper schHlp = new SchedulerHelper();
                results.Results = schHlp.ReturnMasterIds(results.Results, model);
            }
            return results;
        }

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModel(DBInfoModel dbInfo, PricelistModel item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.Update(db, Mapper.Map<PricelistDTO>(item));
            }
            return results;
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel dbInfo, List<PricelistModel> item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.UpdateList(db, Mapper.Map<List<PricelistDTO>>(item));
            }
            return results;
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel dbInfo, PricelistModel item)
        {
            long results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.Insert(db, Mapper.Map<PricelistDTO>(item));
            }
            return results;
        }

        /// <summary>
        /// Return Table Id from field DAId
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="dAId"></param>
        /// <returns></returns>
        public long? GetIdByDAIs(DBInfoModel dbInfo, long dAId, IDbConnection dbTran = null, IDbTransaction dbTransact = null)
        {
            PricelistDTO tmp = null;
            if (dbTran != null)
                tmp = dt.SelectFirst(dbTran, "WHERE DAId = @DAId", new { DAId = dAId }, dbTransact);
            else
            {
                connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    tmp = dt.SelectFirst(db, "WHERE DAId = @DAId", new { DAId = dAId });
                }
            }
            if (tmp == null)
                return null;
            else
                return tmp.Id;
        }
    }
}
