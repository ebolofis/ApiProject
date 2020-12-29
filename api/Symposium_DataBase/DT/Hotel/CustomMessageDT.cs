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
    public class CustomMessageDT :  ICustomMessageDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<HotelCustomMessagesDTO> genMessage;

        public CustomMessageDT(
            IUsersToDatabasesXML usersToDatabases,
            IGenericDAO<HotelCustomMessagesDTO> genMessage
            )
        {
            this.usersToDatabases = usersToDatabases;
            this.genMessage = genMessage;
        }
    }
}
