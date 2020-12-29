using Dapper;
using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT.DeliveryAgent
{
    public class DA_StaffDT : IDA_StaffDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;

        public DA_StaffDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Authenticate Staff 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>StaffId</returns>
        public long LoginStaff(DBInfoModel Store, DALoginStaffModel loginStaffModel)
        {
            List<DA_StaffModel> staffModel = new List<DA_StaffModel>();
            long staffId = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string staffSQL = @"SELECT * FROM Staff AS dc WHERE dc.Code=@code AND dc.[Password]=@password";
                staffModel = db.Query<DA_StaffModel>(staffSQL, new { code = loginStaffModel.Username, password = loginStaffModel.Password }).ToList();

                if (staffModel.Count > 0)
                {
                    staffId = staffModel.Where(s => s.Code == loginStaffModel.Username && s.Password == loginStaffModel.Password).Select(f => f.Id).FirstOrDefault();
                }
                else
                {
                    throw new BusinessException(Symposium.Resources.Errors.USERLOGINFAILED);
                }
            }
            return staffId;
        }

        public DA_StaffModel GetStaffById(DBInfoModel Store, long id)
        {
            DA_StaffModel staff = new DA_StaffModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string staffSQL = @"SELECT * FROM Staff WHERE Id=@staffId AND (IsDeleted IS NULL OR IsDeleted = 0)";
                staff = db.Query<DA_StaffModel>(staffSQL, new { staffId = id }).FirstOrDefault();
            }
            return staff;
        }

    }
}
