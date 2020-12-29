using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    public interface IDA_OrderNoDT
    {
        long FetchOrderNo(IDbConnection db);
    }
}
