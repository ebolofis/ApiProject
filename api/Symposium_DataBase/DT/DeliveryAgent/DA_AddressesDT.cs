using Dapper;
using log4net;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
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
using System.Transactions;

namespace Symposium.WebApi.DataAccess.DT.DeliveryAgent
{
    public class DA_AddressesDT : IDA_AddressesDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<DA_AddressesDTO> daAddressesDao;
        LocalConfigurationHelper configHlp;
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        PhoneticAbstHelper phoneticsHlp;

        public DA_AddressesDT(IUsersToDatabasesXML usersToDatabases, IGenericDAO<DA_AddressesDTO> daAddressesDao, LocalConfigurationHelper configHlp, PhoneticAbstHelper phoneticsHlp)
        {
            this.usersToDatabases = usersToDatabases;
            this.daAddressesDao = daAddressesDao;
            this.configHlp = configHlp;
            this.phoneticsHlp = phoneticsHlp;
        }

        /// <summary>
        /// Add new Address 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public long AddAddress(DBInfoModel Store, DA_AddressModel Model)
        {
            configHlp.CheckDeliveryAgent();
            long addressId = 0;
          //   string geosql = "UPDATE DA_Addresses SET geographyColumn = geography::Point(ISNULL(Latitude,0),ISNULL(Longtitude,0),4326) WHERE id=@id";
            DA_AddressesDTO dto = AutoMapper.Mapper.Map<DA_AddressesDTO>(Model);
            AddPhonetics(dto);
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //using (var scope = new TransactionScope())
                //{
                    addressId = daAddressesDao.Insert(db, dto);
                   // daAddressesDao.Execute(db, geosql, new { id = addressId });
                //    scope.Complete();
                //}
            }
            return addressId;
        }

      

        /// <summary>
        /// Get All Active addresses (billing & shipping) for a Customer
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="Id">Customer Id</param>
        /// <returns></returns>
        public List<DA_AddressModel> getCustomerAddresses(DBInfoModel Store, long Id)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            List<DA_AddressesDTO> dto = null;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                dto = daAddressesDao.Select(db, "where OwnerId=@Id and AddressType<>2 and IsDeleted=0 ", new { Id= Id });
            }
           return AutoMapper.Mapper.Map<List<DA_AddressModel>>(dto);
        }

        /// <summary>
        /// return an address based on Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id">Address Id</param>
        /// <returns></returns>
        public DA_AddressModel getAddress(DBInfoModel Store, long Id)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            DA_AddressesDTO dto = null;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                dto = daAddressesDao.SelectFirst(db, "where Id=@Id", new { Id = Id });
            }
            if (dto == null) return null;
            return AutoMapper.Mapper.Map<DA_AddressModel>(dto);
        }


        /// <summary>
        /// from customer's addresses return the one that is far from a certain point less than x meters.
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="Id">Customer Id</param>
        /// <param name="Latitude">certain point's Latitude</param>
        /// <param name="Longtitude">certain point's Longitude</param>
        /// <param name="Distance">the maximum distance in meters (default value 32m)</param>
        /// <returns></returns>
        public DA_AddressModel proximityCustomerAddress(DBInfoModel Store, long Id, float Latitude,float Longtitude, int Distance=32)
        {
            string sql = $@"
                DECLARE @g geography;
                SET @g = geography::STPointFromText('POINT({Longtitude.ToString().Replace(",", ".")} {Latitude.ToString().Replace(",", ".")})', 4326);
                SELECT * FROM DA_Addresses 
                   where 
                      OwnerId={Id} and 
                      AddressType<>2 and 
                      IsDeleted=0 and  
                      geography::STGeomFromText('POINT('+convert(varchar(20),[Longtitude])+' '+convert(varchar(20),[Latitude])+')',4326).STDistance (@g)<{Distance}";
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            DA_AddressesDTO dto = null;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                dto = daAddressesDao.SelectFirst(sql,null,db);
            }
            return AutoMapper.Mapper.Map<DA_AddressModel>(dto);
        }

        /// <summary>
        /// Update an Address. Return rows affected
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public long UpdateAddress(DBInfoModel Store, DA_AddressModel Model)
        {
            configHlp.CheckDeliveryAgent();
            long rowsAffected = 0;
            long addressId = Model.Id;
            // string geosql = "UPDATE DA_Addresses SET geographyColumn = geography::Point(ISNULL(Latitude,0),ISNULL(Longtitude,0),4326) WHERE id=@id";

            DA_AddressesDTO dto = AutoMapper.Mapper.Map<DA_AddressesDTO>(Model);
            AddPhonetics(dto);
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //using (var scope = new TransactionScope())
                //{
                rowsAffected = daAddressesDao.Update(db, dto); 
                 //   daAddressesDao.Execute(db, geosql, new { id = Model.Id });
                //    scope.Complete();
                //}
                
            }
            return addressId;
        }

        /// <summary>
        /// Delete Address OR set the IsDeleted = 1. If address is deleted then return 1, if set IsDeleted = 1 then return 0
        /// </summary>
        /// <param name="Id">Address id</param>
        /// <returns></returns>
        public long DeleteAddress(DBInfoModel Store, long Id)
        {
            configHlp.CheckDeliveryAgent();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                   return daAddressesDao.Delete(db, Id);
                }
                catch(Exception ex)
                {
                    string addressSQL = @"UPDATE DA_Addresses SET IsDeleted = 1 WHERE Id =@ID";
                    daAddressesDao.Execute(db,addressSQL, new { ID = Id });
                    return 0;
                }              
            }
        }

        public long GetCustomerAddressById(DBInfoModel dbinfo, long Id)
        {
            long addressId = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(dbinfo);
            string sql = "";
            
              sql = @"SELECT Id  FROM DA_Addresses  WHERE OwnerId=" + Id;
            
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var res = db.Query(sql).SingleOrDefault();
                addressId = res.Id;
            }
            return addressId;
        }



        public long GetCustomerAddressByExtId(DBInfoModel dbinfo, string ExtId)
        {
            long addressId = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(dbinfo);
            string sql = "";

           
                sql = @"SELECT id FROM DA_Addresses where ExtId2='" +ExtId +"'";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var res = db.Query(sql).FirstOrDefault();
                addressId = res.id;
            }
            return addressId;
        }

        /// <summary>
        /// Change OwnerId
        /// </summary>
        /// <param name="Id">DA_Addresses Id</param>
        /// <param name="OwnerId">OwnerId</param>
        /// <returns>AddressId</returns>
        public long ChangeOwnerId(DBInfoModel dbinfo, long Id, long OwnerId)
        {
            long addressId = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(dbinfo);
            string sql = "";
            sql = @"UPDATE DA_Addresses SET OwnerId =@ownerID WHERE Id =@AddressId";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    addressId = db.Execute(sql, new { ownerID = OwnerId, AddressId = Id });
                    logger.Info($"UPDATE Anonymous Customer ADDRESS.  OwnerId for AddressId: {Id.ToString()}, OwnerId: {OwnerId.ToString()}");
                }
                catch (Exception ex)
                {
                    logger.Error("ERROR UPDATE OwnerId in DA_Addresses: " + ex.ToString());
                }
            }
            return addressId;
        }


        /// <summary>
        /// Add phonetics to  a DA_AddressesDTO
        /// </summary>
        /// <param name="dto"></param>
        private void AddPhonetics(DA_AddressesDTO dto)
        {
            dto.Phonetics = phoneticsHlp.CreatePhonetics(dto.AddressStreet, dto.AddressNo, dto.VerticalStreet);
            dto.PhoneticsArea = phoneticsHlp.CreatePhonetics(dto.Area, "");
        }
    }
}
