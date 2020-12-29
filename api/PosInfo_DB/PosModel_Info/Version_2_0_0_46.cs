using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [DisplayName("Ver : 2.0.0.46")]
    public class Version_2_0_0_46
    {
        public List<string> Ver_2_0_0_46 { get; }

        public Version_2_0_0_46()
        {
            Ver_2_0_0_46 = new List<string>();
            Ver_2_0_0_46.Add(@"
                CREATE NONCLUSTERED INDEX [IX_DA_Customers_AuthToken] ON [dbo].[DA_Customers]
                (
                    [AuthToken] ASC
                )");
            Ver_2_0_0_46.Add(@"
                ALTER PROCEDURE [dbo].[CancelReceipt] (@invoiceId BIGINT, @posInfo BIGINT, @staffId BIGINT, @NewInvoiceId BIGINT OUTPUT) AS  
                BEGIN   
 
	                DECLARE @OrderDetailId BIGINT 
	                DECLARE @COUNTER INT 
	                DECLARE @Hash nvarchar(200) 
	                DECLARE @MSG VARCHAR(500) 
	                DECLARE @ErrorSeverity INT 
	                DECLARE @ErrorState INT														 
	                DECLARE @ErrorMessage NVARCHAR(1000)				 

	                BEGIN TRANSACTION;   
 
	                BEGIN TRY  
 
		                --******************************************************************************* 
		                --UPDATE COUNT 
		                --*******************************************************************************		 
		                UPDATE PosInfoDetail  
		                SET [Counter] = [Counter] + 1 
		                WHERE PosInfoDetail.PosInfoId = @posInfo and PosInfoDetail.GroupId = 3; 
 
		                SELECT @COUNTER = [Counter] 
		                FROM PosInfoDetail 
		                WHERE PosInfoDetail.PosInfoId = @posInfo and PosInfoDetail.GroupId = 3; 
		                --******************************************************************************* 
		                --******************************************************************************* 
		                --******************************************************************************* 
 
 
		                --******************************************************************************* 
		                --INVOICES 
		                --******************************************************************************* 
		                SELECT * INTO #invoices FROM invoices WHERE IsVoided = 0 AND Id = @invoiceId 
 
		                IF (@@ROWCOUNT = 0) 
		                BEGIN 
			                SET @MSG = 'Invoice Id ' + LTRIM(STR(@invoiceId)) + ' not found !' 
			                RAISERROR (@MSG, 16, 0) 
		                END 
 
		                SELECT @Hash = CONVERT (varchar(19), #invoices.Day, 120) + 
								                ltrim(str(#invoices.IsInvoiced)) + 
								                ltrim(str(#invoices.PosInfoDetailId)) +  
								                ltrim(str(#invoices.InvoiceTypeId)) +  
								                RIGHT('000000000' + CONVERT(VARCHAR, CAST(#invoices.Total AS DECIMAL(18,2))), 12) +  
								                ltrim(str(#invoices.StaffId)) + 
								                ltrim(str(#invoices.Counter)) 
		                FROM #invoices 
 
 
		                SET @Hash = CONVERT(NVARCHAR(200), HashBytes('MD5', @Hash), 2) 
 
		                UPDATE #invoices SET #invoices.PaidTotal = -PaidTotal,  
							                 #invoices.IsVoided = 1, 
							                 #invoices.Description = PosInfoDetail.Description, 
							                 #invoices.InvoiceTypeId = PosInfoDetail.InvoicesTypeId, 
							                 #invoices.Counter = @COUNTER, 
							                 #invoices.PosInfoDetailId = PosInfoDetail.Id, 
							                 #invoices.Day = CASE WHEN #invoices.EndOfDayId IS NULL THEN GETDATE() ELSE #invoices.Day END, 
							                 #invoices.PosInfoId = @posInfo, 
							                 #invoices.StaffId = @staffId, 
							                 #invoices.Hashcode = @Hash 
		                FROM PosInfoDetail  
		                WHERE PosInfoDetail.PosInfoId = @posInfo and PosInfoDetail.GroupId = 3 


		                ALTER TABLE #invoices DROP COLUMN id;	 
		                INSERT INTO invoices SELECT * FROM #invoices; 
		                SET @NewInvoiceId = SCOPE_IDENTITY() 
 
		                UPDATE Invoices 
		                SET IsVoided = 1 
		                WHERE Id = @invoiceId		 
		                --******************************************************************************* 
		                --******************************************************************************* 
		                --******************************************************************************* 
 
 
 
		                --******************************************************************************* 
		                --ORDER DETAIL INVOICES 
		                --******************************************************************************* 
		                SELECT * INTO #OrderDetailInvoices FROM OrderDetailInvoices WHERE InvoicesId = @invoiceId 
		 
		                update #OrderDetailInvoices SET #OrderDetailInvoices.InvoicesId = @NewInvoiceId,  
										                #OrderDetailInvoices.Abbreviation = PosInfoDetail.Abbreviation, 
										                #OrderDetailInvoices.PosInfoDetailId = PosInfoDetail.Id, 
										                #OrderDetailInvoices.Counter = @COUNTER, 
										                #OrderDetailInvoices.CreationTS = GETDATE(), 
										                #OrderDetailInvoices.InvoiceType = 3, 
										                #OrderDetailInvoices.PosInfoId = @posInfo, 
					    				                #OrderDetailInvoices.StaffId = @staffId 
		                FROM PosInfoDetail  
		                WHERE PosInfoDetail.PosInfoId = @posInfo and PosInfoDetail.GroupId = 3		 
		                ALTER TABLE #OrderDetailInvoices DROP COLUMN id; 
		                INSERT INTO OrderDetailInvoices SELECT * FROM #OrderDetailInvoices; 
		                --******************************************************************************* 
		                --******************************************************************************* 
		                --******************************************************************************* 
		 
 
		                --******************************************************************************* 
		                --ORDER DETAIL 
		                --*******************************************************************************		 
		                UPDATE OrderDetail SET [Status] = 5 
		                WHERE Id in (SELECT OrderDetailId from #OrderDetailInvoices)		 
		                --******************************************************************************* 
		                --******************************************************************************* 
		                --******************************************************************************* 
 
 
		                --******************************************************************************* 
		                --INSERT OrderStatus 
		                --******************************************************************************* 
		                SELECT DISTINCT OrderDetail.OrderId INTO #INVOICEORDERS  
			                FROM OrderDetail INNER JOIN #OrderDetailInvoices  
				                ON OrderDetail.Id = #OrderDetailInvoices.OrderDetailId 
					                WHERE NOT EXISTS(SELECT * FROM OrderDetail B WHERE B.OrderId = OrderDetail.OrderId AND B.Status <> 5)			 
 
		                INSERT OrderStatus (Status, TimeChanged, OrderId, StaffId) 
		                SELECT 5, GETDATE(), #INVOICEORDERS.OrderId, @staffId 
		                FROM #INVOICEORDERS; 
		                --******************************************************************************* 
		                --******************************************************************************* 
		                --******************************************************************************* 
 
		                DECLARE @NEW_ID BIGINT 
 
		                CREATE TABLE #TRANS (NEW_ID BIGINT PRIMARY KEY, OLD_ID BIGINT); 
 
		                DECLARE @Id BIGINT, @Day DATETIME, @PosInfoId BIGINT, @Staff_Id BIGINT, @OrderId BIGINT, 
		                       @TransactionType SMALLINT, @Amount MONEY, @DepartmentId BIGINT,  
			                   @Description NVARCHAR(250), @AccountId BIGINT, @InOut SMALLINT, @Gross DECIMAL(9, 4), @Net DECIMAL(9, 4), @Vat DECIMAL(9, 4), @Tax DECIMAL(9, 4), 
			                   @EndOfDayId BIGINT, @ExtDescription NVARCHAR(500), @InvoicesId BIGINT, @IsDeleted BIT 
 
 
		                DECLARE db_cursor CURSOR LOCAL FOR  
		                SELECT Id, Day, PosInfoId, StaffId, OrderId, 
		                       TransactionType, Amount, DepartmentId,  
			                   Description, AccountId, InOut, Gross, Net, Vat, Tax, 
			                   EndOfDayId, ExtDescription, InvoicesId, IsDeleted 
		                FROM Transactions 
		                WHERE InvoicesId = @invoiceId 
		 		 
		                OPEN db_cursor   
		                FETCH NEXT FROM db_cursor INTO @Id, @Day, @PosInfoId, @Staff_Id, @OrderId, 
		                       @TransactionType, @Amount, @DepartmentId,  
			                   @Description, @AccountId, @InOut, @Gross, @Net, @Vat, @Tax, 
			                   @EndOfDayId, @ExtDescription, @InvoicesId, @IsDeleted   
 
		                WHILE @@FETCH_STATUS = 0   
		                BEGIN   
 
			                INSERT INTO Transactions SELECT CASE WHEN @EndOfDayId IS NULL THEN GETDATE() ELSE @Day END,  
				                   @posInfo, @staffId, @OrderId, 
				                   4, -@Amount, @DepartmentId,  
				                   @Description, @AccountId, 1, @Gross, @Net, @Vat, @Tax, 
				                   @EndOfDayId, @ExtDescription, @NewInvoiceId, @IsDeleted, NULL
 
			                SELECT @NEW_ID = SCOPE_IDENTITY(); 
			                INSERT INTO #TRANS VALUES(@NEW_ID, @ID); 
 
			                FETCH NEXT FROM db_cursor INTO @Id, @Day, @PosInfoId, @Staff_Id, @OrderId, 
				                   @TransactionType, @Amount, @DepartmentId,  
				                   @Description, @AccountId, @InOut, @Gross, @Net, @Vat, @Tax, 
				                   @EndOfDayId, @ExtDescription, @InvoicesId, @IsDeleted
 
		                END   
 
		                CLOSE db_cursor   
		                DEALLOCATE db_cursor  


		                --******************************************************************************* 
		                --CREDIT TRANSACTIONS 
		                --******************************************************************************* 
		                SELECT * INTO #CreditTransactions FROM CreditTransactions WHERE TransactionId in (select OLD_ID FROM #TRANS ) 
		 
		                update #CreditTransactions SET  #CreditTransactions.PosInfoId = @posInfo, 
					    				                #CreditTransactions.StaffId = @staffId, 
										                #CreditTransactions.InvoiceId = @NewInvoiceId,  
										                #CreditTransactions.TransactionId = #TRANS.NEW_ID,										 
										                #CreditTransactions.Type = 1, 
										                #CreditTransactions.Amount = -#CreditTransactions.Amount, 
										                #CreditTransactions.Description = 'Cancel payment for receipt id ' + LTRIM(STR(#CreditTransactions.InvoiceId)), 
										                #CreditTransactions.CreationTS = GETDATE()									 
		                FROM #TRANS INNER JOIN #CreditTransactions ON #CreditTransactions.TransactionId = #TRANS.OLD_ID		 
 
		                ALTER TABLE #CreditTransactions DROP COLUMN id; 
		                INSERT INTO CreditTransactions SELECT * FROM #CreditTransactions; 
		                --******************************************************************************* 
		                --******************************************************************************* 
		                --******************************************************************************* 
 
 
		                --******************************************************************************* 
		                --TRANSFER TO PMS 
		                --******************************************************************************* 
		                SELECT * INTO #TransferToPms FROM TransferToPms WHERE TransactionId in (select OLD_ID FROM #TRANS ) 
		 
		                update #TransferToPms SET  #TransferToPms.PosInfoId = @posInfo, 
					    				                #TransferToPms.ReceiptNo = @COUNTER,										 
										                #TransferToPms.TransactionId = #TRANS.NEW_ID,										 
										                #TransferToPms.Total = -#TransferToPms.Total, 
										                #TransferToPms.ErrorMessage = NULL, 
										                #TransferToPms.PmsResponseId = NULL, 
										                #TransferToPms.SendToPmsTS = GETDATE(), 
										                #TransferToPms.TransferIdentifier = NEWID(), 
										                #TransferToPms.Status = 0 
		                FROM #TRANS INNER JOIN #TransferToPms ON #TransferToPms.TransactionId = #TRANS.OLD_ID		 
 
		                update #TransferToPms SET #TransferToPms.PosInfoDetailId = PosInfoDetail.Id 
		                FROM PosInfoDetail  
		                WHERE PosInfoDetail.PosInfoId = @posInfo and PosInfoDetail.GroupId = 3 
 
		                update #TransferToPms  
		                SET #TransferToPms.SendToPMS = CASE WHEN Accounts.SendsTransfer = 1 THEN 1 ELSE 0 END 
		                FROM Transactions inner join Accounts on (Transactions.AccountId = Accounts.Id) 
		                WHERE Transactions.Id = #TransferToPms.TransactionId 
 
		                ALTER TABLE #TransferToPms DROP COLUMN id; 
		                INSERT INTO TransferToPms SELECT * FROM #TransferToPms; 
		                --******************************************************************************* 
		                --******************************************************************************* 
		                --******************************************************************************* 
 
	                END TRY   
	                BEGIN CATCH   
 
			                ROLLBACK TRANSACTION;   
 
			                IF CURSOR_STATUS('local','db_cursor') >= 0  
			                BEGIN 
				                CLOSE db_cursor 
				                DEALLOCATE db_cursor  
			                END 
 
			                SELECT @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE() ,@ErrorMessage = ERROR_MESSAGE();   
			                RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState); 
			                SELECT ERROR_SEVERITY(), ERROR_STATE(), ERROR_MESSAGE(); 
 
			                RETURN -1 
	                END CATCH;   
 
	                COMMIT TRANSACTION;  
 
	                RETURN 1 

                END");

        }
    }
}
