using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Classes
{
  public  class VersionsHelper
    {
        /// <summary>
        /// Read file Versions/versions.json and return the content as Dictionary
        /// </summary>
        /// <returns></returns>
        public  Dictionary<string, string> ReadVersions()
        {
            string versionsPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "Versions/versions.json";
            Dictionary<string, string> dictionary;
            string rawData = File.ReadAllText(versionsPath, Encoding.Default);
            dictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(rawData);
            return dictionary;
        }
    }
}
