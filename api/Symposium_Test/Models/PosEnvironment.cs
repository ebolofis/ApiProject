using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_Test.Models
{
    public class PosEnvironment
    {
        public List<AccountDTO> Accounts { get; set; }
        public DepartmentDTO Department { get; set; }
        public PosInfoDTO PosInfo { get; set; }
        public List<InvoiceTypesDTO> InvoiceTypes { get; set; }
        public List<PosInfoDetailDTO> PosInfoDetails { get; set; }
        public StaffDTO Staff { get; set; }
        public EndOfDayDTO EndOFDay { get; set; }
        public NFCcardDTO NFCCard { get; set; }
    }
}
