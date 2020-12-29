using Symposium.Models.Models;
using Symposium.Models.Models.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.Infrastructure
{
    public interface ISuppliersTasks
    {
        List<SupplierModel> GetSuppliers(DBInfoModel Store);

        List<SupplierModel> InsertSupplier(DBInfoModel Store, SupplierModel model);

        List<SupplierModel> UpdateSupplier(DBInfoModel Store, SupplierModel model);

        List<SupplierModel> DeleteSupplier(DBInfoModel Store, long Id);
    }
}
