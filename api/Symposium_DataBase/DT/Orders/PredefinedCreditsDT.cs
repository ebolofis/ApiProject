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
    public class PredefinedCreditsDT : IPredefinedCreditsDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;

        public PredefinedCreditsDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// return all predefined credits (προκαθορισμένα  ποσά  για Ticket Restaurant/κουπόνια). 
        ///  GET api/PredefinedCredits
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns>list of PredefinedCredits</returns>
        public PredefinedCreditsModelsPreview GetPredefinedCredits(DBInfoModel Store, string storeid)
        {
            // get the results
            PredefinedCreditsModelsPreview getPredefinedCreditsModel = new PredefinedCreditsModelsPreview();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string predefinedCreditsQuery = "SELECT * FROM PredefinedCredits AS pc";

                List<PredefinedCreditsModel> PredefinedCredits = db.Query<PredefinedCreditsModel>(predefinedCreditsQuery).ToList();
                getPredefinedCreditsModel.PredefinedCreditsModel = PredefinedCredits;
            }

            return getPredefinedCreditsModel;
        }

    }
}
