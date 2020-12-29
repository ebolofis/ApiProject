using Symposium.Models.Models.Hotel;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.CashedObjects
{
    public interface IHotelMacrosDT
    {
        ICashedDT<MacroModel, HotelMacrosDTO> CashedDT { get; set; }
    }
}

