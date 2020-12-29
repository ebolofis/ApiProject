using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Interfaces
{
    /// <summary>
    /// Contains DB Connection Info
    /// </summary>
    public interface IDbBearer
    {
        /// <summary>
        /// DB Connection info
        /// </summary>
         Store DBInfo { get; set; }
    }
}
