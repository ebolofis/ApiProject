using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT
{
    public class HotelInfoDT : IHotelInfoDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<HotelInfoDTO> genHotelInfoDAO;

        public HotelInfoDT(IUsersToDatabasesXML usersToDatabases, IGenericDAO<HotelInfoDTO> genHotelInfoDAO)
        {
            this.usersToDatabases = usersToDatabases;
            this.genHotelInfoDAO = genHotelInfoDAO;
        }

        public HotelsInfoModel selectHotelInfoById(DBInfoModel store, long Id)
        {
            HotelInfoDTO hotelInfo;
            connectionString = usersToDatabases.ConfigureConnectionString(store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                hotelInfo = genHotelInfoDAO.Select(db, Id);
            }
            return AutoMapper.Mapper.Map<HotelsInfoModel>(hotelInfo);
        }

        public HotelsInfoModel selectHotelInfoByHotelId(DBInfoModel store, long hotelId)
        {
            HotelInfoDTO hotelInfo;
            connectionString = usersToDatabases.ConfigureConnectionString(store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                hotelInfo = genHotelInfoDAO.SelectFirst(db, "where HotelId = @hotelid", new { hotelid = hotelId });
            }
            return AutoMapper.Mapper.Map<HotelsInfoModel>(hotelInfo);
        }

        /// <summary>
        /// Get's first record from hotelInfo table
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public HotelsInfoModel SelectFirstHotelInfo(DBInfoModel store)
        {
            HotelInfoDTO hotelInfo;
            connectionString = usersToDatabases.ConfigureConnectionString(store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                hotelInfo = genHotelInfoDAO.Select(db).FirstOrDefault();
            }
            return AutoMapper.Mapper.Map<HotelsInfoModel>(hotelInfo);
        }
    }
}
