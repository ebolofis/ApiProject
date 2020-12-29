using Dapper;
using log4net;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT.DeliveryAgent
{
    public class DA_ScheduledTaskesDT : IDA_ScheduledTaskesDT
    {
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IGenericDAO<DA_ScheduledTaskesDTO> dt;
        IUsersToDatabasesXML usersToDatabases;
        string connectionString;

        public DA_ScheduledTaskesDT(IGenericDAO<DA_ScheduledTaskesDTO> dt, IUsersToDatabasesXML usersToDatabases)
        {
            this.dt = dt;
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Return's List of records to update Store
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Deleted"> List Of Deleted Records  </param>
        /// <param name="ClientId"> If Gratter than 0 then specific store else all stores </param>
        /// <returns></returns>
        public List<RecordsForUpdateStoreModel> GetListDataToUpdateFromServer(DBInfoModel Store, out List<RecordsForUpdateStoreModel> Deleted, long? ClientId)
        {
            StringBuilder SQL = new StringBuilder();
            List<long> stores = new List<long>();
            List<RecordsForUpdateStoreModel> ret = new List<RecordsForUpdateStoreModel>();
            Deleted = new List<RecordsForUpdateStoreModel>();
            try
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    SQL.Clear();
                    SQL.Append("SELECT DISTINCT dst.StoreId \n"
                              + "FROM DA_ScheduledTaskes AS dst \n"
                              + "WHERE CASE WHEN @ClientId < 1 THEN 1 \n"
                              + "          WHEN @ClientId > 0 AND @ClientId = dst.StoreId THEN 1 \n"
                              + "        ELSE 0 END = 1");
                    stores = db.Query<long>(SQL.ToString(), new { ClientId = ClientId ?? 0 }).ToList();
                    foreach (long item in stores)
                    {
                        SQL.Clear();
                        SQL.Append(ReturnTablesRecords(false, item));
                        using (var multQuery = db.QueryMultiple(SQL.ToString()))
                        {
                            RecordsForUpdateStoreModel tmp = new RecordsForUpdateStoreModel();
                            tmp.StoreId = item;
                            tmp.Accounts = multQuery.Read<AccountSched_Model>().ToList();
                            tmp.Categories = multQuery.Read<CategoriesSched_Model>().ToList();
                            tmp.IngedProdAssoc = multQuery.Read<IngedProdCategAssocSched_Model>().ToList();
                            tmp.IngredCategories = multQuery.Read<IngredCategoriesSched_Model>().ToList();
                            tmp.PageSet = multQuery.Read<PageSetSched_Model>().ToList();
                            tmp.PriceListMaster = multQuery.Read<PriceListMasterSched_Model>().ToList();
                            tmp.SalesType = multQuery.Read<SalesTypeSched_Model>().ToList();
                            tmp.Taxes = multQuery.Read<TaxSched_Model>().ToList();
                            tmp.Units = multQuery.Read<UnitsSched_Model>().ToList();
                            tmp.Vats = multQuery.Read<VatSched_Model>().ToList();
                            tmp.Ingredients = multQuery.Read<IngredientsSched_Model>().ToList();
                            tmp.Pages = multQuery.Read<PagesSched_Model>().ToList();
                            tmp.PriceList = multQuery.Read<PriceListSched_Model>().ToList();
                            tmp.PriceListEffectHours = multQuery.Read<PriceList_EffHoursSched_Model>().ToList();
                            tmp.ProductCategories = multQuery.Read<ProductCategoriesSched_Model>().ToList();
                            tmp.Products = multQuery.Read<ProductSched_Model>().ToList();
                            tmp.ProductBarcodes = multQuery.Read<ProductBarcodesSched_Model>().ToList();
                            tmp.ProductRecipies = multQuery.Read<ProductRecipeSched_Model>().ToList();
                            tmp.ProductExtras = multQuery.Read<ProductExtrasSched_Model>().ToList();
                            tmp.PriceListDetails = multQuery.Read<PriceListDetailSched_Model>().ToList();
                            tmp.PageButtons = multQuery.Read<PageButtonSched_Model>().ToList();
                            tmp.PageButtonDetails = multQuery.Read<PageButtonDetSched_Model>().ToList();
                            tmp.PromotionsHeaders = multQuery.Read<PromotionsHeaderSched_Model>().ToList();
                            tmp.PromotionCombos = multQuery.Read<PromotionsCombosSched_Model>().ToList();
                            tmp.PromotionDiscount = multQuery.Read<PromotionsDiscountsSched_Model>().ToList();
                            ret.Add(tmp);
                        }
                        SQL.Clear();
                        SQL.Append(ReturnTablesRecords(true, item));
                        using (var multQuery = db.QueryMultiple(SQL.ToString()))
                        {
                            RecordsForUpdateStoreModel tmp = new RecordsForUpdateStoreModel();
                            tmp.StoreId = item;
                            tmp.Accounts = multQuery.Read<AccountSched_Model>().ToList();
                            tmp.Categories = multQuery.Read<CategoriesSched_Model>().ToList();
                            tmp.IngedProdAssoc = multQuery.Read<IngedProdCategAssocSched_Model>().ToList();
                            tmp.IngredCategories = multQuery.Read<IngredCategoriesSched_Model>().ToList();
                            tmp.PageSet = multQuery.Read<PageSetSched_Model>().ToList();
                            tmp.PriceListMaster = multQuery.Read<PriceListMasterSched_Model>().ToList();
                            tmp.SalesType = multQuery.Read<SalesTypeSched_Model>().ToList();
                            tmp.Taxes = multQuery.Read<TaxSched_Model>().ToList();
                            tmp.Units = multQuery.Read<UnitsSched_Model>().ToList();
                            tmp.Vats = multQuery.Read<VatSched_Model>().ToList();
                            tmp.Ingredients = multQuery.Read<IngredientsSched_Model>().ToList();
                            tmp.Pages = multQuery.Read<PagesSched_Model>().ToList();
                            tmp.PriceList = multQuery.Read<PriceListSched_Model>().ToList();
                            tmp.PriceListEffectHours = multQuery.Read<PriceList_EffHoursSched_Model>().ToList();
                            tmp.ProductCategories = multQuery.Read<ProductCategoriesSched_Model>().ToList();
                            tmp.Products = multQuery.Read<ProductSched_Model>().ToList();
                            tmp.ProductBarcodes = multQuery.Read<ProductBarcodesSched_Model>().ToList();
                            tmp.ProductRecipies = multQuery.Read<ProductRecipeSched_Model>().ToList();
                            tmp.ProductExtras = multQuery.Read<ProductExtrasSched_Model>().ToList();
                            tmp.PriceListDetails = multQuery.Read<PriceListDetailSched_Model>().ToList();
                            tmp.PageButtons = multQuery.Read<PageButtonSched_Model>().ToList();
                            tmp.PageButtonDetails = multQuery.Read<PageButtonDetSched_Model>().ToList();
                            tmp.PromotionsHeaders = multQuery.Read<PromotionsHeaderSched_Model>().ToList();
                            tmp.PromotionCombos = multQuery.Read<PromotionsCombosSched_Model>().ToList();
                            tmp.PromotionDiscount = multQuery.Read<PromotionsDiscountsSched_Model>().ToList();
                            Deleted.Add(tmp);
                        }

                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error("GetListDataFromServerForStoreId [SQL: " + SQL.ToString() + "] \r\n" + ex.ToString());
            }
            return ret;
        }

        private string ReturnTablesRecords(bool Deleted, long StoreId)
        {
            StringBuilder SQL = new StringBuilder();
            if (!Deleted)
                SQL.Append("DECLARE @StorId INT \n"
                        + " \n"
                        + "SET @StorId = " + StoreId.ToString() + " \n"
                        + " \n"
                        + "/*Accounts*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN Accounts AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @StorId AND dst.[Action] IN (0,2) AND dst.Short = 1 \n"
                        + " \n"
                        + "/*Categories*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN Categories AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @StorId AND dst.[Action] IN (0,2) AND dst.Short = 5 \n"
                        + " \n"
                        + "/*Ingredient_ProdCategoryAssoc*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN Ingredient_ProdCategoryAssoc AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @StorId AND dst.[Action] IN (0,2) AND dst.Short = 10 \n"
                        + " \n"
                        + "/*IngredientCategories*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN IngredientCategories AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @StorId AND dst.[Action] IN (0,2) AND dst.Short = 15 \n"
                        + " \n"
                        + "/*PageSet*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN PageSet AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @StorId AND dst.[Action] IN (0,2) AND dst.Short = 20 \n"
                        + " \n"
                        + "/*PricelistMaster*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN PricelistMaster AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @StorId AND dst.[Action] IN (0,2) AND dst.Short = 25 \n"
                        + " \n"
                        + "/*SalesType*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN SalesType AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @StorId AND dst.[Action] IN (0,2) AND dst.Short = 30 \n"
                        + " \n"
                        + "/*Tax*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN Tax AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @StorId AND dst.[Action] IN (0,2) AND dst.Short = 35 \n"
                        + " \n"
                        + "/*Units*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN Units AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @StorId AND dst.[Action] IN (0,2) AND dst.Short = 40 \n"
                        + " \n"
                        + "/*Vat*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN Vat AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @StorId AND dst.[Action] IN (0,2) AND dst.Short = 45 \n"
                        + " \n"
                        + "/*Ingredients*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN Ingredients AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @storId AND dst.[Action] IN (0,2) AND dst.Short = 50 \n"
                        + " \n"
                        + "/*Pages*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN Pages AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @storId AND dst.[Action] IN (0,2) AND dst.Short = 55 \n"
                        + " \n"
                        + "/*PriceList*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN PriceList AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @storId AND dst.[Action] IN (0,2) AND dst.Short = 60 \n"
                        + " \n"
                        + "/*PriceList_EffectiveHours*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN PriceList_EffectiveHours AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @storId AND dst.[Action] IN (0,2) AND dst.Short = 65 \n"
                        + " \n"
                        + "/*ProductCategories*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN ProductCategories AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @storId AND dst.[Action] IN (0,2) AND dst.Short = 70 \n"
                        + " \n"
                        + "/*Product*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN Product AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @storId AND dst.[Action] IN (0,2) AND dst.Short = 75 \n"
                        + " \n"
                        + "/*ProductBarcodes*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN ProductBarcodes AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @storId AND dst.[Action] IN (0,2) AND dst.Short = 77 \n"
                        + " \n"
                        + "/*ProductRecipe*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN ProductRecipe AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @storId AND dst.[Action] IN (0,2) AND dst.Short = 78 \n"
                        + " \n"
                        + "/*ProductExtras*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN ProductExtras AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @storId AND dst.[Action] IN (0,2) AND dst.Short = 80 \n"
                        + " \n"
                        + "/*PriceListDetail*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN PriceListDetail AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @storId AND dst.[Action] IN (0,2) AND dst.Short = 85 \n"
                        + " \n"
                        + "/*PageButton*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN PageButton AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @storId AND dst.[Action] IN (0,2) AND dst.Short = 90 \n"
                        + " \n"
                        + "/*PageButtonDetail*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN PageButtonDetail AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @storId AND dst.[Action] IN (0,2) AND dst.Short = 95 \n"
                        + " \n"
                        + "/*PromotionsHeaders*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN Promotions_Headers AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @storId AND dst.[Action] IN (0,2) AND dst.Short = 100 \n"
                        + " \n"
                        + "/*PromotionsCombos*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN Promotions_Combos AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @storId AND dst.[Action] IN (0,2) AND dst.Short = 105 \n"
                        + " \n"
                        + "/*PromotionsDiscounts*/ \n"
                        + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                        + "FROM DA_ScheduledTaskes AS dst \n"
                        + "INNER JOIN Promotions_Discounts AS i ON i.Id = dst.TableId \n"
                        + "WHERE dst.StoreId = @storId AND dst.[Action] IN (0,2) AND dst.Short = 110 \n");
            else
                SQL.Append("DECLARE @StorId INT \n"
                    + " \n"
                    + "SET @StorId = " + StoreId.ToString() + " \n"
                    + " \n"
                    + "/*Accounts*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN Accounts AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @StorId AND dst.[Action] IN (1) AND dst.Short = 1 \n"
                    + "ORDER BY dst.Id \n"
                    + " \n"
                    + "/*Categories*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN Categories AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @StorId AND dst.[Action] IN (1) AND dst.Short = 5 \n"
                    + "ORDER BY dst.Id \n"
                    + " \n"
                    + "/*Ingredient_ProdCategoryAssoc*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN Ingredient_ProdCategoryAssoc AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @StorId AND dst.[Action] IN (1) AND dst.Short = 10 \n"
                    + "ORDER BY dst.Id \n"
                    + " \n"
                    + "/*IngredientCategories*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN IngredientCategories AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @StorId AND dst.[Action] IN (1) AND dst.Short = 15 \n"
                    + "ORDER BY dst.Id \n"
                    + " \n"
                    + "/*PageSet*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN PageSet AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @StorId AND dst.[Action] IN (1) AND dst.Short = 20 \n"
                    + "ORDER BY dst.Id \n"
                    + " \n"
                    + "/*PricelistMaster*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN PricelistMaster AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @StorId AND dst.[Action] IN (1) AND dst.Short = 25 \n"
                    + "ORDER BY dst.Id \n"
                    + " \n"
                    + "/*SalesType*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN SalesType AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @StorId AND dst.[Action] IN (1) AND dst.Short = 30 \n"
                    + "ORDER BY dst.Id \n"
                    + " \n"
                    + "/*Tax*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN Tax AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @StorId AND dst.[Action] IN (1) AND dst.Short = 35 \n"
                    + "ORDER BY dst.Id \n"
                    + " \n"
                    + "/*Units*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN Units AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @StorId AND dst.[Action] IN (1) AND dst.Short = 40 \n"
                    + "ORDER BY dst.Id \n"
                    + " \n"
                    + "/*Vat*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN Vat AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @StorId AND dst.[Action] IN (1) AND dst.Short = 45 \n"
                    + "ORDER BY dst.Id \n"
                    + " \n"
                    + "/*Ingredients*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN Ingredients AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @storId AND dst.[Action] IN (1) AND dst.Short = 50 \n"
                    + "ORDER BY dst.Id \n"
                    + " \n"
                    + "/*Pages*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN Pages AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @storId AND dst.[Action] IN (1) AND dst.Short = 55 \n"
                    + "ORDER BY dst.Id \n"
                    + " \n"
                    + "/*PriceList*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN PriceList AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @storId AND dst.[Action] IN (1) AND dst.Short = 60 \n"
                    + "ORDER BY dst.Id \n"
                    + " \n"
                    + "/*PriceList_EffectiveHours*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN PriceList_EffectiveHours AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @storId AND dst.[Action] IN (1) AND dst.Short = 65 \n"
                    + "ORDER BY dst.Id \n"
                    + " \n"
                    + "/*ProductCategories*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN ProductCategories AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @storId AND dst.[Action] IN (1) AND dst.Short = 70 \n"
                    + "ORDER BY dst.Id \n"
                    + " \n"
                    + "/*Product*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN Product AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @storId AND dst.[Action] IN (1) AND dst.Short = 75 \n"
                    + "ORDER BY dst.Id \n"
                    + " \n"
                    + "/*ProductBarcodes*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN ProductBarcodes AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @storId AND dst.[Action] IN (1) AND dst.Short = 77 \n"
                    + "ORDER BY dst.Id \n"
                    + " \n"
                    + "/*ProductRecipe*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN ProductRecipe AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @storId AND dst.[Action] IN (1) AND dst.Short = 78 \n"
                    + "ORDER BY dst.Id \n"
                    + " \n"
                    + "/*ProductExtras*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN ProductExtras AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @storId AND dst.[Action] IN (1) AND dst.Short = 80 \n"
                    + "ORDER BY dst.Id \n"
                    + " \n"
                    + "/*PriceListDetail*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN PriceListDetail AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @storId AND dst.[Action] IN (1) AND dst.Short = 85 \n"
                    + "ORDER BY dst.Id \n"
                    + " \n"
                    + "/*PageButton*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN PageButton AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @storId AND dst.[Action] IN (1) AND dst.Short = 90 \n"
                    + "ORDER BY dst.Id \n"
                    + " \n"
                    + "/*PageButtonDetail*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN PageButtonDetail AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @storId AND dst.[Action] IN (1) AND dst.Short = 95 \n"
                    + "ORDER BY dst.Id \n"
                    + " \n"
                    + "/*PromotionsHeader*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN Promotions_Headers AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @storId AND dst.[Action] IN (1) AND dst.Short = 100 \n"
                    + "ORDER BY dst.Id \n"
                    + " \n"
                    + "/*PromotionsCombos*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN Promotions_Combos AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @storId AND dst.[Action] IN (1) AND dst.Short = 105 \n"
                    + "ORDER BY dst.Id \n"
                    + " \n"
                    + "/*PromotionsDiscounts*/ \n"
                    + "SELECT dst.Id MasterId, dst.StoreFullURL, dst.TableId, dst.[Action],  i.*  \n"
                    + "FROM DA_ScheduledTaskes AS dst \n"
                    + "LEFT OUTER JOIN Promotions_Discounts AS i ON i.Id = dst.TableId \n"
                    + "WHERE dst.StoreId = @storId AND dst.[Action] IN (1) AND dst.Short = 110 \n"
                    + "ORDER BY dst.Id \n");
            return SQL.ToString();
        }
    }
}
