using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{
    public class HotelInfoTasks : IHotelInfoTasks
    {
        IHotelInfoDT hotelInfoDT;

        public HotelInfoTasks(IHotelInfoDT hotelInfoDT)
        {
            this.hotelInfoDT = hotelInfoDT;
        }

        public HotelsInfoModel SelectHotelInfoById(DBInfoModel store, long Id)
        {
            return hotelInfoDT.selectHotelInfoById(store, Id);
        }

        public HotelsInfoModel selectHotelInfoByHotelId(DBInfoModel store, long hotelId)
        {
            return hotelInfoDT.selectHotelInfoByHotelId(store, hotelId);
        }
    }
}
