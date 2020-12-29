using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [DisplayName("Ver : 2.0.0.5")]
    public class Version_2_0_0_5
    {
        public List<string> Ver_2_0_0_5 { get; }

        public Version_2_0_0_5()
        {
            Ver_2_0_0_5 = new List<string>();
            Ver_2_0_0_5.Add("CREATE NONCLUSTERED INDEX [HashCode] ON [dbo].[Invoices] ([Hashcode] ASC)");
        }
    }
}
