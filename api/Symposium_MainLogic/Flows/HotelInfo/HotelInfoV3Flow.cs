using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using Symposium.WebApi.MainLogic.Interfaces.Flows.HotelInfo;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.HotelInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.HotelInfo
{


   public class HotelInfoV3Flow : IHotelInfoV3Flow
    {

        IHotelInfoV3Tasks HoteilInfoV3Tasks;


        public HotelInfoV3Flow(IHotelInfoV3Tasks tasks)
        {
            this.HoteilInfoV3Tasks = tasks;
        }


        public List<HotelsInfoModel> GetHotelInfo(DBInfoModel dbInfo)
        {
            List<HotelsInfoModel> hotelinfomodel;

            hotelinfomodel = HoteilInfoV3Tasks.GetHotelInfo(dbInfo);
            return hotelinfomodel;
        }

        public List<HotelInfoBaseModel> GetHotelInfoBase(DBInfoModel dbInfo)
        {
            List<HotelInfoBaseModel> hotelInfos;
            hotelInfos = HoteilInfoV3Tasks.GetHotelInfoBase(dbInfo);
            return hotelInfos;
        }

        public HotelsInfoModel GetHotelInfoById(DBInfoModel dbInfo, long hotelInfoId)
        {
            HotelsInfoModel hotelInfo;
            hotelInfo = HoteilInfoV3Tasks.GetHotelInfoById(dbInfo, hotelInfoId);
            return hotelInfo;
        }

        public List<TransferMappingsModel> GetTransferMappings(DBInfoModel DBInfo, long HotelId, long ProdCatId)
        {
            List<TransferMappingsModel> transfermappingsmodel;
            transfermappingsmodel = HoteilInfoV3Tasks.GetTransferMappings(DBInfo,HotelId,ProdCatId);
            return transfermappingsmodel;
        }

        public void UpdateTransferMappings(DBInfoModel DBInfo, long newPmsDepId, string newPmsDescr, long ProdCatId, long HotelId, long OldPmsDepartmentId)
        {
            HoteilInfoV3Tasks.UpdateTransferMappings(DBInfo, newPmsDepId, newPmsDescr, ProdCatId, HotelId, OldPmsDepartmentId);
        }

    }
}
