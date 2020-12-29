using log4net;
using Newtonsoft.Json;
using Pos_WebApi.Helpers;
using Pos_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pos_WebApi.Repositories.BORepos {
    public class DynSingleGridRepository //: IDynSingleGridRepository
    {
        protected PosEntities DbContext;
        protected LookUpFactoryRepository repo;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DynSingleGridRepository(PosEntities db) {
            this.DbContext = db;
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
            repo = new LookUpFactoryRepository(db);
        }

        public DynSingleGridCreatorModel GetDynamicSingleGrid(string entityName) {
            var res = new DynSingleGridCreatorModel();
            res.SimpleGrid = GetAllDynGridModels.GetAll().Where(q => q.EntityName == entityName).FirstOrDefault();
            try {
                if (res.SimpleGrid != null && res.SimpleGrid.ColumnDependencies != "" && res.SimpleGrid.ColumnDependencies != null) {
                    var objRes = JsonConvert.DeserializeObject<dynamic>(res.SimpleGrid.ColumnDependencies);
                    string objStr = objRes["enumIdentifier"];
                    var lookupInt = ((EntitiesForLookUpFactoryEnum)Enum.Parse(typeof(EntitiesForLookUpFactoryEnum), objStr));
                    var lookUpResult = repo.GetLookUpsForEntity(lookupInt);
                    res.LookUpEntities = lookUpResult;
                } else { res.LookUpEntities = null; }
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                int pa = 0;
                //ex fails on DeserializeObject
                //ex fail on entity mapping 
                //ex fail on lookupload
                //todo exception handler over ex fail
            }
            return res;
        }
        /// <summary>
        /// Function to return results over givven lookupEntity name
        /// Provide a name to LookupFactory and get an object of lists as a lookup
        /// </summary>
        /// <param name="entityName">Provide modelEntity name</param>
        /// <returns></returns>
        public EntityLookupsModel GetDynamicEntityLookups(string entityName) {
            EntityLookupsModel res = new EntityLookupsModel();
            try {
                if (entityName != null && entityName != "") {
                    var lookUpResults = repo.GetLookUpsForEntity((EntitiesForLookUpFactoryEnum)Enum.Parse(typeof(EntitiesForLookUpFactoryEnum), entityName));
                    res.LookUpEntities = lookUpResults;
                } else { res.LookUpEntities = null; }
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                //ex fails on DeserializeObject //ex fail on entity mapping  //ex fail on lookupload //todo exception handler over ex fail
            }
            return res;
        }


    }
    /// <summary>
    /// A single Return Object for Lookupsinfo
    /// </summary>
    public class EntityLookupsModel {
        public Object LookUpEntities { get; set; }

    }
}

