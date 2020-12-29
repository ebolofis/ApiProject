using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows
{
    public class HotelInfoFlows : IHotelInfoFlows
    {
        IHotelInfoTasks hotelInfoTasks;

        public HotelInfoFlows(IHotelInfoTasks hotelInfoTasks)
        {
            this.hotelInfoTasks = hotelInfoTasks;
        }

        public HotelsInfoModel selectHotelInfoByHotelId(DBInfoModel dbInfo, long hotelInfoId)
        {
            return hotelInfoTasks.selectHotelInfoByHotelId(dbInfo, hotelInfoId);
        }

    }
}
