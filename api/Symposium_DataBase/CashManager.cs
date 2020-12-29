using Symposium.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess
{
    /// <summary>
    /// Manage Cashed lists for types IGuidModel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CashManager<T> where T: IGuidModel
    {
        /// <summary>
        /// Lists of cashed items by DB (userstodatabases's Guid)
        /// </summary>
        private Dictionary<Guid, List<T>> CashedObj;

        public CashManager()
        {
            CashedObj = new Dictionary<Guid, List<T>>();
        }

        #region "Checkers"
        /// <summary>
        /// Return true if list of T for a specific DB(Guid) exists
        /// </summary>
        /// <param name="guidDb">userstodatabases's Guid</param>
        /// <returns></returns>
        public bool ListExists(Guid guidDb)
        {
            return CashedObj.ContainsKey(guidDb);
        }

        /// <summary>
        /// Return the position of an item in the list of DB(Guid)
        /// </summary>
        /// <param name="guidDb">userstodatabases's Guid</param>
        /// <returns></returns>
        public int ItemExists(Guid guidDb, T item)
        {
            return CashedObj[guidDb].FindIndex(x => x.Id == item.Id);
        }

        #endregion

        #region "GRUD Operations"

        /// <summary>
        /// Add an item to an existing DB's list
        /// </summary>
        /// <param name="guidDb">userstodatabases's Guid</param>
        /// <param name="item">the item T to add into the cashed list</param>
        public void AddItem(Guid guidDb, T item)
        {
            lock (CashedObj)
            {
                CashedObj[guidDb].Add(item);
            }
        }

        /// <summary>
        /// Update an item to an existing DB's list. Return true if update was successful.
        /// </summary>
        /// <param name="guidDb">userstodatabases's Guid</param>
        /// <param name="item">the item T to update into the cashed list</param>
        public bool UpdateItem(Guid guidDb, T item)
        {
            lock (CashedObj)
            {
                int p = CashedObj[guidDb].FindIndex(x => x.Id == item.Id);
                if (p < 0) return false;
                CashedObj[guidDb][p]= item;
            }
            return true;
        }

        /// <summary>
        /// Remove an item from an existing DB's list. Return true if delete was successful.
        /// </summary>
        /// <param name="guidDb">userstodatabases's Guid</param>
        /// <param name="Id">the item Id to remove from the cashed list</param>
        public bool DeleteItem(Guid guidDb, Guid Id)
        {
            lock (CashedObj)
            {
                int p = CashedObj[guidDb].FindIndex(x => x.Id == Id);
                if (p < 0) return false;
                CashedObj[guidDb].RemoveAt(p);
            }
            return true;
        }

        /// <summary>
        /// Add a new list of cashed objects from a DB(Guid)
        /// </summary>
        /// <param name="guidDb">userstodatabases's Guid</param>
        /// <returns></returns>
        public void AddNewList(Guid guidDb, List<T> list)
        {
            //lock (CashedObj)
            //{
              if(ListExists(guidDb)) DeleteList(guidDb);
                CashedObj.Add(guidDb, list);
           // }
        }


        /// <summary>
        /// Remove a list of cashed objects from DB(Guid)
        /// </summary>
        /// <param name="guidDb">userstodatabases's Guid</param>
        /// <returns></returns>
        public bool DeleteList(Guid guidDb)
        {
            lock (CashedObj)
            {
                return CashedObj.Remove(guidDb);
            }
        }

        #endregion

        #region "Select Operations"

        /// <summary>
        /// return an existing cashed list
        /// </summary>
        /// <param name="guidDb">userstodatabases's Guid</param>
        /// <returns></returns>
        public List<T> GetList(Guid guidDb)
        {
           return new List<T>(CashedObj[guidDb]);
        }

        /// <summary>
        /// return an existing cashed list filtered by a predicate
        /// </summary>
        /// <param name="guidDb">userstodatabases's Guid</param>
        /// <param name="wherePredicate">where predicate, ex: x=>x.Eneble==true</param>
        /// <returns></returns>
        public List<T> GetList(Guid guidDb, Func<T,bool> wherePredicate)
        {
            return new List<T>(CashedObj[guidDb].Where(wherePredicate));
        }

        #endregion

    }
}
