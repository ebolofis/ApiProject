using Symposium.WebApi.DataAccess.Interfaces.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CsvHelper;
using System.IO;
using Symposium.Models.Models;

namespace Symposium_Test.MockupObjs
{
    public class GenericDAOMock<T> : IGenericDAO<T> where T : class
    {
        public int Delete(IDbConnection db, T item)
        {
            throw new NotImplementedException();
        }

        public int Delete(IDbConnection db, long Id)
        {
            throw new NotImplementedException();
        }

        public int DeleteList(IDbConnection db, string whereConditions, object whereParameters = null)
        {
            throw new NotImplementedException();
        }

        public long GetMaxId(IDbConnection db, string table)
        {
            throw new NotImplementedException();
        }

        public List<T> GetPage(IDbConnection db, int pageNumber, int rowsPerPage, string conditions, string orderBy, object parameters = null)
        {
            throw new NotImplementedException();
        }

        public long Insert(IDbConnection db, T item)
        {
            throw new NotImplementedException();
        }

        public int RecordCount(IDbConnection db, string whereConditions, object whereParameters = null)
        {
            throw new NotImplementedException();
        }

        public List<T> Select(IDbConnection db)
        {
            throw new NotImplementedException();
        }

        public List<T> Select(IDbConnection db, string table)
        {
            throw new NotImplementedException();
        }

        public T Select(IDbConnection db, long Id)
        {
            throw new NotImplementedException();
        }

        public T Select(IDbConnection db, int Id)
        {
            throw new NotImplementedException();
        }

        public List<T> Select(string table, IDbConnection db)
        {
            throw new NotImplementedException();

        }

        public List<T> Select(string sql, object whereParameters, IDbConnection db)
        {
            throw new NotImplementedException();
        }

        public List<T> Select(IDbConnection db, string whereConditions, object whereParameters = null)
        {
            
            Type typeParameterType = typeof(T);
            string filePath = "" ;

            switch (typeParameterType.Name) {
                case "AccountDTO":
                    filePath = @"\MockupData\Accounts.txt";
                    break;
                case "ActionDTO":
                    filePath = @"\MockupData\Ftiakse to arxeio kai bale to path edo";
                    break;
            }

            CsvReader csv = null;
            using (TextReader reader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + filePath))
            {
                csv = new CsvReader(reader);
                csv.Configuration.Delimiter = ";";
                return csv.GetRecords<T>().ToList<T>();
            }
        }

        public Tuple<List<T>, List<U>> Select2Queries<U>(IDbConnection db, string sql1, string sql2, object parameters = null) where U : class
        {
            throw new NotImplementedException();
        }

        public Tuple<List<T>, List<U>, List<W>> Select3Queries<U, W>(IDbConnection db, string sql1, string sql2, string sql3, object parameters = null)
            where U : class
            where W : class
        {
            throw new NotImplementedException();
        }
        public Tuple<List<T>, List<U>, List<W>, List<Z>> Select4Queries<U, W, Z>(IDbConnection db, string sql1, string sql2, string sql3, string sql4, object parameters = null)
    where U : class
    where W : class
    where Z : class
        {
            throw new NotImplementedException();
        }

        public T SelectFirst(IDbConnection db, string whereConditions, object whereParameters)
        {
            throw new NotImplementedException();
        }

        public T SelectFirst(string sql, object whereParameters, IDbConnection db)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Select the first object using where statetment and db Transaction
        /// </summary>
        /// <param name="db"></param>
        /// <param name="whereConditions"></param>
        /// <param name="whereParameters"></param>
        /// <param name="dbTransact"></param>
        /// <returns></returns>
        public T SelectFirst(IDbConnection db, string whereConditions, object whereParameters, IDbTransaction dbTransact)
        {
            throw new Exception();
        }

        public int Update(IDbConnection db, T item)
        {
            throw new NotImplementedException();
        }

        public int UpdateList(IDbConnection db, List<T> list)
        {
            throw new NotImplementedException();
        }

        public int Execute(IDbConnection db, string sql, object parameters = null)
        {
            throw new NotImplementedException();
        }

        public PaginationModel<T> GetPaginationResult(IDbConnection db, int pageNumber, int rowsPerPage, string conditions, string orderBy, object parameters = null)
        {
            throw new NotImplementedException();
        }
        public PaginationModel<T> GetPaginationQueryResult(IDbConnection db, string sqlData, string sWhere, string sqlCount, int pageNumber, int rowsPerPage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Insert a new record to DB. Return the new Id (primary key) and error if cannot insert record
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="item">the item to insert </param>
        /// <param name="db">DB connection</param>
        /// <param name="ErrorMess">Error On Insert</param>
        /// <returns>Return the new Id (primary key)</returns>
        public long Insert(IDbConnection db, T item, out string ErrorMess)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update a record to DB matched by Id. Return the number of raws affected and error if exception throwed
        /// If no row found return 0.
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="item">the item to insert </param>
        /// <param name="db">DB connection</param>
        /// <param name="ErrorMess">Error on update</param>
        /// <returns>Return the number of raws affected</returns>
        public int Update(IDbConnection db, T item, out string ErrorMess)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Upsert a list of T model
        /// </summary>
        /// <param name="db"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public UpsertListResultModel Upsert(IDbConnection db, List<T> list)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return's A record of the table with specific DAId
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dAId"></param>
        /// <returns></returns>
        public T GetByDAId(IDbConnection db, long dAId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete's or set field IsDeleted to true from a list of object T
        /// </summary>
        /// <param name="db"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public UpsertListResultModel DeleteOrSetIsDeletedList(IDbConnection db, List<T> list)
        {
            throw new Exception();
        }

        /// <summary>
        /// Return Table Name based DTO's attribute TableAttribute or DTO's class name
        /// </summary>
        /// <returns></returns>
        public string getTableName()
        {
            throw new Exception();
        }

        public U Insert<U>(IDbConnection db, T item)
        {
            throw new NotImplementedException();
        }

        public int Delete(IDbConnection db, string Id)
        {
            throw new NotImplementedException();
        }

        public long Upsert(IDbConnection db, T item, out string ErrorMess)
        {
            throw new NotImplementedException();
        }

        public T Select(IDbConnection db, long Id, IDbTransaction dbTransact)
        {
            throw new NotImplementedException();
        }

        public long Insert(IDbConnection db, T item, IDbTransaction dbTransact, out string ErrorMess)
        {
            throw new NotImplementedException();
        }

        public int Update(IDbConnection db, T item, IDbTransaction dbTrsanct, out string ErrorMess)
        {
            throw new NotImplementedException();
        }

        public long Upsert(IDbConnection db, T item, IDbTransaction dbTransact, out string ErrorMess)
        {
            throw new NotImplementedException();
        }

        public int Delete(IDbConnection db, IDbTransaction dbTransaction, T item)
        {
            throw new NotImplementedException();
        }

        public List<T> Select(IDbConnection db, string whereConditions, object whereParameters, IDbTransaction dbTransact)
        {
            throw new NotImplementedException();
        }
    }
}
