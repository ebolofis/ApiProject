using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pos_WebApi.Models
{
    public class OnlineRegistrations
    {
        public string BarCode { get; set; }
        public string FirtName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public int Dates { get; set; }
        public int Children { get; set; }
        public int Adults { get; set; }
        public int PaymentType { get; set; }
        public decimal ChildTicket { get; set; }
        public decimal AdultTicket { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int TotalChildren { get; set; }
        public int TotalAdults { get; set; }
        public int ChildrenEntered { get; set; }
        public int AdultsEntered { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public int Status { get; set; }
        public string ReturnMessage { get; set; }

    }
}
