using System.Collections.Generic;
using System.Web.Http;
using Pos_WebApi.Repositories.BORepos;
using Pos_WebApi.Models;
using System;
using System.Linq;

namespace Pos_WebApi.Controllers
{
    public class LookUpController : ApiController
    {
        LookUpRepository svc;
        private PosEntities db = new PosEntities(false);
        public LookUpController()
        {
            svc = new LookUpRepository(db);
        }

        public IEnumerable<TransferMappingsLookupModel> GetTransferMappingLookupForDepartment(long depId)
        {
            return svc.GetTransferMappingLookUps(x => x.PosDepartmentId == depId);
        }

        /// <summary>
        /// Get information about the Departments from PMS. Call directly the pms database
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="pmsDepartments"></param>
        /// <returns>return a list of PmsDepartmentModel</returns>
        public IEnumerable<PmsDepartmentModel> GetPmsDepartments(long? hotelid=null, bool pmsDepartments = true)
        {
            if (hotelid == null)
            {
                var hi = db.HotelInfo.FirstOrDefault();
                if (hi != null)
                    hotelid = hi.HotelId;
            }
            return svc.GetPmsDeparmentsLookUps(null, hotelid);
        }


        [Route("api/{storeId}/LookUps/DepartmentsForPda")]
        public IEnumerable<dynamic> GetDepartmentsForPda(string storeId)
        {
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new LookUpRepository(db);
            return svc.GetDepartmentsForPDA();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();

            base.Dispose(disposing);
        }
        //  [Route("LookUps/TransferMappingUsedDetails/{selectedVat, departmentId, pricelistId }")]
        //public IEnumerable<TransferMappingUsedDetailsLookUps> GetTransferMappingUsedDetailsLookUps(int selectedVat, long departmentid, long pricelistId)
        //{
        //    return svc.GetTransferMappingUsedDetailsLookUps(selectedVat, x => x.PricelistId == pricelistId && x.PosDepartmentId == departmentid);
        //}


    }

}
