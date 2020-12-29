using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class LockersModel
    {
        public long Id { get; set; }
        public bool HasLockers { get; set; }
        public int TotalLockers { get; set; }
        public decimal TotalLockersAmount { get; set; }
        public int Paidlockers { get; set; }
        public decimal PaidlockersAmount { get; set; }

        //anixta lockers
        public int OccLockers { get; set; }

        //sinoliko poso apo nixta lockers
        public decimal OccLockersAmount { get; set; }
        public Nullable<long> EndOfDayId { get; set; }

        public long PosInfoId { get; set; }

        public int TotalCash { get; set; }
        public int TotalSplashCash { get; set; }
        public int ReturnCash { get; set; }
        public int ReturnSplashCash { get; set; }
        public int CloseCash { get; set; }
        public int CloseSplashCash { get; set; }
    }
}
