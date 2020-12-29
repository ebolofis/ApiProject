using Dapper;
using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using Symposium.WebApi.DataAccess.Interfaces.DT.TableReservations;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT.TableReservations
{
    public class EmailDT : IEmailDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;

        public EmailDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Send an Email to Customers
        /// </summary>
        /// <returns></returns>
        public string SendEmailToCustomers(DBInfoModel Store, ExtendedReservetionModel Model)
        {
            string restaurantName = "";
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //Get Restaurant Name By Id
                string restaurantQuery = @"DECLARE @rLang VARCHAR(10)
                                                  SET @rLang = @setLang
                                                  SELECT CASE WHEN UPPER(@rLang) = 'GR' THEN tr.NameGR
				                                                     WHEN UPPER(@rLang) = 'EN' THEN tr.NameEn
				                                                     WHEN UPPER(@rLang) = 'RU' THEN tr.NameRu
				                                                     WHEN UPPER(@rLang) = 'FR' THEN tr.NameFr
				                                                     WHEN UPPER(@rLang) = 'DE' THEN tr.NameDe
				                                                ELSE tr.NameGR END RestaurantName
                                                  FROM TR_Restaurants AS tr
                                                  WHERE tr.Id=@ID";
                restaurantName = db.Query<string>(restaurantQuery, new { ID = Model.Reservation.RestId, setLang = Model.Language }).FirstOrDefault();

            }
            return restaurantName;
        }
    }
}
