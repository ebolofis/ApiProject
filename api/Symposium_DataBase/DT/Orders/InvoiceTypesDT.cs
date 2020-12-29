using Dapper;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT
{
    /// <summary>
    /// Class that handles data related to Invoice Types
    /// </summary>
    public class InvoiceTypesDT : IInvoiceTypesDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<InvoiceTypesDTO> invoiceTypesGenericDao;

        public InvoiceTypesDT(IUsersToDatabasesXML usersToDatabases, IGenericDAO<InvoiceTypesDTO> invoiceTypesGenericDao)
        {
            this.usersToDatabases = usersToDatabases;
            this.invoiceTypesGenericDao = invoiceTypesGenericDao;
        }

        /// <summary>
        /// Returns invoice type with selected Id
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="InvoiceTypeId"> Invoice Type </param>
        /// <returns> Invoice type  model. See: <seealso cref="Symposium.Models.Models.InvoiceTypeModel"/> </returns>
        public InvoiceTypeModel GetSingleInvoiceType(DBInfoModel Store, long InvoiceTypeId)
        {
            InvoiceTypesDTO invoiceType;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                invoiceType = invoiceTypesGenericDao.Select(db, InvoiceTypeId);
            }

            return AutoMapper.Mapper.Map<InvoiceTypeModel>(invoiceType);
        }

        /// <summary>
        /// Get's First InvoiceType Record using Field Type
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public InvoiceTypeModel GetInvoiceTypeByType(DBInfoModel Store, Int16 Type)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string SQL = "SELECT * FROM InvoiceTypes WHERE Type = " + Type.ToString();
                return AutoMapper.Mapper.Map<InvoiceTypeModel>(db.Query<InvoiceTypesDTO>(SQL).FirstOrDefault());
            }
        }

    }
}
