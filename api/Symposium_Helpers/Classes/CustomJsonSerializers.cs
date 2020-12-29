using Newtonsoft.Json;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Classes
{
    /// <summary>
    /// custom json serializers (The absolute fastest way to serialize objects to json)
    /// </summary>
    public class CustomJsonSerializers: ICustomJsonSerializers
    {
        public DBInfoModel JsonToStore(string json)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serialise Store object to json
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public string StoreToJson(DBInfoModel store)
        {
            StringWriter sw = new StringWriter();
            JsonTextWriter writer = new JsonTextWriter(sw);
            writer.WriteStartObject();

            writer.WritePropertyName("DataBase");
            writer.WriteValue(store.DataBase);
            writer.WritePropertyName("DataBasePassword");
            writer.WriteValue(store.DataBasePassword);
            writer.WritePropertyName("DataBaseUsername");
            writer.WriteValue(store.DataBaseUsername);
            writer.WritePropertyName("DataSource");
            writer.WriteValue(store.DataSource);
            writer.WritePropertyName("Id");
            writer.WriteValue(store.Id);
            writer.WritePropertyName("IsIntegrated");
            writer.WriteValue(store.IsIntegrated);
            writer.WritePropertyName("Password");
            writer.WriteValue(store.Password);
            writer.WritePropertyName("Role");
            writer.WriteValue(store.Role);
            writer.WritePropertyName("Username");
            writer.WriteValue(store.Username);

            writer.WriteEndObject();

            return sw.ToString();

        }

        /// <summary>
        /// Provide a dynamic Object to seriallize it or throw ex on failed seriallize
        /// </summary>
        /// <param name="obj"></param>
        /// <returns> string of seriallized object </returns>
        public string DynamicToJson(dynamic obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj);
            }
            catch (Exception ex)
            {
                throw new Exception("Object failed to serialize on helper ex:" + ex.ToString());
            }
        }

        /// <summary>
        /// Deseriallize obj from string provided
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public dynamic DynamicDeseriallize(string str) {
            try
            {
                return JsonConvert.DeserializeObject(str);
            }
            catch (Exception ex)
            {
                throw new Exception("Object failed to deserialize on helper ex:" + ex.ToString());
            }
        }


    }

    public class CustomGenericJsonSerializers<T> : ICustomGenericJsonSerializers<T> where T : class
    {
        /// <summary>
        /// Deseriallize obj from string provided
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public T GenericDeseriallize(string str)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(str);
            }
            catch (Exception ex)
            {
                throw new Exception("Object failed to deserialize on helper ex:" + ex.ToString());
            }
        }
    }
}
