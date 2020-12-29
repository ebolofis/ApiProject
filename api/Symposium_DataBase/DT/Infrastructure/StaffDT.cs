using Dapper;
using log4net;
using Symposium.Helpers;
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
    public class StaffDT : IStaffDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<StaffDTO> staffGenericDAO;
        private ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public StaffDT(IUsersToDatabasesXML usersToDatabases, IGenericDAO<StaffDTO> _staffGenericDAO)
        {
            this.usersToDatabases = usersToDatabases;
            this.staffGenericDAO = _staffGenericDAO;
        }

        /// <summary>
        /// Gets all staff from db
        /// </summary>
        /// <param name="DBInfo"></param>
        /// <returns></returns>
        public List<DA_StaffModel> GetAllStaff(DBInfoModel DBInfo)
        {
            List<StaffDTO> staff;
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                staff = staffGenericDAO.Select(db);
            }
            return AutoMapper.Mapper.Map<List<DA_StaffModel>>(staff);
        }

        /// <summary>
        /// get all active staff for a specific pos. Assigned Positions and Actions per staff are included
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="forlogin">always true</param>
        /// <param name="posid">posinfo.id</param>
        /// <returns>
        /// for every staff return: 
        ///               Id, Code, FirstName, LastName, 
        ///               list of AssignedPositions,  
        ///               IsCheckedIn, ImageUri, 
        ///               list of ActionsId, 
        ///               list of ActionsDescription, 
        ///               password, Identification
        /// </returns>
        public StaffModelsPreview GetStaffs(DBInfoModel Store, string storeid, bool forlogin, long posid)
        {
            // get the results
            StaffModelsPreview getStaffModel = new StaffModelsPreview();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string staffQuery = "SELECT distinct s.Id, s.Code, s.FirstName, s.LastName, s.ImageUri, s.[Password],s.Identification \n"
                                  + "FROM Staff AS s  \n"
                                  + "LEFT OUTER JOIN AssignedPositions AS ap ON ap.StaffId = s.Id \n"
                                  + "LEFT OUTER JOIN PosInfo_StaffPositin_Assoc AS pispa ON pispa.StaffPositionId = ap.StaffPositionId  \n"
                                  + "WHERE pispa.PosInfoId =@pId AND (s.IsDeleted IS NULL OR s.IsDeleted = 0) AND pispa.StaffPositionId IS NOT NULL";

                string assignedPositionQuery = "SELECT * FROM AssignedPositions AS ap WHERE ap.StaffId =@sId";

                string actionIdQuery = "SELECT agd.ActionId \n"
                                     + "FROM AuthorizedGroupDetail AS agd \n"
                                     + "LEFT OUTER JOIN Actions AS a ON agd.ActionId = a.Id  \n"
                                     + "LEFT OUTER JOIN StaffAuthorization AS sa ON sa.AuthorizedGroupId = agd.AuthorizedGroupId \n"
                                     + "LEFT OUTER JOIN Staff AS s ON s.Id = sa.StaffId \n"
                                     + "WHERE s.Id =@sId";

                string ActionsDescriptionQuery = "SELECT a.[Description] AS ActionsDescription \n"
                                    + "FROM AuthorizedGroupDetail AS agd \n"
                                    + "LEFT OUTER JOIN Actions AS a ON agd.ActionId = a.Id  \n"
                                    + "LEFT OUTER JOIN StaffAuthorization AS sa ON sa.AuthorizedGroupId = agd.AuthorizedGroupId \n"
                                    + "LEFT OUTER JOIN Staff AS s ON s.Id = sa.StaffId \n"
                                    + "WHERE s.Id =@sId";

                string ischeckedQuery = "SELECT COUNT(ws.[Type]) FROM WorkSheet AS ws WHERE ws.StaffId =@sId";

                List<StaffModels> getStaff = db.Query<StaffModels>(staffQuery, new { pId = posid }).ToList();
                foreach(StaffModels sm in getStaff)
                {
                    List<AssignedPositionsModel> assignedPosition = db.Query<AssignedPositionsModel>(assignedPositionQuery, new { sId = sm.Id }).ToList();
                    sm.AssignedPositions = assignedPosition;

                    List<long> actId = db.Query<long>(actionIdQuery, new { sId = sm.Id }).ToList();
                    sm.ActionsId = actId;

                    List<string> actDescription = db.Query<string>(ActionsDescriptionQuery, new { sId = sm.Id }).ToList();
                    sm.ActionsDescription = actDescription;

                    int count = db.Query<int>(ischeckedQuery, new { sId = sm.Id }).FirstOrDefault();
                    if(count > 0)
                    {
                        sm.IsCheckedIn = true;
                    }
                    else
                    {
                        sm.IsCheckedIn = false;
                    }
                }
                getStaffModel.StaffModels = getStaff;
            }

            return getStaffModel;
        }

        /// <summary>
        /// Return  the DAStore for a specific (virtual) staff
        /// </summary>
        /// <param name="Store">db connection</param>
        /// <param name="staddId">staff.Id</param>
        /// <returns></returns>
        public long GetDaStore(DBInfoModel Store, long staffId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            string sql = "select DAStore from staff where id= @staffId";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    return db.QueryFirst<long>(sql, new { staffId = staffId });
                }catch(Exception ex)
                {
                    logger.Warn("Error getting DAStore from staff with id "+ staffId.ToString() + " :" + ex.Message+ " . ==>  Probably NO DAStore have set for the (virtual) staff.");
                    return 0;
                }
               
            }
        }

        /// <summary>
        /// Return's a Staff Model by Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public StaffModels GetStaffById(DBInfoModel Store, long Id)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string SQL = "SELECT * FROM Staff WHERE Id = " + Id.ToString();
                return AutoMapper.Mapper.Map<StaffModels>(db.Query<StaffDTO>(SQL).FirstOrDefault());
            }
        }

        /// <summary>
        /// return staff model by code
        /// </summary>
        /// <param name="Store">DBInfoModel</param>
        /// <param name="code">string</param>
        /// <returns>StaffModels</returns>
        public StaffModels GetStaffByCode(DBInfoModel Store, string code)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string SQL = "SELECT * FROM Staff WHERE Code = '" + code + "'";

                return AutoMapper.Mapper.Map<StaffModels>(db.Query<StaffDTO>(SQL).FirstOrDefault());
            }
        }

        public long LoyaltyAdminAuthorization(DBInfoModel DBInfo, string username, string pass)
        {

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                long staffid;
                StaffDTO model;
                string SQL = "SELECT * FROM Staff WHERE Code =@loyaltyadminusername and Password= @loyaltyadminpassword";
                model=(db.Query<StaffDTO>(SQL, new { loyaltyadminusername = username, loyaltyadminpassword = pass }).FirstOrDefault());
                if (model==null)
                    throw new BusinessException(Symposium.Resources.Errors.UNAUTHORIZEDADMINUSER);
             
              
                bool isAdmin = model.isAdmin;
                
            if(isAdmin == true)
                {// return the StaffId
                    staffid = model.Id;
                }
                else
                {//if not throw exception -- user is unathorized aka not an admin
                    logger.Error("Unauthorized Administrator - Can not Add Loyalty Points ");
                    throw new BusinessException(Symposium.Resources.Errors.UNAUTHORIZEDADMINUSER);
                }
                return staffid;
            }
        }

        /// <summary>
        /// check staff credentials supplied from webpos_reports login page to authorize staff to view reports
        /// </summary>
        /// <param name="reportsusername">string</param>
        /// <param name="reportspassword">string</param>
        /// <returns>bool</returns>
        public bool ReportsAuthorization(DBInfoModel DBInfo, string reportsusername, string reportspassword)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Staff WHERE Code = @reportsusername and Password = @reportspassword";

                StaffDTO model = (db.Query<StaffDTO>(query, new { reportsusername = reportsusername, reportspassword = reportspassword }).FirstOrDefault());

                if (model == null)
                {
                    logger.Error("Web Pos Report Login Failed: No staff found with supplied credentials");
                    throw new BusinessException(Symposium.Resources.Errors.WEBPOSREPORTSTAFFLOGINNOTFOUND);
                }
                else if (model.IsDeleted != null && model.IsDeleted == true)
                {
                    logger.Error("Web Pos Report Login Failed: Staff matching supplied credentials is inactive");
                    throw new BusinessException(Symposium.Resources.Errors.WEBPOSREPORTSTAFFLOGININACTIVE);
                }
                else if (model.AllowReporting == null || model.AllowReporting == false)
                {
                    logger.Error("Web Pos Report Login Failed: Staff matching supplied credentials is not authorized to access reports");
                    throw new BusinessException(Symposium.Resources.Errors.WEBPOSREPORTSTAFFLOGINUNAUTHORIZED);
                }

                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DBInfo"></param>
        /// <param name="usr"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public bool externalAppAuth(DBInfoModel DBInfo, string usr, string pass)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Staff WHERE Code = @usr and Password = @pass";

                StaffDTO model = (db.Query<StaffDTO>(query, new { usr = usr, pass = pass }).FirstOrDefault());

                if (model == null)
                {
                    logger.Error("External app authorization failed. No staff found with supplied credentials");
                    throw new BusinessException(Symposium.Resources.Errors.WEBPOSREPORTSTAFFLOGINNOTFOUND);
                }
                else if (model.IsDeleted != null && model.IsDeleted == true)
                {
                    logger.Error("External app authorization failed: Staff matching supplied credentials is inactive");
                    throw new BusinessException(Symposium.Resources.Errors.WEBPOSREPORTSTAFFLOGININACTIVE);
                }

                return true;
            }
        }
    }
}

