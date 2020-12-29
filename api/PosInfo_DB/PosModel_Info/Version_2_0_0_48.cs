using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [DisplayName("Ver : 2.0.0.48")]
    public class Version_2_0_0_48
    {
        public List<string> Ver_2_0_0_48 { get; }

        public Version_2_0_0_48()
        {
            Ver_2_0_0_48 = new List<string>();
            Ver_2_0_0_48.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes AS i WHERE i.name = 'IX_AssignedPositions_StaffId') \n"
                           + "	CREATE NONCLUSTERED INDEX [IX_AssignedPositions_StaffId] ON [dbo].[AssignedPositions] \n"
                           + "	( \n"
                           + "		[StaffId] ASC \n"
                           + "	)");

            Ver_2_0_0_48.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes AS i WHERE i.name = 'IX_PayrollNew_DateFrom') \n"
                           + "	CREATE NONCLUSTERED INDEX [IX_PayrollNew_DateFrom] ON [dbo].[PayrollNew] \n"
                           + "	( \n"
                           + "		[DateFrom] ASC \n"
                           + "	)");

            Ver_2_0_0_48.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes AS i WHERE i.name = 'IX_PayrollNew_PosInfoId') \n"
                           + "	CREATE NONCLUSTERED INDEX [IX_PayrollNew_PosInfoId] ON [dbo].[PayrollNew] \n"
                           + "	( \n"
                           + "		[PosInfoId] ASC \n"
                           + "	)");

            Ver_2_0_0_48.Add("IF NOT EXISTS (SELECT 1 FROM sys.indexes AS i WHERE i.name = 'IX_PayrollNew_StaffId') \n"
                           + "	CREATE NONCLUSTERED INDEX [IX_PayrollNew_StaffId] ON [dbo].[PayrollNew] \n"
                           + "	( \n"
                           + "		[StaffId] ASC \n"
                           + "	)");
        }
    }
}
