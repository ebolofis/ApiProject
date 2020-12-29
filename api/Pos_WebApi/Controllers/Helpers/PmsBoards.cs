using log4net;
using Newtonsoft.Json;
using PMSConnectionLib;
using Pos_WebApi.Controllers.Helpers;
using Pos_WebApi.Models;
using Symposium.Helpers.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Pos_WebApi.Helpers
{
    public  class PmsBoards
    {
        public  PosEntities dbCont = new PosEntities(false);
        public  ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public  IEnumerable<object> GetPmsBoards(string hoteluri, int hotelid)
        {
            //using (var w = new WebClient())
            //{
            //    string url = hoteluri+"/Public/GetAllBoards?hotelid=" + hotelid.ToString()+"&all=true";
            //    w.Encoding = System.Text.Encoding.UTF8;
            //    var json_data = string.Empty;
            //    // attempt to download JSON data as a string
            //    try
            //    {
            //        json_data = w.DownloadString(url);

            //        // if string with JSON data is not empty, deserialize it to class and return its instance 
            //        var result = !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<List<PmsBoardsModel>>(json_data) : new List<PmsBoardsModel>();

            //        return result;
            //    }
            //    catch (Exception ex)
            //    {
            //        return new List<PmsDepartmentModel>();
            //    }

            //}
            var hi = dbCont.HotelInfo.Find(hotelid);
            string command = string.Empty;

            if (hi.Type != 10)
            {
                PMSConnection pmsConn = new PMSConnection();

                if (hi != null)
                {
                    string connStr = "server=" + hi.ServerName + ";user id=" + hi.DBUserName +
                                                           ";password=" + StringCipher.Decrypt(hi.DBPassword) + ";database=" + hi.DBName + ";";
                    
                    pmsConn.initConn(connStr);
                }
                else
                {
                    logger.Warn("Cannot connect to protel Server while getting Departments for hotelid:"+ hotelid.ToString());
                    throw new Exception("Cannot connect to protel Server while getting Departments");
                }

                command = string.Empty;
                switch (hi.HotelType)
                {
                    case "PROTEL":
                        command = pmsConn.retAllBoardsProtel(hi.DBUserName);
                        break;
                    case "ERMIS":
                        command = fillErmisBoards();
                        break;
                }
                pmsConn.closeConnection();
            }
                

            yield return string.IsNullOrEmpty(command) ? JsonConvert.DeserializeObject<List<PmsBoardsModel>>(command) : new List<PmsBoardsModel>();
        }

       
        public  string fillErmisBoards()
        {
            var tmp = new List<PmsBoardsModel>();

            tmp[0].BoardId = 0.ToString();
            tmp[0].Code = 0.ToString();
            tmp[0].BoardDescr = "RR";
            tmp[1].BoardId = 1.ToString();
            tmp[1].Code = 1.ToString();
            tmp[1].BoardDescr = "BB";
            tmp[2].BoardId = 2.ToString();
            tmp[2].Code = 2.ToString();
            tmp[2].BoardDescr = "HB";
            tmp[3].BoardId = 3.ToString();
            tmp[3].Code = 3.ToString();
            tmp[3].BoardDescr = "FB";
            tmp[4].BoardId = 4.ToString();
            tmp[4].Code = 4.ToString();
            tmp[4].BoardDescr = "AI";

            return tmp.ToString();
        }
    }

    /// <summary>
    /// Get the FOday from a pms server
    /// </summary>
    public class FODay
    {
        public  ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        /// <summary>
        /// Get the FOday from a pms server
        /// </summary>
        /// <param name="hoteluri"></param>
        /// <param name="hotelid"></param>
        /// <returns></returns>
        public  string GetFODay(string hoteluri, int hotelid, PosEntities dbCont)
        {
            logger.Info("Getting FODay from hotelId: " + hotelid.ToString());
            try
            {
                var hi = dbCont.HotelInfo.Where(w => w.HotelId == hotelid).FirstOrDefault();
                PMSConnection pmsConn = new PMSConnection();
                    if (hi != null)
                    {
                        logger.Info("Connecting to PMS : " + "server=" + hi.ServerName + ";user id=" + hi.DBUserName + ";password=*****;database=" + hi.DBName);

                        string connStr = "server=" + hi.ServerName + ";user id=" + hi.DBUserName + ";password=" + StringCipher.Decrypt(hi.DBPassword) + ";database=" + hi.DBName + ";";
                        pmsConn.initConn(connStr);
                    }
                    else
                    {
                        logger.Warn("Cannot find PMS Server with hotelId: " + hotelid.ToString());
                        //throw new Exception("Cannot connect to protel Server while getting Departments");
                        return "";
                    }
                    String result = pmsConn.getFODay(hi.DBUserName, hi.HotelType, (short)hi.MPEHotel).ToString("yyyy-MM-dd");
                pmsConn.closeConnection();
                //}
                return result;//pmsConn.getFODay(hi.DBUserName, hi.HotelType, (short)hi.MPEHotel).ToString("yyyy-MM-dd");
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString()+Environment.NewLine+">>> Returning Now as date...");
                var g = ex.Message; 
                return DateTime.Now.ToString("yyyy-MM-dd");
            }


        }
    }
}