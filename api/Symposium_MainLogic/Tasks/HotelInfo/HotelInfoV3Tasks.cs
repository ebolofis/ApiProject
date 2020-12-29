using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using Symposium.WebApi.DataAccess.DAOs.HotelInfo;
using Symposium.WebApi.DataAccess.Interfaces.DAO.HotelInfo;
using Symposium.WebApi.DataAccess.Interfaces.DT.HotelInfo;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.HotelInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks.HotelInfo
{
    public class HotelInfoV3Tasks : IHotelInfoV3Tasks
    {
        IHotelInfoV3DT HotelInfoV3DT;
                   

        public HotelInfoV3Tasks(IHotelInfoV3DT HotelInfoV3DT)
        {

            this.HotelInfoV3DT = HotelInfoV3DT;
        }

        public List<HotelsInfoModel> GetHotelInfo( DBInfoModel dbInfo)
        {
            return HotelInfoV3DT.GetHotelInfo( dbInfo);

        }

        public List<HotelInfoBaseModel> GetHotelInfoBase(DBInfoModel dbInfo)
        {
            return HotelInfoV3DT.GetHotelInfoBase(dbInfo);
        }

        public HotelsInfoModel GetHotelInfoById(DBInfoModel dbInfo, long hotelInfoId)
        {
            return HotelInfoV3DT.GetHotelInfoById(dbInfo, hotelInfoId);
        }

       public List<TransferMappingsModel> GetTransferMappings(DBInfoModel DBInfo , long HotelId, long ProdCatId)
        {
            return HotelInfoV3DT.GetTransferMappings(DBInfo, HotelId, ProdCatId);
        }

       public void UpdateTransferMappings(DBInfoModel DBInfo, long newPmsDepId, string newPmsDescr, long ProdCatId, long HotelId, long OldPmsDepartmentId)
        {
             HotelInfoV3DT.UpdateTransferMappings(DBInfo, newPmsDepId, newPmsDescr,ProdCatId, HotelId, OldPmsDepartmentId);
        }
    }
}




