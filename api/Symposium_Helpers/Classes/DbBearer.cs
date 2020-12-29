using Symposium.Helpers.Interfaces;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Classes
{
    /// <summary>
    /// Class that contains DB Connection Info
    /// </summary>
   public class DbBearer: IDbBearer
    {
        /// <summary>
        /// DB Connection info
        /// </summary>
        public Store DBInfo { get; set; }

    }
}
