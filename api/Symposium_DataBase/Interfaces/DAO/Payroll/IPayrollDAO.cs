using Symposium.Models.Models.DateRange;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DAO.Payroll
{
 public interface IPayrollDAO
    {
        List<PayrollNewDTO> GetPayrole(IDbConnection db);
        long DeletePayroll(IDbConnection db, long Id);
        long InsertPayroll(IDbConnection db, PayrollNewDTO model);
        long Update(IDbConnection db, PayrollNewDTO model);
    }
}
