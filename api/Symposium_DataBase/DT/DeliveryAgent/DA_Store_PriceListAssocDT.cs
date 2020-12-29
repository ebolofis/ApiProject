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
    public class DA_Store_PriceListAssocDT : IDA_Store_PriceListAssocDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;

        public DA_Store_PriceListAssocDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Επιστρέφει όλες τις  pricelist ανα κατάστημα
        /// </summary>
        /// <returns>DAStore_PriceListAssocModel</returns>
        public List<DAStore_PriceListAssocModel> GetDAStore_PriceListAssoc(DBInfoModel dbInfo)
        {
            List<DAStore_PriceListAssocModel> storePriselistAssoc = new List<DAStore_PriceListAssocModel>();

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlData = @"SELECT dpla.*, p.[Description] AS PriceListDescription, ds.Title AS StoreTitle
                                FROM DAStore_PriceListAssoc AS dpla
                                INNER JOIN Pricelist AS p ON p.Id = dpla.PriceListId
                                INNER JOIN DA_Stores AS ds ON ds.Id = dpla.DAStoreId";

                storePriselistAssoc = db.Query<DAStore_PriceListAssocModel>(sqlData).ToList();
            }

            return storePriselistAssoc;
        }
    }
}
