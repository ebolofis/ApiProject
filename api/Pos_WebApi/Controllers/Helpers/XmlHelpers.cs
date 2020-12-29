using log4net;
using Pos_WebApi.Models;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Pos_WebApi.Helpers
{
    public static class XmlHelpers
    {
        public static Installations ParseXmlDoc(string xml)
        {
            Installations stores = new Installations();
            ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
        public static RemoteDefinedActionModel ParseXmlActionsDoc(string xml) {
            RemoteDefinedActionModel definedactions = new RemoteDefinedActionModel();
            ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            try {
                XDocument xdoc = XDocument.Load(xml);
                var templ = Deserialize<RemoteDefinedActionModel>(xdoc);
                definedactions = templ;
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                var tf = ex.Message;
            }
            return definedactions;
        }

        public static T Deserialize<T>(XDocument doc)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using (var reader = doc.Root.CreateReader())
            {
                return (T)xmlSerializer.Deserialize(reader);
            }
        }
    }
}