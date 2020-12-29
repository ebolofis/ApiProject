using Dapper;
using Newtonsoft.Json;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
using Symposium.Models.Interfaces;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs;
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
    /// Class that cashes the list of objects (INoSql), and responsible for CRUD operations
    /// </summary>
    public class CashedDT<T,DTO>: ICashedDT<T, DTO> where T : IGuidModel where DTO : INoSql,new()
    {
        CashManager<T> CashManager;
        IUsersToDatabasesXML users;

        string connectionString;

        public CashedDT(CashManager<T> cashManager, IUsersToDatabasesXML users)
        {
            this.CashManager = cashManager;
            this.users = users;
        }

        /// <summary>
        /// Select data from cash or db
        /// </summary>
        /// <param name="Store">DBInfoModel</param>
        /// <returns>the list of Data (the entire table)</returns>
        public List<T> Select(DBInfoModel Store)
        {
            getListFromDB(Store);
            return CashManager.GetList(Store.Id);
        }

        /// <summary>
        /// Select data from cash or db filtered by a predicate
        /// </summary>
        /// <param name="Store">DBInfoModel</param>
        /// <param name="wherePredicate">where predicate, ex: x=>x.Eneble==true</param>
        /// <returns>the list of Data </returns>
        public List<T> Select(DBInfoModel Store, Func<T, bool> wherePredicate)
        {
            getListFromDB(Store);
            return CashManager.GetList(Store.Id, wherePredicate);
        }

        /// <summary>
        /// Insert a new item to DB and into Cash
        /// </summary>
        /// <param name="Store">DBInfoModel</param>
        /// <param name="Model">the model to insert</param>
        /// <returns>the new Id (Guid)</returns>
        public Guid Insert(DBInfoModel Store, T Model)
        {
            //1. set cash
            getListFromDB(Store);

            //2. create DTO object
            Model.Id =  Guid.NewGuid();
            DTO dto = new DTO();
            dto.Id = Model.Id;
            dto.Model = JsonConvert.SerializeObject(Model);

            //3. insert into DB
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Insert<Guid>(dto);
            }

            //4. insert into cash
            CashManager.AddItem(Store.Id, Model);

            return Model.Id;
        }

        /// <summary>
        /// Update a new item to DB and into Cash
        /// </summary>
        /// <param name="Store">DBInfoModel</param>
        /// <param name="Model">the model to insert</param>
        /// <returns>the number of lines affected</returns>
        public int Update(DBInfoModel Store, T Model)
        {
            //1. set cash
            getListFromDB(Store);

            //2. create DTO object
            DTO dto = new DTO();
            dto.Id = Model.Id;
            dto.Model = JsonConvert.SerializeObject(Model);

            //3. Update into DB
            int c = 0;
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
               c= db.Update(dto);
            }

            //4. Update into cash
           if(c>0) CashManager.UpdateItem(Store.Id, Model);

            return c;
        }

        /// <summary>
        /// Update a new item to DB and into Cash
        /// </summary>
        /// <param name="Store">DBInfoModel</param>
        /// <param name="Model">the model to insert</param>
        /// <returns>the number of lines affected</returns>
        public int Delete(DBInfoModel Store, Guid Id)
        {
            //1. set cash
            getListFromDB(Store);

            //2. Delete from DB
            int c = 0;
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                c = db.Delete<DTO>(Id);
            }

            //3. Delete from cash
            if (c > 0)
                CashManager.DeleteItem(Store.Id, Id);

            return c;
        }

       



        /// <summary>
        /// Get a list of objects from Db and insert them into Cash
        /// </summary>
        /// <param name="Store"></param>
        private void getListFromDB(DBInfoModel Store)
        {
            if (CashManager.ListExists(Store.Id)) return;

            lock (CashManager)
            {
                //1. get from DB
                connectionString = users.ConfigureConnectionString(Store);
                List<DTO> DbList;
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    DbList = db.GetList<DTO>().ToList();
                }
                if (DbList == null) DbList = new List<DTO>();

                //2. insert  into Cash
                List<T> list = new List<T>();
                foreach (string json in DbList.Select(x => x.Model))
                {
                    try
                    {
                        list.Add(JsonConvert.DeserializeObject<T>(json));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error Deserializing Model : " + json, ex);
                    }
                }
                CashManager.AddNewList(Store.Id, list);
            }
           
          
        }


    }
}
