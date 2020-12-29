using Symposium_DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DAO
{
  public  interface IGenericITableDAO<T> where T : ITables
    {
        /// <summary>
        /// Insert a List of DTO objects to DB. Return tha same list with the Ids given by the DB
        /// </summary>
        ///  <param name="db">DB connection</param>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="list">the list of DTO objects to insert </param>
        /// <returns>Return tha same list with the Ids given by the DB</returns>
        List<T> InsertList(IDbConnection db, List<T> list);


        /// <summary>
        /// Insert or Update an Item to DB.
        /// Return the same item. For inserted item new Id is included
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        T Upsert(IDbConnection db, T item);


        /// <summary>
        /// Insert or Update a list of Items to DB.
        /// Return the list of items. For inserted items new Id's are included
        /// </summary>
        /// <param name="db"></param>
        /// <param name="items"></param>
         List<T> UpsertList(IDbConnection db, List<T> items);
    }


  
}
