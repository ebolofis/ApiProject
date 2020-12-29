using AutoMapper;
using Dapper;
using log4net;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.Products;
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
    public class PageButtonDT : IPageButtonDT
    {

        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IGenericDAO<PageButtonDTO> dt;
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IPricelistDT prlDT;
        IPagesDT pagesDT;
        IProductDT prodDT;

        public PageButtonDT(IUsersToDatabasesXML usersToDatabases, IGenericDAO<PageButtonDTO> dt,
            IPricelistDT prlDT, IPagesDT pagesDT, IProductDT prodDT)
        {
            this.usersToDatabases = usersToDatabases;
            this.dt = dt;
            this.prlDT = prlDT;
            this.pagesDT = pagesDT;
            this.prodDT = prodDT;
        }

        /// <summary>
        /// This is an Enum characterizing Product Extras Type on PageButtons and other ProductDetails Flat Calls
        /// </summary>
        public enum ProductDetailTypeEnum
        {
            Recipe = 0,
            Extras = 1,
            ExtrasAssoc = 2,
        }

        /// <summary>
        /// Return page buttons for a specific Pos, PageId and Pricelist
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="posid"></param>
        /// <param name="pageid"></param>
        /// <param name="pricelistid"></param>
        public PageButtonPreviewModel GetPageButtons(DBInfoModel Store, string storeid, int posid, int pageid, int pricelistid, bool isPos = false)
        {
            // get the results
            PageButtonPreviewModel pageButtonsModel = new PageButtonPreviewModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string pButtonsQuery = "SELECT pb.Id, p.PageSetId, pb.ProductId, pb.[Description], p2.ExtraDescription, p2.SalesDescription, \n"
                                     + "ISNULL(p2.PreparationTime, 0) AS PreparationTime, pb.Sort, pb.NavigateToPage, pb.SetDefaultPriceListId,  \n"
                                     + "pb.SetDefaultSalesType, pb.[Type], pb.PageId, pb.Color, pb.Background, p2.KdsId, p2.Code, p2.KitchenId,   \n"
                                     + "k.Code AS KitchenCode, kr.ItemRegion, kr.RegionPosition,kr.Abbr AS ItemRegionAbbr, p2.ProductCategoryId \n"
                                     + "FROM PageButton AS pb  \n"
                                     + "LEFT OUTER JOIN Pages AS p ON p.Id = pb.PageId \n"
                                     + "LEFT OUTER JOIN Product AS p2 ON p2.Id = pb.ProductId \n"
                                     + "LEFT OUTER JOIN Kitchen AS k ON k.Id = p2.KitchenId \n"
                                     + "LEFT OUTER JOIN KitchenRegion AS kr ON kr.Id = p2.KitchenRegionId \n"
                                     + "WHERE pb.PageId =@pId";

                string pricelistQuery = "SELECT pd.Id, pd.PricelistId, pd.ProductId, pd.Price, pd.VatId, pd.TaxId  \n"
                                      + "FROM PageButton AS pb  \n"
                                      + "INNER JOIN Product AS p2 ON p2.Id = pb.ProductId \n"
                                      + "INNER JOIN PricelistDetail AS pd ON pd.ProductId = p2.Id \n"
                                      + "WHERE pb.PageId =@pId AND pb.Id =@ID";

                string vatQuery = "SELECT v.Id, v.[Description], v.Percentage, v.Code \n"
                                + "FROM Vat AS v  \n"
                                + "WHERE v.Id =@vId";

                string pButtonDetailsQuery = "SELECT distinct pb.Id AS PageButtonId, ipca.IngredientId AS ProductId, i.[Description], i.Background,  \n"
                                           + "i.Color, 1 AS MaxQty, 0 AS MinQty, 0 AS Qty, ipca.Sort   \n"
                                           + "FROM Ingredient_ProdCategoryAssoc AS ipca \n"
                                           + "INNER JOIN Product AS p ON p.ProductCategoryId = ipca.ProductCategoryId \n"
                                           + "INNER JOIN Ingredients AS i ON ipca.IngredientId = i.Id \n"
                                           + "INNER JOIN PricelistDetail AS pd ON ipca.IngredientId = pd.IngredientId \n"
                                           + "INNER JOIN PageButton AS pb ON pb.ProductId = p.Id \n"
                                           + "WHERE pb.Id =@pButtonId";

                string pButtonPriceListDeatailsQuery = "SELECT pd.Id, pd.PricelistId, pd.ProductId , ipca.IngredientId, pd.Price, pd.VatId,  \n"
                                                      + "pd.TaxId, pd.PriceWithout, pd.MinRequiredExtras FROM PricelistDetail AS pd  \n"
                                                      + "LEFT OUTER JOIN Ingredient_ProdCategoryAssoc AS ipca ON ipca.IngredientId = pd.IngredientId \n"
                                                      + "WHERE ipca.IngredientId =@ingredientId";

                string vatQuery2 = "SELECT v.Id, v.[Description], v.Percentage, v.Code \n"
                                 + "FROM Vat AS v  \n"
                                 + "WHERE v.Id =@vatId";


                List<PageButtonExtentModel> pButtons = db.Query<PageButtonExtentModel>(pButtonsQuery, new { pId = pageid }).ToList();

                foreach (PageButtonExtentModel pb in pButtons)
                {
                    List<PriceListDetailsModel> priceList = db.Query<PriceListDetailsModel>(pricelistQuery, new { pId = pageid, ID = pb.Id }).ToList();
                    pb.PricelistDetails = priceList;
                    foreach (PriceListDetailsModel item in pb.PricelistDetails)
                    {
                        VatDetailModel vat = db.Query<VatDetailModel>(vatQuery, new { vId = item.VatId }).FirstOrDefault();
                        item.Vat = vat;
                    }

                    List<PageButtonDetailsModel> pButtonDetails = db.Query<PageButtonDetailsModel>(pButtonDetailsQuery, new { pButtonId = pb.Id }).ToList();
                    pb.PageButtonDetail = pButtonDetails;
                    foreach (PageButtonDetailsModel details in pb.PageButtonDetail)
                    {
                        List<PriceListDetailModel> pricelistDetails = db.Query<PriceListDetailModel>(pButtonPriceListDeatailsQuery, new { ingredientId = details.ProductId }).ToList();
                        details.PricelistDetails = pricelistDetails;
                        foreach (PriceListDetailModel pd in details.PricelistDetails)
                        {
                            VatDetailModel vat2 = db.Query<VatDetailModel>(vatQuery2, new { vatId = pd.VatId }).FirstOrDefault();
                            pd.Vat = vat2;
                        }
                    }
                }
                pageButtonsModel.PageButtons = pButtons;
            }

            return pageButtonsModel;
        }

        public List<PageButtonPricelistDetailsAssoc> SearchExternalProduct(DBInfoModel DBInfo, string description)
        {

            List<PageButtonPricelistDetailsAssoc> model = new List<PageButtonPricelistDetailsAssoc>();
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            string Sql = @"select  * FROM PageButton AS pb where pb.[Description] like '%" + description + "%'";
            string SqlData = @"SELECT * FROM PricelistDetail AS pd WHERE pd.ProductId =@productId";
            string SqlVat = @"SELECT * FROM Vat AS v WHERE v.Id =@ID";
            string SqlProdCat = @"select ProductCategoryId from Product where  id =@ID";
            try
            {

                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    model = db.Query<PageButtonPricelistDetailsAssoc>(Sql).ToList();
                    foreach (PageButtonPricelistDetailsAssoc pbExtModel in model)
                    {
                        pbExtModel.PricelistDetails = db.Query<PricelistDetailNewModel>(SqlData, new { productId = pbExtModel.ProductId }).ToList();
                        foreach (PricelistDetailNewModel priceModel in pbExtModel.PricelistDetails)
                        {
                            priceModel.Vat = db.Query<VatModel>(SqlVat, new { ID = priceModel.VatId }).FirstOrDefault();
                        }
                    }

                    foreach (PageButtonPricelistDetailsAssoc mod in model)
                    {
                        mod.ProductCategoryId = db.Query<int>(SqlProdCat, new { ID = mod.ProductId }).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("The fetching of the Page Buttons was not completed - " + Sql.ToString() + " \r\n" + ex.ToString());
                throw new Exception(ex.ToString());
            }

            return model;
        }
        /// <summary>
        /// Return's list of Page Buttons after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<PageButtonSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (PageButtonSched_Model item in model)
                {
                    item.PriceListId = prlDT.GetIdByDAIs(Store, item.PriceListId ?? 0);
                    item.PageId = pagesDT.GetIdByDAIs(Store, item.PageId ?? 0);
                    item.ProductId = prodDT.GetIdByDAIs(Store, item.ProductId ?? 0);
                    item.NavigateToPage = pagesDT.GetIdByDAIs(Store, item.NavigateToPage ?? 0);
                }
                results = this.dt.Upsert(db, Mapper.Map<List<PageButtonDTO>>(model));
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
        public UpsertListResultModel DeleteRecordsSendedFromDAServer(DBInfoModel Store, List<PageButtonSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();

            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            List<PageButtonDTO> tmpDTO = new List<PageButtonDTO>();
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (PageButtonSched_Model item in model)
                    tmpDTO.Add(dt.SelectFirst(db, "WHERE DAId = @DAId", new { DAId = item.TableId }));

                results = dt.DeleteOrSetIsDeletedList(db, tmpDTO);

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
        public int UpdateModel(DBInfoModel Store, PageButtonModel item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.Update(db, Mapper.Map<PageButtonDTO>(item));
            }
            return results;
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel Store, List<PageButtonModel> item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.UpdateList(db, Mapper.Map<List<PageButtonDTO>>(item));
            }
            return results;
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel Store, PageButtonModel item)
        {
            long results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.Insert(db, Mapper.Map<PageButtonDTO>(item));
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
                PageButtonDTO tmp = dt.SelectFirst(db, "WHERE DAId = @DAId", new { DAId = dAId });
                if (tmp == null)
                    return null;
                else
                    return tmp.Id;
            }
        }
    }
}
