using Dapper;
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
    /// Generic DAO class for DTOs containing long Id method (and implementing ITables interface). 
    /// </summary>
    /// <typeparam name="T"></typeparam>
   public class GenericITableDAO<T>: IGenericITableDAO<T> where T:ITables
    {
        /// <summary>
        /// Insert a List of DTO objects to DB. Return tha same list with the Ids given by the DB
        /// </summary>
        ///  <param name="db">DB connection</param>
        /// <typeparam name="T">DTO class</typeparam>
        /// <param name="list">the list of DTO objects to insert </param>
        /// <returns>Return tha same list with the Ids given by the DB</returns>
        public List<T> InsertList(IDbConnection db, List<T> list)
        {
            foreach(T item in list)
            {
                item.Id = db.Insert<long>(item);
            }
            return list;
        }

        /// <summary>
        /// Insert or Update an Item to DB.
        /// Return the same item. For inserted item new Id is included
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public T Upsert(IDbConnection db, T item)
        {
           int count = db.Update(item);
           if (count == 0) item.Id = db.Insert<long>(item);
           return item;
        }

        /// <summary>
        /// Insert or Update a list of Items to DB.
        /// Return the list of items. For inserted items new Id's are included
        /// </summary>
        /// <param name="db"></param>
        /// <param name="items"></param>
        public List<T> UpsertList(IDbConnection db, List<T> items) 
        {
            foreach (T k in items)
            {
                Upsert(db,k);
            }
            return items;
         }

      }
  }


