using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models {
    public class MealConsumptionModel
    {
        public long Id { get; set; }
        public long GuestId { get; set; }

        //katanalothenta geumata enilikon
        public int ConsumeMeals { get; set; }
        //Timestamp katanalosis
        public DateTime ConsumedTS { get; set; }
        public Nullable<bool> IsDeleted { get; set; }

        //kodikos dikeoumenou
        public string BoardCode { get; set; }

        //Id kratisis
        public long ReservationId { get; set; }

        //katanalothenta geumata paidion
        public int ConsumeMealsChild { get; set; }

        //tmima pou egine i katanalosi
        public long DepartmentId { get; set; }
        public long EndOfDayId { get; set; }
        public long PosInfoId { get; set; }      
        public string Code { get; set; }
    }
}
