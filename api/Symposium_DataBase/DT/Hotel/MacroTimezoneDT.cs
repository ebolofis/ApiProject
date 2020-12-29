using Newtonsoft.Json;
using Symposium.Models.Models;
using Symposium.Models.Models.Hotel;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT.Hotel;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT.Hotel
{
    public class MacroTimezoneDT : IMacroTimezoneDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<HotelMacroTimezoneDTO> genTimezone;

        public MacroTimezoneDT(
            IUsersToDatabasesXML usersToDatabases,
            IGenericDAO<HotelMacroTimezoneDTO> genTimezone
            )
        {
            this.usersToDatabases = usersToDatabases;
            this.genTimezone = genTimezone;
        }

        public List<MacroTimezoneModel> GetTimezonesFromDatabase(DBInfoModel Store)
        {
            List<MacroTimezoneModel> result = new List<MacroTimezoneModel>();

            List<MacroTimezoneDBModel> dbData = new List<MacroTimezoneDBModel>();

            connectionString = usersToDatabases.ConfigureConnectionString(Store);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                dbData = AutoMapper.Mapper.Map<List<MacroTimezoneDBModel>>(genTimezone.Select(db,null,null));
            }

            foreach (MacroTimezoneDBModel row in dbData)
            {
                result.Add(JsonConvert.DeserializeObject<MacroTimezoneModel>(row.Model));
            }
            
            return result;
        }
    }
}
