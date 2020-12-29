using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pos_WebApi.Models.TransferMappingsBO
{
    public class UpdateTransferMappingsModel
    {
        public long HotelId { get; set; }
        public long ProdCatId { get; set; }
       public long newPmsDepartmentId { get; set; }
        public string newPmsDescr { get; set; }
        public long OldPmsDepartmentId { get; set; }
    }
}