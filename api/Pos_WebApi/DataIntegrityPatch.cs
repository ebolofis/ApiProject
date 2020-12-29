using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pos_WebApi {
    public abstract class DataIntegrityPatch {
        public abstract void ApplyPatch( PosEntities dbContext );
    }
}