using Newtonsoft.Json;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.IO;
using log4net;
using System.Text;

namespace Symposium.Helpers.Classes
{
      /// <summary>
        /// custom json deserializers (The absolute fastest way to deserialize json to object)
        /// </summary>
        public class CustomJsonDeserializers: ICustomJsonDeserializers
    {
        /// <summary>
        /// Deserialise json to Store object
        /// </summary>
        /// <param name="json">string json</param>
        /// <returns></returns>
        public DBInfoModel JsonToStore(string json)
        {
            DBInfoModel store = new DBInfoModel();
            try
            {
                JsonTextReader reader = new JsonTextReader(new StringReader(json));
                reader.Read();
                reader.Read(); reader.Read();
                store.DataBase = reader.Value.ToString();
                reader.Read(); reader.Read();
                store.DataBasePassword = reader.Value.ToString();
                reader.Read(); reader.Read();
                store.DataBaseUsername = reader.Value.ToString();
                reader.Read(); reader.Read();
                store.DataSource = reader.Value.ToString();
                reader.Read(); reader.Read();
                store.Id = new Guid(reader.Value.ToString());
                reader.Read(); reader.Read();
                store.IsIntegrated = reader.Value.ToString();
                reader.Read(); reader.Read();
                store.Password = reader.Value.ToString();
                reader.Read(); reader.Read();
                store.Role = reader.Value.ToString();
                reader.Read(); reader.Read();
                store.Username = reader.Value.ToString();
            }
            catch (Exception ex)
            {
                ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                StringBuilder sb = new StringBuilder();
                sb.Append(Environment.NewLine);
                sb.Append("======================================================================================");
                sb.Append(Environment.NewLine); sb.Append(Environment.NewLine);
                sb.Append(" ERROR Deserializing Store ( from UsersToDatabases.XML) : ");
                sb.Append(Environment.NewLine);
                sb.Append(json ?? "<NULL>");
                sb.Append(Environment.NewLine); sb.Append(Environment.NewLine);
                sb.Append("         >>>-----> CHECK FILE UsersToDatabases.XML  <----<<<"); sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
                sb.Append(ex.ToString());
                sb.Append(Environment.NewLine);
                sb.Append("======================================================================================");
                sb.Append(Environment.NewLine);
                logger.Error(sb.ToString());

                throw;
            }
            return store;
        }
    }
}
