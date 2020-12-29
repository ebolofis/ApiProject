using log4net;
using log4net.Repository.Hierarchy;
using Pos_WebApi.Helpers;
using Pos_WebApi.Models;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers.BOControllers {
    [RoutePrefix("api/{storeId}/RemoteActions")]
    public class RemoteActionsController : ApiController {

        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Returns an Instance of loaded Queries on the file to display choices on UI
        /// </summary>
        /// <returns></returns>
        // GET api/<controller>/5
        [Route("GetAll")]
        public RemoteDefinedActionModel GetRemoteActions() {
            try {
                string xml = System.Web.HttpContext.Current.Server.MapPath("~/") + "Xml\\PredefinedQuickActions.xml";
                RemoteDefinedActionModel quickactions = XmlHelpers.ParseXmlActionsDoc(xml);
                return quickactions;
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Loads resource of predefined queries and search for an id to return target Q
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("GetById/{id}")]
        public RemoteDefinedActionModel GetRemoteAction(string id) {
            try {
                string xml = System.Web.HttpContext.Current.Server.MapPath("~/") + "Xml\\PredefinedQuickActions.xml";
                RemoteDefinedActionModel quickactions = XmlHelpers.ParseXmlActionsDoc(xml);
                RemoteDefinedActionModel ret = new RemoteDefinedActionModel { Actions = quickactions.Actions.Where(w => w.Id == id).ToList() };
                return ret;
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// A Resouce function that gets an id of predefined actins loade in UI
        /// applies query action on database and return results on a HTTP responce msg
        /// </summary>
        /// <param name="storeId">Parameter is needed and parsed througn Uri to initiallize an instance of db connection </param>
        /// <param name="id">is The variable mapped to the query</param>
        /// <returns>A Query Result msg or a message of error </returns>
        [Route("ExecuteQuery/{id}")]
        [HttpPost]
        public HttpResponseMessage ExecuteQuery(string storeId, string id) {
            HttpResponseMessage response;
            if (String.IsNullOrEmpty(id))
                return Request.CreateResponse(HttpStatusCode.LengthRequired, Symposium.Resources.Errors.NOQUERYIDEXECUTED);

            try {
                string xml = System.Web.HttpContext.Current.Server.MapPath("~/") + "Xml\\PredefinedQuickActions.xml";
                RemoteDefinedActionModel quickactions = XmlHelpers.ParseXmlActionsDoc(xml);
                string query = quickactions.Actions.Where(w => w.Id == id).Select(q => q.Query).FirstOrDefault().ToString();

                if (String.IsNullOrEmpty(query))
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.QUERYIDNOTEXIST);

                db = new PosEntities(false, Guid.Parse(storeId));
                //var dbCtxTxn = db.Database.BeginTransaction();
                try {
                    db.Database.CommandTimeout = 50000;
                    var res = db.Database.ExecuteSqlCommand(query);
                    db.SaveChanges();
                    //dbCtxTxn.Commit();
                    response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;
                } catch (Exception ex) {
                    //dbCtxTxn.Rollback();
                    logger.Error(ex.ToString());
                    //throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, ex));
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, ex);
                }
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, ex);
                //throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, ex));
            }
        }

        /// <summary>
        /// A function that collect and Triggers Executions on row.
        /// A multi action property is defined by multiple queries that are selected on UI and applied to DB base on StoreId provided
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("ExecuteMultipleQueries")]
        [HttpPost]
        public HttpResponseMessage ExecuteMultipleQueries(List<string> id) {
            HttpResponseMessage response;
            if (id.Count == 0)
                return Request.CreateResponse(HttpStatusCode.LengthRequired, Symposium.Resources.Errors.NOQUERYLISTEXECUTED);

            try {
                string xml = System.Web.HttpContext.Current.Server.MapPath("~/") + "Xml\\PredefinedQuickActions.xml";
                RemoteDefinedActionModel quickactions = XmlHelpers.ParseXmlActionsDoc(xml);

                var queries = quickactions.Actions.Where(w => id.Contains(w.Id)).Select(q => new { id = q.Id, Query = q.Query }).ToList();
                if (queries.Count == 0)
                    return Request.CreateResponse(HttpStatusCode.LengthRequired, Symposium.Resources.Errors.NOQUERYLISTEXECUTED);

                var dbCtxTxn = db.Database.BeginTransaction();
                List<DynamicQueryRes> res = new List<DynamicQueryRes>();

                try {
                    foreach (var q in queries) {
                        string exq = q.Query.ToString();
                        if (String.IsNullOrEmpty(exq))
                            return Request.CreateResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.QUERYIDNOTEXIST);

                        var r = db.Database.ExecuteSqlCommand(exq);

                        DynamicQueryRes ri = new DynamicQueryRes() { Id = q.id, Result = r };
                        res.Add(ri);
                    }
                    db.SaveChanges();
                    dbCtxTxn.Commit();
                    response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;
                } catch (Exception ex) {
                    dbCtxTxn.Rollback();
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.NotModified, ex);
                }
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, ex);
            }
        }
        public class DynamicQueryRes {
            public string Id { get; set; }
            public dynamic Result { get; set; }
        }
    }
}
