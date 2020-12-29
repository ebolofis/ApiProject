using Pos_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Pos_WebApi.Helpers;
using Newtonsoft.Json;
using System.Data.Entity;
using log4net;
using Symposium.Models.Models;
using Symposium.Helpers.Classes;

namespace Pos_WebApi.Controllers {
    public class SecurityController : ApiController {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [AllowAnonymous]
        public string GetLogin(string id, string pid) {
            string xml = System.Web.HttpContext.Current.Server.MapPath("~/") + "Xml\\UsersToDatabases.xml";
            Installations installations = XmlHelpers.ParseXmlDoc(xml);
            var user = installations.Stores.Where(w => w.Username == id && w.Password == pid);
            if (user.Count() > 0) {
                //var isHotel = false;
                //using(var db =  new Pos_WebApi.PosEntities(false))
                //{
                //    isHotel = db.HotelInfo.Count() > 0;
                //}
                //return new { userId = user.FirstOrDefault().Id.ToString(), IsHotel = isHotel };
                return user.FirstOrDefault().Id.ToString();
            } else {
                logger.Warn("USER NOT FOUND with id:"+(id??"<null>")+ " and pid:"+(pid??"<null>"));
                return "NOT FOUND";
            }
        }
        /// <summary>
        /// An external login with storeId to initialize Store instance 
        /// </summary>
        /// <param name="stid">StoreId to databases registered</param>
        /// <returns></returns>
        [AllowAnonymous]
        public Symposium.Models.Models.DBInfoModel GetLoginWithStore(string stid) {
            string xml = System.Web.HttpContext.Current.Server.MapPath("~/") + "Xml\\UsersToDatabases.xml";
            Installations installations = XmlHelpers.ParseXmlDoc(xml);
            IEnumerable<Symposium.Models.Models.DBInfoModel> sts = installations.Stores.Where(w => w.Id == new Guid(stid));
            if (sts.Count() > 0) {
                Symposium.Models.Models.DBInfoModel ret = sts.FirstOrDefault();
                return ret;
            } else {
                logger.Warn("Guid Store Login NOT FOUND: "+ (stid ?? "<null>"));
                return null;
            }
        }

        /// <summary>
        /// Registration from POS:
        /// 1. check if user (id,pid) exists in UsersToDatabases.xml
        /// 2. from UsersToDatabases.xml get the connection string and storeId
        /// 3. in DB check if there is a posinfo with the requested ip.
        /// </summary>
        /// <param name="id"> username </param>
        /// <param name="pid"> password </param>
        /// <param name="ip"> ip tou pos </param>
        /// <param name="clientCode"> kwdikos tou client pos (pos με μειωμένα actions) ή null </param>
        /// <returns> storeId </returns>
        [AllowAnonymous]
        public string GetLoginWithClientCheck(string id, string pid, string ip, string clientCode) {
            string xml = System.Web.HttpContext.Current.Server.MapPath("~/") + "Xml\\UsersToDatabases.xml";
            Installations installations = XmlHelpers.ParseXmlDoc(xml);
            var users = installations.Stores.Where(w => w.Username == id && w.Password == pid);


            if (users.Count() == 1) {
                Symposium.Models.Models.DBInfoModel user = users.FirstOrDefault();
                try {
                    using (var db = new PosEntities(false, user.Id)) {
                        var posinfo = new PosInfo();
                        posinfo = db.PosInfo.Where(w => w.IPAddress == ip).AsNoTracking().FirstOrDefault();
                        if (posinfo != null) {
                            if (clientCode != null) {
                                var clientinfo = new ClientPos();
                                clientinfo = db.ClientPos.Where(w => w.Code == clientCode).AsNoTracking().FirstOrDefault();
                                if (clientinfo != null) {
                                    if (posinfo.Id != clientinfo.PosInfoId) {
                                        logger.Error("Wrong association between Pos and Client!");
                                        return Symposium.Resources.Errors.WRONGASSOCIATION;
                                    }
                                } else {
                                    logger.Error("Wrong client code!");
                                    return Symposium.Resources.Errors.WRONGCLIENTCODE;
                                }
                            }
                        } else {
                            logger.Error("No PosInfo found for ");
                            return Symposium.Resources.Errors.WRONGIP;
                        }
                    }
                    return user.Id.ToString();
                } catch (Exception ex) {
                    logger.Error(ex.ToString());
                    return Symposium.Resources.Errors.ERROROCCURED;
                }
            } else if (users.Count() < 1) {
                logger.Warn("USER NOT FOUND");
                return Symposium.Resources.Errors.NOTFOUND;
            } else {
                logger.Error("Too many users found with the given Store Username and Store Password");
                return Symposium.Resources.Errors.TOOMANYFOUND;
            }
        }

        [AllowAnonymous]
        public string GetLoginWithRoles(string id, string pid, int dumint) {
            string xml = System.Web.HttpContext.Current.Server.MapPath("~/") + "Xml\\UsersToDatabases.xml";
            Installations installations = XmlHelpers.ParseXmlDoc(xml);
            var user = installations.Stores.Where(w => w.Username == id && w.Password == pid);
            if (user.Count() > 0) {
                //var isHotel = false;
                //using(var db =  new Pos_WebApi.PosEntities(false))
                //{
                //    isHotel = db.HotelInfo.Count() > 0;
                //}
                //return new { userId = user.FirstOrDefault().Id.ToString(), IsHotel = isHotel };
                return JsonConvert.SerializeObject(user.Select(s => new { Id = s.Id, Role = s.Role }).FirstOrDefault());
            } else {
                logger.Warn("USER NOT FOUND");
                return Symposium.Resources.Errors.NOTFOUND;
            }
        }


