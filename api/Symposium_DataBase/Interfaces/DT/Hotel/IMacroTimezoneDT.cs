using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symposium.Models.Models;
using Symposium.Models.Models.Hotel;
using Symposium_DTOs.PosModel_Info;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.Hotel
{
    public interface IMacroTimezoneDT
    {
        List<MacroTimezoneModel> GetTimezonesFromDatabase(DBInfoModel DBInfo);
    }
}
