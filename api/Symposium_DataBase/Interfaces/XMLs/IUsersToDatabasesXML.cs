using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.XMLs
{
    /// <summary>
    /// Handle file UsersToDatabases.xml
    /// </summary>
    public interface IUsersToDatabasesXML
    {
        string ConfigureConnectionString(Guid storeid);
        /// <summary>
        /// Construct Connection Strin from a Store object
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        string ConfigureConnectionString(DBInfoModel Store);

    }
}
