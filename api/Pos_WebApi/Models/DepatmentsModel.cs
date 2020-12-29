using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pos_WebApi.Models
{
    public class DepatmentsModel
    {
    }

    public class GroupedProducts
    {
        public string Category { get; set; }

      

        public IEnumerable<Prod> Products { get; set; }
    }

    public class Prod
    {
        public string Description { get; set; }

        public Nullable<long> Id { get; set; }

        public IEnumerable<decimal?> Vats { get; set; } 
        
        public Nullable<long> CategoryId { get; set; }
    }

    public class TransferObject
    {
        public long? Id { get; set; }
        public Guid? TransferIdentifier { get; set; }

        public int HotelId {get; set;}
        public string HotelUri { get; set; }
        public decimal amount { get; set; }
        public int resId { get; set; }
        public string profileName { get; set; }
        public string description { get; set; }
        public int departmentId { get; set; }
        public string pmsDepartmentDescription { get; set; }
        public string RoomName { get; set; }

        public double? VatPercentage { get; set; }
    }
}