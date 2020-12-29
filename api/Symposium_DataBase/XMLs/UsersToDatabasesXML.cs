using log4net;
using Symposium.Helpers;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Symposium.WebApi.DataAccess.XMLs
{
    /// <summary>
    /// Handle file UsersToDatabases.xml
    /// </summary>
    public class UsersToDatabasesXML: IUsersToDatabasesXML
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        IStoreIdsPropertiesHelper storesHlp;

        public UsersToDatabasesXML(IStoreIdsPropertiesHelper storesHlp)
        {
            this.storesHlp = storesHlp;
        }


        /// <summary>
        /// Construct Connection String based on Store
        /// </summary>
        /// <param name="Store"></param>
        /// <returns>connection string</returns>
        public string ConfigureConnectionString(DBInfoModel Store)
        {
            return constructConnectionString(Store);
        }

        /// <summary>
        /// Construct Connection String based on StoreId
        /// </summary>
        /// <returns>connection string</returns>
        public string ConfigureConnectionString(Guid storeid)
        {
            DBInfoModel st;

            //IEnumerable<Symposium.Models.Models.Store> stores = null;
            ////1. Get all Stores/Installations from xml file
            //string xml = AppDomain.CurrentDomain.BaseDirectory + "Xml\\UsersToDatabases.xml";
            //Installations installations = ParseXmlDoc(xml);
            //storesHlp.GetStoreById(storeid);
            ////2. get proper store based on storeid
            //stores = installations.Stores.Where(w => w.Id == storeid);

            //if (stores == null)
            //{
            //    string err = "Store not found (<null>) for StoreId: " + storeid.ToString();
            //    logger.Error(err);
            //    throw new Exception(err);
            //}
            //else if(stores.Count() == 0)
            //{
            //    string err = "Store not found for StoreId: " + storeid.ToString();
            //    logger.Error(err);
            //    throw new Exception(err);
            //}
            //else
            //{
            //    st = stores.FirstOrDefault();
            //}

            //3. construct connection string
            st = storesHlp.GetStoreById(storeid);
            return constructConnectionString(st);
        }

        /// <summary>
        /// construct connection string
        /// </summary>
        /// <param name="st">the Store</param>
        /// <returns></returns>
        private string constructConnectionString(DBInfoModel st)
        {
            if (st == null) throw new Exception("The Store is Empty. Check xml file");
            return @"data source=" + st.DataSource + @";initial catalog=" + st.DataBase + @";persist security info=True;user id=" + st.DataBaseUsername + ";password=" + st.DataBasePassword + ";MultipleActiveResultSets=True;App=HIT.SymPOSium";
        }


        /// <summary>
        /// Parse UsersToDatabases.xml and return a list of Stores (Installations)
        /// </summary>
        /// <param name="path">the path of UsersToDatabases.xml</param>
        /// <returns>a list of Store (Installations)</returns>
        private Installations ParseXmlDoc(string path)
        {
            Installations stores = new Installations();
            try
            {
                XDocument xdoc = XDocument.Load(path);
                XmlHelper xmlHelper = new XmlHelper();
                var templ = xmlHelper.Deserialize<Installations>(xdoc);
                stores = templ;
            }
            catch (Exception ex)
            {
                logger.Error("Unable to load file "+ path +"    ERROR: "+ ex.ToString());
                throw new Exception("Unable to load file "+ path +"    ERROR: "+ ex.Message);
            }
            return stores;
        }
    }
}
