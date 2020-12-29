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
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT.Hotel
{
    public class MacroDT : IMacroDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<HotelMacrosDTO> genMacro;

        public MacroDT(
            IUsersToDatabasesXML usersToDatabases,
            IGenericDAO<HotelMacrosDTO> genMacro
            )
        {
            this.usersToDatabases = usersToDatabases;
            this.genMacro = genMacro;
        }
    }
}
