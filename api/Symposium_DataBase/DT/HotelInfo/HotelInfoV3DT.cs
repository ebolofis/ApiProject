using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DAO.HotelInfo;
using Symposium.WebApi.DataAccess.Interfaces.DT.HotelInfo;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT.HotelInfo
{
   public class HotelInfoV3DT : IHotelInfoV3DT
    {
        string connectionString;
        IHotelInfoV3DAO HotelInfoV3DAO;
        IGenericDAO<HotelInfoDTO> hotelInfoGenericDAO;
        IUsersToDatabasesXML usersToDatabases;

        public HotelInfoV3DT(IHotelInfoV3DAO HotelInfoV3DAO, IGenericDAO<HotelInfoDTO> hotelInfoGenericDAO, IUsersToDatabasesXML usertodbs)
        {
            this.HotelInfoV3DAO = HotelInfoV3DAO;
            this.hotelInfoGenericDAO = hotelInfoGenericDAO;
            this.usersToDatabases = usertodbs;
           
            // endOfYear2Dao = Resolve<IGenericITableDAO<EndOfYearDTO>>();
        }

        public List<HotelsInfoModel> GetHotelInfo(DBInfoModel dbInfo)
        {
            List<HotelInfoDTO> HotelInfo;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                HotelInfo = HotelInfoV3DAO.GetHotelInfo(db);
            }
            return AutoMapper.Mapper.Map<List<HotelsInfoModel>>(HotelInfo);
        }

        public List<HotelInfoBaseModel> GetHotelInfoBase(DBInfoModel dbInfo)
        {
            List<HotelInfoDTO> HotelInfos;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                HotelInfos = HotelInfoV3DAO.GetHotelInfo(db);
            }
            return AutoMapper.Mapper.Map<List<HotelInfoBaseModel>>(HotelInfos);
        }

        public HotelsInfoModel GetHotelInfoById(DBInfoModel dbInfo, long hotelInfoId)
        {
            HotelInfoDTO hotelInfo;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                hotelInfo = hotelInfoGenericDAO.Select(db, hotelInfoId);
            }
            return AutoMapper.Mapper.Map<HotelsInfoModel>(hotelInfo);
        }

        public List<TransferMappingsModel> GetTransferMappings(DBInfoModel DBInfo,long HotelId, long ProdCatId)
        {
            List<TransferMappingsDTO> model;
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                model = HotelInfoV3DAO.GetTransferMappings(db,HotelId,ProdCatId);
            }
            return AutoMapper.Mapper.Map<List<TransferMappingsModel>>(model);
        }

        public void UpdateTransferMappings(DBInfoModel DBInfo, long newPmsDepId, string newPmsDescr, long ProdCatId, long HotelId,long OldPmsDepartmentId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                HotelInfoV3DAO.UpdateTransferMappings(db, newPmsDepId, newPmsDescr, ProdCatId, HotelId, OldPmsDepartmentId);
            }
          
        }
    }
}


