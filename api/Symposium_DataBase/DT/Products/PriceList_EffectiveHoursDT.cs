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
    class PriceList_EffectiveHoursDT : IPriceList_EffectiveHoursDT
    {
        IGenericDAO<PriceList_EffectiveHoursDTO> dt;
        IUsersToDatabasesXML usersToDatabases;
        IPricelistDT prListDT;
        string connectionString;

        public PriceList_EffectiveHoursDT(IGenericDAO<PriceList_EffectiveHoursDTO> dt, IUsersToDatabasesXML usersToDatabases,
            IPricelistDT prListDT)
        {
            this.dt = dt;
            this.usersToDatabases = usersToDatabases;
            this.prListDT = prListDT;
        }

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<PriceList_EffHoursSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (PriceList_EffHoursSched_Model item in model)
                {
                    item.PricelistId = prListDT.GetIdByDAIs(Store, item.PricelistId ?? 0);
                }

                results = this.dt.Upsert(db, Mapper.Map<List<PriceList_EffectiveHoursDTO>>(model));
                //{TODO Make It Better }
                SchedulerHelper schHlp = new SchedulerHelper();
                results.Results = schHlp.ReturnMasterIds(results.Results, model);
            }
            return results;
        }

        public PriceList_EffectiveHoursDTO GetModelByDAId(DBInfoModel Store, long DAId)
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
        public UpsertListResultModel DeleteRecordsSendedFromDAServer(DBInfoModel Store, List<PriceList_EffHoursSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (PriceList_EffHoursSched_Model item in model)
                {
                    item.DAId = item.TableId;
                    PriceList_EffectiveHoursDTO tmp = GetModelByDAId(Store, item.TableId ?? 0);
                    if (tmp != null)
                        item.Id = tmp.Id;
                }

                results = dt.DeleteOrSetIsDeletedList(db, Mapper.Map<List<PriceList_EffectiveHoursDTO>>(model));
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
        public int UpdateModel(DBInfoModel Store, PriceList_EffectiveHoursModel item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.Update(db, Mapper.Map<PriceList_EffectiveHoursDTO>(item));
            }
            return results;
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel Store, List<PriceList_EffectiveHoursModel> item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.UpdateList(db, Mapper.Map<List<PriceList_EffectiveHoursDTO>>(item));
            }
            return results;
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel Store, PriceList_EffectiveHoursModel item)
        {
            long results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.Insert(db, Mapper.Map<PriceList_EffectiveHoursDTO>(item));
            }
            return results;
        }
    }
}
