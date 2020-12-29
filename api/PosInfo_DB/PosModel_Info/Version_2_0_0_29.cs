﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [DisplayName("Ver : 2.0.0.29")]
    public class Version_2_0_0_29
    {
        public List<string> Ver_2_0_0_29 { get; }

        public Version_2_0_0_29()
        {
            Ver_2_0_0_29 = new List<string>();
            Ver_2_0_0_29.Add("ALTER VIEW[dbo].[VIEW_RPT01_InvoiceHeader] AS \n"
                           + "      SELECT i.Id InvoiceId \n"
                           + "           , i.TableSum TableTotal \n"
                           + "           , 0 PaymentTypeId \n"
                           + "           , 'false' PrintKitchen \n"
                           + "           , ISNULL(pid.[Description] \n"
                           + "           , it.[Description]) ReceiptTypeDescription \n"
                           + "           , d.[Description] DepartmentTypeDescription \n"
                           + "           , NULL PaidAmount \n"
                           + "           , st.[Description] SalesTypeDescription \n"
                           + "           , NULL ItemsCount \n"
                           + "           , ISNULL(o.Couver,0) Couver \n"
                           + "           , NULL PrintFiscalSign \n"
                           + "           , 0 FiscalType \n"
                           + "           , NULL DetailsId \n"
                           + "           , pid.InvoiceId InvoiceIndex \n"
                           + "		   , i.OrderNo OrderId \n"
                           + "           , ISNULL(t.Code,'') TableNo \n"
                           + "           , ISNULL(trTop.Room,'') Room \n"
                           + "           , ISNULL(s.LastName,'') Waiter \n"
                           + "           , ISNULL(s.Code,'') WaiterNo \n"
                           + "           , i.PosInfoId Pos \n"
                           + "           , ISNULL(pif.[Description],'') PosDescr \n"
                           + "           , d.[Description] DepartmentDesc \n"
                           + "           , pif.DepartmentId Department \n"
                           + "           , ISNULL(a.AccountDescription, ISNULL(i.PaymentsDesc,'')) AccountDescription \n"
                           + "           , CASE WHEN ISNULL(isd.CustomerName,'') <> '' THEN isd.CustomerName \n"
                           + "                  WHEN ISNULL(trTop.GuestName,'') <> '' THEN trTop.GuestName \n"
                           + "                  ELSE 'Πελάτης Λιανικής' END CustomerName \n"
                           + "           , ISNULL(isd.BillingAddress,'') CustomerAddress \n"
                           + "           , ISNULL(isd.ShippingAddress,'') CustomerDeliveryAddress \n"
                           + "           , ISNULL(isd.Phone,'') CustomerPhone \n"
                           + "           , ISNULL(isd.[Floor],'') [Floor] \n"
                           + "           , ISNULL(isd.BillingCity,'') City \n"
                           + "           , ISNULL(isd.CustomerRemarks,'') CustomerComments \n"
                           + "           , ISNULL(isd.BillingVatNo,'') CustomerAfm \n"
                           + "           , ISNULL(isd.BillingDOY,'') CustomerDoy \n"
                           + "           , ISNULL(isd.BillingJob,'') CustomerJob \n"
                           + "           , ISNULL(hpc.company_name,'') CompanyName \n"
                           + "           , NULL RegNo \n"
                           + "           , NULL SumOfLunches \n"
                           + "           , NULL SumofConsumedLunches \n"
                           + "           , ISNULL(trTop.BoardCode,'') GuestTerm \n"
                           + "           , ISNULL(trTop.Adults,0) Adults \n"
                           + "           , ISNULL(trTop.Children,0) Kids \n"
                           + "           , it.[Type] InvoiceType \n"
                           + "           , 0 TotalVat \n"
                           + "           , 0 TotalVat1 \n"
                           + "           , 0 TotalVat2 \n"
                           + "           , 0 TotalVat3 \n"
                           + "           , 0 TotalVat4 \n"
                           + "           , 0 TotalVat5 \n"
                           + "           , CASE WHEN i.Discount <> 0 THEN i.Discount ELSE ISNULL(odi.Discount,0) END TotalDiscount \n"
                           + "           , 0 Bonus \n"
                           + "           , 0 PriceList \n"
                           + "           , i.[Counter] ReceiptNo \n"
                           + "           , i.OrderNo OrderNo \n"
                           + "           , NULL OrderComments \n"
                           + "           , ISNULL(i.Net,0) TotalNet \n"
                           + "           , ISNULL(i.Total,0) Total \n"
                           + "           , NULL Change \n"
                           + "           , ISNULL(i.CashAmount,0) CashAmount \n"
                           + "           , ISNULL(i.BuzzerNumber,'') BuzzerNumber \n"
                           + "           , i.IsVoided IsVoid \n"
                           + "           , NULL CreditTransactions \n"
                           + "           , i.endofdayid \n"
                           + "		   ,convert(varchar(10),i.day,120)as foday \n"
                           + "           , i.Day \n"
                           + "		   , ISNULL(inv.counter,-1) CanceledCounter \n"
                           + "	FROM Invoices i \n"
                           + "	INNER JOIN InvoiceTypes it ON it.Id = i.InvoiceTypeId \n"
                           + "	left outer JOIN endofday ib ON ib.Id = i.EndOfDayId \n"
                           + "	LEFT OUTER JOIN PosInfoDetail pid ON pid.Id = i.PosInfoDetailId \n"
                           + "	OUTER APPLY ( \n"
                           + "        SELECT SUM(o.Couver) Couver, o.ExtKey \n"
                           + "        FROM [Order] o   \n"
                           + "        WHERE o.OrderNo = i.OrderNo \n"
                           + "        GROUP BY o.ExtKey \n"
                           + "	) o \n"
                           + "	OUTER APPLY ( \n"
                           + "		SELECT MAX(hpo.start_time) start_time, MAX(hpo.Payment) Payment, MAX(hpo.room) room \n"
                           + "		FROM HitPosOrders hpo \n"
                           + "		WHERE hpo.orderno = CAST(LTRIM(RTRIM(o.ExtKey)) AS INT) \n"
                           + "	) hpo \n"
                           + "	OUTER APPLY ( \n"
                           + "		select top 1 * \n"
                           + "		from HitPosCustomers hpc \n"
                           + "		where hpc.customerid = hpo.room \n"
                           + "	) hpc \n"
                           + "	INNER JOIN PosInfo pif ON pif.Id = i.PosInfoId \n"
                           + "    INNER JOIN Department d ON d.Id = pif.DepartmentId \n"
                           + "	OUTER APPLY ( \n"
                           + "		SELECT odi.SalesTypeId, SUM(odi.Discount) Discount, SUM(odi.Net) Net, SUM(odi.Total) Total \n"
                           + "		FROM OrderDetailInvoices odi \n"
                           + "		WHERE odi.InvoicesId = i.Id \n"
                           + "		GROUP BY odi.SalesTypeId \n"
                           + "	) odi \n"
                           + "	OUTER APPLY ( \n"
                           + "		SELECT TOP 1 * \n"
                           + "		FROM SalesType st WHERE st.Id = odi.SalesTypeId   \n"
                           + "	) st \n"
                           + "	LEFT OUTER JOIN [Table] t ON t.Id = i.TableId \n"
                           + "    OUTER APPLY ( \n"
                           + "        SELECT TOP 1 ISNULL(g.Room,'') Room \n"
                           + "                    , tr.AccountId \n"
                           + "                    , ISNULL(g.FirstName,'') + ' ' + ISNULL(g.LastName,'') GuestName \n"
                           + "                    , g.Id GuestId \n"
                           + "                    , g.BoardCode \n"
                           + "                    , g.Adults \n"
                           + "                    , g.Children \n"
                           + "        FROM Transactions tr \n"
                           + "        LEFT OUTER JOIN Invoice_Guests_Trans igt ON igt.TransactionId = tr.Id \n"
                           + "        LEFT OUTER JOIN Guest g ON g.Id = igt.GuestId \n"
                           + "        WHERE tr.InvoicesId = i.Id   \n"
                           + "	) trTop \n"
                           + "    INNER JOIN Staff s ON s.Id = i.StaffId \n"
                           + "	OUTER APPLY ( \n"
                           + "		SELECT TOP 1 a.[Description] AccountDescription \n"
                           + "		FROM Accounts a   \n"
                           + "		WHERE a.Id = trTop.AccountId \n"
                           + "	) a \n"
                           + "	LEFT OUTER JOIN InvoiceShippingDetails isd ON \n"
                           + "              isd.InvoicesId = CASE WHEN it.Type <> 3 THEN i.Id \n"
                           + "								ELSE (SELECT TOP 1 inv.ID  \n"
                           + "								      FROM Invoices inv \n"
                           + "								      INNER JOIN InvoiceTypes AS itm ON itm.Id = inv.InvoiceTypeId AND itm.[Type] <> 3  \n"
                           + "								      WHERE LTRIM(RTRIM(inv.OrderNo)) = LTRIM(RTRIM(i.OrderNo)) AND inv.PosInfoId = i.PosInfoId \n"
                           + "								) END \n"
                           + "	LEFT OUTER JOIN Invoices inv ON inv.Id = isd.InvoicesId");
        }

    }
}
