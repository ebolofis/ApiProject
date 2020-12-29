using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Orders
{
    public class PMSChargeModel
    {
        [Required]
        public long HotelInfoId { get; set; }
        [Required]
        public PMSCustomerModel Customer { get; set; }
        [Required]
        public List<PMSDepartmentModel> Departments { get; set; }
    }

    public class PMSCustomerModel
    {
        [Required]
        public int ReservationId { get; set; }
        [Required]
        public int ProfileNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public int RoomId { get; set; }
        [Required]
        public string Room { get; set; }
    }

    public class PMSDepartmentModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }

    public class PMSReceiptHTMLModel
    {
        [Required]
        public long InvoiceId { get; set; }
        [Required]
        public string HtmlReceipt { get; set; }
    }

}
