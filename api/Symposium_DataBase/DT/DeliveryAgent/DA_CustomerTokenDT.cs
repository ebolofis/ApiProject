using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT.DeliveryAgent
{
    public class DA_CustomerTokenDT : IDA_CustomerTokenDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<DA_CustomersTokensDTO> daCustomerTokenDao;

        public DA_CustomerTokenDT(IUsersToDatabasesXML usersToDatabases, IGenericDAO<DA_CustomersTokensDTO> daCustomerTokenDao)
        {
            this.usersToDatabases = usersToDatabases;
            this.daCustomerTokenDao = daCustomerTokenDao;
        }

        /// <summary>
        /// Select customer token by customer id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="customerId"></param>
        public DA_CustomerTokenModel SelectCustomerToken(DBInfoModel Store, long customerId)
        {
            DA_CustomersTokensDTO customerToken = new DA_CustomersTokensDTO();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                customerToken = daCustomerTokenDao.SelectFirst(db, "WHERE CustomerId = @customerId", new { customerId = customerId });
            }
            return AutoMapper.Mapper.Map<DA_CustomerTokenModel>(customerToken);
        }

        /// <summary>
        /// Insert customer token
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public long InsertCustomerToken(DBInfoModel Store, DA_CustomerTokenModel model)
        {
            long customerTokenId = 0;
            DA_CustomersTokensDTO customerToken = AutoMapper.Mapper.Map<DA_CustomersTokensDTO>(model);
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                customerTokenId = daCustomerTokenDao.Insert(db, customerToken);
            }
            return customerTokenId;
        }

        /// <summary>
        /// Update customer token
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public long UpdateCustomerToken(DBInfoModel Store, DA_CustomerTokenModel model)
        {
            long customerTokenId = 0;
            DA_CustomersTokensDTO customerToken = AutoMapper.Mapper.Map<DA_CustomersTokensDTO>(model);
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                int rowsAffected = daCustomerTokenDao.Update(db, customerToken);
                customerTokenId = customerToken.Id;
            }
            return customerTokenId;
        }

        /// <summary>
        /// Delete customer token
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public long DeleteCustomerToken(DBInfoModel Store, long id)
        {
            long customerTokenId = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                int rowsAffected = daCustomerTokenDao.Delete(db, id);
                customerTokenId = id;
            }
            return customerTokenId;
        }

    }
}
