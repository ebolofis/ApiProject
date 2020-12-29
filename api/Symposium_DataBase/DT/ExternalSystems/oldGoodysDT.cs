using Dapper;
using log4net;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT.ExternalSystems;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT.ExternalSystems
{
    public class oldGoodysDT : IoldGoodysDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public oldGoodysDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        public long DeleteAddress(DBInfoModel dbInfo, long Id, long CustomerId)
        {

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {

                string sql = @"DELETE FROM DA_Addresses WHERE Id=@Id and OwnerId=@CustomerId";

                try
                {
                    db.Execute(sql, new { Id = Id, CustomerId = CustomerId });
                }
                catch (Exception e)
                {
                    logger.Error("spcMessage:" + e.ToString());
                }
            }

            return 5;
        }

        public long getAddressbyExtId(DBInfoModel dbInfo, string extId, int k)
        {
            throw new NotImplementedException();
        }

        public long getCustomerbyExtId(DBInfoModel dbinfo, string ExtId, int k)
        {
            long id = -1;

            connectionString = usersToDatabases.ConfigureConnectionString(dbinfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {

                string sql = @"SELECT Id FROM DA_Customers WHERE ExtId"+ k + "= '"+ExtId +"' ";

                try
                {
                   var res = db.Query(sql).SingleOrDefault();
                    id = res.Id;
                }
                catch (Exception e)
                {
                    logger.Error("spcMessage:" + e.ToString());
                }
            }
            return id;
        }
    }
}