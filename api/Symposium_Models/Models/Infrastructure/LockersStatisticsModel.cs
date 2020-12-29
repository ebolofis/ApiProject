using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class LockersStatisticsModel
    {
        public int TotalLockers { get; set; }
        public int TotalCash { get; set; }
        public int ReturnCash { get; set; }
        public int CloseCash { get; set; }
        public int TotalSplashCash { get; set; }
        public int ReturnSplashCash { get; set; }
        public int CloseSplashCash { get; set; }
    }
}
