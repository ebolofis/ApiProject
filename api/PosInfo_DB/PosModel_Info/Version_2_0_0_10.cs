﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [DisplayName("Ver : 2.0.0.10")]
    public class Version_2_0_0_10
    {
        public List<string> Ver_2_0_0_10 { get; }

        public Version_2_0_0_10()
        {
            Ver_2_0_0_10 = new List<string>();
            Ver_2_0_0_10.Add("CREATE PROCEDURE [dbo].[CancelReceipt] (@invoiceId BIGINT, @posInfo BIGINT, @staffId BIGINT, @NewInvoiceId BIGINT OUTPUT) AS  \n"
                           + "BEGIN   \n"
                           + " \n"
                           + "	DECLARE @OrderDetailId BIGINT \n"
                           + "	DECLARE @COUNTER INT \n"
                           + "	DECLARE @Hash nvarchar(200) \n"
                           + "	DECLARE @MSG VARCHAR(500) \n"
                           + "	DECLARE @ErrorSeverity INT \n"
                           + "	DECLARE @ErrorState INT														 \n"
                           + "	DECLARE @ErrorMessage NVARCHAR(1000)				 \n"
                           + " \n"
                           + " \n"
                           + "	BEGIN TRANSACTION;   \n"
                           + " \n"
                           + "	BEGIN TRY  \n"
                           + " \n"
                           + "		--******************************************************************************* \n"
                           + "		--UPDATE COUNT \n"
                           + "		--*******************************************************************************		 \n"
                           + "		UPDATE PosInfoDetail  \n"
                           + "		SET [Counter] = [Counter] + 1 \n"
                           + "		WHERE PosInfoDetail.PosInfoId = @posInfo and PosInfoDetail.GroupId = 3; \n"
                           + " \n"
                           + "		SELECT @COUNTER = [Counter] \n"
                           + "		FROM PosInfoDetail \n"
                           + "		WHERE PosInfoDetail.PosInfoId = @posInfo and PosInfoDetail.GroupId = 3; \n"
                           + "		--******************************************************************************* \n"
                           + "		--******************************************************************************* \n"
                           + "		--******************************************************************************* \n"
                           + " \n"
                           + " \n"
                           + "		--******************************************************************************* \n"
                           + "		--INVOICES \n"
                           + "		--******************************************************************************* \n"
                           + "		SELECT * INTO #invoices FROM invoices WHERE IsVoided = 0 AND Id = @invoiceId \n"
                           + " \n"
                           + "		IF (@@ROWCOUNT = 0) \n"
                           + "		BEGIN \n"
                           + "			SET @MSG = 'Invoice Id ' + LTRIM(STR(@invoiceId)) + ' not found !' \n"
                           + "			RAISERROR (@MSG, 16, 0) \n"
                           + "		END \n"
                           + " \n"
                           + "		SELECT @Hash = CONVERT (varchar(19), #invoices.Day, 120) + \n"
                           + "								ltrim(str(#invoices.IsInvoiced)) + \n"
                           + "								ltrim(str(#invoices.PosInfoDetailId)) +  \n"
                           + "								ltrim(str(#invoices.InvoiceTypeId)) +  \n"
                           + "								RIGHT('000000000' + CONVERT(VARCHAR, CAST(#invoices.Total AS DECIMAL(18,2))), 12) +  \n"
                           + "								ltrim(str(#invoices.StaffId)) + \n"
                           + "								ltrim(str(#invoices.Counter)) \n"
                           + "		FROM #invoices \n"
                           + " \n"
                           + " \n"
                           + "		SET @Hash = CONVERT(NVARCHAR(200), HashBytes('MD5', @Hash), 2) \n"
                           + " \n"
                           + "		UPDATE #invoices SET #invoices.PaidTotal = -PaidTotal,  \n"
                           + "							 #invoices.IsVoided = 1, \n"
                           + "							 #invoices.Description = PosInfoDetail.Description, \n"
                           + "							 #invoices.InvoiceTypeId = PosInfoDetail.InvoicesTypeId, \n"
                           + "							 #invoices.Counter = @COUNTER, \n"
                           + "							 #invoices.PosInfoDetailId = PosInfoDetail.Id, \n"
                           + "							 #invoices.Day = CASE WHEN #invoices.EndOfDayId IS NULL THEN GETDATE() ELSE #invoices.Day END, \n"
                           + "							 #invoices.PosInfoId = @posInfo, \n"
                           + "							 #invoices.StaffId = @staffId, \n"
                           + "							 #invoices.Hashcode = @Hash \n"
                           + "		FROM PosInfoDetail  \n"
                           + "		WHERE PosInfoDetail.PosInfoId = @posInfo and PosInfoDetail.GroupId = 3 \n"
                           + " \n"
                           + " \n"
                           + "		 \n"
                           + " \n"
                           + " \n"
                           + "		ALTER TABLE #invoices DROP COLUMN id;	 \n"
                           + "		INSERT INTO invoices SELECT * FROM #invoices; \n"
                           + "		SET @NewInvoiceId = SCOPE_IDENTITY() \n"
                           + " \n"
                           + "		UPDATE Invoices \n"
                           + "		SET IsVoided = 1 \n"
                           + "		WHERE Id = @invoiceId		 \n"
                           + "		--******************************************************************************* \n"
                           + "		--******************************************************************************* \n"
                           + "		--******************************************************************************* \n"
                           + " \n"
                           + " \n"
                           + " \n"
                           + "		--******************************************************************************* \n"
                           + "		--ORDER DETAIL INVOICES \n"
                           + "		--******************************************************************************* \n"
                           + "		SELECT * INTO #OrderDetailInvoices FROM OrderDetailInvoices WHERE InvoicesId = @invoiceId \n"
                           + "		 \n"
                           + "		update #OrderDetailInvoices SET #OrderDetailInvoices.InvoicesId = @NewInvoiceId,  \n"
                           + "										#OrderDetailInvoices.Abbreviation = PosInfoDetail.Abbreviation, \n"
                           + "										#OrderDetailInvoices.PosInfoDetailId = PosInfoDetail.Id, \n"
                           + "										#OrderDetailInvoices.Counter = @COUNTER, \n"
                           + "										#OrderDetailInvoices.CreationTS = GETDATE(), \n"
                           + "										#OrderDetailInvoices.InvoiceType = 3, \n"
                           + "										#OrderDetailInvoices.PosInfoId = @posInfo, \n"
                           + "					    				#OrderDetailInvoices.StaffId = @staffId \n"
                           + "		FROM PosInfoDetail  \n"
                           + "		WHERE PosInfoDetail.PosInfoId = @posInfo and PosInfoDetail.GroupId = 3		 \n"
                           + "		ALTER TABLE #OrderDetailInvoices DROP COLUMN id; \n"
                           + "		INSERT INTO OrderDetailInvoices SELECT * FROM #OrderDetailInvoices; \n"
                           + "		--******************************************************************************* \n"
                           + "		--******************************************************************************* \n"
                           + "		--******************************************************************************* \n"
                           + "		 \n"
                           + " \n"
                           + "		--******************************************************************************* \n"
                           + "		--ORDER DETAIL \n"
                           + "		--*******************************************************************************		 \n"
                           + "		UPDATE OrderDetail SET [Status] = 5 \n"
                           + "		WHERE Id in (SELECT OrderDetailId from #OrderDetailInvoices)		 \n"
                           + "		--******************************************************************************* \n"
                           + "		--******************************************************************************* \n"
                           + "		--******************************************************************************* \n"
                           + " \n"
                           + " \n"
                           + "		--******************************************************************************* \n"
                           + "		--INSERT OrderStatus \n"
                           + "		--******************************************************************************* \n"
                           + "		SELECT DISTINCT OrderDetail.OrderId INTO #INVOICEORDERS  \n"
                           + "			FROM OrderDetail INNER JOIN #OrderDetailInvoices  \n"
                           + "				ON OrderDetail.Id = #OrderDetailInvoices.OrderDetailId \n"
                           + "					WHERE NOT EXISTS(SELECT * FROM OrderDetail B WHERE B.OrderId = OrderDetail.OrderId AND B.Status <> 5)			 \n"
                           + " \n"
                           + "		INSERT OrderStatus (Status, TimeChanged, OrderId, StaffId) \n"
                           + "		SELECT 5, GETDATE(), #INVOICEORDERS.OrderId, @staffId \n"
                           + "		FROM #INVOICEORDERS; \n"
                           + "		--******************************************************************************* \n"
                           + "		--******************************************************************************* \n"
                           + "		--******************************************************************************* \n"
                           + " \n"
                           + "		DECLARE @NEW_ID BIGINT \n"
                           + " \n"
                           + "		CREATE TABLE #TRANS (NEW_ID BIGINT PRIMARY KEY, OLD_ID BIGINT); \n"
                           + " \n"
                           + "		DECLARE @Id BIGINT, @Day DATETIME, @PosInfoId BIGINT, @Staff_Id BIGINT, @OrderId BIGINT, \n"
                           + "		       @TransactionType SMALLINT, @Amount MONEY, @DepartmentId BIGINT,  \n"
                           + "			   @Description NVARCHAR(250), @AccountId BIGINT, @InOut SMALLINT, @Gross DECIMAL(9, 4), @Net DECIMAL(9, 4), @Vat DECIMAL(9, 4), @Tax DECIMAL(9, 4), \n"
                           + "			   @EndOfDayId BIGINT, @ExtDescription NVARCHAR(500), @InvoicesId BIGINT, @IsDeleted BIT \n"
                           + " \n"
                           + " \n"
                           + "		DECLARE db_cursor CURSOR FOR  \n"
                           + "		SELECT Id, Day, PosInfoId, StaffId, OrderId, \n"
                           + "		       TransactionType, Amount, DepartmentId,  \n"
                           + "			   Description, AccountId, InOut, Gross, Net, Vat, Tax, \n"
                           + "			   EndOfDayId, ExtDescription, InvoicesId, IsDeleted \n"
                           + "		FROM Transactions \n"
                           + "		WHERE InvoicesId = @invoiceId \n"
                           + "		 		 \n"
                           + "		OPEN db_cursor   \n"
                           + "		FETCH NEXT FROM db_cursor INTO @Id, @Day, @PosInfoId, @Staff_Id, @OrderId, \n"
                           + "		       @TransactionType, @Amount, @DepartmentId,  \n"
                           + "			   @Description, @AccountId, @InOut, @Gross, @Net, @Vat, @Tax, \n"
                           + "			   @EndOfDayId, @ExtDescription, @InvoicesId, @IsDeleted   \n"
                           + " \n"
                           + "		WHILE @@FETCH_STATUS = 0   \n"
                           + "		BEGIN   \n"
                           + " \n"
                           + "			INSERT INTO Transactions SELECT CASE WHEN @EndOfDayId IS NULL THEN GETDATE() ELSE @Day END,  \n"
                           + "				   @posInfo, @staffId, @OrderId, \n"
                           + "				   4, -@Amount, @DepartmentId,  \n"
                           + "				   @Description, @AccountId, 1, @Gross, @Net, @Vat, @Tax, \n"
                           + "				   @EndOfDayId, @ExtDescription, @NewInvoiceId, @IsDeleted \n"
                           + " \n"
                           + "			SELECT @NEW_ID = SCOPE_IDENTITY(); \n"
                           + "			INSERT INTO #TRANS VALUES(@NEW_ID, @ID); \n"
                           + " \n"
                           + "			FETCH NEXT FROM db_cursor INTO @Id, @Day, @PosInfoId, @Staff_Id, @OrderId, \n"
                           + "				   @TransactionType, @Amount, @DepartmentId,  \n"
                           + "				   @Description, @AccountId, @InOut, @Gross, @Net, @Vat, @Tax, \n"
                           + "				   @EndOfDayId, @ExtDescription, @InvoicesId, @IsDeleted   \n"
                           + " \n"
                           + "		END   \n"
                           + " \n"
                           + "		CLOSE db_cursor   \n"
                           + "		DEALLOCATE db_cursor  \n"
                           + " \n"
                           + "		 \n"
                           + " \n"
                           + "		--******************************************************************************* \n"
                           + "		--CREDIT TRANSACTIONS \n"
                           + "		--******************************************************************************* \n"
                           + "		SELECT * INTO #CreditTransactions FROM CreditTransactions WHERE TransactionId in (select OLD_ID FROM #TRANS ) \n"
                           + "		 \n"
                           + "		update #CreditTransactions SET  #CreditTransactions.PosInfoId = @posInfo, \n"
                           + "					    				#CreditTransactions.StaffId = @staffId, \n"
                           + "										#CreditTransactions.InvoiceId = @NewInvoiceId,  \n"
                           + "										#CreditTransactions.TransactionId = #TRANS.NEW_ID,										 \n"
                           + "										#CreditTransactions.Type = 1, \n"
                           + "										#CreditTransactions.Amount = -#CreditTransactions.Amount, \n"
                           + "										#CreditTransactions.Description = 'Cancel payment for receipt id ' + LTRIM(STR(#CreditTransactions.InvoiceId)), \n"
                           + "										#CreditTransactions.CreationTS = GETDATE()									 \n"
                           + "		FROM #TRANS INNER JOIN #CreditTransactions ON #CreditTransactions.TransactionId = #TRANS.OLD_ID		 \n"
                           + " \n"
                           + "		ALTER TABLE #CreditTransactions DROP COLUMN id; \n"
                           + "		INSERT INTO CreditTransactions SELECT * FROM #CreditTransactions; \n"
                           + "		--******************************************************************************* \n"
                           + "		--******************************************************************************* \n"
                           + "		--******************************************************************************* \n"
                           + " \n"
                           + " \n"
                           + "		--******************************************************************************* \n"
                           + "		--TRANSFER TO PMS \n"
                           + "		--******************************************************************************* \n"
                           + "		SELECT * INTO #TransferToPms FROM TransferToPms WHERE TransactionId in (select OLD_ID FROM #TRANS ) \n"
                           + "		 \n"
                           + "		update #TransferToPms SET  #TransferToPms.PosInfoId = @posInfo, \n"
                           + "					    				#TransferToPms.ReceiptNo = @COUNTER,										 \n"
                           + "										#TransferToPms.TransactionId = #TRANS.NEW_ID,										 \n"
                           + "										#TransferToPms.Total = -#TransferToPms.Total, \n"
                           + "										#TransferToPms.ErrorMessage = NULL, \n"
                           + "										#TransferToPms.PmsResponseId = NULL, \n"
                           + "										#TransferToPms.SendToPmsTS = GETDATE(), \n"
                           + "										#TransferToPms.TransferIdentifier = NEWID(), \n"
                           + "										#TransferToPms.Status = 0 \n"
                           + "		FROM #TRANS INNER JOIN #TransferToPms ON #TransferToPms.TransactionId = #TRANS.OLD_ID		 \n"
                           + " \n"
                           + "		update #TransferToPms SET #TransferToPms.PosInfoDetailId = PosInfoDetail.Id \n"
                           + "		FROM PosInfoDetail  \n"
                           + "		WHERE PosInfoDetail.PosInfoId = @posInfo and PosInfoDetail.GroupId = 3 \n"
                           + " \n"
                           + "		update #TransferToPms  \n"
                           + "		SET #TransferToPms.SendToPMS = CASE WHEN Accounts.SendsTransfer = 1 THEN 1 ELSE 0 END \n"
                           + "		FROM Transactions inner join Accounts on (Transactions.AccountId = Accounts.Id) \n"
                           + "		WHERE Transactions.Id = #TransferToPms.TransactionId \n"
                           + " \n"
                           + "		ALTER TABLE #TransferToPms DROP COLUMN id; \n"
                           + "		INSERT INTO TransferToPms SELECT * FROM #TransferToPms; \n"
                           + "		--******************************************************************************* \n"
                           + "		--******************************************************************************* \n"
                           + "		--******************************************************************************* \n"
                           + " \n"
                           + "	END TRY   \n"
                           + "	BEGIN CATCH   \n"
                           + " \n"
                           + "			ROLLBACK TRANSACTION;   \n"
                           + " \n"
                           + "			IF CURSOR_STATUS('global','db_cursor') >= 0  \n"
                           + "			BEGIN \n"
                           + "				CLOSE db_cursor \n"
                           + "				DEALLOCATE db_cursor  \n"
                           + "			END \n"
                           + " \n"
                           + "			SELECT @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE() ,@ErrorMessage = ERROR_MESSAGE();   \n"
                           + "			RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState); \n"
                           + "			SELECT ERROR_SEVERITY(), ERROR_STATE(), ERROR_MESSAGE(); \n"
                           + " \n"
                           + "			RETURN -1 \n"
                           + "	END CATCH;   \n"
                           + " \n"
                           + "	COMMIT TRANSACTION;  \n"
                           + " \n"
                           + "	RETURN 1 \n"
                           + " \n"
                           + " \n"
                           + " \n"
                           + "END");
        }

    }
}
