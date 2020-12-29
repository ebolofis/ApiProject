using Pos_WebApi.Helpers;
using Pos_WebApi.Models;
using Pos_WebApi.Repositories.BORepos;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using Pos_WebApi.Models.DTOModels;
using log4net;
using System.Data.SqlClient;

namespace Pos_WebApi.Controllers {
    public class StaffController : ApiController {
        private PosEntities db = new PosEntities(false);
        GenericRepository<Staff> svc;
        StaffRepository staffRepo;
        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public StaffController( ) {
            svc = new GenericRepository<Staff>(db);
            staffRepo = new StaffRepository(db);

        }

        [Route("api/{storeId}/Staff/GetAll")]
        public IEnumerable<StaffDTO> GetAll( string storeId ) {
            db = new PosEntities(false, Guid.Parse(storeId));
            staffRepo = new StaffRepository(db);

            return staffRepo.GetAll();
        }

        [Route("api/{storeId}/Staff/GetPaged")]
        public PagedResult<StaffDTO> GetPaged( string storeId, string filters, int page, int pageSize ) {
            //  var flts = JsonConvert.DeserializeObject<PageSetFilter>(filters);
            db = new PosEntities(false, Guid.Parse(storeId));
            staffRepo = new StaffRepository(db);

            return staffRepo.GetPaged(x => true, x1 => x1.LastName, page, pageSize);
        }


        [Route("api/{storeId}/Staff/Add")]
        [HttpPost]
        public HttpResponseMessage InsertStaff(string storeId, StaffDTO model) {
            db = new PosEntities(false, Guid.Parse(storeId));
            staffRepo = new StaffRepository(db);

            staffRepo.Add(model);
            try {
                if (staffRepo.SaveChanges() == 0 )
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, model);
                return response;
            } catch ( DbUpdateConcurrencyException ex ) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
        }

