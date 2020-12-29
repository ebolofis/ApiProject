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
    public class PricelistDetailDT : IPricelistDetailDT
    {
        string connectionString;
        IPricelistDetailDAO pricelistDetailDAO;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<PricelistDetailDTO> dt;
        IPricelistDT prlDT;
        IProductDT prodDT;
        IIngredientsDT ingredDT;
        ITaxDT taxDT;
        IVatDT vatDT;

        public PricelistDetailDT(IUsersToDatabasesXML usersToDatabases, IPricelistDetailDAO pricelistDetailDAO,
                IGenericDAO<PricelistDetailDTO> dt, IPricelistDT prlDT, IProductDT prodDT, IIngredientsDT ingredDT,
                ITaxDT taxDT, IVatDT vatDT)
        {
            this.usersToDatabases = usersToDatabases;
            this.pricelistDetailDAO = pricelistDetailDAO;
            this.dt = dt;
            this.prlDT = prlDT;
            this.prodDT = prodDT;
            this.ingredDT = ingredDT;
            this.taxDT = taxDT;
            this.vatDT = vatDT;
        }


        /// <summary>
        /// Selects pricelist detail for specific product and specific pricelist
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="productId"> Id of product </param>
        /// <param name="pricelistId"> Id of pricelist </param>
        /// <returns> Pricelist details model </returns>
        public PricelistDetailModel SelectPricelistDetailForProductAndPricelist(DBInfoModel Store, long productId, long pricelistId, 
            IDbConnection dbTran = null, IDbTransaction dbTransact = null)
        {
            PricelistDetailDTO pricelistDetail;

            if (dbTran != null)
                pricelistDetail = pricelistDetailDAO.SelectPricelistDetailForProductAndPricelist(dbTran, productId, pricelistId, dbTransact);
            else
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    pricelistDetail = pricelistDetailDAO.SelectPricelistDetailForProductAndPricelist(db, productId, pricelistId);
                }
            }
            return AutoMapper.Mapper.Map<PricelistDetailModel>(pricelistDetail);
        }

        public PricelistDetailModel UpdateExtraPrice(DBInfoModel dbInfo, long IngredientId, long PriceListDetailId, double newPrice)
        {
            PricelistDetailModel Model = new PricelistDetailModel();
            string SqlData = "update PricelistDetail set Price =@price where Id =@priceListDetailId";
            string sqlSelect = "select * from PricelistDetail where Id =@priceListDetailId";
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Query<PricelistDetailModel>(SqlData, new { price = newPrice, priceListDetailId = PriceListDetailId }).FirstOrDefault();
                Model = db.Query<PricelistDetailModel>(sqlSelect, new { priceListDetailId = PriceListDetailId }).FirstOrDefault();
            }
            return Model;

        }

        /// <summary>
        /// Updates pricelist detail
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="pricelistDetail"> Pricelist detail model to update </param>
        /// <returns> Pricelist detail model </returns>
        public PricelistDetailModel UpdatePricelistDetail(DBInfoModel Store, PricelistDetailModel pricelistDetail)
        {
            PricelistDetailDTO pricelistDetailToReturn;
            PricelistDetailDTO pricelistDetailToAdd = AutoMapper.Mapper.Map<PricelistDetailDTO>(pricelistDetail);
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {

                pricelistDetailToReturn = pricelistDetailDAO.UpdatePricelistDetail(db, pricelistDetailToAdd);
            }
            return AutoMapper.Mapper.Map<PricelistDetailModel>(pricelistDetailToReturn);
        }

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<PriceListDetailSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (PriceListDetailSched_Model item in model)
                {
                    item.PricelistId = prlDT.GetIdByDAIs(Store, item.PricelistId ?? 0);
                    item.ProductId = prodDT.GetIdByDAIs(Store, item.ProductId ?? 0);
                    item.IngredientId = ingredDT.GetIdByDAIs(Store, item.IngredientId ?? 0);
                    item.TaxId = taxDT.GetIdByDAIs(Store, item.TaxId ?? 0);
                    item.VatId = vatDT.GetIdByDAIs(Store, item.VatId ?? 0);
                }
                results = dt.Upsert(db, Mapper.Map<List<PricelistDetailDTO>>(model));
                //{TODO Make It Better }

                SchedulerHelper schHlp = new SchedulerHelper();
                results.Results = schHlp.ReturnMasterIds(results.Results, model);

                //foreach (UpsertResultsModel item in results.Results)
                //{

                //    var fld = model.Find(f => f.TableId == item.DAID);
                //    if (fld != null)
                //        item.MasterID = fld.MasterId;
                //}
            }
            return results;
        }

        public PricelistDetailDTO GetModelByDAId(DBInfoModel Store, long DAId)
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
        public UpsertListResultModel DeleteRecordsSendedFromDAServer(DBInfoModel Store, List<PriceListDetailSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (PriceListDetailSched_Model item in model)
                {
                    item.DAId = item.TableId;
                    PricelistDetailDTO tmp = GetModelByDAId(Store, item.TableId ?? 0);
                    if (tmp != null)
                        item.Id = tmp.Id;
                }

                results = dt.DeleteOrSetIsDeletedList(db, Mapper.Map<List<PricelistDetailDTO>>(model));
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
        public int UpdateModel(DBInfoModel Store, PricelistDetailModel item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.Update(db, Mapper.Map<PricelistDetailDTO>(item));
            }
            return results;
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel Store, List<PricelistDetailModel> item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.UpdateList(db, Mapper.Map<List<PricelistDetailDTO>>(item));
            }
            return results;
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel Store, PricelistDetailModel item)
        {
            long results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.Insert(db, Mapper.Map<PricelistDetailDTO>(item));
            }
            return results;
        }

    }
}
