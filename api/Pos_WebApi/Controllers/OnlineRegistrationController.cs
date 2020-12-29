using log4net;
using Newtonsoft.Json;
using Pos_WebApi.Helpers;
using Pos_WebApi.Models;
using Pos_WebApi.Repositories.BORepos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers {
    [Authorize]
    public class OnlineRegistrationController : ApiController {
        /// <summary>
        /// Παράμετρος για την σύνδεση με την βάση.
        /// </summary>

        GenericRepository<OnlineRegistrations> svc;
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public OnlineRegistrationController() {
            svc = new GenericRepository<OnlineRegistrations>(db);
        }


        /// <summary>
        /// Επιστρέφει λίστα από κρατήσεις. Παράμετροι το BarCode, Όνομα και επόνυμο.
        /// Όποιο έχει τιμή θα υπολογιστεί για τα αποτελέσματα.
        /// Σετ Περίπτωση που δεν υπάρχουν ενεργές Κρατήσεις τότε επιστρέφει μήνυμα με τις 
        /// ολοκληρωμένες κρατήσεις που ικανοποιούν τις παραμέτρος αναζήτησης.
        /// Η αναζήτηση γίνεται πάντα για το τρέχον έτος (Η ημερομηνία του SQL SERVER).
        /// Όταν η παράμετρος CurYear είναι false τότε επιστρέφει όλες τις κρατήσεις που έχουν status Ενεργή.
        /// </summary>
        /// <param name="BarCode"></param>
        /// <param name="FirtName"></param>
        /// <param name="LastName"></param>
        /// <param name="CurYear"></param>
        /// <returns></returns>
        public List<OnlineRegistrations> GetRegistrations(string BarCode, string FirtName, string LastName, string Mobile, bool CurYear = true) {
            //OnlineRegistration Registration = null;
            //db.Database.Connection.ConnectionString = "data source=sisifos\\sql2014;initial catalog=WebPos_Base;persist security info=True;user id=sa;password=hitprog;MultipleActiveResultSets=True;App=EntityFramework";

            List<OnlineRegistrations> result = new List<OnlineRegistrations>();


            //db.Database.Connection.ConnectionString = 
            //if CurYear == true ... get filtered active results
            if (!CurYear) {
                //var ActiveRegistrations = db.OnlineRegistration.Where(w => (w.Status == (int)OnlineRegistrationStatus.Active) && (w.RegistrationDate.Year < DateTime.Now.Year));
                var ActiveRegistrations = from q in db.OnlineRegistration.Where(w => (w.Status == (int)OnlineRegistrationStatus.Active) && (w.RegistrationDate.Year <= DateTime.Now.Year)) select q;
                foreach (var item in ActiveRegistrations) {
                    OnlineRegistrations res = new OnlineRegistrations();
                    res.Adults = item.Adults;
                    res.AdultsEntered = item.AdultsEntered;
                    res.AdultTicket = item.AdultTicket;
                    res.BarCode = item.BarCode;
                    res.Children = item.Children;
                    res.ChildrenEntered = item.ChildrenEntered;
                    res.ChildTicket = item.ChildTicket;
                    res.Dates = item.Dates;
                    res.FirtName = item.FirtName;
                    res.LastName = item.LastName;
                    res.Mobile = item.Mobile;
                    res.PaymentType = item.PaymentType;
                    res.RegistrationDate = item.RegistrationDate;
                    res.RemainingAmount = item.RemainingAmount;
                    res.ReturnMessage = "";
                    res.Status = item.Status;
                    res.TotalAdults = item.TotalAdults;
                    res.TotalAmount = item.TotalAmount;
                    res.TotalChildren = item.TotalChildren;

                    result.Add(res);
                }
            } else {
                //if CurYear == true ... get filtered active results
                var Registration = from q in db.OnlineRegistration.Where(w => (w.Status == (int)OnlineRegistrationStatus.Active) && (w.RegistrationDate.Year == DateTime.Now.Year)
                                           && (string.IsNullOrEmpty(BarCode) ? true : w.BarCode.Contains(BarCode))
                                           && (string.IsNullOrEmpty(FirtName) ? true : w.FirtName.Contains(FirtName))
                                           && (string.IsNullOrEmpty(LastName) ? true : w.LastName.Contains(LastName))
                                           && (string.IsNullOrEmpty(Mobile) ? true : w.Mobile.Contains(Mobile))

                                   )
                                   select q;
                if (Registration == null || Registration.Count() < 1) {

                    Registration = from q in db.OnlineRegistration.Where(w => (w.Status != (int)OnlineRegistrationStatus.Active) && (w.RegistrationDate.Year == DateTime.Now.Year)
                                           && (string.IsNullOrEmpty(BarCode) ? true : w.BarCode.Contains(BarCode))
                                           && (string.IsNullOrEmpty(FirtName) ? true : w.FirtName.Contains(FirtName))
                                           && (string.IsNullOrEmpty(LastName) ? true : w.LastName.Contains(LastName))
                                           && (string.IsNullOrEmpty(Mobile) ? true : w.Mobile.Contains(Mobile))

                                   )
                                   select q;
                    if (Registration != null && Registration.Count() > 0) {
                        OnlineRegistrations res = new OnlineRegistrations();
                        res.Adults = 0;
                        res.AdultsEntered = 0;
                        res.AdultTicket = 0;
                        res.BarCode = "";
                        res.Children = 0;
                        res.ChildrenEntered = 0;
                        res.ChildTicket = 0;
                        res.Dates = 0;
                        res.FirtName = "";
                        res.LastName = "";
                        res.Mobile = "";
                        res.PaymentType = -1;
                        res.RegistrationDate = new DateTime(1900, 01, 01);
                        res.RemainingAmount = 0;
                        res.ReturnMessage = string.Format(Symposium.Resources.Messages.COMPLETEDREGISTRATIONS, Registration.Count().ToString());
                        res.Status = -1;
                        res.TotalAdults = 0;
                        res.TotalAmount = 0;
                        res.TotalChildren = 0;

                        result.Add(res);
                    }
                } else {
                    foreach (var item in Registration) {
                        OnlineRegistrations res = new OnlineRegistrations();
                        res.Adults = item.Adults;
                        res.AdultsEntered = item.AdultsEntered;
                        res.AdultTicket = item.AdultTicket;
                        res.BarCode = item.BarCode;
                        res.Children = item.Children;
                        res.ChildrenEntered = item.ChildrenEntered;
                        res.ChildTicket = item.ChildTicket;
                        res.Dates = item.Dates;
                        res.FirtName = item.FirtName;
                        res.LastName = item.LastName;
                        res.Mobile = item.Mobile;
                        res.PaymentType = item.PaymentType;
                        res.RegistrationDate = item.RegistrationDate;
                        res.RemainingAmount = item.RemainingAmount;
                        res.ReturnMessage = "";
                        res.Status = item.Status;
                        res.TotalAdults = item.TotalAdults;
                        res.TotalAmount = item.TotalAmount;
                        res.TotalChildren = item.TotalChildren;

                        result.Add(res);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// return the products mapped on online registration
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/OnlineRegistrationProduct")]
        public Product GetOnlineRegistrationProduct() {
            try {
                Product p = db.Product.Include("PricelistDetail").Where(pp => pp.Id == db.ExternalProductMapping.Where(epm => epm.ProductEnumType == (int)ExternalProductMappingEnum.OnlineRegistration).Select(q => q.ProductId).FirstOrDefault()).FirstOrDefault();
                return p;
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                return null;
            }
            return null;
        }
        public class UpdateOnlineRegModel {
            public string BarCode { get; set; }
            public int Children { get; set; }
            public int Adults { get; set; }
        }

        /// <summary>
        /// Αναβάθμιση της κράτησης με την εισαγωγή ατόμων. Απαραίτητη παράμετρος είναι το BarCode.
        /// Ο αριθμός των ατόμων και το παιδίων θα χρησιμοποιηθούν για να γίνουν οι απαραίτητοι 
        /// υπολογισμοί.Όταν τα ατόμα ή τα παιδιά που απομένουν στην κράτηση είναι λιγότερα από 
        /// αυτά των παραμέτρων τότε επιστρέφεται μήνυμα λάθους ότι υπάρχει αρνητικός αριθμός ατόμων.
        /// Το σύνολο των ατόμων και παιδιών μειώνεται κατά το πλήθος των παραμέτρων.
        /// Το σύνολο των εισαχθένων ατόμων και παιδιών αυξανεται κατά το αριθμό των παραμέτρων.
        /// Το σύνολο του υπολοίπου της κράτησης μειώνεται κατά το ποσό των ατόμων επί την τιμή του ατόμου συν
        /// το ποσό των παιδιών επί την τιμή των παιδιών. 
        /// Τέλος αν το σύνολο των παιδιών και των ατόμων που απομένουν στην κράτηση είναι 0 (μηδέν) τότε
        /// η κράτηση γίνεται ολοκληρωμένη.
        /// </summary>
        /// <param name="Barcode"></param>
        /// <param name="Adults"></param>
        /// <param name="Children"></param>
        /// <returns></returns>
        [HttpPut]
        //[Route("api/{storeid}/OnlineRegistration/PutRegistrations/")]
        public HttpResponseMessage PutRegistrations(string storeid, UpdateOnlineRegModel enterReg) {
            try {
                string Barcode = enterReg.BarCode; int Adults = enterReg.Adults; int Children = enterReg.Children;
                OnlineRegistration Registration = db.OnlineRegistration.Where(kk => kk.BarCode == Barcode).FirstOrDefault();

                if (Registration == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, Symposium.Resources.Errors.REGISTRATIONNOTFOUND);
                if ((Registration.TotalAdults - Adults) < 0 || (Registration.TotalChildren - Children) < 0 || Registration.RemainingAmount <= 0 ||
                     (Registration.TotalAdults - Registration.AdultsEntered - Adults) < 0 || (Registration.TotalChildren - Registration.ChildrenEntered - Children) < 0)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Symposium.Resources.Errors.NEGATIVEBALANCE);
                if (Registration.Status == (int)OnlineRegistrationStatus.Finished)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Symposium.Resources.Errors.REGISTRATIONALREADYCLOSED);


                Registration.AdultsEntered = Registration.AdultsEntered + Adults;
                Registration.ChildrenEntered = Registration.ChildrenEntered + Children;
                Registration.RemainingAmount = Registration.RemainingAmount - ((Adults * Registration.AdultTicket) + (Children * Registration.ChildTicket));
                if (Registration.TotalAdults == Registration.AdultsEntered && Registration.TotalChildren == Registration.ChildrenEntered)
                    Registration.Status = (int)OnlineRegistrationStatus.Finished;

                db.Entry(Registration).State = EntityState.Modified;
                try {
                    //public enum OnlineRegistrationPayments { Paypal = 0, Card = 1, PayOnDelivery = 2 }
                    db.SaveChanges();
                } catch (DbUpdateConcurrencyException ex) {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.UPDATEFAILED);
                    //return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, Registration);
                return response;
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message + "  Inner Exception : " + ex.InnerException != null ? ex.InnerException.Message : "");
            }

        }
        /// <summary>
        /// Όλες οι κρατήσεις που βρίσκονται στην λίστα του BarCodes θα πάρουν status Ολοκληρωμενη.
        /// Η χρήση της είναι για το τέλος έτους ώστε όλες οι κρατήσεις να ολοκληρωθούν.
        /// </summary>
        /// <param name="BarCodes"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/OnlineRegistration/CloseActiveRegistrations")]
        public HttpResponseMessage CloseActiveRegistrations(string storeid, CloseYearModel clm) {
            //db.Database.Connection.ConnectionString = "data source=sisifos\\sql2014;initial catalog=WebPos_Base;persist security info=True;user id=sa;password=hitprog;MultipleActiveResultSets=True;App=EntityFramework";
            try {
                foreach (var item in clm.BarCodes) {
                    OnlineRegistration Registration = db.OnlineRegistration.Where(x => x.BarCode == item).FirstOrDefault();
                    if (Registration != null) {
                        Registration.Status = (int)OnlineRegistrationStatus.Finished;
                        db.Entry(Registration).State = EntityState.Modified;
                    }
                }
                try {
                    db.SaveChanges();
                } catch (DbUpdateConcurrencyException ex) {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.UPDATEFAILED);
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, clm);
                return response;
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message + "  Inner Exception : " + ex.InnerException != null ? ex.InnerException.Message : "");
            }
        }
        public class CloseYearModel {
            public List<string> BarCodes { get; set; }

        }


        /// <summary>
        /// Προσθήκη νέας κράτησης. Η παράμετρος είναι JSON Converted string και είναι το 
        /// μοντέλο του πίνακα OnlineRegistration. Το BarCode δεν πρέπει να έχει καταχωρηθεί.
        /// Στη περίπτωση που υπάρχει τότε επιστρέφει μήνυμα λάθους ότι το barcode υπάρχει.
        /// </summary>
        /// <param name="Registration"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AddNewRegistration(string Registration) {
            string barCode = "";
            try {
                //db.Database.Connection.ConnectionString = "data source=sisifos\\sql2014;initial catalog=WebPos_Base;persist security info=True;user id=sa;password=hitprog;MultipleActiveResultSets=True;App=EntityFramework";

                OnlineRegistration startObject = JsonConvert.DeserializeObject<OnlineRegistration>(Registration);

                OnlineRegistration reg = new OnlineRegistration();
                reg.Adults = startObject.Adults;
                reg.AdultsEntered = 0;
                reg.AdultTicket = startObject.AdultTicket;
                reg.BarCode = startObject.BarCode;
                barCode = startObject.BarCode;
                reg.Children = startObject.Children;
                reg.ChildrenEntered = 0;
                reg.ChildTicket = startObject.ChildTicket;
                reg.Dates = startObject.Dates;
                reg.FirtName = startObject.FirtName;
                reg.LastName = startObject.LastName;
                reg.Mobile = startObject.Mobile;
                reg.PaymentType = startObject.PaymentType;
                reg.RegistrationDate = DateTime.Now;
                reg.RemainingAmount = (reg.TotalAdults * reg.AdultTicket) + (reg.TotalChildren * reg.ChildTicket);
                reg.Status = (int)OnlineRegistrationStatus.Active;
                reg.TotalAdults = startObject.Adults * startObject.Dates;
                reg.TotalChildren = startObject.Children * startObject.Dates;
                reg.TotalAmount = (reg.TotalAdults * reg.AdultTicket) + (reg.TotalChildren * reg.ChildTicket);

                OnlineRegistration newRegistration = db.OnlineRegistration.Add(reg);

                db.Entry(newRegistration).State = EntityState.Added;
                if (ModelState.IsValid) {
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                } else
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Symposium.Resources.Errors.INSERTBADMODEL);
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                if (ex.InnerException != null && ex.InnerException.InnerException.Message.Contains("Violation of PRIMARY KEY"))
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Symposium.Resources.Errors.BARCODEEXISTS,barCode));
                else
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message + "  Inner Exception : " + ex.InnerException != null ? ex.InnerException.Message : "");
            }
        }

        /// <summary>
        /// Return the statistics for online registration for peaple entered, expected and totals.
        /// The returnng Dictionary contains three keys: "ENTER", "EXPECT", "TOTAL"
        /// </summary>
        /// <returns>
        /// Return a json like the following:
        /// 
        /// {
        ///"ENTER":{"Adults": 4, "Children": 6, "Total": 10},
        ///"EXPECT":{"Adults": 12, "Children": 7, "Total": 19},
        ///"TOTAL":{"Adults": 16, "Children": 13, "Total": 29}
        ///}
        /// 
        /// </returns>
        [HttpGet]
        [Route("api/onlineRegistrationStats")]
        public Dictionary<string, OnlineRegistrationsStats> GetStatistics() {
            OnlineRegistrationsStats totalStats, enterStats, expectStats;
            Dictionary<string, OnlineRegistrationsStats> stats = new Dictionary<string, OnlineRegistrationsStats>();

            //stats for peaple entered
            var enter = from c in db.OnlineRegistration
                        where c.RegistrationDate.Year == DateTime.Now.Year
                        group c by c.RegistrationDate.Year into g
                        select new OnlineRegistrationsStats {
                            Adults = g.Sum(i => i.AdultsEntered),
                            Children = g.Sum(i => i.ChildrenEntered),
                            Total = g.Sum(i => i.AdultsEntered + i.ChildrenEntered)
                        };

            //total stats
            var total = from c in db.OnlineRegistration
                        where c.RegistrationDate.Year == DateTime.Now.Year
                        group c by c.RegistrationDate.Year into g
                        select new OnlineRegistrationsStats {
                            Adults = g.Sum(i => i.TotalAdults),
                            Children = g.Sum(i => i.TotalChildren),
                            Total = g.Sum(i => i.TotalAdults + i.TotalChildren)
                        };


            enterStats = enter.FirstOrDefault();
            totalStats = total.FirstOrDefault();
            //stats for expected peaple.
            expectStats = new OnlineRegistrationsStats {
                Adults = totalStats.Adults - enterStats.Adults,
                Children = totalStats.Children - enterStats.Children,
                Total = totalStats.Adults - enterStats.Adults + totalStats.Children - enterStats.Children
            };


            stats.Add("ENTER", enterStats);
            stats.Add("EXPECT", expectStats);
            stats.Add("TOTAL", totalStats);
            return stats;
        }


    }
}
