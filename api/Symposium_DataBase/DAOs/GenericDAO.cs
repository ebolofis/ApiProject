using Dapper;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium_DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DAOs
{

    /// <summary>
    /// Generic DAO class for simple CRUD operations
    /// </summary>
    /// <typeparam name="T">DTO classes ONLY</typeparam>
    public class GenericDAO<T> : IGenericDAO<T> where T : class
    {
        /// <summary>
        /// Select table's Data from the Source DB
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="table">the table to select from. Table must be the name of T class</param>
        /// /// <param name="db">DB connection</param>
        /// <returns>the list of Data (the entire table)</returns>
        public List<T> Select(IDbConnection db, string table)
        {
            string select = "Select * From [" + table + "]";
            return db.Query<T>(select).ToList();
        }

        /// <summary>
        /// Select table's Data from the Source DB
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="sql">the sql select statement. ex: "Select * From Author WHERE  age = @Age or Name like @Name"</param>
        /// <param name="whereParameters">where parameters (optional). Parameters must match whereConditions. ex: new {Age = 10, Name = "Tom%"}</param>
        /// <param name="db">DB connection</param>
        /// <returns>the list of Data</returns>
        public List<T> Select(string sql, object whereParameters, IDbConnection db)
        {
            return db.Query<T>(sql, whereParameters).ToList();
        }

        /// <summary>
        /// Select the First Object from the Source DB
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="sql">the sql select statement. ex: "Select * From Author WHERE  age = @Age or Name like @Name"</param>
        /// <param name="whereParameters">where parameters (optional). Parameters must match whereConditions. ex: new {Age = 10, Name = "Tom%"}</param>
        /// <param name="db">DB connection</param>
        /// <returns>an Object or null</returns>
        public T SelectFirst(string sql, object whereParameters, IDbConnection db)
        {
            return db.Query<T>(sql, whereParameters).FirstOrDefault<T>();
        }

        /// <summary>
        /// Select one object based on id (long)
        /// </summary>
        /// <param name="db">DB connection</param>
        /// <param name="Id">long Id, DB's key</param>
        /// <param name="dbTransact"></param>
        /// <returns></returns>
        public T Select(IDbConnection db, long Id, IDbTransaction dbTransact)
        {
            return db.Get<T>(Id, dbTransact);
        }

        /// <summary>
        /// Select one object based on id (long)
        /// </summary>
        /// <param name="db">DB connection</param>
        /// <param name="Id">long Id, DB's key</param>
        /// <returns></returns>
        public T Select(IDbConnection db, long Id)
        {
            return db.Get<T>(Id);
        }

        /// <summary>
        /// Select one object based on id (int)
        /// </summary>
        /// <param name="db">DB connection</param>
        /// <param name="Id">int Id, DB's key</param>
        /// <returns></returns>
        public T Select(IDbConnection db, int Id)
        {
            return db.Get<T>(Id);
        }


        /// <summary>
        /// Select table's Data from the Source DB
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="db">DB connection</param>
        /// <returns>the list of Data (the entire table)</returns>
        public List<T> Select(IDbConnection db)
        {
            return db.GetList<T>().ToList();
        }

        /// <summary>
        /// Select a list of T using Transaction
        /// </summary>
        /// <param name="db"></param>
        /// <param name="whereConditions"></param>
        /// <param name="whereParameters"></param>
        /// <param name="dbTransact"></param>
        /// <returns></returns>
        public List<T> Select(IDbConnection db, string whereConditions, object whereParameters, IDbTransaction dbTransact)
        {
            return db.GetList<T>(whereConditions, whereParameters, dbTransact).ToList();
        }

        /// <summary>
        /// Select table's Data using where statetment
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="whereConditions">where conditions. ex: "where age = @Age or Name like @Name"</param>
        /// <param name="whereParameters">where parameters (optional). Parameters must match whereConditions. ex: new {Age = 10, Name = "Tom%"}</param>
        /// <param name="db">DB connection</param>
        /// <returns>the list of Data (the entire table)</returns>
        public List<T> Select(IDbConnection db, string whereConditions, object whereParameters)
        {
            return db.GetList<T>(whereConditions, whereParameters).ToList();
        }

        /// <summary>
        /// Select the first object using where statetment
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="whereConditions">where conditions. ex: "where age = @Age or Name like @Name"</param>
        /// <param name="whereParameters">where parameters (optional). Parameters must match whereConditions. ex: new {Age = 10, Name = "Tom%"}</param>
        /// <param name="db">DB connection</param>
        /// <returns>the first object or null</returns>
        public T SelectFirst(IDbConnection db, string whereConditions, object whereParameters)
        {
            return db.GetList<T>(whereConditions, whereParameters).FirstOrDefault<T>();
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
            return db.GetList<T>(whereConditions, whereParameters, dbTransact).FirstOrDefault<T>();
        }

        

        /// <summary>
        /// Return's A record of the table with specific DAId
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dAId"></param>
        /// <returns></returns>
        public T GetByDAId(IDbConnection db, long dAId)
        {
            return Select(db, "WHERE ISNULL(DAId,0) = @DAId", new { DAId = dAId }).FirstOrDefault();
        }


        /// <summary>
        /// Insert a new record to DB using Transaction Scope. Return the new Id (primary key)
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="item">the item to insert </param>
        /// <param name="db">DB connection</param>
        /// <param name="dbTransact"></param>
        /// <param name="ErrorMess"></param>
        /// <returns>Return the new Id (primary key)</returns>
        public long Insert(IDbConnection db, T item, IDbTransaction dbTransact, out string ErrorMess)
        {
            ErrorMess = "";
            try
            {
                return db.Insert<long>(item, dbTransact);
            }
            catch (Exception ex)
            {
                ErrorMess = ex.ToString();
                return -1;
            }
        }


        /// <summary>
        /// Insert a new record to DB. Return the new Id (primary key)
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="item">the item to insert </param>
        /// <param name="db">DB connection</param>
        /// <returns>Return the new Id (primary key)</returns>
        public long Insert(IDbConnection db, T item)
        {
            return db.Insert<long>(item);
        }

        /// <summary>
        /// Insert a new record to DB. Return the new Id (primary key)
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        ///  <typeparam name="U">the type of  the new Id (primary key)</typeparam>
        /// <param name="item">the item to insert </param>
        /// <param name="db">DB connection</param>
        /// <returns>Return the new Id (primary key)</returns>
        public U Insert<U>(IDbConnection db, T item)
        {
            return db.Insert<U>(item);
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
            ErrorMess = "";
            try
            {
                return db.Insert<long>(item);
            }
            catch (Exception ex)
            {
                ErrorMess = ex.ToString();
                return -1;
            }
        }

        /// <summary>
        /// Update a record to DB matched by Id. Return the number of raws affected.
        /// If no row found return 0.
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="item">the item to insert </param>
        /// <param name="db">DB connection</param>
        /// <param name="dbTrsanct"></param>
        /// <param name="ErrorMess"></param>
        /// <returns>Return the number of raws affected</returns>
        public int Update(IDbConnection db, T item, IDbTransaction dbTrsanct, out string ErrorMess)
        {
            ErrorMess = "";
            try
            {
                return db.Update(item, dbTrsanct);
            }
            catch (Exception ex)
            {
                ErrorMess = ex.ToString();
                return -1;
            }
        }

        /// <summary>
        /// Update a record to DB matched by Id. Return the number of raws affected.
        /// If no row found return 0.
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="item">the item to insert </param>
        /// <param name="db">DB connection</param>
        /// <returns>Return the number of raws affected</returns>
        public int Update(IDbConnection db, T item)
        {
            return db.Update(item);
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
            ErrorMess = "";
            try
            {
                return db.Update(item);
            }
            catch (Exception ex)
            {
                ErrorMess = ex.ToString();
                return 0;
            }
        }


        /// <summary>
        /// Upsert an Object T
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <param name="dbTransact"></param>
        /// <param name="ErrorMess"></param>
        /// <returns></returns>
        public long Upsert(IDbConnection db, T item, IDbTransaction dbTransact, out string ErrorMess)
        {
            ErrorMess = "";
            try
            {
                long ID = 0;
                if (item.GetType().GetProperty("Id") != null)
                    ID = long.Parse(item.GetType().GetProperty("Id").GetValue(item, null).ToString());
                else
                    ID = 0;
                T tmp = Select(db, ID, dbTransact);
                if (tmp == null)
                    return Insert(db, item, dbTransact, out ErrorMess);
                else
                {
                    int tmpId = Update(db, item, dbTransact, out ErrorMess);
                    if (tmpId < 1)
                        return tmpId;
                    else
                        return ID;
                }
            }
            catch (Exception ex)
            {
                ErrorMess = ex.ToString();
                return 0;
            }
        }

        /// <summary>
        /// Upsert an Object T
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <param name="ErrorMess"></param>
        /// <returns></returns>
        public long Upsert(IDbConnection db, T item, out string ErrorMess)
        {
            ErrorMess = "";
            try
            {
                long ID = 0;
                if (item.GetType().GetProperty("Id") != null)
                    ID = long.Parse(item.GetType().GetProperty("Id").GetValue(item, null).ToString());
                else
                    ID = 0;
                T tmp = Select(db, ID);
                if (tmp == null)
                    return Insert(db, item);
                else
                {
                    db.Update(item);
                    return ID;
                }
                    
            }
            catch (Exception ex)
            {
                ErrorMess = ex.ToString();
                return 0;
            }
        }


        /// <summary>
        /// Update a  list of DTO objects (T) to DB matched by Id. Return the number of raws affected
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="list">the list of items to insert </param>
        /// <param name="db">DB connection</param>
        /// <returns>Return the number of raws affected</returns>
        public int UpdateList(IDbConnection db, List<T> list)
        {
            int rowsAffected = 0;
            foreach (T item in list)
            {
                rowsAffected = rowsAffected + db.Update(item);
            }
            return rowsAffected;
        }

        /// <summary>
        /// Execute a parameterized query (insert or update)
        /// </summary>
        /// <param name="db">DB connection</param>
        /// <param name="sql">Insert or Update query.  ex: "UPDATE Author SET FirstName = @FirstName, LastName = @LastName " + "WHERE Id = @Id";</param>
        /// <param name="parameters">parameters (optional). Parameters must match query parameters. ex: new {FirstName="Smith",Lastname="Tom", Id=30 } </param>
        /// <returns>the number of affected rows</returns>
        public int Execute(IDbConnection db, string sql, object parameters = null)
        {
            return db.Execute(sql, parameters);
        }

        /// <summary>
        /// Delete a record from DB matched by Id. Return the number of raws affected
        /// </summary>
        /// <param name="Id">the Id to delete</param>
        /// <param name="db">DB connection</param>
        /// <returns>Return the number of raws affected</returns>
        public int Delete(IDbConnection db, long Id)
        {
            return db.Delete<T>(Id);
        }

        /// <summary>
        /// Delete a record from DB matched by Id. Return the number of raws affected
        /// </summary>
        /// <param name="Id">the Id to delete</param>
        /// <param name="db">DB connection</param>
        /// <returns>Return the number of raws affected</returns>
        public int Delete(IDbConnection db, string Id)
        {
            return db.Delete<T>(Id);
        }

        /// <summary>
        /// Delete a record from DB matched by Id. Return the number of raws affected
        /// </summary>
        /// <param name="item">the item to delete (only Id is required) </param>
        /// <param name="db">DB connection</param>
        /// <returns>Return the number of raws affected</returns>
        public int Delete(IDbConnection db, T item)
        {
            return db.Delete(item);
        }

        /// <summary>
        /// Delete a record from DB matched by Id using a transaction. Return the number of raws affected
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dbTransaction"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int Delete(IDbConnection db, IDbTransaction dbTransaction, T item)
        {
            return db.Delete(item, dbTransaction);
        }

        /// <summary>
        /// Delete multiple records with where clause
        /// </summary>
        /// <param name="db">DB connection</param>
        /// <param name="whereConditions">where conditions. ex: "where age = @Age or Name like @Name"</param>
        /// <param name="whereConditions">where parameters (optional). Parameters must match whereConditions. ex: new {Age = 10, Name = "Tom%"}</param>
        /// <returns>Return the number of raws affected</returns>
        public int DeleteList(IDbConnection db, string whereConditions, object whereParameters = null)
        {
            return db.DeleteList<T>(whereConditions, whereParameters);
        }

        /// <summary>
        /// Get count of records 
        /// </summary>
        /// <param name="db">DB connection</param>
        /// <param name="whereConditions">where conditions. ex: "where age = @Age or Name like @Name"</param>
        /// <param name="whereConditions">where parameters (optional). Parameters must match whereConditions. ex: new {Age = 10, Name = "Tom%"}</param>
        /// <returns>Return the number of raws affected</returns>
        public int RecordCount(IDbConnection db, string whereConditions, object whereParameters = null)
        {
            return db.RecordCount<T>(whereConditions, whereParameters);
        }

        /// <summary>
        /// Pagenation
        /// </summary>
        /// <param name="db">DB connection</param>
        /// <param name="pageNumber">page Number</param>
        /// <param name="rowsPerPage">rows Per Page</param>
        /// <param name="conditions">where conditions. ex: "where age = @Age or Name like @Name"</param>
        /// <param name="orderBy">order by conditions ex: "Description desc"</param>
        /// <param name="parameters">where parameters (optional). Parameters must match whereConditions. ex: new {Age = 10, Name = "Tom%"}</param>
        /// <returns>the list of items for the page</returns>
        public List<T> GetPage(IDbConnection db, int pageNumber, int rowsPerPage, string conditions, string orderBy, object parameters = null)
        {
            return db.GetListPaged<T>(pageNumber, rowsPerPage, conditions, orderBy, parameters).ToList<T>();
        }

        /// <summary>
        /// Pagenation
        /// </summary>
        /// <param name="db">DB connection</param>
        /// <param name="pageNumber">page Number</param>
        /// <param name="rowsPerPage">rows Per Page</param>
        /// <param name="conditions">where conditions. ex: "where age = @Age or Name like @Name"</param>
        /// <param name="orderBy">order by conditions ex: "Description desc"</param>
        /// <param name="parameters">where parameters (optional). Parameters must match whereConditions. ex: new {Age = 10, Name = "Tom%"}</param>
        /// <returns>the list of items for the page</returns>
        public virtual PaginationModel<T> GetPaginationResult(IDbConnection db, int pageNumber, int rowsPerPage, string conditions, string orderBy, object parameters = null)
        {
            PaginationModel<T> pagination = new PaginationModel<T>();
            if (pageNumber > 0)
            {
                pagination.PageList = db.GetListPaged<T>(pageNumber, rowsPerPage, conditions, orderBy, parameters).ToList<T>();
                pagination.CurrentPage = pageNumber;
                pagination.ItemsCount = RecordCount(db, conditions, parameters);
                pagination.PageLength = pagination.PageList.Count();
                pagination.PagesCount = getNumberOfPages(pagination.ItemsCount, rowsPerPage);
            }
            else
            {
                pagination.PageList = db.GetList<T>(parameters).ToList();
                pagination.ItemsCount = pagination.PageList.Count;
                pagination.CurrentPage = 0;
                pagination.PageLength = pagination.ItemsCount;
                pagination.PagesCount = 0;
            }
            return pagination;
        }
        /// <summary>
        /// Custom Pagination. Return a PaginationModel where T is a Model (typically no DTO). 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sqlData">sql query returning data. For paging it requires parameters: @StartRow and @EndRow</param>
        /// <param name="sWhere">Where for SQLData statment</param>
        /// <param name="sqlCount">sql query returning the total num of records (return one int ONLY). Required inly if pageNumber > 0</param>
        /// <param name="pageNumber">page Number</param>
        /// <param name="rowsPerPage">rows Per Page</param>
        /// <example>
        ///      string sqlData = @"SELECT *
        ///            FROM(
        ///             SELECT ROW_NUMBER() OVER(ORDER BY hc.Id) rowId, hc.*, 
        ///
        ///                    ISNULL(hr.Descr,'') Region,  ISNULL(h.Descr,'') City, ISNULL(hd.Descr,'') District, ISNULL(hg.Descr,'') GeographicalAreaCode, ISNULL(hcc.Descr,'') Country
        ///                     FROM HESPostCode AS hc
        ///                     LEFT OUTER JOIN HESRegion AS hr ON hr.Id = hc.RegionId
        ///                     LEFT OUTER JOIN HESCity AS h ON h.Id = hc.CityId
        ///                     LEFT OUTER JOIN HESDistrict AS hd ON hd.Id = hc.DistrictId
        ///                     LEFT OUTER JOIN HESGeographicArea AS hg ON hg.Id = hc.GeographicalAreaCodeId
        ///                     LEFT OUTER JOIN HESCountry AS hcc ON hcc.Id = hc.CountryId
        ///                     ) fin";
        ///         string sWhere = "WHERE fin.rowId BETWEEN @StartRow AND @EndRow";
        ///
        ///      string sqlCount = @"SELECT COUNT(*)  FROM HESPostCode AS hc";
        /// 
        /// </example>
        /// <returns>a List of Models (typically no DTO)</returns>
        public virtual PaginationModel<T> GetPaginationQueryResult(IDbConnection db, string sqlData, string sWhere, string sqlCount, int pageNumber, int rowsPerPage)
        {
            PaginationModel<T> pagination = new PaginationModel<T>();
            if (pageNumber > 0)
            {
                using (var multipleresult = db.QueryMultiple(sqlData + " " + sWhere + ";" + sqlCount, new { StartRow = ((pageNumber - 1) * rowsPerPage) + 1, EndRow = ((pageNumber - 1) * rowsPerPage) + rowsPerPage }))
                {
                    pagination.PageList = multipleresult.Read<T>().ToList();
                    pagination.ItemsCount = multipleresult.Read<int>().FirstOrDefault<int>();
                }

                pagination.CurrentPage = pageNumber;
                pagination.PageLength = pagination.PageList.Count();
                pagination.PagesCount = getNumberOfPages(pagination.ItemsCount, rowsPerPage);
            }
            else
            {
                pagination.PageList = Select(sqlData, null, db);
                pagination.ItemsCount = pagination.PageList.Count;
                pagination.CurrentPage = 0;
                pagination.PageLength = pagination.ItemsCount;
                pagination.PagesCount = 0;
            }
            return pagination;
        }
        /// <summary>
        /// On pagination, return the number of pages
        /// </summary>
        /// <param name="totalCount">the total number oc rows</param>
        /// <param name="pageSize">the page size</param>
        /// <returns></returns>
        protected int getNumberOfPages(int totalCount, int pageSize)
        {
            int pageNum = 0;
            if (pageSize > 0)
            {
                pageNum = totalCount / pageSize;
                if (totalCount % pageSize > 0) pageNum++;
            }
            return pageNum;
        }


        /// <summary>
        /// Return 2 select statetments unrelated each other. Return a tuple object
        /// </summary>
        /// <typeparam name="U">DTO or Model</typeparam>
        /// <param name="db">DB connection</param>
        /// <param name="sql1">1st select statetment. Results returned as List<T> into the Tuple.  ex: Select * From Product </param>
        /// <param name="sql2">2nd select statetment. Results returned as List<U> into the Tuple.  ex: select * from Actions</param>
        /// <param name="parameters">where parameters (optional). Parameters must match whereConditions. ex: new {Age = 10, Name = "Tom%"}</param>
        /// <returns>Return a tuple object</returns>
        public Tuple<List<T>, List<U>> Select2Queries<U>(IDbConnection db, string sql1, string sql2, object parameters = null) where U : class
        {
            Tuple<List<T>, List<U>> tuple;
            string sql = sql1 + ";" + sql2;
            using (var multipleresult = db.QueryMultiple(sql, parameters))
            {
                var t = multipleresult.Read<T>().ToList();
                var u = multipleresult.Read<U>().ToList();
                tuple = new Tuple<List<T>, List<U>>(t, u);
            }
            return tuple;
        }


        /// <summary>
        /// Return 3 select statetments unrelated each other. Return a tuple object
        /// </summary>
        /// <typeparam name="U">DTO or Model</typeparam>
        /// <param name="db">DB connection</param>
        /// <param name="sql1">1st select statetment. Results returned as List<T> into the Tuple.  ex: Select * From Product </param>
        /// <param name="sql2">2nd select statetment. Results returned as List<U> into the Tuple.  ex: select * from Actions</param>
        /// <param name="sql3">3nd select statetment. Results returned as List<W> into the Tuple.  ex: select * from Discount</param>
        /// <param name="parameters">where parameters (optional). Parameters must match whereConditions. ex: new {Age = 10, Name = "Tom%"}</param>
        /// <returns>Return a tuple object</returns>
        public Tuple<List<T>, List<U>, List<W>> Select3Queries<U, W>(IDbConnection db, string sql1, string sql2, string sql3, object parameters = null) where U : class where W : class
        {
            Tuple<List<T>, List<U>, List<W>> tuple;
            string sql = sql1 + ";" + sql2 + ";" + sql3;
            using (var multipleresult = db.QueryMultiple(sql, parameters))
            {
                var t = multipleresult.Read<T>().ToList();
                var u = multipleresult.Read<U>().ToList();
                var w = multipleresult.Read<W>().ToList();
                tuple = new Tuple<List<T>, List<U>, List<W>>(t, u, w);
            }
            return tuple;
        }

        /// <summary>
        /// Return 4 select statetments unrelated each other. Return a tuple object
        /// </summary>
        /// <typeparam name="U">DTO or Model</typeparam>
        /// <param name="db">DB connection</param>
        /// <param name="sql1">1st select statetment. Results returned as List<T> into the Tuple.  ex: Select * From Product </param>
        /// <param name="sql2">2nd select statetment. Results returned as List<U> into the Tuple.  ex: select * from Actions</param>
        /// <param name="sql3">3nd select statetment. Results returned as List<W> into the Tuple.  ex: select * from Discount</param>
        /// <param name="sql4">3nd select statetment. Results returned as List<W> into the Tuple.  ex: select * from Pricelist</param>
        /// <param name="parameters">where parameters (optional). Parameters must match whereConditions. ex: new {Age = 10, Name = "Tom%"}</param>
        /// <returns>Return a tuple object</returns>
        public Tuple<List<T>, List<U>, List<W>, List<Z>> Select4Queries<U, W, Z>(IDbConnection db, string sql1, string sql2, string sql3, string sql4, object parameters = null) where U : class where W : class where Z : class
        {
            Tuple<List<T>, List<U>, List<W>, List<Z>> tuple;
            string sql = sql1 + ";" + sql2 + ";" + sql3 + ";" + sql4;
            using (var multipleresult = db.QueryMultiple(sql, parameters))
            {
                var t = multipleresult.Read<T>().ToList();
                var u = multipleresult.Read<U>().ToList();
                var w = multipleresult.Read<W>().ToList();
                var z = multipleresult.Read<Z>().ToList();
                tuple = new Tuple<List<T>, List<U>, List<W>, List<Z>>(t, u, w, z);
            }
            return tuple;
        }

        /// <summary>
        /// Return the max Id for a table into the DB. If no record found then return 0
        /// </summary>
        /// <param name="db">DB connection</param>
        /// <param name="table">the table name</param>
        /// <returns></returns>
        public long GetMaxId(IDbConnection db, string table)
        {
            string select = "Select max(Id) From [" + table + "]";
            return db.Query<long>(select).FirstOrDefault<long>();
        }


        /// <summary>
        /// Upsert a list of T model
        /// </summary>
        /// <param name="db"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public UpsertListResultModel Upsert(IDbConnection db, List<T> list)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            long DAId = 0;
            try
            {
                results.Results = new List<UpsertResultsModel>();
                results.TotalRecords = list.Count;
                results.TotalDeleted = 0;
                results.TotalFailed = 0;
                results.TotalInserted = 0;
                results.TotalSucceded = 0;
                results.TotalUpdated = 0;

                StringBuilder SQL = new StringBuilder();
                string ErrorMess;
                foreach (T item in list)
                {
                    UpsertResultsModel itemModel = new UpsertResultsModel();

                    if (item.GetType().GetProperty("DAId") != null)
                    {
                        DAId = long.Parse(item.GetType().GetProperty("Id").GetValue(item, null).ToString());
                        T tmp = GetByDAId(db, DAId);
                        if (tmp == null)
                            itemModel.ClientID = 0;
                        else
                            itemModel.ClientID = long.Parse(tmp.GetType().GetProperty("Id").GetValue(tmp, null).ToString());
                        if (itemModel.ClientID > 0)
                        {
                            //long.Parse(tmp.GetType().GetProperty("Id").GetValue(tmp, null).ToString());
                            //object value = tmp.GetType().GetProperty("Id").GetValue(tmp, null);
                            item.GetType().GetProperty("Id").SetValue(item, itemModel.ClientID);

                            int upd = Update(db, item, out ErrorMess);
                            if (upd > 0 || string.IsNullOrEmpty(ErrorMess))
                            {
                                itemModel.ClientID = itemModel.ClientID;
                                itemModel.Succeded = true;
                                itemModel.DAID = DAId;
                                results.TotalUpdated += 1;
                                results.TotalSucceded += 1;
                            }
                            else
                            {
                                itemModel.ClientID = -1;
                                itemModel.Succeded = false;
                                results.TotalUpdated += 1;
                                results.TotalFailed += 1;
                                itemModel.ErrorReason = ErrorMess;
                            }
                        }
                        else
                        {
                            string tblName = getTableName();

                            if (!tblName.Contains("SalesType"))
                                item.GetType().GetProperty("Id").SetValue(item, null);
                            long ins = Insert(db, item, out ErrorMess);
                            //{TODO: ins not correct}
                            if (ins > 0)
                            {
                                itemModel.ClientID = ins;
                                itemModel.Succeded = true;
                                itemModel.DAID = DAId;
                                results.TotalInserted += 1;
                                results.TotalSucceded += 1;
                            }
                            else
                            {
                                itemModel.ClientID = -1;
                                itemModel.Succeded = false;
                                results.TotalInserted += 1;
                                results.TotalFailed += 1;
                                itemModel.ErrorReason = ErrorMess;
                            }
                        }
                        results.Results.Add(itemModel);
                    }
                    else
                    {
                        itemModel.Succeded = false;
                        itemModel.ErrorReason = "No field DAId exists";
                        itemModel.ClientID = -1;
                        itemModel.Status = -1;
                        itemModel.DAID = long.Parse(item.GetType().GetProperty("Id").GetValue(item, null).ToString());
                        results.Results.Add(itemModel);
                    }
                }
            }
            catch(Exception ex)
            {
                UpsertResultsModel itemModel = new UpsertResultsModel();
                itemModel.Succeded = false;
                itemModel.ErrorReason = ex.ToString();
                itemModel.ClientID = -1;
                itemModel.Status = -1;
                itemModel.DAID = DAId;
            }
            return results;
        }

        /// <summary>
        /// Delete's or set field IsDeleted to true from a list of object T
        /// </summary>
        /// <param name="db"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public UpsertListResultModel DeleteOrSetIsDeletedList(IDbConnection db, List<T> list)
        {

            UpsertListResultModel results = new UpsertListResultModel();
            UpsertResultsModel itemModel = new UpsertResultsModel();
            long DAId = 0;
            long ClientId = 0;
            try
            {
                results.Results = new List<UpsertResultsModel>();
                results.TotalRecords = list.Count;
                results.TotalDeleted = 0;
                results.TotalFailed = 0;
                results.TotalInserted = 0;
                results.TotalSucceded = 0;
                results.TotalUpdated = 0;
                
                StringBuilder SQL = new StringBuilder();
                foreach (T item in list)
                {
                    if (item == null)
                    {
                        UpsertResultsModel notFoundModel = new UpsertResultsModel();
                        notFoundModel.ClientID = -1;
                        notFoundModel.Status = -1;
                        notFoundModel.DAID = -1;
                        notFoundModel.Succeded = false;
                        results.TotalFailed += 1;
                        notFoundModel.ErrorReason = "Item is null. No Record found";
                        results.Results.Add(notFoundModel);
                        continue;
                    }


                    if (item.GetType().GetProperty("DAId") != null)
                    {
                        DAId = long.Parse(item.GetType().GetProperty("DAId").GetValue(item, null).ToString());
                        //T itm = GetByDAId(db, DAId);
                        if (item.GetType().GetProperty("DAId") != null)
                            ClientId = long.Parse(item.GetType().GetProperty("Id").GetValue(item, null).ToString());

                        if (item != null)
                        {
                            itemModel.ClientID = ClientId;
                            itemModel.Status = -1;
                            itemModel.DAID = DAId;
                            try
                            {
                                db.Delete(item);
                                itemModel.Succeded = true;
                                itemModel.ErrorReason = "";
                                results.TotalDeleted += 1;
                                results.TotalSucceded += 1;
                                //results.Results.Add(itemModel);
                            }
                            catch (Exception ex)
                            {
                                itemModel.ErrorReason = ex.ToString();
                                string tblName = getTableName();
                                SQL.Clear();
                                SQL.Append("IF EXISTS(SELECT 1 FROM sys.columns WHERE NAME = 'IsDeleted' AND OBJECT_NAME(object_id) = '" + tblName + "') \n"
                                       + "	SELECT 1 \n"
                                       + "ELSE \n"
                                       + "	SELECT 0");
                                int bExists = db.Query<int>(SQL.ToString()).FirstOrDefault();
                                if (bExists != 0)
                                {
                                    SQL.Clear();
                                    SQL.Append("UPDATE " + tblName + " SET IsDeleted = 1 WHERE DAId = " + DAId.ToString());
                                    bExists = Execute(db, SQL.ToString());
                                    itemModel.Succeded = true;
                                    itemModel.ErrorReason = "";
                                    results.TotalDeleted += 1;
                                    results.TotalSucceded += 1;
                                }
                                else
                                {
                                    itemModel.Succeded = false;
                                    itemModel.ErrorReason += "\r\n  Field IsDeleted Not Exists";
                                    results.TotalFailed += 1;
                                }
                            }
                        }
                        else
                        {
                            itemModel.Succeded = false;
                            itemModel.ErrorReason = "Record for DAID = " + DAId.ToString() + " not exists";
                            results.TotalFailed += 1;
                        }
                        results.Results.Add(itemModel);
                    }
                    else
                    {
                        itemModel.Succeded = false;
                        itemModel.ErrorReason = "No field DAId exists";
                        itemModel.ClientID = -1;
                        itemModel.Status = -1;
                        itemModel.DAID = long.Parse(item.GetType().GetProperty("Id").GetValue(item, null).ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                itemModel.Succeded = false;
                itemModel.ErrorReason = ex.ToString();
                itemModel.ClientID = -1;
                itemModel.Status = -1;
                itemModel.DAID = DAId;
                results.TotalFailed += 1;
            }
            return results;
        }


        /// <summary>
        /// Return Table Name based DTO's attribute TableAttribute or DTO's class name
        /// </summary>
        /// <returns></returns>
        public string getTableName()
        {
            Type cl = typeof(T);//.Assembly.GetType(typeof(U).Name);
            var classCustomAttributes = (Dapper.TableAttribute[])System.Attribute.GetCustomAttributes(cl, typeof(Dapper.TableAttribute));
            if (classCustomAttributes.Length > 0)
            {
                var myAttribute = classCustomAttributes[0];
                return myAttribute.Name;
            }
            else
            {
                return typeof(T).Name;
            }

        }
    }
}
