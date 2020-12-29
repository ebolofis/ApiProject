using Dapper;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
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
   public class DA_PhoneticsDT:IDA_PhoneticsDT
    {
        PhoneticAbstHelper phoneticsHlp;
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        //star is true: every word (except shorthands & numbers) will end with '*'
        bool star;
        //andFlag is true: the connection between the words of the address (or erea) ia 'and', otherwise the connection is 'or'
        bool andFlag;

        public DA_PhoneticsDT(IUsersToDatabasesXML usersToDatabases, PhoneticAbstHelper phoneticsHlp)
        {
            this.phoneticsHlp = phoneticsHlp;
            this.usersToDatabases = usersToDatabases;
            star = (bool)MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_AddressSearchStar");
            andFlag = (bool)MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_AddressSearchAnd");
        }


        #region "DA Address Phonetics"
        /// <summary>
        /// Fill columns Phonetics and PhoneticsArea to ALL rows into table DA_Addresses with the respective phonetic values.
        /// </summary>
        public void CreateAllAddressPhonetics(DBInfoModel dbinfo)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(dbinfo);
            List<DA_AddressPhoneticModel> addresses;
            int c = 10;
            long lastId = 0;
            while (c > 0)
            {
                //1. read data from DA_Addresses
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    addresses = db.Query<DA_AddressPhoneticModel>($"Select top 1000 Id,AddressStreet,AddressNo,VerticalStreet,Area From DA_Addresses where Id>{lastId}").ToList();
                    c = addresses.Count();
                    //2. create phonetics for AddressStreet,AddressNo,VerticalStreet and Area
                    foreach (var addr in addresses)
                    {
                        if (addr.Id > lastId) lastId = addr.Id;

                        StringBuilder sb = new StringBuilder(addr.AddressStreet);
                        if (!string.IsNullOrWhiteSpace(addr.AddressNo)) sb.Append(" ").Append(addr.AddressNo);
                        if (!string.IsNullOrWhiteSpace(addr.VerticalStreet)) sb.Append(" ").Append(addr.VerticalStreet);

                        string addrThesaurus = phoneticsHlp.CreatePhoneticsToString(sb.ToString());

                        string areaThesaurus = null;
                        if (!string.IsNullOrWhiteSpace(addr.Area))
                        {
                            areaThesaurus = phoneticsHlp.CreatePhoneticsToString(addr.Area.Replace(",", " "));
                        }

                        //3. update phonetics to DB
                        string sqlQuery = "UPDATE DA_Addresses SET Phonetics = @Thesaurus,  PhoneticsArea = @ThesaurusArea  WHERE Id = @Id";
                        int rowsAffected = db.Execute(sqlQuery, new { Thesaurus = addrThesaurus, ThesaurusArea = areaThesaurus, Id = addr.Id });

                    }
                }
            }
        }

     


        /// <summary>
        /// Search addresses using phonetic data
        /// </summary>
        /// <param name="DBInfoModel"></param>
        /// <param name="search">Address[, Area]</param>
        /// <returns></returns>
        public List<DA_AddressModel> SearchAddressPhonetics(DBInfoModel dbinfo, string search)
        {

            if (string.IsNullOrWhiteSpace(search) || search.Trim() == ",") return new List<DA_AddressModel>();

            string sql = ConstructAddressSqlQuery(search);
            if (string.IsNullOrWhiteSpace(sql)) return new List<DA_AddressModel>();
            string connectionString = usersToDatabases.ConfigureConnectionString(dbinfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<DA_AddressModel>(sql).ToList();
            }
        }

        /// <summary>
        /// construct sql query to search da_addresses table
        /// </summary>
        /// <param name="search">adress's words</param>
        /// <returns></returns>
        public string ConstructAddressSqlQuery(string search)
        {
            string sql = "";

            var parts = search.Split(',');
            string addr = parts[0].Trim();
            string area = null;
            string sqlAddr = createValuesFullTextPhon(addr);
            string sqlArea = "";
            if (parts.Count() > 1 && !string.IsNullOrWhiteSpace(parts[1]))
            {
                area = parts[1].Trim();
                sqlArea = createValuesFullTextPhon(area);
            }

            if (sqlAddr != "" && sqlArea == "")
                sql = $"SELECT * FROM da_addresses where  CONTAINS(Phonetics, '{sqlAddr}');";
            else if (sqlAddr != "" && sqlArea != "")
                sql = $"SELECT * FROM da_addresses where  CONTAINS(Phonetics, '{sqlAddr}') and CONTAINS(PhoneticsArea, '{sqlArea}');";
            else if (sqlAddr == "" && sqlArea != "")
                sql = $"SELECT * FROM da_addresses where  CONTAINS(PhoneticsArea, '{sqlArea}');";

            Console.WriteLine(sql);
            return sql;
        }

        /// <summary>
        /// construct sql query to search da_customers table
        /// </summary>
        /// <param name="search">adress's words<</param>
        /// <returns></returns>
        public string ConstructCustomersSqlQuery(string search)
        {
            string sql = "";

            var parts = search.Split(',');
            string addr = parts[0].Trim();
            string area = null;
            string sqlAddr = createValuesFullTextPhon(addr);
            string sqlArea = "";
            if (parts.Count() > 1 && !string.IsNullOrWhiteSpace(parts[1]))
            {
                area = parts[1].Trim();
                sqlArea = createValuesFullTextPhon(area);
            }

            sql=$@"SELECT dc.* FROM DA_Customers AS dc INNER JOIN DA_Addresses AS da on da.OwnerId = dc.Id where ";

            if (sqlAddr != "" && sqlArea == "")
                sql = sql + $" CONTAINS(da.Phonetics, '{sqlAddr}');";
            else if (sqlAddr != "" && sqlArea != "")
                sql = sql + $"  CONTAINS(da.Phonetics, '{sqlAddr}') and CONTAINS(da.PhoneticsArea, '{sqlArea}');";
            else if (sqlAddr == "" && sqlArea != "")
                sql = sql + $"  CONTAINS(da.PhoneticsArea, '{sqlArea}');";
            else
                sql = "";
            Console.WriteLine(sql);
            return sql;
        }


        private string createValuesFullTextPhon(string search)
        {
            string logical = null;
            if (andFlag)
                logical = "and";
            else
                logical = "or";
            string ret = "";
            if (string.IsNullOrWhiteSpace(search)) return ret;

            var parts = search.Split(' ').ToList();
            parts.RemoveAll(x => string.IsNullOrWhiteSpace(x));
            for (int j = 0; j < parts.Count(); j++)
            {
                List<string> phon = phoneticsHlp.CreatePhonetics(parts[j]);
                string sql = "";
                for (int i = 0; i < phon.Count; i++)
                {
                    if (i == 0 && phon.Count > 1) sql = "(";
                    if (i > 0 && phon.Count > 1) sql = sql + " or ";
                    if (star && !phoneticsHlp.IsShorthand(phon[i]) && !phon[i].EndsWith("*") && !int.TryParse(phon[i], out _)) phon[i] = phon[i] + "*";
                    if (phon[i].EndsWith("*"))
                        sql = sql + $"\"{phon[i]}\"";
                    else
                        sql = sql + phon[i];

                }
                if (phon.Count > 1) sql = sql + ")";

                if (j == 0)
                    ret = sql;
                else
                {
                    if (string.IsNullOrWhiteSpace(ret))
                        ret = $"{sql}";
                    else
                        ret = $"{ret} {logical} {sql}";
                }

            }

            return ret;
        }


        #endregion 
    }
}
