using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [DisplayName("Ver : 2.0.0.27")]
    public class Version_2_0_0_27
    {
        public List<string> Ver_2_0_0_27 { get; }

        public Version_2_0_0_27()
        {
            Ver_2_0_0_27 = new List<string>();
            Ver_2_0_0_27.Add("CREATE NONCLUSTERED INDEX [InvoiceShippingDetails_InvoicesId_ShippingAddess] ON [dbo].[InvoiceShippingDetails] \n"
                           + "( \n"
                           + "    [InvoicesId] ASC, \n"
                           + "    [ShippingAddress] ASC \n"
                           + ")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)");
        }
    }
}
