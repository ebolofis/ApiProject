using log4net;
using Newtonsoft.Json;
using PMSConnectionLib;
using Pos_WebApi.Controllers.Helpers;
using Pos_WebApi.Models;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
using Symposium.Models.Models.Hotelizer;
using Symposium.WebApi.MainLogic.Flows.Hotelizer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;

namespace Pos_WebApi.Helpers
{
    public static class PmsDepartments
    {

        public static IEnumerable<object> GetPmsDepartments(string hoteluri, int hotelid)
        {
            ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            //using (var w = new WebClient())
            //{
            //    string url = hoteluri + "/Public/GetDepartments?hotelid=" + hotelid.ToString();
            //    w.Encoding = System.Text.Encoding.UTF8;
            //    var json_data = string.Empty;
            //    // attempt to download JSON data as a string
            //    try
            //    {
            //        json_data = w.DownloadString(url);
            //        // if string with JSON data is not empty, deserialize it to class and return its instance 
            //        var result = !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<List<PmsDepartmentModel>>(json_data) : new List<PmsDepartmentModel>();
            //        return result;
            //    }
            //    catch (Exception)
            //    {
            //        return new List<PmsDepartmentModel>();
            //    }
            //}
            try
            {
                //Hotelizer
                //checks if nedd to call Api Url for Hotelizer or other external systems
                bool extApi = false;
                extApi = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "isExternalApi");
                if (extApi)
                {
                    HotelizerFlows hotelizer = new HotelizerFlows();
                    IEnumerable<HotelizerDepartmentModel> hotelizerDepart = hotelizer.GetHotelizerDepartments();
                    List<PmsDepartmentModel> result = new List<PmsDepartmentModel>();
                    foreach (HotelizerDepartmentModel item in hotelizerDepart)
                    {
                        PmsDepartmentModel tmp = new PmsDepartmentModel();
                        tmp.DepartmentId = item.id;
                        tmp.Description = item.name;
                        tmp.Vat = 0;
                        result.Add(tmp);
                    }
                    return result;
                }


                PosEntities db = new PosEntities(false);
                var hi = db.HotelInfo.Find(hotelid);
                //if (PMSConnection.checkConnection())
                //{
                dynamic res = null;

                    if (hi != null)
                    {
                        string connStr = "server=" + hi.ServerName + ";user id=" + hi.DBUserName +
                                                               ";password=" + StringCipher.Decrypt(hi.DBPassword) + ";database=" + hi.DBName + ";";
                    PMSConnection pmsConn = new PMSConnection();
                    pmsConn.initConn(connStr);
                    SqlDataReader sqlR = pmsConn.returnResult("exec " + hi.DBUserName + ".GetDepartments " + hi.HotelId.ToString() + ",1");

                    res = pmsConn.DataTableToJSON(sqlR, true);
                    pmsConn.closeConnection();

                }
                    else
                    {
                        throw new Exception("Cannot connect to protel Server while getting Departments");
                    }

                //}
                

                return !string.IsNullOrEmpty(res) ? JsonConvert.DeserializeObject<List<PmsDepartmentModel>>(res) : new List<PmsDepartmentModel>();
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return new List<PmsDepartmentModel>();
            }
            
        }

    }
}