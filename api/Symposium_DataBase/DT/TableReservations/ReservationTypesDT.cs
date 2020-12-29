using AutoMapper;
using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT.TableReservations;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT.TableReservations
{
    public class ReservationTypesDT : IReservationTypesDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<TR_ReservationTypesDTO> reservationTypesGDao;

        public ReservationTypesDT(IUsersToDatabasesXML _usersToDatabases, IGenericDAO<TR_ReservationTypesDTO> _reservationTypesGDao)
        {
            this.usersToDatabases = _usersToDatabases;
            this.reservationTypesGDao = _reservationTypesGDao;
        }

        /// <summary>
        /// Get all reservation types from db
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public List<ReservationTypeModel> GetAllReservationTypes(DBInfoModel dbInfo)
        {
            List<TR_ReservationTypesDTO> reservationTypes = null;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                reservationTypes = reservationTypesGDao.Select(db);
            }
            return Mapper.Map<List<ReservationTypeModel>>(reservationTypes);
        }
    }
}
