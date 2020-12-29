using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [DisplayName("Ver : 2.0.0.17")]
    public class Version_2_0_0_17
    {
        public List<string> Ver_2_0_0_17 { get; }

        public Version_2_0_0_17()
        {
            Ver_2_0_0_17 = new List<string>();
            Ver_2_0_0_17.Add("IF NOT EXISTS (SELECT 1 FROM TR_Restrictions AS tr WHERE tr.Id = 1) \n"
                           + "	INSERT INTO TR_Restrictions (Id,[Description]) VALUES (1,'Έως Ν φορές την εβδομάδα στο ίδιο εστιατόριο'); \n"
                           + "IF NOT EXISTS (SELECT 1 FROM TR_Restrictions AS tr WHERE tr.Id = 2) \n"
                           + "	INSERT INTO TR_Restrictions (Id,[Description]) VALUES (2,'Έως Ν Άτομα ανά κράτηση'); \n"
                           + "IF NOT EXISTS (SELECT 1 FROM TR_Restrictions AS tr WHERE tr.Id = 3) \n"
                           + "	INSERT INTO TR_Restrictions (Id,[Description]) VALUES (3,'Έως Ν φορές την εβδομάδα δείπνο σε αυτό το εστιατόριο'); \n"
                           + "IF NOT EXISTS (SELECT 1 FROM TR_Restrictions AS tr WHERE tr.Id = 4) \n"
                           + "	INSERT INTO TR_Restrictions (Id,[Description]) VALUES (4,'Έως Ν φορές την εβδομάδα γεύμα σε αυτό το εστιατόριο'); \n"
                           + "IF NOT EXISTS (SELECT 1 FROM TR_Restrictions AS tr WHERE tr.Id = 5) \n"
                           + "	INSERT INTO TR_Restrictions (Id,[Description]) VALUES (5,'Έως Ν κρατήσεις  την ημέρα ανά δωμάτιο');");
            Ver_2_0_0_17.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes AS i WHERE i.name = 'IX_OrderDetailInvoicesInvoicesId') \n"
                           + "	CREATE NONCLUSTERED INDEX [IX_OrderDetailInvoicesInvoicesId] ON [dbo].[OrderDetailInvoices] \n"
                           + "	( \n"
                           + "		[InvoicesId] ASC \n"
                           + "	)");
        }
    }
}
