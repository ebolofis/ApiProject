using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DAO.Goodys
{
    public interface IGoodysDAO
    {
        OrderDTO GetGoodysExternalOrderID(IDbConnection db, long InvoiceId);
        long GetOpenOrders(IDbConnection db);

        long GetInvoiceid(IDbConnection db, long orderno);
    }
}
