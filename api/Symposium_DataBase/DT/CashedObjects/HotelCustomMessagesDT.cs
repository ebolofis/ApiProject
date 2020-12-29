using Symposium.Models.Models.Hotel;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.DT.CashedObjects;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT.CashedObjects
{
    public class HotelCustomMessagesDT : IHotelCustomMessagesDT
    {
        public ICashedDT<CustomMessageModel, HotelCustomMessagesDTO> CashedDT { get; set; }

        public HotelCustomMessagesDT(ICashedDT<CustomMessageModel, HotelCustomMessagesDTO> cashedDT)
        {
            CashedDT = cashedDT;
        }
    }
}
