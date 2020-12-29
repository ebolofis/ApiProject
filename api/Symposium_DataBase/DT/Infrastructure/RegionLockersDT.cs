using log4net;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.DAOs;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium.WebApi.DataAccess.XMLs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symposium.Helpers.Interfaces;
using Symposium_DTOs.PosModel_Info;
using System.Transactions;

namespace Symposium.WebApi.DataAccess.DT
{
    public class RegionLockersDT : IRegionLockersDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<RegionLockerProductDTO> regionLockerProductGenericDao;


        public RegionLockersDT(IUsersToDatabasesXML usersToDatabases, IGenericDAO<RegionLockerProductDTO> regionLockerProductGenericDao)
        {
            this.usersToDatabases = usersToDatabases;
            this.regionLockerProductGenericDao = regionLockerProductGenericDao;
        }

        /// <summary>
        /// Get RegionLockers Products
        /// </summary>
        /// <param name="Store">StoreId</param>
        public List<RegionLockersModel> GetProducts(DBInfoModel Store)
        {
            List<RegionLockerProductDTO> regionLockerProductDTO = new List<RegionLockerProductDTO>();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    regionLockerProductDTO = regionLockerProductGenericDao.Select("SELECT *from RegionLockerProduct", null, db);
                    if (regionLockerProductDTO.Count > 0)
                        return AutoMapper.Mapper.Map<List<RegionLockersModel>>(regionLockerProductDTO);
                    else
                        return null;
                }
            }
        }
    }
}
