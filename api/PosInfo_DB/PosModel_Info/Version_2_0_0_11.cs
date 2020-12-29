using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [DisplayName("Ver : 2.0.0.11")]
    public class Version_2_0_0_11
    {
        public List<string> Ver_2_0_0_11 { get; }

        public Version_2_0_0_11()
        {
            Ver_2_0_0_11 = new List<string>();
            Ver_2_0_0_11.Add("CREATE NONCLUSTERED INDEX [IX_Order_EndOfDayId] ON [dbo].[Order] \n "
                            + " ( \n"
                            + "     [EndOfDayId] ASC \n"
                            + " )");
            Ver_2_0_0_11.Add("CREATE NONCLUSTERED INDEX [IX_MealConsumption_EndOfDayId] ON [dbo].[MealConsumption] \n "
                            + " ( \n"
                            + "     [EndOfDayId] ASC \n"
                            + " )");
            Ver_2_0_0_11.Add("CREATE NONCLUSTERED INDEX [IX_CreditTransactions_EndOfDayId] ON [dbo].[CreditTransactions] \n "
                            + " ( \n"
                            + "     [EndOfDayId] ASC \n"
                            + " )");
            Ver_2_0_0_11.Add("CREATE NONCLUSTERED INDEX [IX_KitchenInstructionLogger_EndOfDayId] ON [dbo].[KitchenInstructionLogger] \n "
                            + " ( \n"
                            + "     [EndOfDayId] ASC \n"
                            + " )");
            Ver_2_0_0_11.Add("CREATE NONCLUSTERED INDEX [IX_Order_EndOfDayId_PosId] ON [dbo].[Order] \n "
                            + " ( \n"
                            + "     [EndOfDayId] ASC, \n"
                            + "     [PosId] ASC \n"
                            + " )");
            Ver_2_0_0_11.Add("CREATE NONCLUSTERED INDEX [IX_Invoices_EndOfDayId_PosInfoId] ON [dbo].[Invoices] \n "
                            + " ( \n"
                            + "     [EndOfDayId] ASC, \n"
                            + "     [PosInfoId] ASC \n"
                            + " )");
            Ver_2_0_0_11.Add("CREATE NONCLUSTERED INDEX [IX_OrderDetailInvoices_EndOfDayId_PosInfoId] ON [dbo].[OrderDetailInvoices] \n "
                            + " ( \n"
                            + "     [EndOfDayId] ASC, \n"
                            + "     [PosInfoId] ASC \n"
                            + " )");
            Ver_2_0_0_11.Add("CREATE NONCLUSTERED INDEX [IX_Transactions_EndOfDayId_PosInfoId] ON [dbo].[Transactions] \n "
                            + " ( \n"
                            + "     [EndOfDayId] ASC, \n"
                            + "     [PosInfoId] ASC \n"
                            + " )");
            Ver_2_0_0_11.Add("CREATE NONCLUSTERED INDEX [IX_TransferToPms_EndOfDayId_PosInfoId] ON [dbo].[TransferToPms] \n "
                            + " ( \n"
                            + "     [EndOfDayId] ASC, \n"
                            + "     [PosInfoId] ASC \n"
                            + " )");
            Ver_2_0_0_11.Add("CREATE NONCLUSTERED INDEX [IX_MealConsumption_EndOfDayId_PosInfoId] ON [dbo].[MealConsumption] \n "
                            + " ( \n"
                            + "     [EndOfDayId] ASC, \n"
                            + "     [PosInfoId] ASC \n"
                            + " )");
            Ver_2_0_0_11.Add("CREATE NONCLUSTERED INDEX [IX_CreditTransactions_EndOfDayId_PosInfoId] ON [dbo].[CreditTransactions] \n "
                            + " ( \n"
                            + "     [EndOfDayId] ASC, \n"
                            + "     [PosInfoId] ASC \n"
                            + " )");
            Ver_2_0_0_11.Add("CREATE NONCLUSTERED INDEX [IX_KitchenInstructionLogger_EndOfDayId_PosInfoId] ON [dbo].[KitchenInstructionLogger] \n "
                            + " ( \n"
                            + "     [EndOfDayId] ASC, \n"
                            + "     [PosInfoId] ASC \n"
                            + " )");
        }
    }
}
