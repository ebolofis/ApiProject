using log4net;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.DAOs;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium.WebApi.DataAccess.XMLs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symposium.Helpers.Interfaces;
using Symposium_DTOs.PosModel_Info;

namespace Symposium.WebApi.DataAccess.DT
{
    /// <summary>
    /// Class that handles data related to End Of Day Procedure
    /// </summary>
    public class LockersDT : ILockersDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<LockersDTO> lockersDao;


        public LockersDT(IUsersToDatabasesXML usersToDatabases, IGenericDAO<LockersDTO> lockersDao)
        {
            this.usersToDatabases = usersToDatabases;
            this.lockersDao = lockersDao;
        }

        /// <summary>
        /// Selects lockers for current or selected day
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="endOfDayId"> Id of end of day </param>
        /// <returns> Lockers model. See: <seealso cref="Symposium.Models.Models.LockersModel"/> </returns>
        public LockersModel GetLockers(DBInfoModel Store, long posInfoId, long? endOfDayId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                List<LockersDTO> lockers;
                if (endOfDayId == null)
                {
                    lockers = lockersDao.Select("select * from Lockers where PosInfoId = @posinfoid and EndOfDayId is null", new { posinfoid = posInfoId }, db);
                }
                else
                {
                    lockers = lockersDao.Select("select * from Lockers where PosInfoId = @posinfoid and EndOfDayId=@endofdayid", new { posinfoid = posInfoId, endofdayid = endOfDayId }, db);
                }
                LockersDTO locker;
                if (lockers.Count() == 0)
                    locker = null;
                else
                    locker = lockers[0];
                return AutoMapper.Mapper.Map<LockersModel>(locker);
            }
        }
    }
}
