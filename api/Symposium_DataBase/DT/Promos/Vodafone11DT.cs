using Dapper;
using Symposium.Models.Models;
using Symposium.Models.Models.Promos;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT.Promos;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Symposium.WebApi.DataAccess.DT.Promos
{
    public class Vodafone11DT : IVodafone11DT
    {

        IUsersToDatabasesXML usersToDatabases;
        string connectionString;
        IGenericDAO<Vodafone11HeadersDTO> vodafoneHeadersDao;
        IGenericDAO<Vodafone11DetailsDTO> vodafoneDetailsDao;

        public Vodafone11DT(IUsersToDatabasesXML usersToDatabases, IGenericDAO<Vodafone11HeadersDTO> vodafoneHeadersDao, IGenericDAO<Vodafone11DetailsDTO> vodafoneDetailsDao)
        {
            this.usersToDatabases = usersToDatabases;
            this.vodafoneHeadersDao = vodafoneHeadersDao;
            this.vodafoneDetailsDao = vodafoneDetailsDao;
        }

        /// <summary>
        /// Get Vodafone 1+1 promos
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <returns></returns>
        public List<Vodafone11Model> GetVodafone11Promos(DBInfoModel dbInfo)
        {
            string sql = @" 
       select Vodafone11Headers.*,Vodafone11Details.id,Vodafone11Details.Headerid,Vodafone11Details.ProdCategoryId,Vodafone11Details.Position
                 from Vodafone11Headers inner join Vodafone11Details 
                 on Vodafone11Headers.id=Vodafone11Details.HeaderId
				 where  Vodafone11Headers.id=Vodafone11Details.HeaderId
                 order by Vodafone11Headers.id, Vodafone11Details.Position";
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            var lookup = new Dictionary<long, Vodafone11Model>();

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var result = db.Query<Vodafone11Model, Vodafone11ProdCategoriesModel, Vodafone11Model>(// <TFirst, TSecond, TReturn>
                          sql,
                          (header, details) =>
                          {
                              Vodafone11Model prod;
                              if (!lookup.TryGetValue(header.Id, out prod))
                              {
                                  prod = header;
                                  lookup.Add(header.Id, header);
                              }

                              prod.ProductCategories.Add(details);
                              return prod;
                          });
                //return lookup.Select(x => x.Value).ToList<Vodafone11Model>();
                return result.Distinct().ToList<Vodafone11Model>();
            }

        }
        public List<Vodafone11Model> GetVodafone11HeaderPromos(DBInfoModel dbInfo)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            var lookup = new Dictionary<long, Vodafone11Model>();

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Vodafone11Model>("Select * from Vodafone11Headers").ToList();
            }
        }

        public List<Vodafone11ProdCategoriesModel> GetVodafone11DetailsPromos(DBInfoModel dbinfo, long HeaderId)
        {
            List<Vodafone11ProdCategoriesModel> DetailList = new List<Vodafone11ProdCategoriesModel>();
           connectionString = usersToDatabases.ConfigureConnectionString(dbinfo);
            var lookup = new Dictionary<long, Vodafone11ProdCategoriesModel>();

            string SqlDetails = @"select d.*, p.Description as ProductDescr
                                                from Vodafone11Details as d
                                                inner join ProductCategories as p on d.ProdCategoryId = p.Id
                                                where d.HeaderId =@headerId";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                DetailList =  db.Query<Vodafone11ProdCategoriesModel>(SqlDetails, new { headerId = HeaderId }).ToList();
            }
            return DetailList;
        }
        //Get OneOfVodafone11Header+Detail
        public List<Vodafone11ProdCategoriesModel> GetVodafoneAll11DetailsPromos(DBInfoModel dbinfo)
        {
            List<Vodafone11ProdCategoriesModel> DetailList = new List<Vodafone11ProdCategoriesModel>();
            connectionString = usersToDatabases.ConfigureConnectionString(dbinfo);
            var lookup = new Dictionary<long, Vodafone11ProdCategoriesModel>();

            string SqlDetails = @"select * from Vodafone11Details";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                DetailList = db.Query<Vodafone11ProdCategoriesModel>(SqlDetails).ToList();
            }
            return DetailList;
        }
        public Vodafone11Model GetOneOffVodafone11Promos(DBInfoModel dbInfo, long id)
        {
            Vodafone11Model model ;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //1. select header
                Vodafone11HeadersDTO headerDto = vodafoneHeadersDao.SelectFirst(db, "where  id=@tableId", new { tableId = id });
                model = AutoMapper.Mapper.Map<Vodafone11Model>(headerDto);
                //2. select details
               List<Vodafone11DetailsDTO> listDto=  vodafoneDetailsDao.Select(db, "where  HeaderId=@headerId", new { headerId = id });
                model.ProductCategories= AutoMapper.Mapper.Map<List<Vodafone11ProdCategoriesModel>>(listDto);
            }
            return model;

        }

        /// <summary>
        /// Insert Vodafone11 
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="Id">vodafone11Headers  Id</param>
        /// <returns></returns>


        public long InsertVodafone11Promos(DBInfoModel dbInfo, Vodafone11Model model)
        {
            long insertedId = 0;
            Vodafone11HeadersDTO modelDto = AutoMapper.Mapper.Map<Vodafone11HeadersDTO>(model);
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                insertedId = vodafoneHeadersDao.Insert(db, modelDto);
            }
            return insertedId;
        }

        public long InsertVodafone11Details(DBInfoModel dbinfo, Vodafone11ProdCategoriesModel model)
        {
            long insertdetailsid = 0;
            Vodafone11DetailsDTO modelDto = AutoMapper.Mapper.Map<Vodafone11DetailsDTO>(model);
            connectionString = usersToDatabases.ConfigureConnectionString(dbinfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                insertdetailsid = vodafoneDetailsDao.Insert(db, modelDto);
            }
            return insertdetailsid;
        }


        public long DeleteVodafone11(DBInfoModel dbInfo, long id)
        {
            long deletedId = id;
           
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                vodafoneHeadersDao.Delete(db, deletedId);
            }
            return deletedId;

        }


        public long DeleteVodafone11Detail(DBInfoModel dbinfo, long id)
        {
            long deletedId = id;
            connectionString = usersToDatabases.ConfigureConnectionString(dbinfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                vodafoneDetailsDao.Delete(db, deletedId);
            }
            return deletedId;
        }
        public Vodafone11Model UpdateVodafone11Promos(DBInfoModel dbinfo, Vodafone11Model model)
        {
            Vodafone11HeadersDTO modelDto = AutoMapper.Mapper.Map<Vodafone11HeadersDTO>(model);
            connectionString = usersToDatabases.ConfigureConnectionString(dbinfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                vodafoneHeadersDao.Update(db, modelDto);
            }
                return model;
        }
        public Vodafone11ProdCategoriesModel UpdateVodafone11DetailsPromos(DBInfoModel dbinfo, Vodafone11ProdCategoriesModel model)
        {
            Vodafone11DetailsDTO modelDto = AutoMapper.Mapper.Map<Vodafone11DetailsDTO>(model);
            connectionString = usersToDatabases.ConfigureConnectionString(dbinfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                vodafoneDetailsDao.Update(db, modelDto);
            }
            return model;
        } 
    }
}
