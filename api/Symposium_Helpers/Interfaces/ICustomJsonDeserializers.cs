using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Interfaces
{
    public interface ICustomJsonDeserializers
    {

        /// <summary>
        /// Deserialise json to Store object
        /// </summary>
        /// <param name="json">string json</param>
        /// <returns></returns>
        DBInfoModel JsonToStore(string json);


    }
}
