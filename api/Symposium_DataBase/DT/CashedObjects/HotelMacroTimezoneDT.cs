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
   public class HotelMacroTimezoneDT: IHotelMacroTimezoneDT
    {
        public ICashedDT<MacroTimezoneModel, HotelMacroTimezoneDTO> CashedDT { get; set; }
        public HotelMacroTimezoneDT(ICashedDT<MacroTimezoneModel, HotelMacroTimezoneDTO> cashedDT)
        {
            CashedDT = cashedDT;
        }

        

    }
}
