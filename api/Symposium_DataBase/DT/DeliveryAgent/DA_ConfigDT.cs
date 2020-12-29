using Dapper;
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
    public class DA_ConfigDT : IDA_ConfigDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;

        public DA_ConfigDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Get PosId
        /// </summary>
        /// <returns>PosId</returns>
        public long GetPosId(DBInfoModel Store)
        {
            long PosId = 0;

            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlGetPosId = @"SELECT pi1.Id FROM PosInfo AS pi1";
                PosId = db.Query<long>(sqlGetPosId).FirstOrDefault();
            }

            return PosId;
        }
    }
}
