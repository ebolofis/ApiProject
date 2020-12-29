using log4net;
using Pos_WebApi.Controllers.Helpers;
using System;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Pos_WebApi.Controllers.BOControllers
{
    [RoutePrefix("api/{storeId}/Upload")]
    public class UploadController : ApiController
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Route("{entity}/{id}")]
        public async Task<HttpResponseMessage> PostFormData(string storeId, string entity, string id)
        {

            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            //var id = HttpContext.Current.Request.Params["id"].Trim();
            //var entity = HttpContext.Current.Request.Params["entity"].Trim();
            //var isLogo = HttpContext.Current.Request.Params["storeId"].Trim();


            string root = HttpContext.Current.Server.MapPath("~/images/");
            var pathWithEntity = root + storeId + "/" + entity;
            var createGuidPath = false;
            if (!Directory.Exists(pathWithEntity))
            {
                Directory.CreateDirectory(pathWithEntity);
                ///   Directory.CreateDirectory(pathWithEntity + @"\" + id);

                //createGuidPath = true;
            };
            //if (!createGuidPath)
            //{
            //    if (!Directory.Exists(pathWithEntity + @"\" + id))
            //        Directory.CreateDirectory(pathWithEntity + @"\" + id);
            //}
            root = pathWithEntity;// + @"\" + id;


            var streamProvider = new CustomMultipartFormDataStreamProvider(root);
            try
            {


                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(streamProvider);

                // This illustrates how to get the file names.
                foreach (MultipartFileData file in streamProvider.FileData)
                {
                    string fileName = "";
                    //if (entity.ToLower() == "store")
                    //{

                    //    fileName = "logo.png";
                    //    if (File.Exists(Path.Combine(root, fileName)))
                    //        File.Delete(Path.Combine(root, fileName));
                    //}
                    //else
                    //{
                        fileName = file.Headers.ContentDisposition.FileName;

                        if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                        {
                            fileName = fileName.Trim('"');
                        }
                        if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                        {
                            fileName = Path.GetFileName(fileName);
                        }
                   // }
                    //}


                    try
                    {

                        // File.Move(file.LocalFileName, Path.Combine(root, file.Headers.ContentDisposition.FileName));
                        File.Move(file.LocalFileName, Path.Combine(root, fileName));

                        Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                        Trace.WriteLine("Server file path: " + file.LocalFileName);
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        logger.Error(ex.ToString());
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                    }


                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                //TODO WE MUST FIND WHY IT IS WORKING BUT EXCEPTION IS RAISED
                return Request.CreateResponse(HttpStatusCode.OK);
                //return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

    }
}
