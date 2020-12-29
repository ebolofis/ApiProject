using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pos_WebApi {
    public class PatchManager {
        private class PatchDescriptor {
            public DataIntegrityPatch Patch {
                get;set;
            }
            public bool HasRun {
                get;set;
            }
        }
        public static readonly PatchManager Instance = new PatchManager();

        List<PatchDescriptor> patchList;
        bool patchesLoaded;
        log4net.ILog logger;
        private PatchManager( ) {
            this.patchList = new List<PatchDescriptor>();
            this.patchesLoaded = false;
            logger = log4net.LogManager.GetLogger(this.GetType());
        }
        private void LoadPatches( ) {
            if ( !this.patchesLoaded ) {
                var patchTypes = this.GetType().Assembly.GetTypes().Where(x => x.BaseType == typeof(DataIntegrityPatch)).ToArray();
                this.patchList.Clear();
                foreach ( var t in patchTypes ) {
                    var d = new PatchDescriptor() {
                        HasRun = false,
                        Patch = (DataIntegrityPatch) Activator.CreateInstance(t),
                    };
                    this.patchList.Add(d);
                }
                this.patchesLoaded = true;
            }
        }
        public void RunPatches( PosEntities dbContext ) {
            lock ( this ) {
                LoadPatches();
                var patchesToRun = this.patchList.Where(x => x.HasRun == false);
                foreach ( var patch in patchesToRun ) {
                    try {
                        patch.Patch.ApplyPatch(dbContext);
                        patch.HasRun = true;
                    }catch(Exception x) {
                        var patchLogger = log4net.LogManager.GetLogger(patch.GetType());
                        patchLogger.Error(x);
                    }
                }
            }
        }
    }
}