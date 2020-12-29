using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs
{
    /// <summary>
    /// Describe a NoSql table
    /// </summary>
   public interface INoSql
    {
      /// <summary>
      /// Unique Guid
      /// </summary>
         Guid Id { get; set; }

        /// <summary>
        /// Model as json
        /// </summary>
         string Model { get; set; }
    }
}
