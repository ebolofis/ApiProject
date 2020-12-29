using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DAO
{
   public interface IGenericDAO<T> where T : class
    {

        /// <summary>
        /// Select one object based on id (long)
        /// </summary>
        /// <param name="db">DB connection</param>
        /// <param name="Id">long Id, DB's key</param>
        /// <param name="dbTransact"></param>
        /// <returns></returns>
        T Select(IDbConnection db, long Id, IDbTransaction dbTransact);

        /// <summary>
        /// Select table's Data from the Source DB (Dapper only, on addons)
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        ///<param name="db">DB connection</param>
        /// <param name="table">the table to select from. Table must be the name of T class</param>
        /// <returns>the list of Data (the entire table)</returns>
        List<T> Select(IDbConnection db,string table);

        /// <summary>
        /// Select table's Data from the Source DB
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="db">DB connection</param>
        /// <returns>the list of Data (the entire table)</returns>
         List<T> Select(IDbConnection db);

        /// <summary>
        /// Select table's Data from the Source DB
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="sql">the sql select statement. ex: "Select * From Author WHERE  age = @Age or Name like @Name"</param>
        /// <param name="whereParameters">where parameters (optional). Parameters must match whereConditions. ex: new {Age = 10, Name = "Tom%"}</param>
        /// <param name="db">DB connection</param>
        /// <returns>the list of Data </returns>
        List<T> Select(string sql, object whereParameters, IDbConnection db);


        /// <summary>
        /// Select table's Data using where statetment
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="whereConditions">where conditions. ex: "where age = @Age or Name like @Name"</param>
        /// <param name="whereConditions">where parameters (optional). Parameters must match whereConditions. ex: new {Age = 10, Name = "Tom%"}</param>
        /// <param name="db">DB connection</param>
        /// <returns>the list of Data (the entire table)</returns>
        List<T> Select(IDbConnection db, string whereConditions, object whereParameters = null);


        /// <summary>
        /// Select one object based on id (long)
        /// </summary>
        /// <param name="db">DB connection</param>
        /// <param name="Id">long Id, DB's key</param>
        /// <returns></returns>
        T Select(IDbConnection db, long Id);

        /// <summary>
        /// Select one object based on id (int)
        /// </summary>
        /// <param name="db">DB connection</param>
        /// <param name="Id">int Id, DB's key</param>
        /// <returns></returns>
        T Select(IDbConnection db, int Id);

        // <summary>
        /// Select a list of T using Transaction
        /// </summary>
        /// <param name="db"></param>
        /// <param name="whereConditions"></param>
        /// <param name="whereParameters"></param>
        /// <param name="dbTransact"></param>
        /// <returns></returns>
        List<T> Select(IDbConnection db, string whereConditions, object whereParameters, IDbTransaction dbTransact);

        /// <summary>
        /// Return's A record of the table with specific DAId
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dAId"></param>
        /// <returns></returns>
        T GetByDAId(IDbConnection db, long dAId);


        /// <summary>
        /// Insert a new record to DB using Transaction Scope. Return the new Id (primary key)
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="item">the item to insert </param>
        /// <param name="db">DB connection</param>
        /// <param name="dbTransact"></param>
        /// <param name="ErrorMess"></param>
        /// <returns>Return the new Id (primary key)</returns>
        long Insert(IDbConnection db, T item, IDbTransaction dbTransact, out string ErrorMess);

        /// <summary>
        /// Insert a new record to DB. Return the new Id (primary key)
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="item">the item to insert </param>
        /// <param name="db">DB connection</param>
        /// <returns>Return the new Id (primary key)</returns>
        long Insert(IDbConnection db, T item);

        /// <summary>
        /// Insert a new record to DB. Return the new Id (primary key)
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        ///  <typeparam name="U">the type of  the new Id (primary key)</typeparam>
        /// <param name="item">the item to insert </param>
        /// <param name="db">DB connection</param>
        /// <returns>Return the new Id (primary key)</returns>
        U Insert<U>(IDbConnection db, T item);

        /// <summary>
        /// Insert a new record to DB. Return the new Id (primary key) and error if cannot insert record
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="item">the item to insert </param>
        /// <param name="db">DB connection</param>
        /// <param name="ErrorMess">Error On Insert</param>
        /// <returns>Return the new Id (primary key)</returns>
        long Insert(IDbConnection db, T item, out string ErrorMess);


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
        int Update(IDbConnection db, T item, IDbTransaction dbTrsanct, out string ErrorMess);


        /// <summary>
        /// Update a  record to DB matched by Id. Return the number of raws affected.
        /// If no row found return 0.
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="item">the item to insert </param>
        /// <param name="db">DB connection</param>
        /// <returns>Return the number of raws affected</returns>
        int Update(IDbConnection db, T item);

        /// <summary>
        /// Update a record to DB matched by Id. Return the number of raws affected and error if exception throwed
        /// If no row found return 0.
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="item">the item to insert </param>
        /// <param name="db">DB connection</param>
        /// <param name="ErrorMess">Error on update</param>
        /// <returns>Return the number of raws affected</returns>
        int Update(IDbConnection db, T item, out string ErrorMess);


        /// <summary>
        /// Update a  list of DTO objects (T) to DB matched by Id. Return the number of raws affected
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="list">the list of items to insert </param>
        /// <param name="db">DB connection</param>
        /// <returns>Return the number of raws affected</returns>
        int UpdateList(IDbConnection db, List<T> list);

        /// <summary>
        /// Upsert an Object T
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <param name="dbTransact"></param>
        /// <param name="ErrorMess"></param>
        /// <returns></returns>
        long Upsert(IDbConnection db, T item, IDbTransaction dbTransact, out string ErrorMess);

        /// <summary>
        /// Upsert an Object T
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <param name="ErrorMess"></param>
        /// <returns></returns>
        long Upsert(IDbConnection db, T item, out string ErrorMess);

        /// <summary>
        /// Execute a parameterized query (insert or update)
        /// </summary>
        /// <param name="db">DB connection</param>
        /// <param name="sql">Insert or Update query.  ex: "UPDATE Author SET FirstName = @FirstName, LastName = @LastName " + "WHERE Id = @Id";</param>
        /// <param name="parameters">parameters (optional). Parameters must match query parameters. ex: new {FirstName="Smith",Lastname="Tom", Id=30 } </param>
        /// <returns>the number of affected rows</returns>
        int Execute(IDbConnection db, string sql, object parameters = null);

        /// <summary>
        /// Delete a record from DB matched by Id. Return the number of raws affected
        /// </summary>
        /// <param name="Id">the Id to delete</param>
        /// <param name="db">DB connection</param>
        /// <returns>Return the number of raws affected</returns>
        int Delete(IDbConnection db, long Id);

        /// <summary>
        /// Delete a record from DB matched by Id. Return the number of raws affected
        /// </summary>
        /// <param name="Id">the Id to delete</param>
        /// <param name="db">DB connection</param>
        /// <returns>Return the number of raws affected</returns>
        int Delete(IDbConnection db, string Id);

        /// <summary>
        /// Delete a record from DB matched by Id. Return the number of raws affected
        /// </summary>
        /// <param name="item">the item to delete (only Id is required) </param>
        /// <param name="db">DB connection</param>
        /// <returns>Return the number of raws affected</returns>
        int Delete(IDbConnection db, T item);

        /// <summary>
        /// Delete a record from DB matched by Id using a transaction. Return the number of raws affected
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dbTransaction"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int Delete(IDbConnection db, IDbTransaction dbTransaction, T item);

        /// <summary>
        /// Delete multiple records with where clause
        /// </summary>
        /// <param name="db">DB connection</param>
        /// <param name="whereConditions">where conditions. ex: "where age = @Age or Name like @Name"</param>
        /// <param name="whereConditions">where parameters (optional). Parameters must match whereConditions. ex: new {Age = 10, Name = "Tom%"}</param>
        /// <returns>Return the number of raws affected</returns>
        int DeleteList(IDbConnection db, string whereConditions, object whereParameters = null);

        /// <summary>
        /// Get count of records 
        /// </summary>
        /// <param name="db">DB connection</param>
        /// <param name="whereConditions">where conditions. ex: "where age = @Age or Name like @Name"</param>
        /// <param name="whereConditions">where parameters (optional). Parameters must match whereConditions. ex: new {Age = 10, Name = "Tom%"}</param>
        /// <returns>Return the number of raws affected</returns>
        int RecordCount(IDbConnection db, string whereConditions, object whereParameters = null);


        /// <summary>
        /// Pagenation
        /// </summary>
        /// <param name="db">DB connection</param>
        /// <param name="pageNumber">page Number</param>
        /// <param name="rowsPerPage">rows Per Page</param>
        /// <param name="conditions">where conditions. ex: "where age = @Age or Name like @Name"</param>
        /// <param name="orderBy">order by conditions</param>
        /// <param name="parameters">where parameters (optional). Parameters must match whereConditions. ex: new {Age = 10, Name = "Tom%"}</param>
        /// <returns>the list of items for the page</returns>
        List<T> GetPage(IDbConnection db, int pageNumber, int rowsPerPage, string conditions, string orderBy, object parameters = null);

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
        PaginationModel<T> GetPaginationResult(IDbConnection db, int pageNumber, int rowsPerPage, string conditions, string orderBy, object parameters = null);

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
        PaginationModel<T> GetPaginationQueryResult(IDbConnection db, string sqlData, string sWhere, string sqlCount, int pageNumber, int rowsPerPage);

        /// <summary>
        /// Return 2 select statetments unrelated each other. Return a tuple object
        /// </summary>
        /// <typeparam name="U">DTO or Model</typeparam>
        /// <param name="db">DB connection</param>
        /// <param name="sql1">1st select statetment. Results returned as List<T> into the Tuple.  ex: Select * From Product </param>
        /// <param name="sql2">2nd select statetment. Results returned as List<U> into the Tuple.  ex: select * from Actions</param>
        /// <param name="parameters">where parameters (optional). Parameters must match whereConditions. ex: new {Age = 10, Name = "Tom%"}</param>
        /// <returns>Return a tuple object</returns>
        Tuple<List<T>, List<U>> Select2Queries<U>(IDbConnection db, string sql1, string sql2, object parameters = null) where U : class;


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
        Tuple<List<T>, List<U>, List<W>> Select3Queries<U, W>(IDbConnection db, string sql1, string sql2, string sql3, object parameters = null) where U : class where W : class;

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
        Tuple<List<T>, List<U>, List<W>, List<Z>> Select4Queries<U, W, Z>(IDbConnection db, string sql1, string sql2, string sql3, string sql4, object parameters = null) where U : class where W : class where Z : class;


        /// <summary>
        /// Return the max Id for a table into the DB
        /// </summary>
        /// <param name="db">DB connection</param>
        /// <param name="table">the table name</param>
        /// <returns></returns>
        long GetMaxId(IDbConnection db, string table);

        /// <summary>
        /// Select the First Object from the Source DB
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="sql">the sql select statement. ex: "Select * From Author WHERE  age = @Age or Name like @Name"</param>
        /// <param name="whereParameters">where parameters (optional). Parameters must match whereConditions. ex: new {Age = 10, Name = "Tom%"}</param>
        /// <param name="db">DB connection</param>
        /// <returns>an Object or null</returns>
        T SelectFirst(string sql, object whereParameters, IDbConnection db);



        /// <summary>
        /// Select the first object using where statetment
        /// </summary>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="whereConditions">where conditions. ex: "where age = @Age or Name like @Name"</param>
        /// <param name="whereParameters">where parameters (optional). Parameters must match whereConditions. ex: new {Age = 10, Name = "Tom%"}</param>
        /// <param name="db">DB connection</param>
        /// <returns>the first object or null</returns>
        T SelectFirst(IDbConnection db, string whereConditions, object whereParameters);

        /// <summary>
        /// Select the first object using where statetment and db Transaction
        /// </summary>
        /// <param name="db"></param>
        /// <param name="whereConditions"></param>
        /// <param name="whereParameters"></param>
        /// <param name="dbTransact"></param>
        /// <returns></returns>
        T SelectFirst(IDbConnection db, string whereConditions, object whereParameters, IDbTransaction dbTransact);

        /// <summary>
        /// Upsert a list of T model
        /// </summary>
        /// <param name="db"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        UpsertListResultModel Upsert(IDbConnection db, List<T> list);


        /// <summary>
        /// Delete's or set field IsDeleted to true from a list of object T
        /// </summary>
        /// <param name="db"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        UpsertListResultModel DeleteOrSetIsDeletedList(IDbConnection db, List<T> list);

        /// <summary>
        /// Return Table Name based DTO's attribute TableAttribute or DTO's class name
        /// </summary>
        /// <returns></returns>
        string getTableName();
    }
}
