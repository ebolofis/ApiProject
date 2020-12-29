using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pos_WebApi.Models
{
    public class PmsDepartmentModel
    {
        public string Description { get; set; }

        public int DepartmentId { get; set; }

        public  double Vat { get; set; }
    }
}