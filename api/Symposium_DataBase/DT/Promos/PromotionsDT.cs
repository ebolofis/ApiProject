using AutoMapper;
using Dapper;
using log4net;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.Promos;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT.Promos;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Symposium.WebApi.DataAccess.DT.Promos
{
    public class PromotionsDT : IPromotionsDT
    {
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<Promotions_HeadersDTO> promoHeaderDAO;
        IGenericDAO<Promotions_CombosDTO> promoCombosDAO;
        IGenericDAO<Promotions_DiscountsDTO> promoDiscountsDAO;
        IGenericDAO<ProductDTO> productDAO;
        IGenericDAO<ProductCategoriesDTO> prodCategDAO;
        IGenericDAO<Promotions_PricelistsDTO> promoPricelistDAO;

        public PromotionsDT(
            IUsersToDatabasesXML usersToDatabases,
            IGenericDAO<Promotions_HeadersDTO> promoHeaderDAO,
            IGenericDAO<Promotions_CombosDTO> promoCombosDAO,
            IGenericDAO<Promotions_DiscountsDTO> promoDiscountsDAO,
            IGenericDAO<ProductDTO> productDAO,
            IGenericDAO<ProductCategoriesDTO> prodCategDAO,
            IGenericDAO<Promotions_PricelistsDTO> promoPricelistDAO
            )
        {
            this.usersToDatabases = usersToDatabases;
            this.promoHeaderDAO = promoHeaderDAO;
            this.promoCombosDAO = promoCombosDAO;
            this.promoDiscountsDAO = promoDiscountsDAO;
            this.productDAO = productDAO;
            this.prodCategDAO = prodCategDAO;
            this.promoPricelistDAO = promoPricelistDAO;
        }

        /// <summary>
        /// Get Promotions Header Only
        /// </summary>
        /// <returns>
        /// PromotionsModels
        /// </returns>
        public List<PromotionsModels> GetPromotionsHeader(DBInfoModel dbInfo)
        {
            List<PromotionsModels> promoModelList = new List<PromotionsModels>();
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);

            string sqlData = "SELECT * FROM Promotions_Headers AS ph WHERE ph.IsDeleted = 0";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    promoModelList = db.Query<PromotionsModels>(sqlData).ToList();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    throw new Exception(Symposium.Resources.Errors.DAMESSAGES);
                }
            }

            return promoModelList;
        }

        /// <summary>
        /// Get Promotions Header Only By HeaderId
        /// </summary>
        /// <param name="Id">HeaderId</param>
        /// <returns>
        /// PromotionsModels
        /// </returns>
        public PromotionsModels GetPromotionsHeaderById(DBInfoModel dbInfo, long Id)
        {
            PromotionsModels promoModel = new PromotionsModels();
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);

            string sqlData = "SELECT * FROM Promotions_Headers AS ph WHERE ph.Id =@headerID AND ph.IsDeleted = 0";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    promoModel = db.Query<PromotionsModels>(sqlData, new { headerID = Id }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    throw new Exception(Symposium.Resources.Errors.FAILEDLOADPROMOTIONS);
                }
            }

            return promoModel;
        }

        /// <summary>
        /// Get List of Promotions Combo By HeaderId
        /// </summary>
        /// <param name="Id">HeaderId</param>
        /// <returns>
        /// List of PromotionsCombos Extended Model
        /// </returns>
        public List<PromotionsCombosExt> GetPromotionsComboById(DBInfoModel dbInfo, long Id)
        {
            List<PromotionsCombosExt> promoCombosModel = new List<PromotionsCombosExt>();
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);

            string sqlData = @"SELECT pc.*, p.[Description] AS ProductDescr, pc2.[Description] AS ProductCatDescr
                                FROM Promotions_Combos AS pc
                                LEFT OUTER JOIN Product AS p ON pc.ItemId = p.Id
                                LEFT OUTER JOIN ProductCategories AS pc2 ON pc.ItemId = pc2.Id
                                WHERE pc.HeaderId =@headerID";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    promoCombosModel = db.Query<PromotionsCombosExt>(sqlData, new { headerID = Id }).ToList();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    throw new Exception(Symposium.Resources.Errors.FAILEDLOADPROMOTIONS);
                }
            }

            return promoCombosModel;
        }

        /// <summary>
        /// Get List of Promotions Discounts By HeaderId
        /// </summary>
        /// <param name="Id">HeaderId</param>
        /// <returns>
        /// List of Promotions Discounts Extended Model
        /// </returns>
        public List<PromotionsDiscountsExt> GetPromotionsDiscountsById(DBInfoModel dbInfo, long Id)
        {
            List<PromotionsDiscountsExt> promoDiscountsModel = new List<PromotionsDiscountsExt>();
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);

            string sqlData = @"SELECT pd.*, p.[Description] AS ProductDescr, pc2.[Description] AS ProductCatDescr
                                FROM Promotions_Discounts AS pd
                                LEFT OUTER JOIN Product AS p ON pd.ItemId = p.Id
                                LEFT OUTER JOIN ProductCategories AS pc2 ON pd.ItemId = pc2.Id
                                WHERE pd.HeaderId =@headerID";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    promoDiscountsModel = db.Query<PromotionsDiscountsExt>(sqlData, new { headerID = Id }).ToList();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    throw new Exception(Symposium.Resources.Errors.FAILEDLOADPROMOTIONS);
                }
            }

            return promoDiscountsModel;
        }

        /// <summary>
        /// Insert Promotions Header Only
        /// </summary>
        /// <returns>
        /// Headers Id
        /// </returns>
        public long InsertPromotion(DBInfoModel dbInfo, PromotionsModels Model)
        {
            long res = 0;
            Promotions_HeadersDTO dtoHeader = AutoMapper.Mapper.Map<Promotions_HeadersDTO>(Model);

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        //1.Insert Promotions Header
                        res = promoHeaderDAO.Insert(db, dtoHeader);

                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        throw new Exception(Symposium.Resources.Errors.FAILEDINSERTPROMOTIONS);
                    }
                    // Commit transaction
                    scope.Complete();
                }
            }

            return res;
        }


        public long InsertCombo(DBInfoModel dbinfo, PromotionsCombos Model)
        {
            long res = 0;
            Promotions_CombosDTO dtoCombos = AutoMapper.Mapper.Map<Promotions_CombosDTO>(Model);

            connectionString = usersToDatabases.ConfigureConnectionString(dbinfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        res= promoCombosDAO.Insert(db, dtoCombos);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        throw new Exception(Symposium.Resources.Errors.FAILEDINSERTPROMOTIONS);
                    }
                    // Commit transaction
                    scope.Complete();
                }
            }

            return res ;
        }

        public long InsertDiscount(DBInfoModel dbinfo, PromotionsDiscounts Model)
        {
            long res = 0;
            Promotions_DiscountsDTO dtoDiscounts = AutoMapper.Mapper.Map<Promotions_DiscountsDTO>(Model);

            connectionString = usersToDatabases.ConfigureConnectionString(dbinfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        res = promoDiscountsDAO.Insert(db, dtoDiscounts);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        throw new Exception(Symposium.Resources.Errors.FAILEDINSERTPROMOTIONS);
                    }
                    // Commit transaction
                    scope.Complete();
                }
            }

            return res;
        }

        /// <summary>
        /// Update Promotions Model (PromotionsHeader, PromotionsCombos and PromotionsDiscounts).
        /// </summary>
        /// <param name="PromotionsModels">Promotions Models</param>
        /// <returns>
        /// HeaderId
        /// </returns>
        public long UpdatePromotion(DBInfoModel dbInfo, PromotionsModels Model)
        {
            long res = 0;
            Promotions_HeadersDTO dtoHeader = AutoMapper.Mapper.Map<Promotions_HeadersDTO>(Model);

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);

           // string deleteCombos = "DELETE FROM Promotions_Combos WHERE HeaderId =@headerID";
           // string deleteDiscounts = "DELETE FROM Promotions_Discounts WHERE HeaderId =@headerID";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        //1.Update Promotions Header
                        res = promoHeaderDAO.Update(db, dtoHeader);

                        //2. Delete Combos
                       // db.Execute(deleteCombos, new { headerID = Model.Id });

                        //3. Delete Discounts
                       // db.Execute(deleteDiscounts, new { headerID = Model.Id });
                        
                        //4.Insert Combos
                        //if (Model.PromoCombo != null)
                        //{
                        //    foreach (PromotionsCombos combo in Model.PromoCombo)
                        //    {
                        //        Promotions_CombosDTO dtoCombos = AutoMapper.Mapper.Map<Promotions_CombosDTO>(combo);

                        //        promoCombosDAO.Insert(db, dtoCombos);
                        //    }
                        //}

                        ////5.Insert Discounts
                        //if (Model.PromoDiscounts != null)
                        //{
                        //    foreach (PromotionsDiscounts discounts in Model.PromoDiscounts)
                        //    {
                        //        Promotions_DiscountsDTO dtoDiscounts = AutoMapper.Mapper.Map<Promotions_DiscountsDTO>(discounts);

                        //        promoDiscountsDAO.Insert(db, dtoDiscounts);
                        //    }
                        //}
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        throw new Exception(Symposium.Resources.Errors.FAILEDUPDATEPROMOTIONS);
                    }
                    // Commit transaction
                    scope.Complete();
                }
            }

            return res;
        }
        public PromotionsCombos UpdateCombo(DBInfoModel dbinfo, PromotionsCombos Model)
        {
            Promotions_CombosDTO res;
            Promotions_CombosDTO dtoCombos = AutoMapper.Mapper.Map<Promotions_CombosDTO>(Model);

            connectionString = usersToDatabases.ConfigureConnectionString(dbinfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    try
                    {
                       promoCombosDAO.Update(db, dtoCombos);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        throw new Exception(Symposium.Resources.Errors.FAILEDUPDATEPROMOTIONS);
                    }
                    // Commit transaction
                    scope.Complete();
                }
            }

            return Model;
        }

        public PromotionsDiscounts UpdateDiscount(DBInfoModel dbinfo, PromotionsDiscounts Model)
        {
            
            Promotions_DiscountsDTO dtoDiscounts = AutoMapper.Mapper.Map<Promotions_DiscountsDTO>(Model);

            connectionString = usersToDatabases.ConfigureConnectionString(dbinfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        promoDiscountsDAO.Update(db, dtoDiscounts);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        throw new Exception(Symposium.Resources.Errors.FAILEDUPDATEPROMOTIONS);
                    }
                    // Commit transaction
                    scope.Complete();
                }
            }

            return Model;
        }
        /// <summary>
        /// Delete Promotions Model (PromotionsHeader, PromotionsCombos and PromotionsDiscounts).
        /// </summary>
        /// <param name="Id">HeaderId</param>
        /// <returns>
        /// HeaderId
        /// </returns>
        public long DeletePromotion(DBInfoModel dbInfo, long Id)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);

            string deleteHeader = "DELETE FROM Promotions_Headers WHERE Id =@ID";
            string deleteCombos = "DELETE FROM Promotions_Combos WHERE HeaderId =@headerID";
            string deleteDiscounts = "DELETE FROM Promotions_Discounts WHERE HeaderId =@headerID";
            string updateHeader = "UPDATE Promotions_Headers SET IsDeleted = 1 WHERE Id =@ID";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        //1. Delete Combos
                        db.Execute(deleteCombos, new { headerID = Id });

                        //2. Delete Discounts
                        db.Execute(deleteDiscounts, new { headerID = Id });

                        //3. Delete Header
                        db.Execute(deleteHeader, new { ID = Id });
                        res = Id;
                    }
                    catch (Exception ex)
                    {
                        db.Execute(updateHeader, new { ID = Id });
                        logger.Error(ex.ToString());
                        throw new Exception(Symposium.Resources.Errors.FAILEDDELETEPROMOTIONS);
                    }
                    // Commit transaction
                    scope.Complete();
                }
            }

            return res;
        }


        public long DeleteCombo(DBInfoModel dbInfo, long Id)
        {
            long res = 0;
            string deleteCombos = "DELETE FROM Promotions_Combos WHERE Id =@ID";
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
          
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Execute(deleteCombos, new { ID = Id });
            }

            return res;
        }

        public long DeleteDiscount(DBInfoModel dbInfo, long Id)
        {
            long res = 0;
             string deleteDiscounts = "DELETE FROM Promotions_Discounts WHERE Id =@ID";
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Execute(deleteDiscounts, new { ID = Id });
            }

            return res;
        }


        /// <summary>
        /// Returns a Header Promotion model by DAId and IDbConnection
        /// </summary>
        /// <param name="db"></param>
        /// <param name="DAId"></param>
        /// <returns></returns>
        public Promotions_HeadersDTO GetPromotionHeaderByDAId(IDbConnection db, long DAId)
        {
            try
            {
                return promoHeaderDAO.SelectFirst(db, "WHERE DAId = @DAId", new { DAId = DAId });
            }
            catch(Exception ex)
            {
                logger.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Returns a Header Promotion model by DAId and DBInfoModel
        /// </summary>
        /// <param name="db"></param>
        /// <param name="DAId"></param>
        /// <returns></returns>
        public Promotions_HeadersDTO GetPromotionHeaderByDAId(DBInfoModel Store, long DAId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return GetPromotionHeaderByDAId(db, DAId);
            }
        }

        
        /// <summary>
        /// Returns a Header Promotion Combo model by DAId and IDbConnection
        /// </summary>
        /// <param name="db"></param>
        /// <param name="DAId"></param>
        /// <returns></returns>
        public Promotions_CombosDTO GetPromotionComboByDAId(IDbConnection db, long DAId)
        {
            return promoCombosDAO.SelectFirst(db, "WHERE DAId = @DAId", new { DAId = DAId });
        }

        /// <summary>
        /// Returns a Header Promotion Combo model by DAId and DBInfoModel
        /// </summary>
        /// <param name="db"></param>
        /// <param name="DAId"></param>
        /// <returns></returns>
        public Promotions_CombosDTO GetPromotionComboByDAId(DBInfoModel Store, long DAId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return GetPromotionComboByDAId(db, DAId);
            }
        }

        /// <summary>
        /// Returns a Header Promotion Discount model by DAId and IDbConnection
        /// </summary>
        /// <param name="db"></param>
        /// <param name="DAId"></param>
        /// <returns></returns>
        public Promotions_DiscountsDTO GetPromotionDiscountByDAId(IDbConnection db, long DAId)
        {
            return promoDiscountsDAO.SelectFirst(db, "WHERE DAId = @DAId", new { DAId = DAId });
        }

        /// <summary>
        /// Returns a Header Promotion Discount model by DAId and DBInfoModel
        /// </summary>
        /// <param name="db"></param>
        /// <param name="DAId"></param>
        /// <returns></returns>
        public Promotions_DiscountsDTO GetPromotionDiscountByDAId(DBInfoModel Store, long DAId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return GetPromotionDiscountByDAId(db, DAId);
            }
        }

        /// <summary>
        /// Update or Insert's new records from Delivery Agent Tables for PromotionHeader
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformPromotionHeaderTablesFromDAServer(DBInfoModel Store, List<PromotionsHeaderSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = promoHeaderDAO.Upsert(db, Mapper.Map<List<Promotions_HeadersDTO>>(model));
                //{TODO Make It Better }
                SchedulerHelper schHlp = new SchedulerHelper();
                results.Results = schHlp.ReturnMasterIds(results.Results, model);
            }
            return results;
        }

        /// <summary>
        /// Update or Insert's new records from Delivery Agent Tables for PromotionCombos
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformPromotionCombosTablesFromDAServer(DBInfoModel Store, List<PromotionsCombosSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            //List of items without header id or produc-product category ID
            UpsertListResultModel failed = new UpsertListResultModel();
            failed.Results = new List<UpsertResultsModel>();

            //List of Ids to remove from model. Ids without header id or produc-product category ID
            List<long> RemoveIds = new List<long>();

            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (PromotionsCombosSched_Model item in model)
                {
                    //Get's Header Id
                    long localHeaderId = 0;
                    Promotions_HeadersDTO head = GetPromotionHeaderByDAId(db, item.HeaderId);
                    if (head != null)
                        localHeaderId = head.Id;
                    //Get's Product Or Product Category Id
                    long localProdOrProdCategId = 0;
                    if (item.ItemIsProduct)
                    {
                        ProductDTO prod = productDAO.SelectFirst(db, "WHERE DAId = @DAId", new { DAId = item.ItemId });
                        if (prod != null)
                            localProdOrProdCategId = prod.Id;
                    }
                    else
                    {
                        ProductCategoriesDTO prodCateg = prodCategDAO.SelectFirst(db, "WHERE DAId = @DAId", new { DAId = item.ItemId });
                        if (prodCateg != null)
                            localProdOrProdCategId = prodCateg.Id;
                    }
                    //If Header Id or Product - Product Category Id not exists then remove from model list
                    if (localHeaderId < 1 || localProdOrProdCategId < 1)
                    {
                        UpsertResultsModel tmp = new UpsertResultsModel();
                        tmp.ClientID = -1;
                        tmp.Succeded = false;
                        tmp.ErrorReason = ((localHeaderId < 1 ? "No Header Id for AgentId " + item.HeaderId.ToString() + " exists." : "")) + " " +
                                         ((localProdOrProdCategId < 1) ? "No id for ItemId " + item.ItemId + " for product or product categoried exists" : "");
                        RemoveIds.Add(item.TableId ?? 0);
                        failed.Results.Add(tmp);
                    }
                    else
                    {
                        //Change Agent Ids with local Ids
                        item.HeaderId = localHeaderId;
                        item.ItemId = localProdOrProdCategId;
                    }
                }

                //Remove items without Header Id or Product - Product Category Id
                if (RemoveIds.Count > 0)
                    model.RemoveAll(r => RemoveIds.Contains(r.TableId ?? 0));

                //Upsert valid model
                results = promoCombosDAO.Upsert(db, Mapper.Map<List<Promotions_CombosDTO>>(model));

                //Add to result faild items
                if (failed.Results != null && failed.Results.Count > 0)
                {
                    results.TotalRecords += failed.Results.Count();
                    results.TotalFailed += failed.Results.Count();
                    results.TotalUpdated += failed.Results.Count();
                    results.Results.AddRange(failed.Results);
                }

                //{TODO Make It Better }
                SchedulerHelper schHlp = new SchedulerHelper();
                results.Results = schHlp.ReturnMasterIds(results.Results, model);

            }
            return results;
        }

        /// <summary>
        /// Update or Insert's new records from Delivery Agent Tables for PromotionDiscount
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformPromotionDiscountTablesFromDAServer(DBInfoModel Store, List<PromotionsDiscountsSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            //List of items without header id or produc-product category ID
            UpsertListResultModel failed = new UpsertListResultModel();
            failed.Results = new List<UpsertResultsModel>();

            //List of Ids to remove from model. Ids without header id or produc-product category ID
            List<long> RemoveIds = new List<long>();

            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (PromotionsDiscountsSched_Model item in model)
                {
                    long localHeaderId = 0;
                    Promotions_HeadersDTO head = GetPromotionHeaderByDAId(db, item.HeaderId);
                    if (head != null)
                        localHeaderId = head.Id;
                    //Get's Product Or Product Category Id
                    long localProdOrProdCategId = 0;
                    if (item.ItemIsProduct)
                    {
                        ProductDTO prod = productDAO.SelectFirst(db, "WHERE DAId = @DAId", new { DAId = item.ItemId });
                        if (prod != null)
                            localProdOrProdCategId = prod.Id;
                    }
                    else
                    {
                        ProductCategoriesDTO prodCateg = prodCategDAO.SelectFirst(db, "WHERE DAId = @DAId", new { DAId = item.ItemId });
                        if (prodCateg != null)
                            localProdOrProdCategId = prodCateg.Id;
                    }

                    //If Header Id or Product - Product Category Id not exists then remove from model list
                    if (localHeaderId < 1 || localProdOrProdCategId < 1)
                    {
                        UpsertResultsModel tmp = new UpsertResultsModel();
                        tmp.ClientID = -1;
                        tmp.Succeded = false;
                        tmp.ErrorReason = ((localHeaderId < 1 ? "No Header Id for AgentId " + item.HeaderId.ToString() + " exists." : "")) + " " +
                                         ((localProdOrProdCategId < 1) ? "No id for ItemId " + item.ItemId + " for product or product categoried exists" : "");
                        RemoveIds.Add(item.TableId ?? 0);
                        failed.Results.Add(tmp);
                    }
                    else
                    {
                        //Change Agent Ids with local Ids
                        item.HeaderId = localHeaderId;
                        item.ItemId = localProdOrProdCategId;
                    }
                }

                //Remove items without Header Id or Product - Product Category Id
                if (RemoveIds.Count > 0)
                    model.RemoveAll(r => RemoveIds.Contains(r.TableId ?? 0));

                //Upsert valid model
                results = promoDiscountsDAO.Upsert(db, Mapper.Map<List<Promotions_DiscountsDTO>>(model));

                //Add to result faild items
                if (failed.Results != null && failed.Results.Count > 0)
                {
                    results.TotalRecords += failed.Results.Count();
                    results.TotalFailed += failed.Results.Count();
                    results.TotalUpdated += failed.Results.Count();
                    results.Results.AddRange(failed.Results);
                }

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
        public UpsertListResultModel DeleteRecordsForPromitonHeaderSendedFromDAServer(DBInfoModel Store, List<PromotionsHeaderSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (PromotionsHeaderSched_Model item in model)
                {
                    item.DAId = item.TableId ?? 0;
                    Promotions_HeadersDTO tmp = GetPromotionHeaderByDAId(db, item.TableId ?? 0);
                    if (tmp != null)
                        item.Id = tmp.Id;
                }

                results = promoHeaderDAO.DeleteOrSetIsDeletedList(db, Mapper.Map<List<Promotions_HeadersDTO>>(model));
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
        public UpsertListResultModel DeleteRecordsForPromotionCombosSendedFromDAServer(DBInfoModel Store, List<PromotionsCombosSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (PromotionsCombosSched_Model item in model)
                {
                    item.DAId = item.TableId ?? 0;
                    Promotions_CombosDTO tmp = GetPromotionComboByDAId(db, item.TableId ?? 0);
                    if (tmp != null)
                        item.Id = tmp.Id;
                }

                results = promoCombosDAO.DeleteOrSetIsDeletedList(db, Mapper.Map<List<Promotions_CombosDTO>>(model));
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
        public UpsertListResultModel DeleteRecordsForPromotionDiscountsSendedFromDAServer(DBInfoModel Store, List<PromotionsDiscountsSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (PromotionsDiscountsSched_Model item in model)
                {
                    item.DAId = item.TableId ?? 0;
                    Promotions_DiscountsDTO tmp = GetPromotionDiscountByDAId(db, item.TableId ?? 0);
                    if (tmp != null)
                        item.Id = tmp.Id;
                }

                results = promoDiscountsDAO.DeleteOrSetIsDeletedList(db, Mapper.Map<List<Promotions_DiscountsDTO>>(model));
                //{TODO Make It Better }
                SchedulerHelper schHlp = new SchedulerHelper();
                results.Results = schHlp.ReturnMasterIds(results.Results, model);
            }
            return results;
        }

        /// <summary>
        /// Get Promotions Pricelists
        /// </summary>
        /// <returns>PromotionsPricelistModel</returns>
        public List<PromotionsPricelistModel> GetPromotionsPricelists(DBInfoModel dbinfo)
        {
            List<PromotionsPricelistModel> promoPricelistModel = new List<PromotionsPricelistModel>();
            connectionString = usersToDatabases.ConfigureConnectionString(dbinfo);
            string sqlData = @"SELECT pp.*, p.[Description] AS PricelistDescr
                                FROM Promotions_Pricelists AS pp
                                INNER JOIN Pricelist AS p ON pp.PricelistId = p.Id";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    promoPricelistModel = db.Query<PromotionsPricelistModel>(sqlData).ToList();
                }
                catch(Exception ex)
                {
                    logger.Error(ex);
                    throw new Exception(ex.Message, ex);
                }
            }

            return promoPricelistModel;
        }

        /// <summary>
        /// Delete ALL Promotions Pricelists and Clear Table
        /// </summary>
        /// <returns></returns>
        public void DeleteAllPromotionPricelists(DBInfoModel dbinfo)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(dbinfo);
            string sqlData = "DELETE FROM Promotions_Pricelists";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    db.Execute(sqlData);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw new Exception(ex.Message, ex);
                }
            }

            return;
        }

        /// <summary>
        /// Upsert Promotions Pricelists On Save
        /// </summary>
        /// <returns></returns>
        public void UpsertPromotionPricelists(DBInfoModel dbinfo, PromotionsPricelistModel Model)
        {
            Promotions_PricelistsDTO dtoPromoPricelist = AutoMapper.Mapper.Map<Promotions_PricelistsDTO>(Model);
            connectionString = usersToDatabases.ConfigureConnectionString(dbinfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    promoPricelistDAO.Insert(db, dtoPromoPricelist);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw new Exception(ex.Message, ex);
                }
            }
            return;
        }
    }
}
