using Symposium.Models.Models;
using Symposium.Models.Models.Infrastructure;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Infrastructure;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.Infrastructure
{
    public class SuppliersFlows : ISuppliersFlows
    {
        ISuppliersTasks suppliersTasks;

        public SuppliersFlows(ISuppliersTasks _suppliersTasks)
        {
            this.suppliersTasks = _suppliersTasks;
        }

        public List<SupplierModel> GetSuppliers(DBInfoModel Store)
        {
            return suppliersTasks.GetSuppliers(Store);
        }

        public List<SupplierModel> InsertSupplier(DBInfoModel Store, SupplierModel model)
        {
            return suppliersTasks.InsertSupplier(Store, model);
        }

        public List<SupplierModel> UpdateSupplier(DBInfoModel Store, SupplierModel model)
        {
            return suppliersTasks.UpdateSupplier(Store, model);
        }

        public List<SupplierModel> DeleteSupplier(DBInfoModel Store, long Id)
        {
            return suppliersTasks.DeleteSupplier(Store, Id);
        }

    }
}
