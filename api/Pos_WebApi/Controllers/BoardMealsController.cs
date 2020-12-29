using log4net;
using Pos_WebApi.Helpers;
using Pos_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers
{
    public class BoardMealsController : ApiController
    {
          //
        // GET: /BoardMeals/
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #region GET
        // GET api/TransferMappings
        public IEnumerable<BoardMeals> GetBoardMeals()
        {
            return db.BoardMeals.AsEnumerable();
        }


         // GET api/TransferMappings/5
         public BoardMeals GetBoardMeals(long id, string storeid)
        {
            BoardMeals mapping = db.BoardMeals.Find(id);
            if (mapping == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return mapping;
        }

         // GET api/TransferMappings/5
         public IEnumerable<BoardMeals> GetBoardMeals(string boardId)
         {
             //var _boardId = long.Parse(boardId);

             var mappings = db.BoardMeals.Include("Product").Where(f => f.BoardId == boardId).AsEnumerable();
             if (mappings == null)
             {
                 throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
             }

             return mappings;
         }

         // GET api/TransferMappings/true
         public object GetPmsBoards(bool g, bool f)
         {
             var hotelinfo = db.HotelInfo.FirstOrDefault();

             if(hotelinfo == null)
                 throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));

            PmsBoards PmsBoards = new PmsBoards();
             var pmsBoards =  PmsBoards.GetPmsBoards(hotelinfo.HotelUri,hotelinfo.HotelId ?? 0);
             // if (pmsBoards.Count() == 0)
             //{
             //    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
             //}

              return pmsBoards;
         }

        public object GetBoardMealsProducts(bool unmapped, string g)
        {
            List<long?> mapedprods  = new List<long?>();
            if (unmapped)
            {
                mapedprods = db.BoardMeals.Select(f => f.ProductId).Distinct().ToList();
            }
            

            var prodsFlat = from d in db.Product.Where(f => !mapedprods.Contains(f.Id))
                    select new
                    {
                        Description = d.Description,
                        Id = d.Id,
                        CategoryDescr = d.ProductCategories.Description,
                        CategoryId = d.ProductCategoryId,
                        Vats = d.PricelistDetail.Select(f => f.Vat.Percentage * 100).Distinct()
                    };
            

            var gr = from d in prodsFlat.ToList()
                     group d by d.CategoryDescr
                         into gg
                         select new GroupedProducts
                         {
                             Category = gg.Key,
                             Products = gg.Select(f => new Prod()
                             {
                                 Description = f.Description,
                                 Id = f.Id,
                                 Vats = f.Vats,
                                 CategoryId = f.CategoryId
                             })
                         };
            return gr;

        }

         #endregion

         #region UPDATE

         // PUT api/Department/5
         public HttpResponseMessage PutBoardMeals(long id, string storeid, BoardMeals mapping)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != mapping.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(mapping).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        #endregion

         #region ADD
         // POST api/Department
         public HttpResponseMessage PostBoardMeals(BoardMeals mapping, string storeid)
        {
            if (ModelState.IsValid)
            {
                //added 12/5/2014
                // check mapping exists
                var mappingExists = db.BoardMeals.Where(f => f.BoardId  == mapping.BoardId && f.ProductId == mapping.ProductId).Count() > 0;
                    

                if(mappingExists)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "BoardMeal exists!");
                //////////////

                db.BoardMeals.Add(mapping);
                db.SaveChanges();

                //var toreturn = db.TransferMappings.FirstOrDefault(f => f.Id == mapping.Id);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, mapping);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = mapping.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        #endregion

         #region DELETE
         // DELETE api/Department/5
         public HttpResponseMessage DeleteBoardMeals(long id, string storeid)
        {
            BoardMeals mapping = db.BoardMeals.Find(id);
            if (mapping == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.BoardMeals.Remove(mapping);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, mapping);
        }

        #endregion

         #region HELP_METHODS
         protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
         #endregion



         [AllowAnonymous]
         public HttpResponseMessage Options()
         {
             return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
         }


    }
    
}
