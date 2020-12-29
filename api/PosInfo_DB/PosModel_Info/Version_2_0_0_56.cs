using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [DisplayName("Ver : 2.0.0.56")]
    public class Version_2_0_0_56
    {
        public List<string> Ver_2_0_0_56 { get; }

        public Version_2_0_0_56()
        {
            Ver_2_0_0_56 = new List<string>();

            Ver_2_0_0_56.Add("  IF NOT EXISTS (SELECT 1 FROM sys.indexes AS i WHERE i.name = 'IX_InvoiceQR_InvoiceId') \n"
                           + "  BEGIN \n"
                           + "	    IF EXISTS(SELECT 1 FROM sys.tables WHERE name = 'InvoiceQR') \n"
                           + "	    BEGIN \n"
                           + "	        CREATE NONCLUSTERED INDEX[IX_InvoiceQR_InvoiceId] ON[dbo].[InvoiceQR] \n"
                           + "	        ( \n"
                           + "	            [InvoiceId] ASC \n"
                           + "	        ); \n"
                           + "	    END; \n"
                           + "	END;");

        }
    }
}
