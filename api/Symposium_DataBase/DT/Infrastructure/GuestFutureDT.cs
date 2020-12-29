using Symposium.Models.Models;
using Symposium.Models.Models.Infrastructure;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT.Infrastructure;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT.Infrastructure
{
    public class GuestFutureDT : IGuestFutureDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<GuestFutureDTO> guestFutureDao;

        public GuestFutureDT(IUsersToDatabasesXML _usersToDatabases, IGenericDAO<GuestFutureDTO> _guestFutureDao)
        {
            this.usersToDatabases = _usersToDatabases;
            this.guestFutureDao = _guestFutureDao;
        }

        public List<GuestFutureModel> GetAllGuestFuture(DBInfoModel Store)
        {
            List<GuestFutureDTO> guestFuture = new List<GuestFutureDTO>();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                guestFuture = guestFutureDao.Select(db);
            }
            return AutoMapper.Mapper.Map<List<GuestFutureModel>>(guestFuture);
        }

    }
}
