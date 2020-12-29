using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.Orders
{
    public interface IPaymentsDT
    {
        bool InsertPmsCharges(DBInfoModel dbInfo, List<TransferToPmsModel> pmsTransfers);
    }
}