        [Route("api/{storeId}/Staff/UpdateRange")]
        [HttpPut]
        public HttpResponseMessage UpdateRange( string storeId, IEnumerable<StaffDTO> model ) {
            db = new PosEntities(false, Guid.Parse(storeId));
            staffRepo = new StaffRepository(db);

            if ( ModelState.IsValid ) {
                staffRepo.UpdateRange(model);
                try {
                    if ( staffRepo.SaveChanges() == 0 )
                        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.UPDATEFAILED);

                } catch ( DbUpdateConcurrencyException ex ) {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, model);
                return response;
            } else {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        [Route("api/{storeId}/Staff/DeleteRange")]
        [HttpDelete]
        public HttpResponseMessage DeleteProduct( string storeId, IEnumerable<long> ids ) {

            db = new PosEntities(false, Guid.Parse(storeId));
            staffRepo = new StaffRepository(db);

        
                try
                {
                    var res = staffRepo.DeleteRange(ids);

                    if (staffRepo.SaveChanges() == 0)
                    {
                        var deletedid = ids.FirstOrDefault();
                        var query = @"UPDATE Staff SET IsDeleted = 1 WHERE Id = @Deletedid";
                        var res1 = db.Database.ExecuteSqlCommand(query, new SqlParameter("@Deletedid", deletedid));
                        db.SaveChanges();
                        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.SAVEDELETEDFAILED);
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var deletedid = ids.FirstOrDefault();
                    var query = @"UPDATE Staff SET IsDeleted = 1 WHERE Id = @Deletedid";
                    var res1 = db.Database.ExecuteSqlCommand(query, new SqlParameter("@Deletedid", deletedid));
                    db.SaveChanges();
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }
            catch(Exception e)
            {
                //Catch Internal Server error 500 regarding StaffRepo.SaveChanges()==0 check
                var deletedid = ids.FirstOrDefault();
                var query = @"UPDATE Staff SET IsDeleted = 1 WHERE Id = @Deletedid";
                var res1 = db.Database.ExecuteSqlCommand(query, new SqlParameter("@Deletedid", deletedid));
                db.SaveChanges();
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        //public StaffController(IGenericService<Staff> staff)
        //{
        //    svc = staff;
        //}

        // GET api/Staff
        public IEnumerable<Staff> GetStaffs( string storeid ) {
            IEnumerable<Staff> g = db.Staff.Include("AssignedPositions").Where(w => w.IsDeleted == null || w.IsDeleted == false).AsEnumerable();
            return g;
        }



        /// <summary>
        /// get all active staff for a specific pos. Assigned Positions and Actions per staff are included
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="forlogin">always true</param>
        /// <param name="posid">posinfo.id</param>
        /// <returns>
        /// for every staff return: 
        ///               Id, Code, FirstName, LastName, 
        ///               list of AssignedPositions,  
        ///               IsCheckedIn, ImageUri, 
        ///               list of ActionsId, 
        ///               list of ActionsDescription, 
        ///               password, Identification
        /// </returns>
        public Object GetStaffs( string storeid, bool forlogin, long posid ) {

            List<StaffModel> result = new List<StaffModel>();
            if ( !String.IsNullOrWhiteSpace(storeid) && forlogin == true ) {

                //1. get all Actions and AuthorizedGroups (staff's group) per action
                var allAuthorizedActions = from q in db.AuthorizedGroupDetail.Include("Actions")
                                           select new {
                                               ActionId = q.ActionId,
                                               ActionDescription = q.Actions.Description,
                                               AuthorizedGroupId = q.AuthorizedGroupId,
                                               StaffAuthorizationGroup = q.AuthorizedGroupId
                                           };
                //2. get all active staffs end their  AuthorizedGroups (staff's group) per staff
                var allAUthorizedStaff = from q in db.StaffAuthorization.Include("Staff").Where(w => w.Staff.IsDeleted == null || w.Staff.IsDeleted == false)
                                         select new {
                                             AuthorizedGroupId = q.AuthorizedGroupId,
                                             StaffId = q.StaffId,
                                             // StaffLastName = q.Staff.LastName,
                                             //Code = q.Staff.Code,
                                             //FirstName = q.Staff.FirstName,
                                             //LastName = q.Staff.LastName,
                                             //WorkSheet = q.Staff.WorkSheet,
                                             //ImageUri = q.Staff.ImageUri,
                                             //Password = q.Staff.Password,
                                             //IsCheckedIn = q.Staff.WorkSheet.Count > 0 ? q.Staff.WorkSheet.OrderByDescending(o => o.Hour).FirstOrDefault().Type != 0 : false,

                                         };

                //3. combine the two lists above to get AuthorizedGroups (staff's group) and actions per active staff
                var compinedAuth = from q in allAUthorizedStaff.ToList()
                                   join qq in allAuthorizedActions.ToList() on q.AuthorizedGroupId equals qq.AuthorizedGroupId into ff
                                   from au in ff.DefaultIfEmpty()
                                   select new {
                                       AuthorizedGroupId = q.AuthorizedGroupId,
                                       StaffId = q.StaffId,
                                       ActionId = au != null ? au.ActionId : -1,
                                       ActionDescription = au != null ? au.ActionDescription : "",
                                   };

                //4. get staffs' positions assosiated with the spesific pos.
                //    Ενα position εκφράζει μία θέση εργασίας (groups) των υπαλλήλων (κάθε group υπαλλήλων είναι διαθέσημο για login σε συγκεκριμένα pos)
                var assoc = db.PosInfo_StaffPositin_Assoc.Where(w => w.PosInfoId == posid);


                var staffPositions = ( from ap in db.AssignedPositions
                                       join j in assoc on ap.StaffPositionId equals j.StaffPositionId into die
                                       from s in die.DefaultIfEmpty()
                                       where s.StaffPositionId != null
                                       select new {
                                           Id = ap.Staff.Id,
                                           Code = ap.Staff.Code,
                                           FirstName = ap.Staff.FirstName,
                                           LastName = ap.Staff.LastName,
                                           StaffAuthorization = ap.Staff.StaffAuthorization.Select(ss => new {
                                               Id = ss.Id,
                                               AuthorizedGroup = ss.AuthorizedGroup,
                                               ActionId = ss.AuthorizedGroup.AuthorizedGroupDetail.Select(sss => new {
                                                   ActionId = sss.ActionId
                                               })
                                           }),
                                           AssignedPositions = ap.Staff.AssignedPositions,
                                           WorkSheet = ap.Staff.WorkSheet,
                                           ImageUri = ap.Staff.ImageUri,
                                           Password = ap.Staff.Password,
                                           Identification = ap.Staff.Identification,
                                           isAdmin = ap.Staff.isAdmin,
                                           LogInAfterOrder = ap.Staff.LogInAfterOrder,
                                       } ).ToList().GroupBy(g => g.Id).AsEnumerable().Select(s => new {
                                           Id = s.Key,
                                           Code = s.FirstOrDefault().Code,
                                           FirstName = s.FirstOrDefault().FirstName,
                                           LastName = s.FirstOrDefault().LastName,
                                           //StaffAuthorization = s.FirstOrDefault().StaffAuthorization,
                                           AssignedPositions = s.FirstOrDefault().AssignedPositions,
                                           IsCheckedIn = s.FirstOrDefault().WorkSheet.Count > 0 ? s.FirstOrDefault().WorkSheet.OrderByDescending(o => o.Hour).FirstOrDefault().Type != (int) StaffWorksheetStatus.CheckedOut : false,
                                           ImageUri = s.FirstOrDefault().ImageUri,
                                           Password = General.GetMd5Hash(s.FirstOrDefault().Code + s.FirstOrDefault().Password),
                                           Identification = s.FirstOrDefault().Identification,
                                           isAdmin = s.FirstOrDefault().isAdmin,
                                           LogInAfterOrder = s.FirstOrDefault().LogInAfterOrder
                                       }).OrderBy(o => o.FirstName);

                var joined = ( from s in staffPositions
                               join j in compinedAuth on s.Id equals j.StaffId
                               select new {
                                   Id = s.Id,
                                   Code = s.Code,
                                   FirstName = s.FirstName,
                                   LastName = s.LastName,
                                   //StaffAuthorization = s.FirstOrDefault().StaffAuthorization,
                                   AssignedPositions = s.AssignedPositions,
                                   IsCheckedIn = s.IsCheckedIn,
                                   ImageUri = s.ImageUri,
                                   Password = s.Password,
                                   Identification = s.Identification,
                                   isAdmin = s.isAdmin,
                                   LogInAfterOrder = s.LogInAfterOrder,
                                   ActionId = j.ActionId,
                                   ActiosDescription = j.ActionDescription
                               } ).GroupBy(g => g.Id);

                // for every staff return: Id, Code, FirstName, LastName, list of AssignedPositions, IsCheckedIn, ImageUri, list of ActionsId, list of ActionsDescription, password, Identification
                var final = joined.Select(s => new {
                    Id = s.Key,
                    Code = s.FirstOrDefault().Code,
                    FirstName = s.FirstOrDefault().FirstName,
                    LastName = s.FirstOrDefault().LastName,
                    AssignedPositions = s.FirstOrDefault().AssignedPositions,
                    IsCheckedIn = s.FirstOrDefault().IsCheckedIn,
                    ImageUri = s.FirstOrDefault().ImageUri,
                    ActionsId = s.Where(w => w.ActionId != -1).Select(ss => ss.ActionId),
                    ActionsDescription = s.Where(w => w.ActionId != -1).Select(ss => ss.ActiosDescription),
                    Password = s.FirstOrDefault().Password,
                    Identification = s.FirstOrDefault().Identification,
                    isAdmin = s.FirstOrDefault().isAdmin,
                    LogInAfterOrder = s.FirstOrDefault().LogInAfterOrder
                });

                return final;
            } else {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }
        }


        /// <summary>
        /// Perform logon operation for a user. 
        /// <para>
        /// If logon is succesfull (valid user) return the staff.
        ///  For every staff return: 
        ///             Id, Code, FirstName, LastName, 
        ///               list of AssignedPositions,  
        ///               IsCheckedIn, ImageUri, 
        ///               list of ActionsId, 
        ///               list of ActionsDescription, 
        ///               password, Identification
        /// </para>
        /// <para>If user is invalid (wrong username/password, inactive user) return HttpResponseException</para>
        /// 
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="user">username</param>
        /// <param name="pass">password</param>
        /// <returns></returns>
        public Object GetStaffs( string storeid, string user, string pass ) {

                logger.Info(">>   USER LOGON : " + user);

            if ( !String.IsNullOrWhiteSpace(storeid) && !String.IsNullOrWhiteSpace(user) && !String.IsNullOrWhiteSpace(pass) ) {
                Staff g = db.Staff
                            .Include(i => i.WorkSheet)
                            .Where(w => w.Code == user && w.Password == pass && w.IsDeleted == null || w.IsDeleted == false)
                            .FirstOrDefault();
                if ( g != null ) {

                    var allAuthorizedActions = from q in db.AuthorizedGroupDetail.Include("Actions")
                                               select new {
                                                   ActionId = q.ActionId,
                                                   ActionDescription = q.Actions.Description,
                                                   AuthorizedGroupId = q.AuthorizedGroupId,
                                                   StaffAuthorizationGroup = q.AuthorizedGroupId
                                               };

                    var allAUthorizedStaff = from q in db.StaffAuthorization.Include("Staff").Where(w => w.Staff.IsDeleted == null || w.Staff.IsDeleted == false)
                                             where q.StaffId == g.Id
                                             select new {
                                                 AuthorizedGroupId = q.AuthorizedGroupId,
                                                 StaffId = q.StaffId,
                                             };

                    var compinedAuth = from q in allAUthorizedStaff.ToList()
                                       join qq in allAuthorizedActions.ToList() on q.AuthorizedGroupId equals qq.AuthorizedGroupId into ff
                                       from au in ff.DefaultIfEmpty()
                                       select new {
                                           AuthorizedGroupId = q.AuthorizedGroupId,
                                           StaffId = q.StaffId,
                                           ActionId = au != null ? au.ActionId : -1,
                                           ActionDescription = au != null ? au.ActionDescription : "",
                                       };


                    var staffPositions = ( from ap in db.AssignedPositions
                                           where ap.StaffId == g.Id
                                           select new {
                                               Id = ap.Staff.Id,
                                               Code = ap.Staff.Code,
                                               FirstName = ap.Staff.FirstName,
                                               LastName = ap.Staff.LastName,
                                               StaffAuthorization = ap.Staff.StaffAuthorization.Select(ss => new {
                                                   Id = ss.Id,
                                                   AuthorizedGroup = ss.AuthorizedGroup,
                                                   ActionId = ss.AuthorizedGroup.AuthorizedGroupDetail.Select(sss => new {
                                                       ActionId = sss.ActionId
                                                   })
                                               }),
                                               AssignedPositions = ap.Staff.AssignedPositions,
                                               WorkSheet = ap.Staff.WorkSheet,
                                               ImageUri = ap.Staff.ImageUri,
                                               Password = ap.Staff.Password,
                                               Identification = ap.Staff.Identification,
                                           }).ToList().GroupBy(gg => gg.Id).AsEnumerable().Select(s => new {
                                               Id = s.Key,
                                               Code = s.FirstOrDefault().Code,
                                               FirstName = s.FirstOrDefault().FirstName,
                                               LastName = s.FirstOrDefault().LastName,
                                               //StaffAuthorization = s.FirstOrDefault().StaffAuthorization,
                                               AssignedPositions = s.FirstOrDefault().AssignedPositions,
                                               IsCheckedIn = s.FirstOrDefault().WorkSheet.Count > 0 ? s.FirstOrDefault().WorkSheet.OrderByDescending(o => o.Hour).FirstOrDefault().Type != (int) StaffWorksheetStatus.CheckedOut : false,
                                               ImageUri = s.FirstOrDefault().ImageUri,
                                               Password = General.GetMd5Hash(s.FirstOrDefault().Code + s.FirstOrDefault().Password),
                                               Identification = s.FirstOrDefault().Identification,
                                           }).OrderBy(o => o.FirstName);

                    var joined = ( from s in staffPositions
                                   join j in compinedAuth on s.Id equals j.StaffId
                                   select new {
                                       Id = s.Id,
                                       Code = s.Code,
                                       FirstName = s.FirstName,
                                       LastName = s.LastName,
                                       //StaffAuthorization = s.FirstOrDefault().StaffAuthorization,
                                       AssignedPositions = s.AssignedPositions,
                                       IsCheckedIn = s.IsCheckedIn,
                                       ImageUri = s.ImageUri,
                                       Password = s.Password,
                                       Identification = s.Identification,

                                       ActionId = j.ActionId,
                                       ActiosDescription = j.ActionDescription
                                   } ).GroupBy(gg => gg.Id);

                    var final = joined.Select(s => new {
                        Id = s.Key,
                        Code = s.FirstOrDefault().Code,
                        FirstName = s.FirstOrDefault().FirstName,
                        LastName = s.FirstOrDefault().LastName,
                        //StaffAuthorization = s.FirstOrDefault().StaffAuthorization,
                        AssignedPositions = s.FirstOrDefault().AssignedPositions,
                        IsCheckedIn = s.FirstOrDefault().IsCheckedIn,
                        ImageUri = s.FirstOrDefault().ImageUri,
                        ActionsId = s.Where(w => w.ActionId != -1).Select(ss => ss.ActionId),
                        ActionsDescription = s.Where(w => w.ActionId != -1).Select(ss => ss.ActiosDescription),
                        Password = s.FirstOrDefault().Password,
                        Identification = s.FirstOrDefault().Identification,

                    }).FirstOrDefault();
                           
                    return final;
                } else
                {
                    logger.Warn(">>   Logon failed for user " + user+ ". Wrong username/password or inactive user");
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                }
                    
            } else {
                logger.Warn(">>   Logon failed for user " + user + ". Empty username/password/storeId");
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
            }
        }


        // GET api/StaffPosition
        public IEnumerable<Staff> GetStaffs( ) {
            return svc.GetAll();
        }

        public PagedResult<Staff> GetStaff( int page, int pageSize ) {
            return svc.GetPaged(x => true, s => "Id", page, pageSize);
        }

        // GET api/Staff/5
        public Staff GetStaff( long id ) {
            return svc.FindBy(x => x.Id == id).FirstOrDefault();
        }

        // PUT api/Staff/5
        [HttpPut]
        public HttpResponseMessage PutStaff( Staff model ) {
            if ( !ModelState.IsValid ) {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            //if (id != model.Id)
            //{
            //    return Request.CreateResponse(HttpStatusCode.BadRequest);
            //}

            svc.Update(model);

            try {
                if ( svc.SaveChanges() == 0 )
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.FAILED);
            } catch ( DbUpdateConcurrencyException ex ) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/Staff
        public HttpResponseMessage PostStaff( Staff model ) {
            if ( ModelState.IsValid ) {
                svc.Add(model);
                try {
                    if ( svc.SaveChanges() == 0 )
                        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
                } catch ( DbUpdateConcurrencyException ex ) {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, model);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new {
                    id = model.Id
                }));
                return response;
            } else {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Staff/5
        public HttpResponseMessage DeleteStaff( long id ) {
            svc.Delete(x => x.Id == id);
            try {
                if ( svc.SaveChanges() == 0 )
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
            } catch ( DbUpdateConcurrencyException ex ) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [AllowAnonymous]
        public HttpResponseMessage Options( ) {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        protected override void Dispose( bool disposing ) {
            db.Dispose();

            base.Dispose(disposing);
        }
    }
}
