using Pos_WebApi.Helpers;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using PMSConnectionLib;
using Pos_WebApi.Controllers.Helpers;
using log4net;
using Symposium.Helpers.Classes;
using System.Threading.Tasks;

namespace Pos_WebApi.Controllers
{
    public class SalesPosLookupsController : ApiController
    {
        //  private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Get the set of data POS needs:
        /// <para>1. get communication info from every pms </para> 
        /// <para>2. create sp ReservationInfo and ProtelDepartments for every pms </para>
        /// <para>3. get Posinfo and the related data based on client type </para>
        /// <para>4. get salesTypes (Τύποι πώλησης) </para>
        /// <para>5. get pricelists (και τα SalesType) που αφορούν το συγκεκριμένο pos </para>
        /// <para>6. get the active staff for the specific pos  </para>
        /// <para>7. get storeinfoid (not needed, client already got store.) </para>
        /// <para>8. get all active Accounts(Οι δυνατοί τρόποι πληρωμής) except Credit Cards (Type=4) </para>
        /// <para>9. get Credit Cards and assign them with pms's rooms </para>
        /// <para>10. get hotelInfo (Πληροφορίες επικοινωνίας με το pms) </para>
        /// <para>11. get TransferMappings (αντιστοιχίες μεταξύ τμημάτων του PMS και των δευτερευόντων κατηγοριών των προϊόντων (ProductCategory) για αποστολή χρεώσεων στα τμήματα του PMS) </para>
        /// <para>12. get KitchenInstructions (Μηνύματα προς τη κουζίνα) for specific POS.  </para>
        /// <para>13. determine CustomerPolicy based on HotelInfo.Type of the first record in table HotelInfo </para>
        /// <para>14. set hasCustomers=true if customerpolicy is HotelInfo or Other or PmsInterface </para>
        /// <para>15. get delivery's url for searching customer </para>
        /// <para>16. get RegionLockerProduct (το product ‘Locker’ )included Product table, Vats </para>
        /// <para>17. get allowedboardMeals (δικαιωμένα) </para>
        /// <para>18. get Vats </para>
        /// </summary>
        /// <param name="ipAddress">ip η οποία χαρακτηρίζει μοναδικά ένα POS σε μία DB. Για Client POS (type=11) έχει δομή ip,clientPosCode  ex: 1.1.1.1,23</param>
        /// <param name="type">τύπος client. 1: POS, 11: client POS, 10: PDA</param>
        /// <returns>Create new anonymous onbect to return the aquired data</returns>
        public Object GetPosByIp(string ipAddress, int type = 1)
        {
            try
            {
                using (PosEntities db = new PosEntities(false))
                {
                    var posinfo = new PosInfo();

                    //1. get communication info from every pms
                    var hi = db.HotelInfo.Where(w => w.Type == 0 || w.Type == 4);

                    //2. for every pms create stored procedures ReservationInfo and ProtelDepartments
                    foreach (var item in hi)
                    {
                        logger.Info("MPEHotel:" + (item.MPEHotel.ToString()) ?? "<null>" + ", Name:" + (item.HotelName.ToString()) ?? "<null>");
                        string connStr = "server=" + item.ServerName + ";user id=" + item.DBUserName +
                                                                ";password=" + StringCipher.Decrypt(item.DBPassword) + ";database=" + item.DBName + ";";
                        PMSConnection PMSConn = new PMSConnection();
                        PMSConn.initConn(connStr);
                        PMSConn.makeProcedure(item.DBUserName, item.HotelType);
                        PMSConn.closeConnection();
                    }


                    //3. get Posinfo and the related data based on client type
                    if (type == 10) // PDA 
                    {
                        long posid = Convert.ToInt64(ipAddress);
                        //> get Posinfo and the related data from tables:
                        //       PosInfoDetail (οι τύποι των παραστατικών που μπορεί το pos να τυπώσει), 
                        //       Department (Τμήματα στο εστιατόριο), 
                        //       PosInfoDetail_Pricelist_Assoc (Αποκλεισμός συγκεκριμένων τιμοκαταλογων από συγκεκριμένο τύπο παραστατικού), 
                        //       PosInfoDetail_Excluded_Accounts (Αποκλεισμός συγκεκριμένων τρόπων πληρωμής από συγκεκριμένο τύπο παραστατικού)
                        posinfo = db.PosInfo.Include(i => i.PosInfoDetail).Include(i => i.Department).Include(i => i.PosInfoDetail.Select(s => s.PosInfoDetail_Pricelist_Assoc))
                            .Include(i => i.PosInfoDetail.Select(s => s.PosInfoDetail_Excluded_Accounts))
                            .Where(w => w.Id == posid).AsNoTracking().FirstOrDefault();
                        posinfo.PosInfoDetail = posinfo.PosInfoDetail.OrderBy(p => p.ButtonDescription).ToList();
                    }
                    else if (type == 11) //Client POS
                    {
                        var ipandcode = ipAddress.Split(',');  //ex: ipAddress=1.1.1.1,23
                        ipAddress = ipandcode[0];
                        string code = ipandcode[1]; // to clientPosCode που δόθηκε από τον χρήστη κατά το registration

                        //> get Posinfo and the related data from tables:
                        //       PosInfoDetail (οι τύποι των παραστατικών που μπορεί το pos να τυπώσει), 
                        //       Department (Τμήματα στο εστιατόριο), 
                        //       PosInfoDetail_Pricelist_Assoc (Αποκλεισμός συγκεκριμένων τιμοκαταλογων από συγκεκριμένο τύπο παραστατικού), 
                        //       PosInfoDetail_Excluded_Accounts (Αποκλεισμός συγκεκριμένων τρόπων πληρωμής από συγκεκριμένο τύπο παραστατικού)
                        //       ClientPos με το συγκεκριμένο code (μία εγγραφή) (Περιγραφή client pos και συσχετισμός pos με cient pos)
                        posinfo = db.PosInfo.Include(i => i.PosInfoDetail).Include(i => i.PosInfoDetail.Select(s => s.PosInfoDetail_Pricelist_Assoc))
                            .Include(i => i.PosInfoDetail.Select(s => s.PosInfoDetail_Excluded_Accounts))
                            .Include(i => i.ClientPos).Include(i => i.Department).Where(w => w.IPAddress == ipAddress && w.ClientPos.Count > 0).AsNoTracking().FirstOrDefault();
                        posinfo.ClientPos = posinfo.ClientPos.Where(w => w.Code == code).ToList();
                        posinfo.PosInfoDetail = posinfo.PosInfoDetail.OrderBy(p => p.ButtonDescription).ToList();
                    }
                    else // POS
                    {
                        //> get Posinfo and the related data from tables:
                        //       PosInfoDetail (οι τύποι των παραστατικών που μπορεί το pos να τυπώσει), 
                        //       Department (Τμήματα στο εστιατόριο), 
                        //       PosInfoDetail_Pricelist_Assoc (Αποκλεισμός συγκεκριμένων τιμοκαταλογων από συγκεκριμένο τύπο παραστατικού), 
                        //       PosInfoDetail_Excluded_Accounts (Αποκλεισμός συγκεκριμένων τρόπων πληρωμής από συγκεκριμένο τύπο παραστατικού)
                        posinfo = db.PosInfo.Include(i => i.PosInfoDetail).Include(i => i.PosInfoDetail.Select(s => s.PosInfoDetail_Pricelist_Assoc))
                            .Include(i => i.PosInfoDetail.Select(s => s.PosInfoDetail_Excluded_Accounts))
                            .Include(i => i.Department).Where(w => w.IPAddress == ipAddress).AsNoTracking().FirstOrDefault();
                        posinfo.PosInfoDetail = posinfo.PosInfoDetail.OrderBy(p => p.ButtonDescription).ToList();
                    }

                    //4. Get salesTypes (Τύποι πώλησης)
                    var salesTypes = db.SalesType.ToList();

                    //5. pricelists (και τα SalesType) που αφορούν το συγκεκριμένο pos
                    var pricelistQuery = //db.Pricelist.Where(w => w.PosInfo_Pricelist_Assoc.Any(a => a.PosInfoId == posinfo.Id)).AsNoTracking();
                                    from p in db.Pricelist.Where(w => w.PosInfo_Pricelist_Assoc.Any(a => a.PosInfoId == posinfo.Id))
                                    join slt in db.SalesType on p.SalesTypeId equals slt.Id into plst
                                    from jj in plst.DefaultIfEmpty()
                                    select new
                                    {
                                        p.Id,
                                        p.Code,
                                        p.Description,
                                        p.LookUpPriceListId,
                                        p.Percentage,
                                        p.Status,
                                        p.ActivationDate,
                                        p.DeactivationDate,
                                        p.SalesTypeId,
                                        p.PricelistMasterId,
                                        p.IsDeleted,
                                        p.Type,
                                    //p.AllowedMealsPerBoard,p.OrderDetailInvoices,p.PosInfo_Pricelist_Assoc,p.PosInfoDetail_Pricelist_Assoc,p.PriceList_EffectiveHours,p.PricelistMaster,p.PricelistDetail,p.RegionLockerProduct,p.TransferMappings,
                                    SalesType = new
                                        {
                                            Id = jj.Id,
                                            Description = jj.Description,
                                            Abbreviation = jj.Abbreviation,
                                            IsDeleted = jj.IsDeleted,
                                        }
                                    };
                    var pricelist = pricelistQuery.ToList();
                    //db.Pricelist.Include(i=>i.SalesType).AsNoTracking();


                    //6. get the active staff for the specific pos 
                    var staffTemp = db.PosInfo_StaffPositin_Assoc.Include("AssignedPositions.Staff").Where(w => w.PosInfoId == posinfo.Id)
                                                             .SelectMany(s => s.StaffPosition.AssignedPositions)
                                                             .Select(ss => ss.Staff).Where(w => (w.IsDeleted ?? false) == false).Distinct().Select(s => s.Id).ToList();
                    var staff = db.Staff.Where(w => staffTemp.Contains(w.Id)).AsNoTracking().ToList();

                    //7. get storeinfoid (not needed, client already got store.)
                    var storeinfoid = db.Store.FirstOrDefault() != null ? db.Store.FirstOrDefault().Id : 0;

                    //8. get all active Accounts(Οι δυνατοί τρόποι πληρωμής) except Credit Cards (Type=4)
                    var query = db.Accounts.Where(w => (w.IsDeleted == null || w.IsDeleted == false) && w.Type != 4).AsNoTracking().ToList();
                    var Accounts = query.OrderBy(o => o.Sort).AsEnumerable().ToList();

                    //9. get Credit Cards and assign them with pms's rooms
                    var CreditCardsQuery = from p in db.Accounts
                                      where p.Type == 4
                                      join j in db.EODAccountToPmsTransfer.Where(w => w.PosInfoId == posinfo.Id) on p.Id equals j.AccountId into lf
                                      from jj in lf.DefaultIfEmpty()
                                      select new
                                      {
                                          Account = p,
                                          Room = jj.PmsRoom
                                      };
                    var CreditCards = CreditCardsQuery.ToList();

                    //10. get hotelInfo (Πληροφορίες επικοινωνίας με το pms)
                    var availableHotels = db.HotelInfo.Select(s => new { HotelInfoId = s.Id, HotelId = s.HotelId, Description = s.HotelName, Type = s.Type, MPEHotel = s.MPEHotel }).ToList();


                    //11. get TransferMappings (αντιστοιχίες μεταξύ τμημάτων του PMS και των δευτερευόντων κατηγοριών των προϊόντων (ProductCategory) για αποστολή χρεώσεων στα τμήματα του PMS)
                    var defaultHotelId = 1;
                    if (availableHotels.Count() > 0 && posinfo.DefaultHotelId != null)
                    {
                        defaultHotelId = posinfo.DefaultHotelId ?? 1;  //older code:  availableHotels.FirstOrDefault().HotelId ?? 1;
                    }
                    IEnumerable<dynamic> transferMappings;
                    if (availableHotels.Count() > 0 && availableHotels.FirstOrDefault().Type == 4)//type = 4 (πολλαπλά ξενοδοχεία), λαμβάνονται υπόψη όλες οι εγγραφές του πίνακα HotelInfo
                    {
                        //every returned object contains: HotelId, ProductCategoryId, list of Pricelists. Objects are grouped by  HotelId,ProductCategoryId
                        transferMappings = db.TransferMappings.Distinct()
                                                                   .Where(w => w.ProductCategoryId != null && w.PosDepartmentId == posinfo.DepartmentId)
                                                                   .Select(s => new { s.HotelId, s.ProductCategoryId, PricelistId = s.PriceListId })
                                                                   .GroupBy(g => new { g.HotelId, g.ProductCategoryId })
                                                                   .Select(ss => new
                                                                   {
                                                                       HotelId = ss.Key.HotelId,
                                                                       ProductCategoryId = ss.Key.ProductCategoryId,
                                                                       Pricelists = ss.Select(sss => sss.PricelistId).Distinct()
                                                                   })
                                                                   .OrderBy(o1 => o1.HotelId).ThenBy(o => o.ProductCategoryId).ToList();
                    }
                    else
                    {
                        //  for type <> 4 (one hotel or no hotel at all),  λαμβάνεται υπόψη η εγγραφή του πίνακα HotelInfo που θεωρήται ως default
                        //  every returned object contains:  ProductCategoryId, list of Pricelists. Objects are grouped by  ProductCategoryId
                        transferMappings = db.TransferMappings.Distinct()
                                                              .Where(w => w.ProductCategoryId != null && w.PosDepartmentId == posinfo.DepartmentId && w.HotelId == defaultHotelId)
                                                              .Select(s => new { s.ProductCategoryId, PricelistId = s.PriceListId })
                                                              .GroupBy(g => g.ProductCategoryId)
                                                              .Select(ss => new
                                                              {
                                                                  ProductCategoryId = ss.Key,
                                                                  Pricelists = ss.Select(sss => sss.PricelistId).Distinct()
                                                              })
                                                              .OrderBy(o => o.ProductCategoryId).ToList();
                    }



                    //12. get KitchenInstructions (Μηνύματα προς τη κουζίνα) for specific POS. 
                    var KitchenInstructions = db.PosInfo_KitchenInstruction_Assoc.Where(f => f.PosInfoId == posinfo.Id).Select(ff => ff.KitchenInstruction).ToList();

                    //13. determine CustomerPolicy based on HotelInfo.Type of the first record in table HotelInfo
                    var hoteltypeType = db.HotelInfo.Count() > 0 ? db.HotelInfo.FirstOrDefault().Type : (byte)255;
                    byte CustomerPolicy = 0;
                    switch (hoteltypeType)
                    {
                        case 0:
                        case 10:
                            CustomerPolicy = (byte)CustomerPolicyEnum.HotelInfo;
                            break;
                        case 2:
                            CustomerPolicy = (byte)CustomerPolicyEnum.Other;
                            break;
                        case 3:
                            CustomerPolicy = (byte)CustomerPolicyEnum.Delivery;
                            break;
                        case 4:
                            CustomerPolicy = (byte)CustomerPolicyEnum.PmsInterface;

                            break;
                        default:
                            CustomerPolicy = (byte)CustomerPolicyEnum.NoCustomers;
                            break;
                    };
                    //14. set hasCustomers=true if customerpolicy is HotelInfo or Other or PmsInterface
                    bool hasCustomers = CustomerPolicy == (byte)CustomerPolicyEnum.HotelInfo || CustomerPolicy == (byte)CustomerPolicyEnum.Other || CustomerPolicy == (byte)CustomerPolicyEnum.PmsInterface;

                    //not used
                    string CustomerServiceProviderUrl = CustomerPolicy == (byte)CustomerPolicyEnum.Delivery || CustomerPolicy == (byte)CustomerPolicyEnum.Other ? db.HotelInfo.FirstOrDefault().HotelUri : "";

                    //15. get delivery's url for searching customer
                    string RedirectToCustomerCard = CustomerPolicy == (byte)CustomerPolicyEnum.Delivery || CustomerPolicy == (byte)CustomerPolicyEnum.Other ? db.HotelInfo.FirstOrDefault().RedirectToCustomerCard : "";

                    //16. get KitchenRegions
                    var ItemRegions = db.KitchenRegion.AsNoTracking().ToList();

                    //15. get InvoiceTypes (Τύποι παραστατικών)
                    var InvoiceTypes = db.InvoiceTypes.AsNoTracking().ToList();

                    //16. get RegionLockerProduct (το product ‘Locker’ )included Product table, Vats
                    var lockerProducts = (from q in db.RegionLockerProduct.Include("Product")
                                          join qq in db.PricelistDetail.Include("Vat") on new { PrId = q.PriceListId, ProdId = q.ProductId } equals new { PrId = qq.PricelistId, ProdId = qq.ProductId }
                                          //join qqq in db.PosInfo_Region_Assoc.Where(w => w.PosInfoId == posinfo.Id) on q.RegionId equals qqq.RegionId
                                          where q.PosInfoId == posinfo.Id
                                          select new
                                          {
                                              PosInfoId = q.PosInfoId,
                                              Discount = q.Discount,
                                              ReturnPaymentId = q.ReturnPaymentpId,
                                              PaymentId = q.PaymentId,
                                              SaleId = q.SaleId,
                                              //Id = s.Key.Id,
                                              PageButton = new
                                              {
                                                  ProductId = q.ProductId,
                                                  Description = q.SalesDescription,
                                                  PreparationTime = 0,
                                                  Sort = 0,
                                                  SetDefaultPriceListId = q.PriceListId,
                                                  Type = 10,// s.FirstOrDefault().Type,
                                                  Code = q.Product.Code,
                                                  PricelistDetails = new
                                                  {
                                                      Id = qq.Id,
                                                      Vat = qq.Vat,
                                                      VatId = qq.VatId,
                                                      TaxId = qq.TaxId,
                                                      Price = q.Price,

                                                  },
                                              }
                                          }).Distinct().ToList();

                    //17. get allowedboardMeals (δικαιωμένα)
                    var allowedboardMeals = db.AllowedMealsPerBoard.Include(i => i.AllowedMealsPerBoardDetails).AsNoTracking().ToList();

                    //18. Get Vats
                    var vats = db.Vat.AsNoTracking().ToList();

                    //Create new anonymous onbect to return the aquired data
                    try
                    {
                        var t = new
                        {
                            storeinfoid,
                            posinfo,
                            salesTypes,
                            vats,
                            pricelist,
                            staff,
                            Accounts,
                            hasCustomers,
                            CustomerPolicy,
                            CustomerServiceProviderUrl,
                            RedirectToCustomerCard,
                            CreditCards,
                            KitchenInstructions,
                            ItemRegions,
                            InvoiceTypes,
                            lockerProducts,
                            allowedboardMeals,
                            transferMappings,
                            availableHotels
                        };
                        return t;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        var p = ex;
                        return null;
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Task.Run(() => GC.Collect());
            }

        }

        public Object GetStatLookups(bool invoiceOnly = false)
        {
            try
            {

                using (PosEntities db = new PosEntities(false))
                {
                    var PosList = (from q in db.EndOfDay.Include("PosInfo")
                                   select new
                                   {
                                       Id = q.PosInfoId,
                                       Description = q.PosInfo.Description,
                                       DepartmentId = q.PosInfo.DepartmentId
                                   }).Distinct().ToList();

                    var StaffList = db.Staff.Select(s => new { Id = s.Id, Name = s.LastName + ' ' + s.FirstName }).ToList();
                    //            var AllInvoiceList = db.PosInfoDetail.GroupBy(g => new { g.GroupId }).Select(s => new { Id = s.FirstOrDefault().GroupId, Description = s.FirstOrDefault().Abbreviation }).ToList();
                    var AllInvoiceList = db.InvoiceTypes.Select(s => new { Id = s.Code, Description = s.Abbreviation }).ToList();

                    var InvoiceOnlyList = db.InvoiceTypes.Where(w => w.Type != (int)InvoiceTypesEnum.Order && w.Type != (int)InvoiceTypesEnum.Void).Select(s => new { Id = s.Code, Description = s.Abbreviation }).ToList();
                    var InvoiceList = invoiceOnly ? InvoiceOnlyList : AllInvoiceList;
                    var DepartmentList = db.Department.ToList();
                    var CategoriesList = db.Categories.ToList();
                    var ProductCategoriesList = db.ProductCategories.ToList();
                    var PriceListList = db.Pricelist.Where(w => w.IsDeleted == null || w.IsDeleted == false).ToList();

                    return new { PosList, StaffList, InvoiceList, DepartmentList, CategoriesList, ProductCategoriesList, PriceListList };

                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Task.Run(() => GC.Collect());
            }

        }

        // GET api/salesposlookups/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/salesposlookups
        public void Post([FromBody]string value)
        {
        }

        // PUT api/salesposlookups/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/salesposlookups/5
        public void Delete(int id)
        {
        }

        [AllowAnonymous]
        [HttpOptions]
        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        protected override void Dispose(bool disposing)
        {
            //  db.Dispose();
            base.Dispose(disposing);
        }
    }
}
