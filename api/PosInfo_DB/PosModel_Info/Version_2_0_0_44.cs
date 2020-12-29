using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [DisplayName("Ver : 2.0.0.44")]
    public class Version_2_0_0_44
    {

        public List<string> Ver_2_0_0_44 { get; }

        public Version_2_0_0_44()
        {
            Ver_2_0_0_44 = new List<string>();
            Ver_2_0_0_44.Add("IF NOT EXISTS (SELECT 1 FROM sys.tables AS t WHERE t.name = 'ReportsPos')  \n"
                           + "	RETURN  \n"
                           + "EXEC('ALTER VIEW [dbo].[SimplyOrdersPerMonthWaiter] AS \n"
                           + "    SELECT lst.Title, lst.Waiter, lst.WaiterName, lst.PosDescription, lst.FO_Month, lst.FO_Year, \n"
                           + "    SUM(lst.Cover) Cover, COUNT(DISTINCT(lst.OrderId)) Orders, \n"
                           + "    SUM(lst.DineIn) DineIn, SUM(lst.Delivery) Delivery, SUM(lst.TakeOut) TakeOut, \n"
                           + "    SUM(lst.DineIn + lst.Delivery + lst.TakeOut) Total \n"
                           + "    FROM( \n"
                           + "      SELECT rp.Waiter, rp.WaiterName, rp.PosDescription, MONTH(rp.eodFO_Day) FO_Month, YEAR(rp.eodFO_Day) FO_Year, rp.Cover, \n"
                           + "          rp.OrderId, ds.Title, \n"
                           + "          CASE WHEN rp.SalesTypeId = 1 THEN rp.Total ELSE 0 END DineIn, \n"
                           + "          CASE WHEN rp.SalesTypeId = 20 THEN rp.Total ELSE 0 END Delivery, \n"
                           + "          CASE WHEN rp.SalesTypeId = 21 THEN rp.Total ELSE 0 END TakeOut \n"
                           + "      FROM ReportsPos AS rp \n"
                           + "      INNER JOIN DA_Stores AS ds ON ds.Id = rp.StoreId \n"
                           + "    ) lst \n"
                           + "    GROUP BY lst.Title, lst.Waiter, lst.WaiterName, lst.PosDescription, lst.FO_Month, lst.FO_Year')	");
        }
    }
}
