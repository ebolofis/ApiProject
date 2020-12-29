using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.AlhuaRecorder
{
    public class PosRecorderModel
    {
        public string ipAddress { get; set; }
        public string port { get; set; }
        public string Message { get; set; }
        public long PosInfoId { get; set; }
    }
}
