using Symposium.Models.Models;
using Symposium.Models.Models.Infrastructure;
using Symposium.WebApi.DataAccess.Interfaces.DT.Infrastructure;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks.Infrastructure
{
    public class SuppliersTasks : ISuppliersTasks
    {
        ISuppliersDT suppliersDT;

        public SuppliersTasks(ISuppliersDT _suppliersDT)
        {
            this.suppliersDT = _suppliersDT;
        }

        public List<SupplierModel> GetSuppliers(DBInfoModel Store)
        {
            return suppliersDT.GetSuppliers(Store);
        }

        public List<SupplierModel> InsertSupplier(DBInfoModel Store, SupplierModel model)
        {
            List<SupplierModel> suppliersModel = new List<SupplierModel>();
            long res = suppliersDT.InsertSupplier(Store, model);
            suppliersModel = GetSuppliers(Store);

            return suppliersModel;
        }

        public List<SupplierModel> UpdateSupplier(DBInfoModel Store, SupplierModel model)
        {
            List<SupplierModel> suppliersModel = new List<SupplierModel>();
            int res = suppliersDT.UpdateSupplier(Store, model);
            suppliersModel = GetSuppliers(Store);

            return suppliersModel;
        }

        public List<SupplierModel> DeleteSupplier(DBInfoModel Store, long Id)
        {
            List<SupplierModel> suppliersModel = new List<SupplierModel>();
            int res = suppliersDT.DeleteSupplier(Store, Id);
            suppliersModel = GetSuppliers(Store);

            return suppliersModel;
        }

    }
}
