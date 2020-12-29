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
    public class StoreMessagesDT : IStoreMessagesDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;

        public StoreMessagesDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Επιστρέφει τα μηνύματα που εμφανίζονται στην κύρια σελίδα
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="filtered"></param>
        /// <returns></returns>
        public StoreMessagesModelsPreview GetStoreMessages(DBInfoModel Store, string storeid, bool filtered)
        {
            // get the results
            StoreMessagesModelsPreview storeMessagesModel = new StoreMessagesModelsPreview();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string storeMessagesQuery = "SELECT *  \n"
                                          + "FROM StoreMessages AS sm \n"
                                          + "WHERE sm.[Status] =@Status ";

                List<StoreMessagesModels> StoreMessages = db.Query<StoreMessagesModels>(storeMessagesQuery, new { Status = 1 }).ToList();
                storeMessagesModel.storeMessages = StoreMessages;
            }

            return storeMessagesModel;
        }
    }
}
