using Pos_WebApi.Models;
using Pos_WebApi.Repositories.BORepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers
{
    public class DynSingleGridController : ApiController
    {
        // private Repository<DynGridModel> repo;
        private PosEntities db = new PosEntities(false);
        private DynSingleGridRepository svc;

        public DynSingleGridController()
        {
            svc = new DynSingleGridRepository(db);
        }

        public IEnumerable<DynGridModel> GetAll()
        {

            return GetAllDynGridModels.GetAll();
        }
       
        public DynSingleGridCreatorModel GetByEntityName(string entityName)
        {
            return svc.GetDynamicSingleGrid(entityName);
        }

        
       public EntityLookupsModel GetLookupsByEntityName(string lookupEntityName) {
            return svc.GetDynamicEntityLookups(lookupEntityName);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();

            base.Dispose(disposing);
        }
    }
}