        public bool GetUserRole(string id, string role, bool dummy = true) {
            string xml = System.Web.HttpContext.Current.Server.MapPath("~/") + "Xml\\UsersToDatabases.xml";
            Installations installations = XmlHelpers.ParseXmlDoc(xml);
            var user = installations.Stores.Where(w => w.Username == id);
            if (user.Count() > 0) {
                return user.FirstOrDefault().Role.Split(',').Contains(role);


            } else {
                return false;
            }
        }
        [AllowAnonymous]
        public string GetRolesForUser(string username) {
            string xml = System.Web.HttpContext.Current.Server.MapPath("~/") + "Xml\\UsersToDatabases.xml";
            Installations installations = XmlHelpers.ParseXmlDoc(xml);
            var user = installations.Stores.Where(w => w.Username == username);
            if (user.Count() > 0) {
                return user.FirstOrDefault().Role;


            } else {
                return "";
            }

        }


        public bool GetStoreIsHotel() {
            using (var db = new PosEntities(false)) {
                return db.HotelInfo.Count() > 0;
                // return false;
            }


        }


        [AllowAnonymous]
        public HttpResponseMessage Options() {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }


        /// <summary>
        /// check client's version compatibility
        /// </summary>
        /// <param name="client">client: </param>
        /// <param name="version"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("api/Security/CheckVersion")]
        public HttpResponseMessage CheckVersion(Clients client, string version) {
            VersionsHelper versionsHlp = new VersionsHelper();
            Dictionary<string, string> versions = versionsHlp.ReadVersions();
            switch (client) {
                case Clients.BO:
                    if (compareVersion(versions["BOversion"], version) == 1) {
                        string err = "Invalid BO version. Current BO version is " + version + ". The expected minimum BO version is " + versions["BOversion"];
                        logger.Error(err);
                        throw new Exception(err);
                    }
                    break;

                case Clients.POS:
                    if (compareVersion(versions["POSversion"], version) == 1) {
                        string err = "Invalid POS version. Current POS version is " + version + ". The expected minimum POS version is " + versions["POSversion"];
                        logger.Error(err);
                        throw new Exception(err);
                    }
                    break;

                case Clients.PDA:
                    if (compareVersion(versions["PDAversion"], version) == 1) {
                        string err = "Invalid PDA version. Current PDA version is " + version + ". The expected minimum PDA version is " + versions["PDAversion"];
                        logger.Error(err);
                        throw new Exception(err);
                    }
                    break;

                case Clients.EXTECR:
                    if (compareVersion(versions["EXTECRversion"], version) == 1) {
                        string err = "Invalid EXTECR version. Current EXTECR version is " + version + ". The expected minimum EXTECR version is " + versions["EXTECRversion"];
                        logger.Error(err);
                        throw new Exception(err);
                    }
                    break;
                case Clients.DA:
                    if (compareVersion(versions["DeliveryAgentversion"], version) == 1)
                    {
                        string err = "Invalid Delivery Agent version. Current Delivery Agent version is " + version + ". The expected minimum Delivery Agent version is " + versions["DeliveryAgentversion"];
                        logger.Error(err);
                        throw new Exception(err);
                    }
                    break;
                case Clients.KDS:
                    if (compareVersion(versions["KDSversion"], version) == 1)
                    {
                        string err = "Invalid KDS version. Current KDS version is " + version + ". The expected minimum KDS version is " + versions["KDSversion"];
                        logger.Error(err);
                        throw new Exception(err);
                    }
                    break;

                default:
                    throw new Exception("Unknown client");
                    break;

            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }



        /// <summary>
        /// Compare versions. 
        /// Return: 1 if Version1>Version2, -1 if Version1<Version2, 0 if Version1=Version2
        /// </summary>
        /// <param name="Version1"></param>
        /// <param name="Version2"></param>
        /// <returns></returns>
        private int compareVersion(string Version1, string Version2) {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"([\d]+)");
            System.Text.RegularExpressions.MatchCollection m1 = regex.Matches(Version1);
            System.Text.RegularExpressions.MatchCollection m2 = regex.Matches(Version2);
            int min = Math.Min(m1.Count, m2.Count);
            for (int i = 0; i < min; i++) {
                if (Convert.ToInt32(m1[i].Value) > Convert.ToInt32(m2[i].Value)) {
                    return 1;
                }
                if (Convert.ToInt32(m1[i].Value) < Convert.ToInt32(m2[i].Value)) {
                    return -1;
                }
            }
            if (m1.Count > m2.Count && Convert.ToInt32(m1[m1.Count - 1].Value) > 0)
                return 1;
            else if (m1.Count < m2.Count && Convert.ToInt32(m2[m2.Count - 1].Value) > 0)
                return -1;
            else

                return 0;
        }


    }


    //public class LoginResponse
    //{
    //    public string Store
    //}
}
