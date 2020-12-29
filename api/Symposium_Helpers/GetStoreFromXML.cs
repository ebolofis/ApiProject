using log4net;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Xml.Linq;
using System.Xml.Serialization;


namespace Symposium.Helpers
{
    public class GetStoreFromXML
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Returns Store Model for a specific store id
        /// </summary>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        public DBInfoModel GetStoreFromStoreId(string xmlName, string StoreId = "")
        {
            DBInfoModel ret = new DBInfoModel();
            try
            {
                bool isDeliveryAgent = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "IsDeliveryAgent");
                string sId = "";
                string xml = xmlName;
                Installations installations = ParseXmlDoc(xml);
                var request = HttpContext.Current.Request;
                if (!string.IsNullOrEmpty(StoreId))
                    sId = StoreId;
                else if (isDeliveryAgent)
                {
                    string sIdRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_StoreId");
                    sId = sIdRaw.Trim().ToLower();
                }
                List<DBInfoModel> Store = new List<DBInfoModel>();
                if (!string.IsNullOrEmpty(sId))
                    ret = installations.Stores.Where(s => s.Id.ToString().Equals(sId)).FirstOrDefault();
                else
                    ret = installations.Stores.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("GetConnectionString : " + ex.ToString());
                ret = null;
            }
            return ret;
        }

        private Installations ParseXmlDoc(string xml)
        {
            Installations stores = new Installations();
            try
            {
                XDocument xdoc = XDocument.Load(xml);
                var templ = Deserialize<Installations>(xdoc);
                stores = templ;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                var tf = ex.Message;
            }
            return stores;
        }

        private T Deserialize<T>(XDocument doc)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using (var reader = doc.Root.CreateReader())
            {
                return (T)xmlSerializer.Deserialize(reader);
            }
        }
    }
}
