using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models {
    public class KitchenInstructionLoggerModel
    {
        public long Id { get; set; }

        //to minima pros tin kouzina
        public long KicthcenInstuctionId { get; set; }
        public long StaffId { get; set; }

        //to PDA ekdosis tou miniatos
        public Nullable<int> PdaModulId { get; set; }

        //to client POS ekdosis minimatos
        public Nullable<int> ClientId { get; set; }
        public long PosInfoId { get; set; }

        //Timestamp apostolis minimatos
        public DateTime SendTS { get; set; }

        //Timestamp lipsis minimatos
        public DateTime ReceivedTS { get; set; }
        public long EndOfDayId { get; set; }

        //1 = energo, 0 = anenergo
        public int Status { get; set; }
        public long TableId { get; set; }
        public string ExtecrName { get; set; }

        //perigrafi minimatos
        public string Description { get; set; }
    }
}
