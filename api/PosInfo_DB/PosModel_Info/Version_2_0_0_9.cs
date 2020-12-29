using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [DisplayName("Ver : 2.0.0.9")]
    public class Version_2_0_0_9
    {
        public List<string> Ver_2_0_0_9 { get; }

        public Version_2_0_0_9()
        {
            Ver_2_0_0_9 = new List<string>();
            Ver_2_0_0_9.Add("ALTER TABLE [dbo].[ReportEntity] ADD  CONSTRAINT [IX_ReportEntity] UNIQUE NONCLUSTERED \n"
                            + "( \n"
                            + "[Url] ASC \n"
                            + ")");
        }

    }
}
