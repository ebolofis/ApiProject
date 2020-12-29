using log4net;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Symposium.Helpers.Classes
{
    /// <summary>
    /// Helper for managing data from  UsersToDatabases.xml
    /// </summary>
    public class StoreIdsPropertiesHelper : IStoreIdsPropertiesHelper
    {
        public List<DBInfoModel> Stores;

        Installations installedStores;

        private ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        string _xdoc;

        public string BaseUrl { get; set; }

        public bool ApiRunning { get; set; }

        public StoreIdsPropertiesHelper(string xdoc)
        {
            if (!string.IsNullOrEmpty(xdoc))
            {
                this._xdoc = xdoc;
                ParseXmlDoc(this._xdoc);
            }
        }
        public StoreIdsPropertiesHelper()
        { }
            /// <summary>
            /// Return's a Store using StoreId
            /// </summary>
            /// <param name="Id"></param>
            /// <returns></returns>
            public DBInfoModel GetStoreById(Guid Id)
        {
            if (Stores == null)
            {
                logger.Error(">---->>> NO Stores found for webconfig's GUID : "+ Id.ToString());
                return null;
            }
            else
                return Stores.Find(f => f.Id == Id);
        }

        /// <summary>
        /// Return's a Store using UserName and Password
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public DBInfoModel GetStoreByUser_Password(string UserName, string Password)
        {
            if (Stores == null)
            {
                logger.Error("GetStoreById No Stores found");
                return null;
            }
            else
                return Stores.Find(f => f.Username == UserName && f.Password == Password);
        }

        /// <summary>
        /// Return's Connection string using Store Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public string ConnectionString(Guid Id)
        {
            DBInfoModel st = GetStoreById(Id);
            if (st == null)
            {
                logger.Error("GetStoreById No Stores found");
                return "";
            }
            else
                return "server=" + st.DataSource + ";user id=" + st.DataBaseUsername + ";password=" + st.DataBasePassword + ";database=" + st.DataBase + ";";
        }

        /// <summary>
        /// Return's ConnectionString Using login User and Password
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public string ConnectionString(string UserName, string Password)
        {
            DBInfoModel st = GetStoreByUser_Password(UserName, Password);
            if (st != null)
                return "data source = " + st.DataSource + "; initial catalog = " + st.DataBase + "; persist security info = True; user id = " + st.DataBaseUsername + "; password = " + st.DataBasePassword + "; MultipleActiveResultSets = True; App = HIT.SymPOSium";
            else
            {
                logger.Error("GetStoreById No Stores found");
                return "";
            }
        }

        /// <summary>
        /// Returns a list of stores
        /// </summary>
        /// <returns></returns>
        public List<DBInfoModel> GetStores()
        {
            return Stores;
        }


        /// <summary>
        /// Get proper Store (db) for DA from web config
        /// </summary>
        /// <returns></returns>
        public DBInfoModel GetDAStore()
        {
            DBInfoModel Store = null;
            bool isDeliveryAgent = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "IsDeliveryAgent");

            if (isDeliveryAgent)
            {
                string guidRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_StoreId");
                string guid = guidRaw.Trim().ToLower();
                var stores = GetStores().Where(s => s.Id.ToString().Equals(guid));
                if (stores == null || stores.Count()==0) throw new Exception("No Store (db) found for DA_StoreId. Check DA_StoreId value in web config");
                Store = stores.ToList()[0];
            }
            else
                throw new Exception("Web Api is not set as DA. Change IsDeliveryAgent to 'true'.");

            return Store;
        }


        /// <summary>
        /// Parse an xmldocument to Installation Model
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private void ParseXmlDoc(string xml)
        {
            //Installations stores = new Installations();
            Stores = new List<DBInfoModel>();
            try
            {
                XDocument xdoc = XDocument.Parse(xml); //XDocument.Load(xml);
                installedStores = Deserialize<Installations>(xdoc);
                
                Stores = installedStores.Stores;
            }
            catch (Exception ex)
            {
                logger.Error("ParseXmlDoc : " + ex.ToString());
                var tf = ex.Message;
            }
           
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
