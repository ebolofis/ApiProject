using System.Web.Http;
using Pos_WebApi.Repositories.BORepos;
using Pos_WebApi.Helpers;

namespace Pos_WebApi.Controllers.BOControllers
{
    public class EntitiesLookUpController : ApiController
    {
        LookUpFactoryRepository svc;
        private PosEntities db = new PosEntities(false);

        public EntitiesLookUpController()
        {
            svc = new LookUpFactoryRepository(db);
        }
        public object GetLookUpForEntity(int entityEnum)
        {
            return svc.GetLookUpsForEntity((EntitiesForLookUpFactoryEnum)entityEnum);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();

            base.Dispose(disposing);
        }
    }


}
