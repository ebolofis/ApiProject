using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.TableReservations
{
    public class ReservationTypeModel
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
    }
}
