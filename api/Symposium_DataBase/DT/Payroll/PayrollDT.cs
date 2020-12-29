using AutoMapper;
using Dapper;
using log4net;
using Symposium.Models.Models;
using Symposium.Models.Models.DateRange;
using Symposium.Models.Models.Payroll;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DAO.Payroll;
using Symposium.WebApi.DataAccess.Interfaces.DT.Payroll;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Symposium.WebApi.DataAccess.DT.Payroll
{
    public class PayrollDT : IPayrollDT
    {

        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<PayrollNewDTO> payroleDao;



        public PayrollDT(IUsersToDatabasesXML usertodbs, IGenericDAO<PayrollNewDTO> payroleDao)
        {
            this.usersToDatabases = usertodbs;
            this.payroleDao = payroleDao;
        }

        public List<PayrollNewModel> GetAllPayrole(DBInfoModel dbInfo)
        {
            List<PayrollNewModel> Model = new List<PayrollNewModel>();
            string sql = @"SELECT pn.*, s.LastName AS StaffDesc, pi1.[Description] AS PosInfoDesc, a.StaffPositionId AS StaffPositionIds
				            FROM PayrollNew AS pn
				            INNER JOIN Staff AS s ON pn.StaffId = s.Id
				            INNER JOIN PosInfo AS pi1 ON pn.PosInfoId = pi1.Id
				            CROSS apply (
					            SELECT STUFF((SELECT ',' + CAST(StaffPositionId AS NVARCHAR(100))
					            FROM AssignedPositions ap
					            WHERE ap.StaffId = pn.StaffId
					            FOR XML PATH('')) ,1,1,'') AS StaffPositionId
				            ) a
                            WHERE ISNULL(pn.IsDeleted,0) = 0
				            ORDER BY pn.DateFrom DESC";

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Model = db.Query<PayrollNewModel>(sql).ToList();
            }
            return Model;
        }

        public PayrollNewModel GetTopRowByType(DBInfoModel dbInfo, long StaffId)
        {
            PayrollNewModel Model = new PayrollNewModel();
            string sql = @"SELECT TOP 1 pn.*, s.LastName AS StaffDesc, pi1.[Description] AS PosInfoDesc, a.StaffPositionId AS StaffPositionIds
				            FROM PayrollNew AS pn
				            INNER JOIN Staff AS s ON pn.StaffId = s.Id
				            INNER JOIN PosInfo AS pi1 ON pn.PosInfoId = pi1.Id
				            CROSS apply (
					            SELECT STUFF((SELECT ',' + CAST(StaffPositionId AS NVARCHAR(100))
					            FROM AssignedPositions ap
					            WHERE ap.StaffId = pn.StaffId
					            FOR XML PATH('')) ,1,1,'') AS StaffPositionId
				            ) a
							WHERE pn.StaffId =@staffId AND ISNULL(pn.IsDeleted,0) = 0 AND convert(varchar(10), isnull(pn.DateFrom, GETDATE()), 102) >= convert(varchar(10), pn.DateFrom, 102)
				            ORDER BY pn.Id DESC";

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Model = db.Query<PayrollNewModel>(sql, new { staffId = StaffId }).FirstOrDefault();
            }
            return Model;
        }

        public long InsertPayroll(DBInfoModel Store, PayrollNewModel model)
        {
            long results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            PayrollNewDTO dto = Mapper.Map<PayrollNewDTO>(model);

            try
            {
                PayrollNewModel lastRow = GetTopRowByType(Store, model.StaffId);

                // check if staff is clocked in
                if (lastRow != null && lastRow.DateFrom != null && lastRow.DateTo == null && (lastRow.IsDeleted == null || lastRow.IsDeleted == false))
                {
                    return lastRow.Id;
                }
            }
            catch(Exception ex)
            {
                logger.Warn("Failed to check last staff payroll: " + ex.ToString());
            }

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                if (model.DateFrom != null && model.DateTo != null)
                {
                    if (model.DateFrom > model.DateTo)
                    {
                        results = 0;
                        throw new Exception("Ο χρήστης ΔΕΝ επιτρέπεται να γίνει clock out με ημερομηνία προηγούμενη του clock in");
                    }
                    else
                    {
                        TimeSpan ts = (DateTime)model.DateTo - (DateTime)model.DateFrom;
                        string total = @"" + ts.Hours + "." + ts.Minutes.ToString("00") + "";
                        dto.TotalHours = total;
                        dto.TotalMinutes = (long)ts.TotalMinutes;
                        dto.IsSend = 0;

                        results = payroleDao.Insert(db, dto);
                    }
                }
                else
                {
                    dto.TotalHours = "0.00";
                    dto.TotalMinutes = 0;
                    dto.IsSend = 0;
                    results = payroleDao.Insert(db, dto);
                }
            }
            return results;
        }

        public int Update(DBInfoModel dbinfo, PayrollNewModel model)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(dbinfo);
            PayrollNewDTO dto = Mapper.Map<PayrollNewDTO>(model);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                if (model.DateFrom != null && model.DateTo != null)
                {
                    if (model.DateFrom > model.DateTo)
                    {
                        results = 0;
                        throw new Exception("Ο χρήστης ΔΕΝ επιτρέπεται να γίνει clock out με ημερομηνία προηγούμενη του clock in");
                    }
                    else
                    {
                        TimeSpan ts = (DateTime)model.DateTo - (DateTime)model.DateFrom;
                        string total = @"" + ts.Hours + "." + ts.Minutes.ToString("00") + "";
                        dto.TotalHours = total;
                        dto.TotalMinutes = (long)ts.TotalMinutes;
                        dto.IsSend = 0;

                        results = payroleDao.Update(db, dto);
                    }
                }
                else
                {
                    dto.TotalHours = "0.00";
                    dto.TotalMinutes = 0;
                    dto.IsSend = 0;
                    results = payroleDao.Update(db, dto);
                }
            }
            return results;
        }

        public bool DeletePayroll(DBInfoModel dbinfo, long Id)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(dbinfo);
            string SqlQuery = "UPDATE PayrollNew SET IsDeleted = 1 WHERE Id =@ID";
            bool result = false;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Execute(SqlQuery, new { ID = Id });
                result = true;
            }
            return result;
        }

    }
}
