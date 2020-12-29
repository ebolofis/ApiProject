﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [DisplayName("Ver : 2.0.0.7")]
    public class Version_2_0_0_7
    {
        public List<string> Ver_2_0_0_7 { get; }

        public Version_2_0_0_7()
        {
            Ver_2_0_0_7 = new List<string>();
            Ver_2_0_0_7.Add("ALTER PROCEDURE EndOfDayTotal @posInfo BIGINT AS \n"
                           + "BEGIN \n"
                           + "	/*Gets data from tables Invoice, Transaction, InvoiceType and Account and return total records for each case*/ \n"
                           + "	SELECT * \n"
                           + "	FROM ( \n"
                           + "		/*	Accounts Total Data*/ \n"
                           + "		SELECT 0 id, a.[Description], COUNT(i.id) ReceiptCount, ISNULL(SUM(t.Amount),0) Amount, a.Id AccountId, a.[Type] AccountType \n"
                           + "		FROM Transactions AS t \n"
                           + "		INNER JOIN Accounts AS a ON a.id = t.AccountId \n"
                           + "		INNER JOIN Invoices AS i ON i.id = t.InvoicesId AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0 \n"
                           + "		INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,3,8,10) \n"
                           + "		WHERE t.PosInfoId = @posInfo AND t.EndOfDayId IS NULL AND ISNULL(t.IsDeleted,0) <> 1 AND  \n"
                           + "			((NOT (t.TransactionType = 9 AND t.InOut = 1))) AND t.TransactionType NOT IN (8,7) \n"
                           + "		GROUP BY a.id, a.[Description], a.[Type] \n"
                           + "		UNION ALL \n"
                           + "		/*	Not Paid Recipies Total Data*/ \n"
                           + "		SELECT fin.Id,fin.[Description], SUM(fin.Reciepts) Reciepts, ISNULL(SUM(fin.amount),0) amount, 0 AccountId, 0 AccountType  \n"
                           + "		FROM (	 \n"
                           + "			/*	Recipies Without Transaction. The Field ID of Invoices id not in InvoicesId of Transaction */ \n"
                           + "			SELECT -99 Id, 'Not Paid' [Description], COUNT(DISTINCT(i.id)) Reciepts, ISNULL(SUM(i.Total),0) amount \n"
                           + "			FROM Invoices AS i \n"
                           + "			INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,3,8,10,11,12) \n"
                           + "			LEFT OUTER JOIN Transactions AS t ON t.InvoicesId = i.Id \n"
                           + "			WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0 \n"
                           + "			 AND t.InvoicesId IS NULL \n"
                           + "			UNION ALL \n"
                           + "			/*	The Field IsPaid of Invoices id equal to 1 */ \n"
                           + "			SELECT -99 Id, 'Not Paid', COUNT(DISTINCT(i.id)) Reciepts, ISNULL(SUM(ISNULL(i.Total,0) - ISNULL(i.PaidTotal,0)),0) amount \n"
                           + "			FROM Invoices AS i \n"
                           + "			INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,3,8,10,11,12) \n"
                           + "			WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0 AND ISNULL(i.IsPaid,0) = 1 \n"
                           + "		) fin \n"
                           + "		GROUP BY fin.Id, fin.[Description] \n"
                           + "		UNION ALL \n"
                           + "		/*	Recipies Total Data*/ \n"
                           + "		SELECT fin.Id,fin.Decription, SUM(fin.Reciepts) Reciepts, ISNULL(SUM(fin.amount),0) amount, 0 AccountId, 0 AccountType \n"
                           + "		FROM ( \n"
                           + "			/*	Recipies With Transaction. Id of Invoice is equal to InvoicesId of Transaction */ \n"
                           + "			SELECT-100 Id, 'Receipt Total' Decription, COUNT(DISTINCT(t.InvoicesId)) Reciepts, ISNULL(SUM(t.Amount),0) amount \n"
                           + "			FROM Transactions AS t \n"
                           + "			INNER JOIN Accounts AS a ON a.id = t.AccountId \n"
                           + "			INNER JOIN Invoices AS i ON i.id = t.InvoicesId AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0 \n"
                           + "			INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,3,8,10) \n"
                           + "			WHERE t.PosInfoId = @posInfo AND t.EndOfDayId IS NULL AND ISNULL(t.IsDeleted,0) <> 1 AND  \n"
                           + "				((NOT (t.TransactionType = 9 AND t.InOut = 1))) AND t.TransactionType NOT IN (8,7) \n"
                           + "			UNION ALL \n"
                           + "			/*	Recipies Without Transaction. InvoicesId of Transaction is null */ \n"
                           + "			SELECT -100 Id, 'Receipt Total' [Description], COUNT(DISTINCT(i.id)) Reciepts, ISNULL(SUM(i.Total),0) amount \n"
                           + "			FROM Invoices AS i \n"
                           + "			INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,3,8,10,11,12) \n"
                           + "			LEFT OUTER JOIN Transactions AS t ON t.InvoicesId = i.Id \n"
                           + "			WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0 \n"
                           + "			 AND t.InvoicesId IS NULL \n"
                           + "			UNION ALL \n"
                           + "			/*	Recipies Without Transaction. The Field Of Invoice IsPaid is equal to 1 */ \n"
                           + "			SELECT -100 Id, 'Receipt Total', COUNT(DISTINCT(i.id)) Reciepts, ISNULL(SUM(ISNULL(i.Total,0) - ISNULL(i.PaidTotal,0)),0) amount \n"
                           + "			FROM Invoices AS i \n"
                           + "			INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,3,8,10,11,12) \n"
                           + "			WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0 AND ISNULL(i.IsPaid,0) = 1 \n"
                           + "		) fin \n"
                           + "		GROUP BY fin.Id, fin.Decription \n"
                           + "		UNION ALL \n"
                           + "		/*	Not Invoiced Recipies Total Data*/ \n"
                           + "		SELECT  -101 Id, 'Not Invoiced', COUNT(DISTINCT(i.id)) Reciepts,  \n"
                           + "			ISNULL(SUM(CASE WHEN ISNULL(i.IsVoided,0) = 1 AND it.[Type] = 8 THEN (-1)*i.Total ELSE i.Total END),0) amount, 0 AccountId, 0 AccountType  \n"
                           + "		FROM Invoices AS i \n"
                           + "		INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] IN (2,8) \n"
                           + "		WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsInvoiced,0) = 0 \n"
                           + "		UNION ALL \n"
                           + "		/*	Canceld Recipies Total Data*/ \n"
                           + "		SELECT -102 Id, 'Canceled', COUNT(DISTINCT(i.id)) Reciepts, ISNULL((-1)*SUM(i.Total),0) amount, 0 AccountId, 0 AccountType \n"
                           + "		FROM Invoices AS i \n"
                           + "		INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] IN (3) \n"
                           + "		WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 1 \n"
                           + "		UNION ALL \n"
                           + "		/*	Not Printed Recipies Total Data*/ \n"
                           + "		SELECT -103 Id, 'Not Printed', COUNT(DISTINCT(i.id)) Reciepts, ISNULL(SUM(i.Total),0) amount, 0 AccountId, 0 AccountType \n"
                           + "		FROM Invoices AS i \n"
                           + "		INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,8,10,11,12) \n"
                           + "		WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 0 AND ISNULL(i.IsVoided,0) = 0  \n"
                           + "		UNION ALL \n"
                           + "		/*	Discount Recipies Total Data*/ \n"
                           + "		SELECT -104 Id, 'Discount Total', COUNT(DISTINCT(i.id)) Reciepts, ISNULL(SUM(i.Discount),0) amount, 0 AccountId, 0 AccountType \n"
                           + "		FROM Invoices AS i \n"
                           + "		INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,3,8,10,12) \n"
                           + "		WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0 AND i.Discount <> 0 \n"
                           + "		UNION ALL \n"
                           + "		/*	Couver From Recipies Total Data*/ \n"
                           + "		SELECT -105 Id, 'Couvers', COUNT(o.Id) Reciepts, ISNULL(SUM(o.Couver),0) amount, 0 AccountId, 0 AccountType \n"
                           + "		FROM [Order] AS o \n"
                           + "		WHERE o.EndOfDayId IS NULL AND o.PosId = @posInfo AND o.Couver <> 0 \n"
                           + "	) st \n"
                           + "END");
            Ver_2_0_0_7.Add("ALTER PROCEDURE dbo.EndOfDayByStaff @posInfo BIGINT AS \n"
                            + "BEGIN  \n"
                            + "	/*Gets data from tables Invoice, Transaction, InvoiceType, Account, Staff and return total records for each case and staff name*/ \n"
                            + "	SELECT * \n"
                            + "	FROM ( \n"
                            + "		/*	Accounts Total Data*/ \n"
                            + "		SELECT 0 id, a.[Description], COUNT(i.id) ReceiptCount, SUM(t.Amount) Amount, a.Id AccountID, a.[Type] AccountType, \n"
                            + "			t.StaffId, s.LastName+' '+s.FirstName staffName \n"
                            + "		FROM Transactions AS t \n"
                            + "		INNER JOIN Accounts AS a ON a.id = t.AccountId \n"
                            + "		INNER JOIN Invoices AS i ON i.id = t.InvoicesId AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0 \n"
                            + "		INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,3,8,10) \n"
                            + "		INNER JOIN Staff AS s ON s.Id = t.StaffId \n"
                            + "		WHERE t.PosInfoId = @posInfo AND t.EndOfDayId IS NULL AND ISNULL(t.IsDeleted,0) <> 1 AND  \n"
                            + "			((NOT (t.TransactionType = 9 AND t.InOut = 1))) AND t.TransactionType NOT IN (8,7) \n"
                            + "		GROUP BY a.id, a.[Description], a.[Type], t.StaffId, s.LastName,s.FirstName \n"
                            + " \n"
                            + "		UNION ALL \n"
                            + "		/*	Not Paid Recipies Total Data*/ \n"
                            + "		SELECT -99 Id, 'Not Paid' [Description], SUM(fin.Reciepts) Reciepts, SUM(fin.amount) amount, 0 AccountID, 0 AccountType,  \n"
                            + "			fin.StaffId,fin.staffName  \n"
                            + "		FROM (	 \n"
                            + "			/*	Recipies Without Transaction. The Field ID of Invoices id not in InvoicesId of Transaction */ \n"
                            + "			SELECT COUNT(DISTINCT(i.id)) Reciepts, SUM(i.Total) amount, i.StaffId, s.LastName+' '+s.FirstName staffName \n"
                            + "			FROM Invoices AS i \n"
                            + "			INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,3,8,10,11,12) \n"
                            + "			INNER JOIN Staff AS s ON s.Id = i.StaffId \n"
                            + "			LEFT OUTER JOIN Transactions AS t ON t.InvoicesId = i.Id \n"
                            + "			WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0 \n"
                            + "			 AND t.InvoicesId IS NULL \n"
                            + "			GROUP BY i.StaffId, s.LastName,s.FirstName \n"
                            + "			UNION ALL \n"
                            + "			/*	The Field IsPaid of Invoices id equal to 1 */ \n"
                            + "			SELECT COUNT(DISTINCT(i.id)) Reciepts, SUM(ISNULL(i.Total,0) - ISNULL(i.PaidTotal,0)) amount, i.StaffId, s.LastName+' '+s.FirstName \n"
                            + "			FROM Invoices AS i \n"
                            + "			INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,3,8,10,11,12) \n"
                            + "			INNER JOIN Staff AS s ON s.Id = i.StaffId \n"
                            + "			WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0 AND ISNULL(i.IsPaid,0) = 1 \n"
                            + "			GROUP BY i.StaffId, s.LastName+' '+s.FirstName \n"
                            + "		) fin \n"
                            + "		GROUP BY fin.StaffId, fin.staffName \n"
                            + "		UNION ALL \n"
                            + "		/*	Recipies Total Data*/ \n"
                            + "		SELECT -100 Id, 'Receipt Total' Decription, SUM(fin.Reciepts) Reciepts, SUM(fin.amount) amount, 0 AccountID,0 AccountType,  \n"
                            + "			fin.StaffId,fin.staffName  \n"
                            + "		FROM ( \n"
                            + "			/*	Recipies With Transaction. Id of Invoice is equal to InvoicesId of Transaction */ \n"
                            + "			SELECT COUNT(DISTINCT(t.InvoicesId)) Reciepts, SUM(t.Amount) amount, t.StaffId, s.LastName+' '+s.FirstName staffName \n"
                            + "			FROM Transactions AS t \n"
                            + "			INNER JOIN Accounts AS a ON a.id = t.AccountId \n"
                            + "			INNER JOIN Invoices AS i ON i.id = t.InvoicesId AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0 \n"
                            + "			INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,3,8,10) \n"
                            + "			INNER JOIN Staff AS s ON s.Id = t.StaffId \n"
                            + "			WHERE t.PosInfoId = @posInfo AND t.EndOfDayId IS NULL AND ISNULL(t.IsDeleted,0) <> 1 AND  \n"
                            + "				((NOT (t.TransactionType = 9 AND t.InOut = 1))) AND t.TransactionType NOT IN (8,7) \n"
                            + "			GROUP BY t.StaffId, s.LastName,s.FirstName \n"
                            + "			UNION ALL \n"
                            + "			/*	Recipies Without Transaction. InvoicesId of Transaction is null */ \n"
                            + "			SELECT COUNT(DISTINCT(i.id)) Reciepts, SUM(i.Total) amount, i.StaffId, s.LastName+' '+s.FirstName staffName \n"
                            + "			FROM Invoices AS i \n"
                            + "			INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,3,8,10,11,12) \n"
                            + "			INNER JOIN Staff AS s ON s.Id = i.StaffId \n"
                            + "			LEFT OUTER JOIN Transactions AS t ON t.InvoicesId = i.Id \n"
                            + "			WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0 \n"
                            + "			 AND t.InvoicesId IS NULL  \n"
                            + "			GROUP BY i.StaffId, s.LastName,s.FirstName \n"
                            + "			UNION ALL \n"
                            + "			/*	Recipies Without Transaction. The Field Of Invoice IsPaid is equal to 1 */ \n"
                            + "			SELECT COUNT(DISTINCT(i.id)) Reciepts, SUM(ISNULL(i.Total,0) - ISNULL(i.PaidTotal,0)) amount, \n"
                            + "				i.StaffId, s.LastName+' '+s.FirstName \n"
                            + "			FROM Invoices AS i \n"
                            + "			INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,3,8,10,11,12) \n"
                            + "			INNER JOIN Staff AS s ON s.Id = i.StaffId \n"
                            + "			WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0 AND ISNULL(i.IsPaid,0) = 1 \n"
                            + "			GROUP BY i.StaffId, s.LastName+' '+s.FirstName \n"
                            + "		) fin \n"
                            + "		GROUP BY fin.StaffId, fin.staffName \n"
                            + "		UNION ALL \n"
                            + "		/*	Not Invoiced Recipies Total Data*/ \n"
                            + "		SELECT  -101 Id, 'Not Invoiced' [Description], COUNT(DISTINCT(i.id)) Reciepts,  \n"
                            + "			SUM(CASE WHEN ISNULL(i.IsVoided,0) = 1 AND it.[Type] = 8 THEN (-1)*i.Total ELSE i.Total END) amount, 0 AccountID,0 AccountType,  \n"
                            + "			i.StaffId, s.LastName+' '+s.FirstName staffName \n"
                            + "		FROM Invoices AS i \n"
                            + "		INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] IN (2,8) \n"
                            + "		INNER JOIN Staff AS s ON s.Id = i.StaffId \n"
                            + "		WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsInvoiced,0) = 0 \n"
                            + "		GROUP BY i.StaffId, s.LastName+' '+s.FirstName \n"
                            + "		UNION ALL \n"
                            + "		/*	Canceld Recipies Total Data*/ \n"
                            + "		SELECT -102 Id, 'Canceled' [Description], COUNT(DISTINCT(i.id)) Reciepts, (-1)*SUM(i.Total) amount, 0 AccountID,0 AccountType,  \n"
                            + "			i.StaffId, s.LastName+' '+s.FirstName staffName \n"
                            + "		FROM Invoices AS i \n"
                            + "		INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] IN (3) \n"
                            + "		INNER JOIN Staff AS s ON s.Id = i.StaffId \n"
                            + "		WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 1 \n"
                            + "		GROUP BY i.StaffId, s.LastName+' '+s.FirstName \n"
                            + "		UNION ALL \n"
                            + "		/*	Not Printed Recipies Total Data*/ \n"
                            + "		SELECT  -103 Id, 'Not Printed' [Description], COUNT(DISTINCT(i.id)) Reciepts, SUM(i.Total) amount, 0 AccountID,0 AccountType,  \n"
                            + "			i.StaffId, s.LastName+' '+s.FirstName staffName \n"
                            + "		FROM Invoices AS i \n"
                            + "		INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,8,10,11,12) \n"
                            + "		INNER JOIN Staff AS s ON s.Id = i.StaffId \n"
                            + "		WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 0 AND ISNULL(i.IsVoided,0) = 0  \n"
                            + "		GROUP BY i.StaffId, s.LastName+' '+s.FirstName \n"
                            + "		UNION ALL \n"
                            + "		/*	Discount Recipies Total Data*/ \n"
                            + "		SELECT -104 Id, 'Discount Total' [Description], COUNT(DISTINCT(i.id)) Reciepts, SUM(i.Discount) amount, 0 AccountID,0 AccountType,  \n"
                            + "			i.StaffId, s.LastName+' '+s.FirstName staffName \n"
                            + "		FROM Invoices AS i \n"
                            + "		INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,3,8,10,12) \n"
                            + "		INNER JOIN Staff AS s ON s.Id = i.StaffId \n"
                            + "		WHERE i.PosInfoId = @posInfo AND i.EndOfDayId IS NULL AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0 AND i.Discount <> 0 \n"
                            + "		GROUP BY i.StaffId, s.LastName+' '+s.FirstName \n"
                            + "		UNION ALL \n"
                            + "		/*	Couver From Recipies Total Data*/ \n"
                            + "		SELECT  -105 Id, 'Couvers' [Description], COUNT(o.Id) Reciepts, SUM(o.Couver) Amount, 0 AccountID,0 AccountType, o.StaffId,  \n"
                            + "			s.LastName+' '+s.FirstName staffName \n"
                            + "		FROM [Order] AS o \n"
                            + "		INNER JOIN Staff AS s ON s.Id = o.StaffId \n"
                            + "		WHERE o.EndOfDayId IS NULL AND o.PosId = @posInfo AND o.Couver <> 0 \n"
                            + "		GROUP BY o.StaffId, s.LastName+' '+s.FirstName \n"
                            + "		UNION ALL \n"
                            + "		/*	Opening Cashier. Cash and Open command from pos.  Total Data*/ \n"
                            + "		SELECT -106 id, 'Open Cashier' [Description], SUM(op.Reciepts) Reciepts, SUM(op.amount) amount, 0 AccountID,0 AccountType, op.StaffId, op.staffName \n"
                            + "		FROM ( \n"
                            + "			/*Cash from Transaction*/ \n"
                            + "			SELECT  1 Reciepts, SUM(CASE WHEN t.TransactionType = 9 THEN (-1)*t.Amount ELSE t.Amount END) amount,  \n"
                            + "				t.StaffId, s.LastName+' '+s.FirstName staffName \n"
                            + "			FROM Transactions AS t \n"
                            + "			INNER JOIN Accounts AS a ON a.id = t.AccountId AND a.[Type] = 1 \n"
                            + "			INNER JOIN Invoices AS i ON i.id = t.InvoicesId AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0 \n"
                            + "			INNER JOIN InvoiceTypes AS it ON it.Id = i.InvoiceTypeId  AND it.[Type] NOT IN (2,3,8,10) \n"
                            + "			INNER JOIN Staff AS s ON s.Id = t.StaffId \n"
                            + "			WHERE t.PosInfoId = @posInfo AND t.EndOfDayId IS NULL AND ISNULL(t.IsDeleted,0) <> 1 AND  \n"
                            + "				((t.TransactionType = 9 AND t.InOut = 1) OR (t.TransactionType <> 9)) \n"
                            + "			GROUP BY t.StaffId, s.LastName,s.FirstName \n"
                            + "			UNION ALL \n"
                            + "			/*Open Cashier Command from pos*/ \n"
                            + "			SELECT COUNT(t.id) Reciepts, SUM(t.Amount) amount, t.StaffId, s.LastName+' '+s.FirstName staffName \n"
                            + "			FROM Transactions AS t \n"
                            + "			INNER JOIN Staff AS s ON s.Id = t.StaffId \n"
                            + "			WHERE t.EndOfDayId IS NULL AND t.PosInfoId = @posInfo AND t.TransactionType = 1 \n"
                            + "			GROUP BY t.StaffId, s.LastName,s.FirstName \n"
                            + "		) op \n"
                            + "		GROUP BY op.StaffId, op.staffName \n"
                            + "		UNION ALL \n"
                            + "		/*	Closing Cashier. Close command from pos.  Total Data*/ \n"
                            + "		SELECT -107 id, 'Close Cashier' [Description], COUNT(t.id) Reciepts, SUM(t.Amount) amount, 0 AccountID,0 AccountType,  \n"
                            + "			t.StaffId, s.LastName+' '+s.FirstName staffName \n"
                            + "		FROM Transactions AS t \n"
                            + "		INNER JOIN Staff AS s ON s.Id = t.StaffId \n"
                            + "		WHERE t.EndOfDayId IS NULL AND t.PosInfoId = @posInfo AND t.TransactionType = 2 AND  t.AccountId = 1 \n"
                            + "		GROUP BY t.StaffId, s.LastName,s.FirstName \n"
                            + "		UNION ALL \n"
                            + "		/*Total amount per account Type for closing cashier*/ \n"
                            + "		SELECT -108 id,a.[Description], a.Reciepts, a.amount, a.AccountId, a.AccountType, a.StaffId, a.staffName \n"
                            + "		FROM ( \n"
                            + "			SELECT a.[Description], COUNT(t.id) Reciepts, SUM(CASE WHEN t.TransactionType = 9 THEN (-1)*t.Amount ELSE t.Amount END) amount,  \n"
                            + "				t.AccountId, a.[Type] AccountType, t.StaffId, s.LastName+' '+s.FirstName staffName \n"
                            + "			FROM Transactions AS t \n"
                            + "			INNER JOIN Staff AS s ON s.Id = t.StaffId \n"
                            + "			INNER JOIN Accounts AS a ON a.Id = t.AccountId \n"
                            + "         LEFT OUTER JOIN Invoices AS i ON i.Id = t.InvoicesId AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0 \n"
                            + "			WHERE t.EndOfDayId IS NULL AND t.PosInfoId = @posInfo AND ISNULL(t.IsDeleted,0) <> 1 AND  \n"
                            + "				((t.TransactionType = 9 AND t.InOut = 1) OR (t.TransactionType <> 9))					 \n"
                            + "			GROUP BY t.AccountId, a.[Description], a.[Type], t.StaffId, s.LastName,s.FirstName \n"
                            + "		) a \n"
                            + "		WHERE a.amount <> 0 \n"
                            + "		UNION ALL \n"
                            + "		/*Money to return the staff on ending his shift*/ \n"
                            + "		SELECT -109 id,'Total To Return' [Description], 1 Reciepts, SUM(CASE WHEN t.TransactionType = 9 THEN (-1)*t.Amount ELSE t.Amount END) amount,  \n"
                            + "			0 AccountID,0 AccountType,t.StaffId, s.LastName+' '+s.FirstName staffName \n"
                            + "		FROM Transactions AS t \n"
                            + "		INNER JOIN Staff AS s ON s.Id = t.StaffId  \n"
                            + "		INNER JOIN Accounts AS a ON a.Id = t.AccountId AND a.[Type] NOT IN (5) \n"
                            + "     LEFT OUTER JOIN Invoices AS i ON i.Id = t.InvoicesId AND ISNULL(i.IsPrinted,0) = 1 AND ISNULL(i.IsVoided,0) = 0 \n"
                            + "		WHERE t.EndOfDayId IS NULL AND t.PosInfoId = @posInfo AND  \n"
                            + "			 ((t.TransactionType = 9 AND t.InOut = 1) OR (t.TransactionType <> 9))  AND ISNULL(t.IsDeleted,0) <> 1  \n"
                            + "		GROUP BY t.StaffId, s.LastName,s.FirstName \n"
                            + "	) st \n"
                            + "	ORDER BY st.StaffId, st.id DESC \n"
                            + "END");
        }
    }
}
