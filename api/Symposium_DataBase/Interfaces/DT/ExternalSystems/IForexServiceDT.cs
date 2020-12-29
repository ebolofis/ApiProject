using Symposium.Models.Models;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    public interface IForexServiceDT
    {

        List<ForexServiceModel> SelectAllForex(DBInfoModel store);

    }
}
