using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [DisplayName("Ver : 2.0.0.39")]
    public class Version_2_0_0_39
    {
        public List<string> Ver_2_0_0_39 { get; }

        public Version_2_0_0_39()
        {
            Ver_2_0_0_39 = new List<string>();
            Ver_2_0_0_39.Add("CREATE VIEW OrdersAnalysis AS  \n"
                           + "	SELECT CASE WHEN ISNULL(i.IsPaid,0) = 0 THEN 0 ELSE 1 END IsPaid, \n"
                           + "		ISNULL(i.IsVoided,0) IsVoided, v.* \n"
                           + "	FROM VIEW_RPT01_InvoiceHeader AS v \n"
                           + "	INNER JOIN Invoices AS i ON i.Id = v.InvoiceId \n"
                           + "	INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId \n"
                           + "	WHERE v.endofdayid = 0 AND   \n"
                           + "		CASE WHEN ISNULL(i.IsInvoiced,0) = 1 AND it.[Type] = 2 THEN 0 \n"
                           + "		ELSE 1 END = 1 AND  \n"
                           + "		CASE WHEN it.[type] NOT IN (3,8) AND ISNULL(i.IsVoided,0) = 1 THEN 0 ELSE 1 END = 1");
        }
    }
}
