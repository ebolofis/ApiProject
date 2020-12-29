using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Interfaces
{
    /// <summary>
    /// Helper for managing data from  UsersToDatabases.xml
    /// </summary>
    public interface IStoreIdsPropertiesHelper
    {
        /// <summary>
        /// Base url
        /// </summary>
         string BaseUrl { get; set; }


        /// <summary>
        /// true: Api is active and all services can activated.
        /// false: Api stoped and all services must be ended
        /// </summary>
        bool ApiRunning { get; set; }

        /// <summary>
        /// Return's a Store using StoreId
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        DBInfoModel GetStoreById(Guid Id);

        /// <summary>
        /// Return's a Store using UserName and Password
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        DBInfoModel GetStoreByUser_Password(string UserName, string Password);

        /// <summary>
        /// Return's Connection string using Store Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        string ConnectionString(Guid Id);

        /// <summary>
        /// Return's ConnectionString Using login User and Password
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        string ConnectionString(string UserName, string Password);

        /// <summary>
        /// Returns a list of stores
        /// </summary>
        /// <returns></returns>
        List<DBInfoModel> GetStores();

        /// <summary>
        /// Get proper Store (db) for DA from web config
        /// </summary>
        /// <returns></returns>
        DBInfoModel GetDAStore();

    }
}
