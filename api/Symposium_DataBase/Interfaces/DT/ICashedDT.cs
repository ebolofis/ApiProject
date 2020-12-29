using Symposium.Models.Interfaces;
using Symposium.Models.Models;
using Symposium_DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
   public interface ICashedDT<T, DTO> where T : IGuidModel where DTO : INoSql, new()
    {

        /// <summary>
        /// Select data from cash or db
        /// </summary>
        /// <param name="Store">the table to select from. Table must be the name of T class</param>
        /// <returns>the list of Data (the entire table)</returns>
        List<T> Select(DBInfoModel Store);

        /// <summary>
        /// Select data from cash or db filtered by a predicate
        /// </summary>
        /// <param name="Store">DBInfoModel</param>
        /// <param name="wherePredicate">where predicate, ex: x=>x.Eneble==true</param>
        /// <returns>the list of Data </returns>
        List<T> Select(DBInfoModel Store, Func<T, bool> wherePredicate);

        /// <summary>
        /// Insert a new item to DB and into Cash
        /// </summary>
        /// <param name="Store">DBInfoModel</param>
        /// <param name="Model">the model to insert</param>
        /// <returns>the new Id (Guid)</returns>
        Guid Insert(DBInfoModel Store, T Model);


        /// <summary>
        /// Update a new item to DB and into Cash
        /// </summary>
        /// <param name="Store">DBInfoModel</param>
        /// <param name="Model">the model to insert</param>
        /// <returns>the number of lines affected</returns>
        int Update(DBInfoModel Store, T Model);

        /// <summary>
        /// Update a new item to DB and into Cash
        /// </summary>
        /// <param name="Store">DBInfoModel</param>
        /// <param name="Model">the model to insert</param>
        /// <returns>the number of lines affected</returns>
        int Delete(DBInfoModel Store, Guid Id);

    }
}
