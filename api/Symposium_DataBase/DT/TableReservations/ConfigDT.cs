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
    public class ConfigDT : IConfigDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;

        public ConfigDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Returns the Config
        /// </summary>
        /// <returns></returns>
        public ConfigModel GetConfig(DBInfoModel Store)
        {
            // get the results
            ConfigModel configDetails = new ConfigModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string getconfigQuery = @"SELECT * FROM TR_Config AS tc";
                ConfigModel config = db.Query<ConfigModel>(getconfigQuery).FirstOrDefault();
                configDetails = config;
            }
            return configDetails;
        }

        /// <summary>
        /// Insert new Config
        /// </summary>
        /// <returns></returns>
        public long insertConfig(DBInfoModel Store, ConfigModel model)
        {
            long res;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string insertQuery = @"INSERT INTO TR_Config(PreviewDays, EmailTemplate, EmailSubject, DefaultHotelId, ExtECR) 
                                       VALUES ( @PreviewDays, @EmailTemplate, @EmailSubject , @DefaultHotelId, @ExtECR )";

                db.Query(insertQuery, new
                {
                    PreviewDays = model.PreviewDays,
                    EmailTemplate = model.EmailTemplate,
                    EmailSubject = model.EmailSubject,
                    DefaultHotelId = model.DefaultHotelId,
                    ExtECR = model.ExtECR
                });

                res = db.Query<long>("SELECT tc.Id FROM TR_Config AS tc ORDER BY tc.id DESC").FirstOrDefault();
                return res;
            }
        }

        /// <summary>
        /// Update a Config
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public ConfigModel UpdateConfig(DBInfoModel Store, ConfigModel Model)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string updateQuery = @"UPDATE TR_Config SET PreviewDays=@PreviewDays, EmailTemplate=@EmailTemplate, EmailSubject=@EmailSubject,
                                                            DefaultHotelId=@DefaultHotelId, ExtECR=@ExtECR WHERE Id=@ID";

                db.Query(updateQuery, new
                {
                    ID = Model.Id,
                    PreviewDays = Model.PreviewDays,
                    EmailTemplate = Model.EmailTemplate,
                    EmailSubject = Model.EmailSubject,
                    DefaultHotelId = Model.DefaultHotelId,
                    ExtECR = Model.ExtECR
                });

                return Model;
            }
        }

        /// <summary>
        /// Delete a Config
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteConfig(DBInfoModel Store, long Id)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string deleteQuery = "DELETE FROM TR_Config WHERE Id=@ID";
                db.Query(deleteQuery, new { ID = Id });
                res = Id;
                return res;
            }
        }
    }
}
