using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class NFCcardModel
    {
        public long Id { get; set; }
        public int Type { get; set; }
        public int RoomSector { get; set; }
        public int FirstDateSector { get; set; }
        public int SecondDateSector { get; set; }
        public bool ValidateDate { get; set; }
    }
}
