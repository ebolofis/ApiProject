using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Interfaces
{
    public interface ICustomJsonSerializers
    {

        /// <summary>
        /// Serialise Store object to json
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        string StoreToJson(DBInfoModel store);

        /// <summary>
        /// Provide a dynamic Object to seriallize it or throw ex on failed seriallize
        /// </summary>
        /// <param name="obj"></param>
        /// <returns> string of seriallized object </returns>
        string DynamicToJson(dynamic obj);

        /// <summary>
        /// Deseriallize obj from string provided
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        dynamic DynamicDeseriallize(string str);
    }

    public interface ICustomGenericJsonSerializers<T> where T : class
    {
        T GenericDeseriallize(string str);
    }
}
