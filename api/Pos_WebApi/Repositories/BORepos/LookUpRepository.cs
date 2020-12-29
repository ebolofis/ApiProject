using log4net;
using Newtonsoft.Json;
using PMSConnectionLib;
using Pos_WebApi.Controllers.Helpers;
using Pos_WebApi.Helpers;
using Pos_WebApi.Hubs;
using Pos_WebApi.Models;
using Pos_WebApi.Repositories.PMSRepositories;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
using Symposium.Models.Models.Hotelizer;
using Symposium.WebApi.MainLogic.Flows.Hotelizer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Pos_WebApi.Repositories.BORepos
{
    public class LookUpRepository //: ILookUpRepository
    {
        //  private readonly static GroupedConnectionMapping _connections = new GroupedConnectionMapping(System.Web.HttpContext.Current.Server.MapPath("~/") + "Xml\\signalRConnections.xml");

        protected PosEntities DBContext;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public LookUpRepository(PosEntities db)
        {
            this.DBContext = db;
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }

        public IQueryable<TransferMappingsLookupModel> GetTransferMappingLookUps(Expression<Func<TransferMappingsLookupModel, bool>> predicate = null)
        {
            var query = (from q in DBContext.PosInfo_Pricelist_Assoc
                         join qq in DBContext.PosInfo on q.PosInfoId equals qq.Id
                         join qqq in DBContext.Department on qq.DepartmentId equals qqq.Id
                         join qqqq in DBContext.Pricelist on q.PricelistId equals qqqq.Id
                         join qqqqq in DBContext.Vat on true equals true
                         join qqqqqq in DBContext.TransferMappings.Select(s => new
                         {
                             TransferMappingsId = s.Id,
                             PosDepartmentId = s.PosDepartmentId,
                             PriceListId = s.PriceListId,
                             VatCode = s.VatCode,
                             PmsDepartmentId = s.PmsDepartmentId,
                             PmsDepDescription = s.PmsDepDescription
                         }).Distinct() on new { PosDepartmentId = qqq.Id, PricelistId = qqqq.Id, VatCode = qqqqq.Code } equals
                                                                     new { PosDepartmentId = qqqqqq.PosDepartmentId ?? 0, PricelistId = qqqqqq.PriceListId ?? 00, VatCode = qqqqqq.VatCode }
                                                                     //   into f from tm in f.DefaultIfEmpty()
                         select new TransferMappingsLookupModel
                         {
                             TransferMappingsId = qqqqqq.TransferMappingsId,//= tm != null ? tm.TransferMappingsId : 0,
                             PosDepartmentId = qqq.Id,
                             PosDepartmentDescription = qqq.Description,
                             PricelistId = q.PricelistId,
                             PricelistDescription = qqqq.Description,
                             VatCode = qqqqq.Code,
                             VatDescription = qqqqq.Description,
                             PmsDepartmentId = qqqqqq.PmsDepartmentId,//tm != null ? tm.PmsDepartmentId : "",
                             PmsDepartmentDescription = qqqqqq.PmsDepDescription,//tm != null ? tm.PmsDepDescription : "",
                         }).Distinct();
            if (predicate != null)
                query = query.Where(predicate);
            return query;
        }

        //public IQueryable<TransferMappingUsedDetailsLookUps> GetTransferMappingUsedDetailsLookUps(int selectedVat, Expression<Func<TransferMappingsLookupModel, bool>> predicate = null)
        //{
        //    var query = GetTransferMappingLookUps(predicate);
        //    var dets = from q in query//.Where(w => w.PosDepartmentId == predicate && w.PricelistId == 1)
        //             //  join qq in DBContext.TransferMappingDetails on q.TransferMappingsId equals qq.TransferMappingsId
        //               select new TransferMappingUsedDetailsLookUps
        //               {
        //                   VatCode = q.VatCode,
        //                   TransferMappingsId = q.TransferMappingsId,
        //                   ProductCategoryId = q.ProductCategoryId,
        //                   IsInCurrentVat = selectedVat == q.VatCode,
        //                   TransferMappingsDetailsId = qq.Id
        //               };
        //    return dets;
        //}

        /// <summary>
        /// Get information about the Departments from PMS. Run stored procedure GetDepartments
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        public IEnumerable<PmsDepartmentModel> GetPmsDeparmentsLookUps(Expression<Func<PmsDepartmentModel, bool>> predicate = null, long? hotelId = null)
        {
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
            
            var hi = hotelId == null ? DBContext.HotelInfo.FirstOrDefault() : DBContext.HotelInfo.FirstOrDefault(x => x.HotelId == hotelId);


            if (hi != null && (hi.Type == (int)CustomerPolicyEnum.NoCustomers  || hi.Type == (int)CustomerPolicyEnum.PmsInterface) )
            {
                //using (var client = new HttpClient())
                //{
                //    client.BaseAddress = new Uri(hi.HotelUri);
                //    //client.BaseAddress = new Uri(hi.HotelUri+ "/TransferMappings?g=true&f=true");
                //    //AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("Basic",
                //    //                Convert.ToBase64String(new ASCIIEncoding().GetBytes(String.Format("{0}:{1}", "nikkibeach", "nikkibeach!123"))));
                //    //client.DefaultRequestHeaders.Authorization = authHeader;
                //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //    //http://94.70.248.151:8888/HotelInterface/Public/GetDepartments?hotelid=1
                //    HttpResponseMessage response = client.GetAsync(hi.HotelUri + "/Public/GetDepartments?hotelid=" + hi.HotelId.ToString()).Result;
                //    System.Diagnostics.Debug.WriteLine(response.Content.ReadAsStringAsync().Result);
                //    var retmodel = JsonConvert.DeserializeObject<IEnumerable<PmsDepartmentModel>>(response.Content.ReadAsStringAsync().Result);
                //    return retmodel;
                //}
                string connStr = string.Empty;
                PMSConnection pmsConn = new PMSConnection();
                if (hi != null)
                {
                    connStr = "server=" + hi.ServerName + ";user id=" + hi.DBUserName + ";password=" + StringCipher.Decrypt(hi.DBPassword) + ";database=" + hi.DBName + ";";
                    pmsConn.initConn(connStr);
                }
                else
                {
                    throw new Exception("Cannot connect to protel Server while getting Departments");
                }
                string command = string.Empty;
                switch (hi.HotelType)
                {
                    case "PROTEL":
                        command = "EXEC " + hi.DBUserName + ".GetDepartments " + (hi.MPEHotel!=null?hi.MPEHotel:1).ToString() + ",1";
                        break;
                    case "ERMIS":
                        command = "EXEC " + hi.DBUserName + ".GetDepartments ";
                        break;
                }
                SqlDataReader sqlR = pmsConn.returnResult(command);
                List<PmsDepartmentModel> pmsL = new List<PmsDepartmentModel>();
                while (sqlR.Read())
                {
                    try
                    {
                        PmsDepartmentModel tPms = new PmsDepartmentModel();
                        tPms.DepartmentId = sqlR.GetInt32(0);
                        tPms.Description = sqlR.GetString(1);
                        tPms.Vat = (Double)sqlR.GetDecimal(4);
                        pmsL.Add(tPms);
                    }
                    catch (Exception exMsg)
                    {
                        logger.Error(exMsg.ToString());
                        var sMsg = exMsg.Message;
                        throw;
                    }
                }
                pmsConn.closeConnection();
                //dynamic res = PMSConnection.DataTableToJSON(sqlR, true);
                // var str =  pmsL.Count == 0  ? JsonConvert.SerializeObject<List<PmsDepartmentModel>>(res) : pmsL;
                return pmsL;

            }
            //else if (hi != null && hi.Type == (int)CustomerPolicyEnum.PmsInterface)
            //{
            //    try
            //    {
            //        ProtelRepository pr = new ProtelRepository(hi.ServerName, hi.DBUserName, hi.DBPassword, hi.DBName, hi.allHotels, hi.HotelType);
            //        return pr.GetPMSDepartments((long)hi.MPEHotel);

            //    }
            //    catch (Exception ex)
            //    {
            //        logger.Error(ex.ToString());
            //        //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("GetPmsDeparmentsLookUps error | " + ex.InnerException != null ? ex.InnerException.Message : ex.Message));
            //        return new HashSet<PmsDepartmentModel>();
            //    }
            //}
            else
                return new HashSet<PmsDepartmentModel>();
        }

        public IEnumerable<dynamic> GetDepartmentsForPDA()
        {
            var pdaCodes = WebPosHub._connections.GetConnectedPDAs();//.Select(s => s.ToString().Split('-'));

            var deps = DBContext.PdaModule//.Where(w => !pdaCodes.Contains(w.Code.Trim()))
                                .Select(s => new
                                {
                                    Pda = s,
                                    DepId = s.PosInfo.DepartmentId,
                                    Department = s.PosInfo.Department
                                }).GroupBy(g => g.DepId).Select(s => new
                                {
                                    DepId = s.Key,
                                    Department = s.FirstOrDefault().Department,
                                    Pda = s.Select(ss => new
                                    {
                                        Id = ss.Pda.Id,
                                        Description = ss.Pda.Description,
                                        Status = ss.Pda.Status,
                                        MaxActiveUsers = ss.Pda.MaxActiveUsers,
                                        PosInfoId = ss.Pda.PosInfoId,
                                        PosInfoDescription = ss.Pda.PosInfo.Description,
                                        PageSetId = ss.Pda.PageSetId,
                                        Code = ss.Pda.Code,
                                        CanUse = ss.Pda.PdaModuleDetail.Where(a => a.LastLogoutTS == null).Count() < ss.Pda.MaxActiveUsers ? true : false
                                    })
                                }).ToList();

            return deps;

        }
    }
}
