using Dapper;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT
{
    public class StoreDT : IStoreDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;

        public StoreDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Επιστρέφει τη περιγραφή του καταστήματος (τυπικά επιστρέφει μία εγγραφή) 
        /// </summary>
        /// <remarks>GET api/Store</remarks>
        /// <param name="storeid"></param>
        /// <returns>
        public StoreDetailsModel GetStores(DBInfoModel Store, string storeid)
        {
            // get the results
            StoreDetailsModel storeDetails = new StoreDetailsModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                List<StoreDetails> storeDetailsList = db.Query<StoreDetails>("SELECT * FROM Store AS s").ToList();
                storeDetails.StoreDetailsPreview = storeDetailsList;
            }

            return storeDetails;
        }
    }
}
