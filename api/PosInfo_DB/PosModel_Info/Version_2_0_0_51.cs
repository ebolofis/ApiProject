using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [DisplayName("Ver : 2.0.0.51")]
    public class Version_2_0_0_51
    {
        public List<string> Ver_2_0_0_51 { get; }
        public Version_2_0_0_51()
        {
            Ver_2_0_0_51 = new List<string>();

            Ver_2_0_0_51.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes AS i WHERE i.name = 'IX_Order_DeliveryRoutingId') \n"
                           + "	CREATE NONCLUSTERED INDEX [IX_Order_DeliveryRoutingId] ON [dbo].[Order] \n"
                           + "	( \n"
                           + "		[DeliveryRoutingId] ASC \n"
                           + "	)");

            Ver_2_0_0_51.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes AS i WHERE i.name = 'IX_DeliveryRouting_Status') \n"
                           + "	CREATE NONCLUSTERED INDEX [IX_DeliveryRouting_Status] ON [dbo].[DeliveryRouting] \n"
                           + "	( \n"
                           + "		[Status] ASC \n"
                           + "	)");

            Ver_2_0_0_51.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes AS i WHERE i.name = 'IX_DeliveryRouting_CreateDate') \n"
                           + "	CREATE NONCLUSTERED INDEX [IX_DeliveryRouting_CreateDate] ON [dbo].[DeliveryRouting] \n"
                           + "	( \n"
                           + "		[CreateDate] ASC \n"
                           + "	)");

            Ver_2_0_0_51.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes AS i WHERE i.name = 'IX_DeliveryRouting_StaffId_AssignStatus') \n"
                           + "	CREATE NONCLUSTERED INDEX [IX_DeliveryRouting_StaffId_AssignStatus] ON [dbo].[DeliveryRouting] \n"
                           + "	( \n"
                           + "		[StaffId] ASC, [AssignStatus] ASC\n"
                           + "	)");
        }
    }
}
