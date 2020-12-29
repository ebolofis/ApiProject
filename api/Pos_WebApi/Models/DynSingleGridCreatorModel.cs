using System;

namespace Pos_WebApi.Models
{
    public class DynSingleGridCreatorModel
    {
        public DynGridModel SimpleGrid { get; set; }
        public Object LookUpEntities { get; set; }

        public static DynSingleGridCreatorModel GetDynamicSingleGrid(object entityName)
        {
            throw new NotImplementedException();
        }
    }
}
