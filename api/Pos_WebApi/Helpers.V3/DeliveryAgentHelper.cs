using log4net;
using Pos_WebApi.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pos_WebApi.Helpers.V3
{
    public class DeliveryAgentHelper
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        /// <summary>
        /// Check's using hub if extecr name exists
        /// </summary>
        /// <param name="extecr"></param>
        /// <returns></returns>
        public bool CheckConnectedExtecr(string extecr)
        {
            try
            {
                GroupedConnectionMapping connectedUsers = new GroupedConnectionMapping();// System.Web.HttpContext.Current.Server.MapPath("~/") + "Xml\\signalRConnections.xml");
                string allUsers = connectedUsers.GetAllConnections();
                List<string> users = allUsers.Split(',').ToList<string>();
                foreach (var u in users)
                {
                    string name = u.Trim();
                    if (name.Equals(extecr))
                    {
                        return true;
                    }
                }
                logger.Error(Environment.NewLine + Environment.NewLine + "                   [ERROR]         ExtECR '" + extecr + "' is NOT connected" + Environment.NewLine + Environment.NewLine);
                return false;
            }
            catch (Exception ex)
            {
                logger.Error("Error checking ExtECR '" + extecr + "' connectivity: " + ex.ToString());
                return false;
            }
        }
    }
}