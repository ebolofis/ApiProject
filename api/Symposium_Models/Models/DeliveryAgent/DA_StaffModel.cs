using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.DeliveryAgent
{
    public class DA_StaffModel
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ImageUri { get; set; }

        public string Password { get; set; }

        public Nullable<bool> IsDeleted { get; set; }

        public string Identification { get; set; }

        public long DAStore { get; set; }

        public Nullable<bool> isAdmin { get; set; }

    }

    public class DALoginStaffModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }

      
    }
}
