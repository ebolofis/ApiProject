using Symposium.Models.Models;
using Symposium.Models.Models.Infrastructure;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT.Infrastructure;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT.Infrastructure
{
    public class SuppliersDT : ISuppliersDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<SuppliersDTO> suppliersGenericDao;

        public SuppliersDT(IUsersToDatabasesXML _usersToDatabases, IGenericDAO<SuppliersDTO> _suppliersGenericDao)
        {
            this.usersToDatabases = _usersToDatabases;
            this.suppliersGenericDao = _suppliersGenericDao;
        }

        public List<SupplierModel> GetSuppliers(DBInfoModel Store)
        {
            List<SuppliersDTO> suppliers;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                suppliers = suppliersGenericDao.Select(db);
            }
            return AutoMapper.Mapper.Map<List<SupplierModel>>(suppliers);
        }

        public long InsertSupplier(DBInfoModel Store, SupplierModel model)
        {
            long res = 0;
            SuppliersDTO supplier = AutoMapper.Mapper.Map<SuppliersDTO>(model);
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                res = suppliersGenericDao.Insert(db, supplier);
            }
            return res;
        }

        public int UpdateSupplier(DBInfoModel Store, SupplierModel model)
        {
            int res = 0;
            SuppliersDTO supplier = AutoMapper.Mapper.Map<SuppliersDTO>(model);
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                res = suppliersGenericDao.Update(db, supplier);
            }
            return res;
        }

        public int DeleteSupplier(DBInfoModel Store, long Id)
        {
            int res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                res = suppliersGenericDao.Delete(db, Id);
            }
            return res;
        }

    }
}
