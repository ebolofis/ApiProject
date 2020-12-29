using Symposium.Models.Models;
using Symposium.Models.Models.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.Infrastructure
{
    public interface ISuppliersDT
    {
        List<SupplierModel> GetSuppliers(DBInfoModel Store);

        long InsertSupplier(DBInfoModel Store, SupplierModel model);

        int UpdateSupplier(DBInfoModel Store, SupplierModel model);

        int DeleteSupplier(DBInfoModel Store, long Id);
    }
}
