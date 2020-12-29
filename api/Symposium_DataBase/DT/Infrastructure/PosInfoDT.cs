using AutoMapper;
using Dapper;
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

namespace Symposium.WebApi.DataAccess.DT {
    /// <summary>
    /// Class that handles data related to PosInfo
    /// </summary>
    public class PosInfoDT : IPosInfoDT {

        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<PosInfoDTO> posInfoGenericDao;
        static object LockObject = 0;

        public PosInfoDT(IUsersToDatabasesXML usersToDatabases, IGenericDAO<PosInfoDTO> posInfoGenericDao) {
            this.usersToDatabases = usersToDatabases;
            this.posInfoGenericDao = posInfoGenericDao;
        }

        /// <summary>
        /// Get pos info according to selected Id
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="PosInfoId"> PosInfo </param>
        /// <returns> Pos info model. See: <seealso cref="Symposium.Models.Models.PosInfoModel"/> </returns>
        public PosInfoModel GetSinglePosInfo(DBInfoModel Store, long PosInfoId) {
            PosInfoDTO posInfo;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString)) {
                posInfo = posInfoGenericDao.Select(db, PosInfoId);
            }

            return AutoMapper.Mapper.Map<PosInfoModel>(posInfo);
        }

        /// <summary>
        /// Get posInfo Models on a generic via department id filter over dtos and parce them to a model list
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DepatmentId">Filter of pos entities by departmentId</param>
        /// <returns>List of Posinfo Models collected from generic Posinfo DAO</returns>
        public List<PosInfoModel> GetDepartmentPosInfo(DBInfoModel Store, long DepatmentId) {
            List<PosInfoDTO> posInfos;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString)) {
                string query = @"Select * From Posinfo Where DepartmentId = @depfilter";
                DynamicParameters paramobj = new DynamicParameters();
                paramobj.Add("depfilter", DepatmentId);

                posInfos = posInfoGenericDao.Select(query, paramobj, db);
            }
            List<PosInfoModel> ret = Mapper.Map<List<PosInfoDTO>, List<PosInfoModel>>(posInfos);
            return ret;
        }

        /// <summary>
        /// Return's ExtEcr Name for specific PosInfo Id 
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="PosInfoId"></param>
        /// <returns></returns>
        public string GetExtEcrName(DBInfoModel Store, long PosInfoId)
        {
            string ret = "";
            string SQL = "";
            try
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    SQL = "SELECT ISNULL(FiscalName,'') FiscalName FROM PosInfo WHERE Id = " + PosInfoId.ToString();
                    ret = db.Query<string>(SQL).FirstOrDefault();
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return ret;
        }

        /// <summary>
        /// Return's next OrderNo from PosInfo
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dbTransact"></param>
        /// <param name="PosInfoId"></param>
        /// <param name="Error">the returned ERROR</param>
        /// <returns></returns>
        public long GetNextOrderNo(IDbConnection db, IDbTransaction dbTransact, long PosInfoId, out string Error)
        {
            lock (LockObject)
            {
                Error = "";
                PosInfoDTO pif = posInfoGenericDao.Select(db, PosInfoId, dbTransact);
                if (pif == null)
                {
                    Error = "Could not find posinfo for Id : " + PosInfoId.ToString();
                    return -1;
                }
                pif.ReceiptCount += 1;
                string sError = "";
                posInfoGenericDao.Update(db, pif, dbTransact, out sError);
                if (!string.IsNullOrEmpty(sError))
                {
                    Error = sError;
                    return -1;
                }
                   
                long orderno = pif.ReceiptCount ?? -1;
                return orderno;
            }
        }

        /// <summary>
        /// Get first pos id
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public long GetFirstPosInfoId(DBInfoModel Store)
        {
            long posInfoId = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlGetPosId = @"SELECT pi.Id FROM PosInfo AS pi";
                posInfoId = db.Query<long>(sqlGetPosId).FirstOrDefault();
            }
            return posInfoId;
        }

        /// <summary>
        /// Get All Configuration Files from PosInfo
        /// </summary>
        /// <returns>List of Pos Info Configuration Files</returns>
        public List<string> GetAllPosInfoConfig(DBInfoModel Store)
        {
            List<string> configList = new List<string>();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlGetConfig = @"SELECT distinct pi1.Configuration FROM PosInfo AS pi1 WHERE pi1.Configuration IS NOT NULL";
                configList = db.Query<string>(sqlGetConfig).ToList();
            }
            return configList;
        }
    }
}
