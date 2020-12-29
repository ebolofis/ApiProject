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
    public class PagesDT : IPagesDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<PagesDTO> dt;
        IPageSetDT pageSetDT;
        public PagesDT(IUsersToDatabasesXML usersToDatabases, IGenericDAO<PagesDTO> dt, IPageSetDT pageSetDT)
        {
            this.usersToDatabases = usersToDatabases;
            this.dt = dt;
            this.pageSetDT = pageSetDT;
        }

        /// <summary>
        /// Return all active pages (pageSets and pageButtons are NOT included) for a specific POS for the current date ordered by Pages.Sort
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="posid"></param>
        /// <returns></returns>
        public PagesModelsPreview GetPagesForPosId(DBInfoModel Store, string storeid, long posid)
        {
            // get the results
            PagesModelsPreview getPagesModel = new PagesModelsPreview();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string pagesQuery = "SELECT p.* \n"
                                  + "FROM Pages AS p \n"
                                  + "LEFT OUTER JOIN PageSet AS ps ON ps.Id = p.PageSetId \n"
                                  + "LEFT OUTER JOIN PagePosAssoc AS ppa ON ppa.PageSetId = ps.Id \n"
                                  + "WHERE ps.ActivationDate <= GETDATE() AND ps.DeactivationDate >= GETDATE() AND ppa.PosInfoId =@pId";

                List<PagesModel> getPages = db.Query<PagesModel>(pagesQuery, new { pId = posid }).ToList();
                getPagesModel.PagesModel = getPages;
            }

            return getPagesModel;
        }

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<PagesSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (PagesSched_Model item in model)
                {
                    item.PageSetId = pageSetDT.GetIdByDAIs(Store, item.PageSetId ?? 0);
                }
                results = this.dt.Upsert(db, Mapper.Map<List<PagesDTO>>(model));
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
        public UpsertListResultModel DeleteRecordsSendedFromDAServer(DBInfoModel Store, List<PagesSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString)) 
            {
                foreach (PagesSched_Model item in model)
                {
                    item.DAId = item.TableId;
                    item.Id = GetIdByDAIs(Store, item.TableId ?? 0) ?? 0;
                }

                results = dt.DeleteOrSetIsDeletedList(db, Mapper.Map<List<PagesDTO>>(model));
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
        public int UpdateModel(DBInfoModel Store, PagesModel item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.Update(db, Mapper.Map<PagesDTO>(item));
            }
            return results;
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel Store, List<PagesModel> item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.UpdateList(db, Mapper.Map<List<PagesDTO>>(item));
            }
            return results;
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel Store, PagesModel item)
        {
            long results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.Insert(db, Mapper.Map<PagesDTO>(item));
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
                PagesDTO tmp = dt.SelectFirst(db, "WHERE DAId = @DAId", new { DAId = dAId });
                if (tmp == null)
                    return null;
                else
                    return tmp.Id;
            }
        }
    }
}
