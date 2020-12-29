using Dapper;
using log4net;
using Symposium.Models.Models;
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
    /// <summary>
    /// Class that handles data related to Pos Info Detail
    /// </summary>
    public class PosInfoDetailDT : IPosInfoDetailDT
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<PosInfoDetailDTO> posInfoDetailGenericDao;

        static object counterlock = 0;

        public PosInfoDetailDT(IUsersToDatabasesXML usersToDatabases, IGenericDAO<PosInfoDetailDTO> posInfoDetailGenericDao)
        {
            this.usersToDatabases = usersToDatabases;
            this.posInfoDetailGenericDao = posInfoDetailGenericDao;
        }

        /// <summary>
        /// Get pos info detail according to selected Id
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="PosInfoDetailId"> PosInfoDetail </param>
        /// <returns> Pos info detail model. See: <seealso cref="Symposium.Models.Models.PosInfoDetailModel"/> </returns>
        public PosInfoDetailModel GetSinglePosInfoDetail(DBInfoModel Store, long PosInfoDetailId,
            IDbConnection dbTran = null, IDbTransaction dbTransact = null)
        {
            PosInfoDetailDTO posInfoDetail;
            if (dbTran != null)
                posInfoDetail = posInfoDetailGenericDao.Select(dbTran, PosInfoDetailId, dbTransact);
            else
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    posInfoDetail = posInfoDetailGenericDao.Select(db, PosInfoDetailId);
                }
            }
            return AutoMapper.Mapper.Map<PosInfoDetailModel>(posInfoDetail);
        }

        /// <summary>
        /// Get pos info detail according to PosInfo Id and GroupId
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="PosInfoDetailId"> PosInfoDetail </param>
        /// <param name="GroupId"></param>
        /// <returns> Pos info detail model. See: <seealso cref="Symposium.Models.Models.PosInfoDetailModel"/> </returns>
        public PosInfoDetailModel GetSinglePosInfoDetail(DBInfoModel Store, long PosInfoId, long GroupId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return AutoMapper.Mapper.Map<PosInfoDetailModel>(posInfoDetailGenericDao.SelectFirst(db, "WHERE PosInfoId = @PosInfoId AND GroupId = @GroupId", new { PosInfoId = PosInfoId, GroupId = GroupId }));
            }
        }

        /// <summary>
        /// Get pos info detail according to selected Id
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="PosInfoDetailId"> PosInfoDetail </param>
        /// <returns> Pos info detail model. See: <seealso cref="Symposium.Models.Models.PosInfoDetailModel"/> </returns>
        public PosInfoDetailModel GetSinglePosInfoDetailByposId(DBInfoModel Store, long PosInfoId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return AutoMapper.Mapper.Map<PosInfoDetailModel>(posInfoDetailGenericDao.Select(db, "WHERE PosInfoId = @PosInfoId", new { PosInfoId = PosInfoId }).FirstOrDefault());
            }

        }

        /// <summary>
        /// Return new counter for specific PosIfo and PosInfoDetail
        /// Update's PosInfoDetail all record's for same GroupId and posinfo id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="PosInfoId"></param>
        /// <param name="PosInfoDetailId"></param>
        /// <returns></returns>
        public long GetNextCounter(DBInfoModel Store, long PosInfoId, long PosInfoDetailId)
        {
            long newCounter = -1;
            StringBuilder SQL = new StringBuilder();
            try
            {
                lock (counterlock)
                {
                    connectionString = usersToDatabases.ConfigureConnectionString(Store);
                    using (IDbConnection db = new SqlConnection(connectionString))
                    {
                        //1. Founing GroupId from PosInfoDetail for specific Id.
                        SQL.Clear();
                        SQL.Append("SELECT TOP 1 GroupId FROM PosInfoDetail WHERE Id = " + PosInfoDetailId.ToString());

                        int groupId = db.Query<int>(SQL.ToString()).FirstOrDefault();

                        List<PosInfoDetailDTO> posInfoDet = new List<PosInfoDetailDTO>();

                        //2. Get's a list of PosInfoDetail where PosIfoId = parameter[1] and GroupId = groupId
                        SQL.Clear();
                        SQL.Append("SELECT * \n"
                                + "FROM PosInfoDetail AS pid \n"
                                + "WHERE pid.PosInfoId = " + PosInfoId.ToString() + " AND pid.GroupId = " + groupId.ToString());

                        posInfoDet = db.Query<PosInfoDetailDTO>(SQL.ToString()).ToList();

                        //3. Must have atleast one record
                        if (posInfoDet == null || posInfoDet.Count == 0)
                            logger.Error("GetNextCounter [SQL:" + SQL.ToString() + "] \r\n "
                                        + "No records found on PosInfoDetail for PosInfo : " + PosInfoId.ToString() + " and GroupId : " + groupId.ToString());
                        else
                        {
                            //4. All items in list have same counter. Take fisrt plus one for newCounter
                            newCounter = (posInfoDet[0].Counter ?? 0) + 1;

                            //5. Update all list items with new counter
                            foreach (PosInfoDetailDTO item in posInfoDet)
                            {
                                item.Counter = newCounter;
                                posInfoDetailGenericDao.Update(db, item);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error("GetNextCounter [SQL:" + SQL.ToString() + "] \r\n" + ex.ToString());
            }
            return newCounter;
        }



        /// <summary>
        /// Return new counter for specific PosIfo and PosInfoDetail
        /// Update's PosInfoDetail all record's for same GroupId and posinfo id. 
        /// On ERROR return -1 
        /// <param name="db"></param>
        /// <param name="dbTransact"></param>
        /// <param name="PosInfoId"></param>
        /// <param name="PosInfoDetailId"></param>
        /// <param name="InvoiceTypeId"></param>
        public long GetNextCounter(IDbConnection db, IDbTransaction dbTransact, long PosInfoId, long PosInfoDetailId, long? InvoiceTypeId)
        {
            long newCounter = -1;
            StringBuilder SQL = new StringBuilder();
            string Error = "";
            try
            {
                lock (counterlock)
                {
                    //1. Founing GroupId from PosInfoDetail for specific Id.
                    SQL.Clear();
                    if (InvoiceTypeId == null || InvoiceTypeId == 0)
                        SQL.Append("SELECT TOP 1 GroupId FROM PosInfoDetail WHERE Id = " + PosInfoDetailId.ToString());
                    else
                        SQL.Append("SELECT DISTINCT TOP 1 GroupId \n"
                                 + "FROM PosInfoDetail pid \n"
                                 + "WHERE pid.Id = " + PosInfoDetailId.ToString() + " AND pid.InvoicesTypeId = " + InvoiceTypeId.ToString());

                    int groupId = db.Query<int>(SQL.ToString(), null, dbTransact).FirstOrDefault();

                    List<PosInfoDetailDTO> posInfoDet = new List<PosInfoDetailDTO>();

                    //2. Get's a list of PosInfoDetail where PosIfoId = parameter[1] and GroupId = groupId
                    SQL.Clear();
                    SQL.Append("SELECT * \n"
                            + "FROM PosInfoDetail AS pid \n"
                            + "WHERE pid.PosInfoId = " + PosInfoId.ToString() + " AND pid.GroupId = " + groupId.ToString());

                    posInfoDet = db.Query<PosInfoDetailDTO>(SQL.ToString(), null, dbTransact).ToList();

                    //3. Must have atleast one record
                    if (posInfoDet == null || posInfoDet.Count == 0)
                        logger.Error("GetNextCounter [SQL:" + SQL.ToString() + "] \r\n "
                                    + "No records found on PosInfoDetail for PosInfo : " + PosInfoId.ToString() + " and GroupId : " + groupId.ToString());
                    else
                    {
                        //4. All items in list have same counter. Take fisrt plus one for newCounter
                        newCounter = (posInfoDet[0].Counter ?? 0) + 1;

                        //5. Update all list items with new counter
                        foreach (PosInfoDetailDTO item in posInfoDet)
                        {
                            item.Counter = newCounter;
                            posInfoDetailGenericDao.Update(db, item, dbTransact, out Error);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                logger.Error("GetNextCounter [SQL:" + SQL.ToString() + "] \r\n" + Error);
                                return -1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("GetNextCounter [SQL:" + SQL.ToString() + "] \r\n" + ex.ToString());
            }
            return newCounter;
        }

        /// <summary>
        /// Return new counter for specific PosIfo and PosInfoDetail
        /// Update's PosInfoDetail all record's for same GroupId and posinfo id. -- DEPRECATED --
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="PosInfoId"></param>
        /// <param name="PosInfoDetailId"></param>
        /// <param name="InvoiceTypeType"></param>
        /// <returns></returns>
        public long GetNextCounter(DBInfoModel Store, long PosInfoId, long PosInfoDetailId, int InvoiceTypeType)
        {
            long newCounter = -1;
            StringBuilder SQL = new StringBuilder();
            try
            {
                lock (counterlock)
                {
                    connectionString = usersToDatabases.ConfigureConnectionString(Store);
                    using (IDbConnection db = new SqlConnection(connectionString))
                    {
                        //1. Founing GroupId from PosInfoDetail for specific Id.
                        SQL.Clear();
                        SQL.Append("SELECT DISTINCT TOP 1 GroupId \n"
                                 + "FROM PosInfoDetail pid \n"
                                 + "INNER JOIN InvoiceTypes AS it ON it.Id = pid.InvoicesTypeId AND it.[Type] = "+ InvoiceTypeType.ToString()+" \n"
                                 + "WHERE pid.Id = " + PosInfoDetailId.ToString());

                        int groupId = db.Query<int>(SQL.ToString()).FirstOrDefault();

                        List<PosInfoDetailDTO> posInfoDet = new List<PosInfoDetailDTO>();

                        //2. Get's a list of PosInfoDetail where PosIfoId = parameter[1] and GroupId = groupId
                        SQL.Clear();
                        SQL.Append("SELECT * \n"
                                + "FROM PosInfoDetail AS pid \n"
                                + "WHERE pid.PosInfoId = " + PosInfoId.ToString() + " AND pid.GroupId = " + groupId.ToString());

                        posInfoDet = db.Query<PosInfoDetailDTO>(SQL.ToString()).ToList();

                        //3. Must have atleast one record
                        if (posInfoDet == null || posInfoDet.Count == 0)
                            logger.Error("GetNextCounter [SQL:" + SQL.ToString() + "] \r\n "
                                        + "No records found on PosInfoDetail for PosInfo : " + PosInfoId.ToString() + " and GroupId : " + groupId.ToString());
                        else
                        {
                            //4. All items in list have same counter. Take fisrt plus one for newCounter
                            newCounter = (posInfoDet[0].Counter ?? 0) + 1;

                            //5. Update all list items with new counter
                            foreach (PosInfoDetailDTO item in posInfoDet)
                            {
                                item.Counter = newCounter;
                                posInfoDetailGenericDao.Update(db, item);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error("GetNextCounter [SQL:" + SQL.ToString() + "] \r\n" + ex.ToString());
            }
            return newCounter;
        }


    }
}
