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
    public class EmailConfigDT : IEmailConfigDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;

        public EmailConfigDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Returns the Email Config
        /// </summary>
        /// <returns></returns>
        public EmailConfigModel GetEmailConfig(DBInfoModel Store)
        {
            // get the results
            EmailConfigModel emailConfigDetails = new EmailConfigModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string getEmailConfigQuery = @"OPEN SYMMETRIC KEY KeyKhwp
                                          DECRYPTION BY CERTIFICATE WHPMAIN
                                          with PASSWORD  = 'pH!t8bb928DGvbD2439h0Ty';
                                          select id
                                               , Smtp
                                                ,Port
                                                ,[Ssl]
                                                ,Username
                                                ,Sender
                                                ,cast(DecryptByKey([Password]) as varchar) AS [Password]
                                                ,IsActive
                                         from EmailConfig 

                                         CLOSE SYMMETRIC KEY KeyKhwp;
                                         ";
                EmailConfigModel emailConfig = db.Query<EmailConfigModel>(getEmailConfigQuery).FirstOrDefault();
                emailConfigDetails = emailConfig;
            }
            return emailConfigDetails;
        }

        /// <summary>
        /// Insert new Email Config
        /// </summary>
        /// <returns></returns>
        public EmailConfigModel InsertEmailConfig(DBInfoModel Store, EmailConfigModel model)
        {
            EmailConfigModel insertedModel = new EmailConfigModel(); 
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string insertEmailConfigQuery = @"OPEN SYMMETRIC KEY KeyKhwp
                                                  DECRYPTION BY CERTIFICATE WHPMAIN
                                                  with PASSWORD  = 'pH!t8bb928DGvbD2439h0Ty';
                                                  insert into EmailConfig (Smtp, Port, [Ssl], Username, Sender, [Password], IsActive)
                                                  values
                                                  (@Smtp,
                                                   @Port, 
                                                   @Ssl,
                                                   @Username,
                                                   @Sender,
                                                   EncryptByKey(Key_GUID('KeyKhwp'), '" + model.Password + @"'),
                                                   @IsActive)                                                
                                                  CLOSE SYMMETRIC KEY KeyKhwp;
                                                  ";

                db.Query(insertEmailConfigQuery, new
                {
                    Smtp = model.Smtp,
                    Port = model.Port,
                    Ssl = model.Ssl,
                    Username = model.Username,
                    Sender = model.Sender,
                    IsActive = model.IsActive
                });

                string sqlData = @"OPEN SYMMETRIC KEY KeyKhwp
                                          DECRYPTION BY CERTIFICATE WHPMAIN
                                          with PASSWORD  = 'pH!t8bb928DGvbD2439h0Ty';
                                          select id
                                               , Smtp
                                                ,Port
                                                ,[Ssl]
                                                ,Username
                                                ,Sender
                                                ,cast(DecryptByKey([Password]) as varchar) AS [Password]
                                                ,IsActive
                                         from EmailConfig 
                                          ORDER BY Id DESC
                                         CLOSE SYMMETRIC KEY KeyKhwp";
                insertedModel = db.Query<EmailConfigModel>(sqlData).FirstOrDefault();
                return insertedModel;
            }
        }

        /// <summary>
        /// Update an Email Config
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public EmailConfigModel UpdateEmailConfig(DBInfoModel Store, EmailConfigModel Model)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string updateEmailConfigQuery = @"OPEN SYMMETRIC KEY KeyKhwp
                                                  DECRYPTION BY CERTIFICATE WHPMAIN
                                                  with PASSWORD  = 'pH!t8bb928DGvbD2439h0Ty';
                                                  UPDATE EmailConfig SET 
                                                              Smtp=@Smtp, 
                                                              Port =@Port,
                                                              [Ssl] =@Ssl, 
                                                              Username=@Username, 
                                                              Sender=@Sender,  
                                                              [Password] = EncryptByKey(Key_GUID('KeyKhwp'), '" + Model.Password + @"'), 
                                                              IsActive=@IsActive
                                                  WHERE Id=@ID
                                                  CLOSE SYMMETRIC KEY KeyKhwp; ";

                db.Query(updateEmailConfigQuery, new
                {
                    ID = Model.Id,
                    Smtp = Model.Smtp,
                    Port = Model.Port,
                    Ssl = Model.Ssl,
                    Username = Model.Username,
                    Sender = Model.Sender,
                    IsActive = Model.IsActive
                });

                return Model;
            }
        }

        /// <summary>
        /// Delete an Email Config
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteEmailConfig(DBInfoModel Store, long Id)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string deleteQuery = "DELETE FROM EmailConfig WHERE Id=@ID";
                db.Query(deleteQuery, new { ID = Id });
                res = Id;
                return res;
            }
        }
    }
}
