﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [DisplayName("Ver : 2.0.0.12")]
    public class Version_2_0_0_12
    {
        public List<string> Ver_2_0_0_12 { get; }

        public Version_2_0_0_12()
        {
            Ver_2_0_0_12 = new List<string>();
            Ver_2_0_0_12.Add("ALTER PROCEDURE [dbo].[EndOfDayTotal] @posInfo BIGINT AS  \n"
                           + "BEGIN  \n"
                           + "	/*Gets data from tables Invoice, Transaction, InvoiceType and Account and return total records for each case*/  \n"
                           + "	SELECT *  \n"
                           + "	FROM (  \n"
                           + "		/*	Accounts Total Data*/  \n"
                           + "		SELECT 0 id, a.[Description], COUNT(i.id) ReceiptCount, ISNULL(SUM(t.Amount),0) Amount, a.Id AccountId, a.[Type] AccountType  \n"
                           + "		FROM Transactions AS t  \n"
                           + "		INNER JOIN Accounts AS a ON a.id = t.AccountId  \n"
                           + "		INNER JOIN Invoices AS i ON i.id = t.InvoicesId AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0  \n"
                           + "		INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,3,8,10)  \n"
                           + "		WHERE t.PosInfoId = @posInfo AND t.EndOfDayId IS NULL AND ISNULL(t.IsDeleted,0) <> 1 AND   \n"
                           + "			((NOT (t.TransactionType = 9 AND t.InOut = 1))) AND t.TransactionType NOT IN (8,7)  \n"
                           + "		GROUP BY a.id, a.[Description], a.[Type]  \n"
                           + "		UNION ALL  \n"
                           + "		/*	Not Paid Recipies Total Data*/  \n"
                           + "		SELECT fin.Id,fin.[Description], SUM(fin.Reciepts) Reciepts, ISNULL(SUM(fin.amount),0) amount, 0 AccountId, 0 AccountType   \n"
                           + "		FROM (	  \n"
                           + "			/*	Recipies Without Transaction. The Field ID of Invoices id not in InvoicesId of Transaction */  \n"
                           + "			SELECT -99 Id, 'Not Paid' [Description], COUNT(DISTINCT(i.id)) Reciepts, ISNULL(SUM(i.Total),0) amount  \n"
                           + "			FROM Invoices AS i  \n"
                           + "			INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,3,8,10,11,12)  \n"
                           + "			LEFT OUTER JOIN Transactions AS t ON t.InvoicesId = i.Id  \n"
                           + "			WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0  \n"
                           + "			 AND t.InvoicesId IS NULL  \n"
                           + "			UNION ALL  \n"
                           + "			/*	The Field IsPaid of Invoices id equal to 1 */  \n"
                           + "			SELECT -99 Id, 'Not Paid', COUNT(DISTINCT(i.id)) Reciepts, ISNULL(SUM(ISNULL(i.Total,0) - ISNULL(i.PaidTotal,0)),0) amount  \n"
                           + "			FROM Invoices AS i  \n"
                           + "			INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,3,8,10,11,12)  \n"
                           + "			WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0 AND ISNULL(i.IsPaid,0) = 1  \n"
                           + "		) fin  \n"
                           + "		GROUP BY fin.Id, fin.[Description]  \n"
                           + "		UNION ALL  \n"
                           + "		/*	Recipies Total Data*/  \n"
                           + "		SELECT fin.Id,fin.Decription, SUM(fin.Reciepts) Reciepts, ISNULL(SUM(fin.amount),0) amount, 0 AccountId, 0 AccountType  \n"
                           + "		FROM (  \n"
                           + "			/*	Recipies With Transaction. Id of Invoice is equal to InvoicesId of Transaction */  \n"
                           + "			SELECT-100 Id, 'Receipt Total' Decription, COUNT(DISTINCT(t.InvoicesId)) Reciepts, ISNULL(SUM(t.Amount),0) amount  \n"
                           + "			FROM Transactions AS t  \n"
                           + "			INNER JOIN Accounts AS a ON a.id = t.AccountId  \n"
                           + "			INNER JOIN Invoices AS i ON i.id = t.InvoicesId AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0  \n"
                           + "			INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,3,8,10)  \n"
                           + "			WHERE t.PosInfoId = @posInfo AND t.EndOfDayId IS NULL AND ISNULL(t.IsDeleted,0) <> 1 AND   \n"
                           + "				((NOT (t.TransactionType = 9 AND t.InOut = 1))) AND t.TransactionType NOT IN (8,7)  \n"
                           + "			UNION ALL  \n"
                           + "			/*	Recipies Without Transaction. InvoicesId of Transaction is null */  \n"
                           + "			SELECT -100 Id, 'Receipt Total' [Description], COUNT(DISTINCT(i.id)) Reciepts, ISNULL(SUM(i.Total),0) amount  \n"
                           + "			FROM Invoices AS i  \n"
                           + "			INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,3,8,10,11,12)  \n"
                           + "			LEFT OUTER JOIN Transactions AS t ON t.InvoicesId = i.Id  \n"
                           + "			WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0  \n"
                           + "			 AND t.InvoicesId IS NULL  \n"
                           + "			UNION ALL  \n"
                           + "			/*	Recipies Without Transaction. The Field Of Invoice IsPaid is equal to 1 */  \n"
                           + "			SELECT -100 Id, 'Receipt Total', COUNT(DISTINCT(i.id)) Reciepts, ISNULL(SUM(ISNULL(i.Total,0) - ISNULL(i.PaidTotal,0)),0) amount  \n"
                           + "			FROM Invoices AS i  \n"
                           + "			INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,3,8,10,11,12)  \n"
                           + "			WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0 AND ISNULL(i.IsPaid,0) = 1  \n"
                           + "		) fin  \n"
                           + "		GROUP BY fin.Id, fin.Decription  \n"
                           + "		UNION ALL  \n"
                           + "		/*	Not Invoiced Recipies Total Data*/  \n"
                           + "		SELECT  -101 Id, 'Not Invoiced', COUNT(DISTINCT(i.id)) Reciepts,   \n"
                           + "			ISNULL(SUM(CASE WHEN ISNULL(i.IsVoided,0) = 1 AND it.[Type] = 8 THEN (-1)*i.Total ELSE i.Total END),0) amount, 0 AccountId, 0 AccountType   \n"
                           + "		FROM Invoices AS i  \n"
                           + "		INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] IN (2,8)  \n"
                           + "		WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsInvoiced,0) = 0  \n"
                           + "		UNION ALL  \n"
                           + "		/*	Canceld Recipies Total Data*/  \n"
                           + "		SELECT -102 Id, 'Canceled', COUNT(DISTINCT(i.id)) Reciepts, ISNULL((-1)*SUM(i.Total),0) amount, 0 AccountId, 0 AccountType  \n"
                           + "		FROM Invoices AS i  \n"
                           + "		INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] IN (3)  \n"
                           + "		WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 1  \n"
                           + "		UNION ALL  \n"
                           + "		/*	Not Printed Recipies Total Data*/  \n"
                           + "		SELECT -103 Id, 'Not Printed', COUNT(DISTINCT(i.id)) Reciepts, ISNULL(SUM(i.Total),0) amount, 0 AccountId, 0 AccountType  \n"
                           + "		FROM Invoices AS i  \n"
                           + "		INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,8,10,11,12)  \n"
                           + "		WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 0 AND ISNULL(i.IsVoided,0) = 0   \n"
                           + "		UNION ALL  \n"
                           + "		/*	Discount Recipies Total Data*/  \n"
                           + "		SELECT -104 Id, 'Discount Total', COUNT(DISTINCT(i.id)) Reciepts, ISNULL(SUM(i.Discount),0) amount, 0 AccountId, 0 AccountType  \n"
                           + "		FROM Invoices AS i  \n"
                           + "		INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,3,8,10,12)  \n"
                           + "		WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0 AND i.Discount <> 0  \n"
                           + "		UNION ALL  \n"
                           + "		/*	Loyalty Discount Recipies Total Data*/  \n"
                           + "		SELECT -110 Id, 'Loyalty Discount', COUNT(DISTINCT(i.id)) Reciepts, ISNULL(SUM(i.LoyaltyDiscount),0) amount, 0 AccountId, 0 AccountType  \n"
                           + "		FROM Invoices AS i  \n"
                           + "		INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,3,8,10,12)  \n"
                           + "		WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0 AND i.Discount <> 0 AND ISNULL(i.LoyaltyDiscount,0)>0 \n"
                           + "		UNION ALL  \n"
                           + "		/*	Couver From Recipies Total Data*/  \n"
                           + "		SELECT -105 Id, 'Couvers', COUNT(o.Id) Reciepts, ISNULL(SUM(o.Couver),0) amount, 0 AccountId, 0 AccountType  \n"
                           + "		FROM [Order] AS o  \n"
                           + "		WHERE o.EndOfDayId IS NULL AND o.PosId = @posInfo AND o.Couver <> 0  \n"
                           + "	) st  \n"
                           + "END");
            Ver_2_0_0_12.Add("CREATE PROCEDURE [dbo].[EndOfDayAnalysisLoyaltyDiscount] @posInfo BIGINT, @staffId BIGINT AS   \n"
                           + "BEGIN   \n"
                           + "	/*Gets a list of invoices with Discount*/  \n"
                           + "	SELECT res.*  \n"
                           + "	FROM (  \n"
                           + "		SELECT ROW_NUMBER() OVER (ORDER BY fin.ReceiptNo) Id, fin.*  \n"
                           + "		FROM (  \n"
                           + "			SELECT   \n"
                           + "				i.[Day], pif.[Description] PosInfoDescription, d.[Description] DepartmentDescription, pif.Id PosInfoId,  \n"
                           + "				it.Abbreviation Abbreviation, i.Cover Couver, s.FirstName + ' ' +s.LastName StaffName, s.Id StaffId,  \n"
                           + "				(i.Total) Total, ISNULL(i.Discount,0) Discount, ISNULL(i.PaidTotal,0) PaidTotal,  \n"
                           + "				odi.ItemsCount, i.OrderNo OrderNo, i.[Counter] ReceiptNo, it.[Type] InvoiceType, ISNULL(i.IsInvoiced,0) IsInvoiced ,  \n"
                           + "				ISNULL(i.IsVoided,0) IsVoided, ISNULL(i.IsPaid,0) IsPaid, i.Id InvoiceId, i.Rooms Room,   \n"
                           + "				ISNULL(tb.Code,'') TableCode, ISNULL(tb.Id,0) TableId   \n"
                           + "			FROM Invoices AS i  \n"
                           + "			INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId AND it.[Type] NOT IN (2,8,10,11,12)  \n"
                           + "			INNER JOIN PosInfo AS pif ON pif.Id = i.PosInfoId  \n"
                           + "			INNER JOIN Department AS d ON d.Id = pif.DepartmentId  \n"
                           + "			LEFT OUTER JOIN [Table] AS tb ON tb.Id = i.TableId  \n"
                           + "			INNER JOIN Staff AS s ON s.Id = i.StaffId AND   \n"
                           + "				CAST(s.Id AS VARCHAR(20)) LIKE CASE WHEN @staffId > 0 THEN CAST(@staffId AS VARCHAR(20)) ELSE '%' END  \n"
                           + "			CROSS APPLY (  \n"
                           + "				SELECT ISNULL(COUNT(*),0)+ISNULL(COUNT(odg.Id),0) ItemsCount  \n"
                           + "				FROM OrderDetailInvoices AS odi  \n"
                           + "				OUTER APPLY (  \n"
                           + "					SELECT odg.Id   \n"
                           + "					FROM OrderDetailIgredients AS odg  \n"
                           + "					WHERE odg.OrderDetailId = odi.Id	  \n"
                           + "				) odg  \n"
                           + "				WHERE odi.InvoicesId = i.Id  \n"
                           + "			)odi  \n"
                           + "			WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0 AND   \n"
                           + "				ISNULL(i.Discount,0) <> 0  AND ISNULL(i.LoyaltyDiscount,0)>0 \n"
                           + "		) fin  \n"
                           + "	) res  \n"
                           + "	ORDER BY res.Id  \n"
                           + "END");
        }
    }
}
