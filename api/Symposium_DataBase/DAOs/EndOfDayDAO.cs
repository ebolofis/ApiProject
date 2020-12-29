using Dapper;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DAOs
{
    public class EndOfDayDAO : IEndOfDayDAO
    {
        /// <summary>
        /// Return a list of EndofDay Totals per Type of Totals.
        /// <seealso cref="Symposium.Models.Enums.EndOfDayReceiptTypes"/>
        /// <para>call the sp EndOfDayTotal</para>
        /// </summary>
        /// <param name="posInfo">posInfo</param>
        /// <param name="db">DB Connection</param>
        /// <returns>List of EndOfDayTotalModel</returns>
        public List<EndOfDayTotalModel> EndOfDayTotal(long posInfo, IDbConnection db)
        {
            return db.Query<EndOfDayTotalModel>("EndOfDayTotal", new { posInfo = posInfo, bySalesType = 0 }, commandType: CommandType.StoredProcedure).ToList<EndOfDayTotalModel>();
        }

        /// <summary>
        /// Return a list of EndofDay Totals per Type of Totals.
        /// <para>call the sp EndOfDayTotal</para>
        /// </summary>
        /// <param name="posInfo">posInfo</param>
        /// <param name="db">DB Connection</param>
        /// <returns>List of EndOfDayTotalModel</returns>
        public List<EndOfDayTotalModel> PreviewSalesType(IDbConnection db, long posInfo)
        {
            return db.Query<EndOfDayTotalModel>("EndOfDayTotal", new { posInfo = posInfo, bySalesType = 1 }, commandType: CommandType.StoredProcedure).ToList<EndOfDayTotalModel>();
        }

        /// <summary>
        /// Return a list of EndofDay Totals per Type per Staff of Totals.
        /// <seealso cref="Symposium.Models.Enums.EndOfDayByStaffModel"/>
        /// <para>call the sp EndOfDayByStaff</para>
        /// </summary>
        /// <param name="posInfo">posInfo</param>
        /// <param name="db">DB Connection</param>
        /// <returns>List of EndOfDayByStaffModel</returns>
        public List<EndOfDayByStaffModel> EndOfDayPerStaffTotal(long posInfo, IDbConnection db)
        {
            return db.Query<EndOfDayByStaffModel>("EndOfDayByStaff", new { posInfo = posInfo }, commandType: CommandType.StoredProcedure).ToList<EndOfDayByStaffModel>();
        }

        /// <summary>
        /// Return the sum of cash and credit cards of barcode transaction types 7 
        /// </summary>
        /// <param name="posInfo"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public EndOfDayBarcodesModel DayBarcodeTotals(long posInfo, IDbConnection db)
        {
            EndOfDayBarcodesModel endOfDayBarcodesModel = new EndOfDayBarcodesModel();
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("PosInfoId", posInfo);

            string query = "SELECT SUM(a.Cash) AS Cash, SUM(a.CreditCard) AS CreditCard FROM ( \n"
                         + "SELECT SUM(ISNULL(CASE WHEN AccountId = 1 THEN (Amount) END, 0)) as Cash, \n"
                         + "       SUM(ISNULL(CASE WHEN AccountId = 3 THEN (Amount) END, 0)) as CreditCard \n"
                         + "                                FROM Transactions \n"
                         + "                                WHERE PosInfoId = @PosInfoId AND TransactionType IN (7) AND EndOfDayId IS NULL AND IsDeleted IS NULL \n"
                         + "GROUP BY AccountId \n"
                         + ") a";
            endOfDayBarcodesModel = db.Query<EndOfDayBarcodesModel>(query, queryParameters).FirstOrDefault<EndOfDayBarcodesModel>();
            return endOfDayBarcodesModel;
        }

        /// <summary>
        /// Return the sum of locker transaction types 8, 9
        /// </summary>
        /// <param name="posInfo"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public long DayLockerTotals(long posInfo, IDbConnection db)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("PosInfoId", posInfo);

            string query = @"SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE SUM(CASE WHEN TransactionType = 8 THEN Amount ELSE (-1) * Amount END) END as Total
                                FROM Transactions
                                WHERE PosInfoId = @PosInfoId AND TransactionType IN (8, 9) AND EndOfDayId IS NULL AND IsDeleted IS NULL";

            long amount = db.Query<long>(query, queryParameters).FirstOrDefault<long>();
            return amount;
        }

        /// <summary>
        /// Return a list of today invoices for a specific accountID (Cash, Credit Cards, Coplimentary each...), POS and Staff (EndOfDayReceiptTypes = 0). 
        /// If staffId=0 then return results for all staffs accosiated to the specific POS 
        /// </summary>
        /// <param name="db">DB Connection</param>
        /// <param name="posInfo">posInfo</param>
        /// <param name="AccountId">AccountId</param>
        /// <param name="staffId">staffId</param>
        /// <returns></returns>
        public List<EODAnalysisReceiptModel> AnalysisByAccount(IDbConnection db, long posInfo, long AccountId, long staffId = 0)
        {
            return db.Query<EODAnalysisReceiptModel>("EndOfDayAnalysisByAccount", new { posInfo = posInfo, AccountId = AccountId, staffId = staffId }, commandType: CommandType.StoredProcedure).ToList<EODAnalysisReceiptModel>();
        }

        /// <summary>
        /// Return a list of not paid invoices for a specific POS and Staff (EndOfDayReceiptTypes = -99). 
        /// If staffId=0 then return results for all staffs accosiated to the specific POS 
        /// </summary>
        /// <param name="db">DB Connection</param>
        /// <param name="posInfo">posInfo</param>
        /// <param name="staffId">staffId</param>
        /// <returns></returns>
        public List<EODAnalysisReceiptModel> AnalysisNotPaid(IDbConnection db, long posInfo, long staffId = 0)
        {
            return db.Query<EODAnalysisReceiptModel>("EndOfDayAnalysisNotPaid", new { posInfo = posInfo, staffId = staffId }, commandType: CommandType.StoredProcedure).ToList<EODAnalysisReceiptModel>();
        }

        /// <summary>
        /// Return a list of all printed and not voiced invoices for a specific POS and Staff (EndOfDayReceiptTypes = -100). 
        /// If staffId=0 then return results for all staffs accosiated to the specific POS.
        /// </summary>
        /// <param name="db">DB Connection</param>
        /// <param name="posInfo">posInfo</param>
        /// <param name="staffId">staffId</param>
        /// <param name="all">if all=1 then for splited transactions return a record per account (way of payment). If all=1 then return one record for all accounts</param>
        /// <returns></returns>
        public List<EODAnalysisReceiptModel> AnalysisReceiptTotal(IDbConnection db, long posInfo, long staffId = 0, int all = 0)
        {
            return db.Query<EODAnalysisReceiptModel>("EndOfDayAnalysisReceiptTotal", new { posInfo = posInfo, staffId = staffId, all = all }, commandType: CommandType.StoredProcedure).ToList<EODAnalysisReceiptModel>();
        }

        /// <summary>
        /// Return a list of not invoiced invoices for a specific POS and Staff (EndOfDayReceiptTypes = -101). 
        /// If staffId=0 then return results for all staffs accosiated to the specific POS 
        /// </summary>
        /// <param name="db">DB Connection</param>
        /// <param name="posInfo">posInfo</param>
        /// <param name="staffId">staffId</param>
        /// <returns></returns>
        public List<EODAnalysisReceiptModel> AnalysisNotInvoiced(IDbConnection db, long posInfo, long staffId = 0)
        {
            return db.Query<EODAnalysisReceiptModel>("EndOfDayAnalysisNotInvoiced", new { posInfo = posInfo, staffId = staffId }, commandType: CommandType.StoredProcedure).ToList<EODAnalysisReceiptModel>();
        }

        /// <summary>
        /// Return a list of canceld invoices for a specific POS and Staff (EndOfDayReceiptTypes =  -102). 
        /// If staffId=0 then return results for all staffs accosiated to the specific POS 
        /// </summary>
        /// <param name="db">DB Connection</param>
        /// <param name="posInfo">posInfo</param>
        /// <param name="staffId">staffId</param>
        /// <returns></returns>
        public List<EODAnalysisReceiptModel> AnalysisCanceled(IDbConnection db, long posInfo, long staffId = 0)
        {
            return db.Query<EODAnalysisReceiptModel>("EndOfDayAnalysisCanceled", new { posInfo = posInfo, staffId = staffId }, commandType: CommandType.StoredProcedure).ToList<EODAnalysisReceiptModel>();
        }

        /// <summary>
        /// Return a list of not printed invoices for a specific POS and Staff (EndOfDayReceiptTypes =  -103). 
        /// If staffId=0 then return results for all staffs accosiated to the specific POS 
        /// </summary>
        /// <param name="db">DB Connection</param>
        /// <param name="posInfo">posInfo</param>
        /// <param name="staffId">staffId</param>
        /// <returns></returns>
        public List<EODAnalysisReceiptModel> AnalysisNotPrinted(IDbConnection db, long posInfo, long staffId = 0)
        {
            return db.Query<EODAnalysisReceiptModel>("EndOfDayAnalysisNotPrinted", new { posInfo = posInfo, staffId = staffId }, commandType: CommandType.StoredProcedure).ToList<EODAnalysisReceiptModel>();
        }

        /// <summary>
        /// Return a list of invoices with Discount for a specific POS and Staff (EndOfDayReceiptTypes =  -104). 
        /// If staffId=0 then return results for all staffs accosiated to the specific POS 
        /// </summary>
        /// <param name="db">DB Connection</param>
        /// <param name="posInfo">posInfo</param>
        /// <param name="staffId">staffId</param>
        /// <returns></returns>
        public List<EODAnalysisReceiptModel> AnalysisDiscount(IDbConnection db, long posInfo, long staffId = 0)
        {
            return db.Query<EODAnalysisReceiptModel>("EndOfDayAnalysisDiscount", new { posInfo = posInfo, staffId = staffId }, commandType: CommandType.StoredProcedure).ToList<EODAnalysisReceiptModel>();
        }

        /// <summary>
        /// Return a list of invoices with Loyalty Discount for a specific POS and Staff (EndOfDayReceiptTypes =  -104). 
        /// If staffId=0 then return results for all staffs accosiated to the specific POS 
        /// </summary>
        /// <param name="db">DB Connection</param>
        /// <param name="posInfo">posInfo</param>
        /// <param name="staffId">staffId</param>
        /// <returns></returns>
        public List<EODAnalysisReceiptModel> AnalysisLoyaltyDiscount(IDbConnection db, long posInfo, long staffId = 0)
        {
            return db.Query<EODAnalysisReceiptModel>("[EndOfDayAnalysisLoyaltyDiscount]", new { posInfo = posInfo, staffId = staffId }, commandType: CommandType.StoredProcedure).ToList<EODAnalysisReceiptModel>();
        }

        /// <summary>
        /// Returns a list of analysis per vat for selected pos for current day
        /// </summary>
        /// <param name="db"> DB connection</param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <returns> List of end of day vat analysis models. See: <seealso cref="Symposium.Models.Models.EodVatAnalysisModel"/> </returns>
        public List<EodVatAnalysisModel> VatAnalysisForCurrentDay(IDbConnection db, long posInfo)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("PosInfoId", posInfo);

            string query = @"select a.Vatid, a.VatRate, sum(a.Gross) Gross, sum(a.Net) Net, sum(a.Tax) Tax, sum(a.Discount) Discount, sum(a.ItemCount) ItemCount, count(distinct a.TicketCount) TicketCount
                                from (
                                    select VatId, Vat.Percentage as VatRate, (case when InvoiceType = 3 then (-1) * Total else Total end) as Gross, (case when InvoiceType = 3 then (-1) * Net else Net end) as Net, isnull(case when InvoiceType = 3 then (-1) * TaxAmount else TaxAmount end, 0) as Tax, (case when InvoiceType = 3 then (-1) * Discount else Discount end) + (case when InvoiceType = 3 then (-1) * ReceiptSplitedDiscount else ReceiptSplitedDiscount end) as Discount, (case when InvoiceType = 3 then -1 else 1 end) as ItemCount, i.Id as TicketCount  
	                                from [OrderDetailInvoices]
	                                inner join Vat on Vat.Id=OrderDetailInvoices.VatId
	                                outer apply (
		                                select  i.Id
		                                from Invoices as i
		                                where i.Id = OrderDetailInvoices.InvoicesId and i.IsVoided = 0 and i.IsDeleted is null
	                                ) i
	                                where endofdayid is null and IsDeleted is null and posinfoid = @PosInfoId and InvoiceType not in (2,8,11,12)
                                ) a
                                group by a.VatId,a.VatRate";

            return db.Query<EodVatAnalysisModel>(query, queryParameters).ToList<EodVatAnalysisModel>();

        }

        /// <summary>
        /// Returns a list of analysis per vat for selected pos for selected day
        /// </summary>
        /// <param name="db"> DB connection </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <param name="endOfDayId"> Id of end of day </param>
        /// <returns> List of end of day vat analysis models. See: <seealso cref="Symposium.Models.Models.EodVatAnalysisModel"/> </returns>
        public List<EodVatAnalysisModel> VatAnalysisForSelectedDay(IDbConnection db, long posInfo, long endOfDayId)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("PosInfoId", posInfo);
            queryParameters.Add("endofdayid", endOfDayId);

            string query = @"select a.Vatid, a.VatRate, sum(a.Gross) Gross, sum(a.Net) Net, sum(a.Tax) Tax, sum(a.Discount) Discount, sum(a.ItemCount) ItemCount, count(distinct a.TicketCount) TicketCount
                                from (
                                    select VatId, Vat.Percentage as VatRate, (case when InvoiceType = 3 then (-1) * Total else Total end) as Gross, (case when InvoiceType = 3 then (-1) * Net else Net end) as Net, isnull(case when InvoiceType = 3 then (-1) * TaxAmount else TaxAmount end, 0) as Tax, (case when InvoiceType = 3 then (-1) * Discount else Discount end) + (case when InvoiceType = 3 then (-1) * ReceiptSplitedDiscount else ReceiptSplitedDiscount end) as Discount, (case when InvoiceType = 3 then -1 else 1 end) as ItemCount, i.Id as TicketCount  
	                                from [OrderDetailInvoices]
	                                inner join Vat on Vat.Id=OrderDetailInvoices.VatId
	                                outer apply (
		                                select  i.Id
		                                from Invoices as i
		                                where i.Id = OrderDetailInvoices.InvoicesId and i.IsVoided = 0 and i.IsDeleted is null
	                                ) i
	                                where endofdayid = @endofdayid and IsDeleted is null and posinfoid = @PosInfoId and InvoiceType not in (2,8,11,12)
                                ) a
                                group by a.VatId,a.VatRate";

            return db.Query<EodVatAnalysisModel>(query, queryParameters).ToList<EodVatAnalysisModel>();
        }

        /// <summary>
        /// Returns a list of analysis per account for selected pos for current day
        /// </summary>
        /// <param name="db"> DB connection </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <returns> List of end of day account analysis models. See: <seealso cref="Symposium.Models.Models.EodAccountAnalysisModel"/> </returns>
        public List<EodAccountAnalysisModel> PaymentAnalysisForCurrentDay(IDbConnection db, long posInfo)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("PosInfoId", posInfo);

            string query = @"select AccountId,Accounts.Description,Accounts.Type as AccountType,sum(Amount) as Amount
                                from Transactions
                                inner join Accounts on Accounts.id= AccountId
                                where EndOfDayId is null and Transactions.IsDeleted is null and posinfoid=@PosInfoId and TransactionType not in (1,2,7,8,9) 
                                group by AccountId,Accounts.Description,Accounts.Type";

            return db.Query<EodAccountAnalysisModel>(query, queryParameters).ToList<EodAccountAnalysisModel>();
        }

        /// <summary>
        /// Returns a list of analysis per account for selected pos for selected day
        /// </summary>
        /// <param name="db"> DB connection </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <param name="endOfDayId"> Id of end of day </param>
        /// <returns> List of end of day account analysis models. See: <seealso cref="Symposium.Models.Models.EodAccountAnalysisModel"/> </returns>
        public List<EodAccountAnalysisModel> PaymentAnalysisForSelectedDay(IDbConnection db, long posInfo, long endOfDayId)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("PosInfoId", posInfo);
            queryParameters.Add("endofdayid", endOfDayId);

            string query = @"select AccountId,Accounts.Description,Accounts.Type as AccountType,sum(Amount) as Amount
                                from Transactions
                                inner join Accounts on Accounts.id= AccountId
                                where EndOfDayId=@endofdayid and Transactions.IsDeleted is null and posinfoid=@PosInfoId and TransactionType not in (1,2,7,8,9) 
                                group by AccountId,Accounts.Description,Accounts.Type";

            return db.Query<EodAccountAnalysisModel>(query, queryParameters).ToList<EodAccountAnalysisModel>();
        }

        /// <summary>
        /// Returns a list of analysis per account of voids for selected pos for current day
        /// </summary>
        /// <param name="db"> DB connection </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <returns> List of end of day account analysis models. See: <seealso cref="Symposium.Models.Models.EodAccountAnalysisModel"/> </returns>
        public List<EodAccountAnalysisModel> VoidAnalysisForCurrentDay(IDbConnection db, long posInfo)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("PosInfoId", posInfo);

            string query = @"select AccountId,Accounts.Description,Accounts.Type as AccountType,sum(Amount) as Amount
                                from Transactions
                                inner join Accounts on Accounts.id= AccountId
                                where EndOfDayId is null and Transactions.IsDeleted is null and posinfoid=@PosInfoId and TransactionType =4
                                group by AccountId,Accounts.Description,Accounts.Type";

            return db.Query<EodAccountAnalysisModel>(query, queryParameters).ToList<EodAccountAnalysisModel>();
        }

        /// <summary>
        /// Returns a list of analysis per account of voids for selected pos for selected day
        /// </summary>
        /// <param name="db"> DB connection </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <param name="endOfDayId"> Id of end of day </param>
        /// <returns> List of end of day account analysis models. See: <seealso cref="Symposium.Models.Models.EodAccountAnalysisModel"/> </returns>
        public List<EodAccountAnalysisModel> VoidAnalysisForSelectedDay(IDbConnection db, long posInfo, long endOfDayId)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("PosInfoId", posInfo);
            queryParameters.Add("endofdayid", endOfDayId);

            string query = @"select AccountId,Accounts.Description,Accounts.Type as AccountType,sum(Amount) as Amount
                                from Transactions
                                inner join Accounts on Accounts.id= AccountId
                                where EndOfDayId=@endofdayid and Transactions.IsDeleted is null and posinfoid=@PosInfoId and TransactionType =4
                                group by AccountId,Accounts.Description,Accounts.Type";

            return db.Query<EodAccountAnalysisModel>(query, queryParameters).ToList<EodAccountAnalysisModel>();
        }

        /// <summary>
        /// Returns a list of analysis per account of barcodes for selected pos for current day
        /// </summary>
        /// <param name="db"> DB connection </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <returns> List of end of day account analysis models. See: <seealso cref="Symposium.Models.Models.EodAccountAnalysisModel"/> </returns>
        public List<EodAccountAnalysisModel> BarcodeAnalysisForCurrentDay(IDbConnection db, long posInfo)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("PosInfoId", posInfo);

            string query = @"select AccountId,Accounts.Description,Accounts.Type as AccountType,sum(Amount) as Amount
                                from Transactions
                                inner join Accounts on Accounts.id= AccountId
                                where EndOfDayId is null and Transactions.IsDeleted is null and posinfoid=@PosInfoId and TransactionType =7
                                group by AccountId,Accounts.Description,Accounts.Type";

            return db.Query<EodAccountAnalysisModel>(query, queryParameters).ToList<EodAccountAnalysisModel>();
        }

        /// <summary>
        /// Returns a list of analysis per account of barcodes for selected pos for selected day
        /// </summary>
        /// <param name="db"> DB connection </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <param name="endOfDayId"> Id of end of day </param>
        /// <returns> List of end of day account analysis models. See: <seealso cref="Symposium.Models.Models.EodAccountAnalysisModel"/> </returns>
        public List<EodAccountAnalysisModel> BarcodeAnalysisForSelectedDay(IDbConnection db, long posInfo, long endOfDayId)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("PosInfoId", posInfo);
            queryParameters.Add("endofdayid", endOfDayId);

            string query = @"select AccountId,Accounts.Description,Accounts.Type as AccountType,sum(Amount) as Amount
                                from Transactions
                                inner join Accounts on Accounts.id= AccountId
                                where EndOfDayId=@endofdayid and Transactions.IsDeleted is null and posinfoid=@PosInfoId and TransactionType =7
                                group by AccountId,Accounts.Description,Accounts.Type";

            return db.Query<EodAccountAnalysisModel>(query, queryParameters).ToList<EodAccountAnalysisModel>();
        }

        /// <summary>
        /// Returns a list of analysis per product for eod pos for current day
        /// </summary>
        /// <param name="db"> DB connection </param>
        /// <returns> List of product for eod analysis models. See: <seealso cref="Symposium.Models.Models.ProductForEodAnalysisModel"/> </returns>
        public List<ProductForEodAnalysisModel> ProductForEodAnalysisForCurrentDay(IDbConnection db, long posInfo)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("PosInfoId", posInfo);

            string query = @"select pfe.ProductId as ProductId, pfe.[Description] as Description, sum(case when InvoiceType = 3 then (-1) * odi.Qty else odi.Qty end) as Quantity, sum(case when InvoiceType = 3 then (-1) * odi.Total else odi.Total end) as Total
                                from ProductsForEOD as pfe
                                inner join OrderDetailInvoices as odi on odi.ProductId = pfe.ProductId
                                where odi.EndOfDayId is null and odi.IsDeleted is null and PosInfoId = @PosInfoId
                                group by pfe.ProductId, pfe.[Description]";

            return db.Query<ProductForEodAnalysisModel>(query, queryParameters).ToList<ProductForEodAnalysisModel>();
        }

        /// <summary>
        /// Returns a list of analysis per product for eod pos for selected day
        /// </summary>
        /// <param name="db"> DB connection </param>
        /// <param name="endOfDayId"> Id of end of day </param>
        /// <returns> List of product for eod analysis models. See: <seealso cref="Symposium.Models.Models.ProductForEodAnalysisModel"/> </returns>
        public List<ProductForEodAnalysisModel> ProductForEodAnalysisForSelectedDay(IDbConnection db, long posInfo, long endOfDayId)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("endofdayid", endOfDayId);
            queryParameters.Add("PosInfoId", posInfo);

            string query = @"select pfe.ProductId as ProductId, pfe.[Description] as Description, sum(case when InvoiceType = 3 then (-1) * odi.Qty else odi.Qty end) as Quantity, sum(case when InvoiceType = 3 then (-1) * odi.Total else odi.Total end) as Total
                                from ProductsForEOD as pfe
                                inner join OrderDetailInvoices as odi on odi.ProductId = pfe.ProductId
                                where odi.EndOfDayId = @endofdayid and odi.IsDeleted is null and PosInfoId = @PosInfoId
                                group by pfe.ProductId, pfe.[Description]";

            return db.Query<ProductForEodAnalysisModel>(query, queryParameters).ToList<ProductForEodAnalysisModel>();
        }

        /// <summary>
        /// Returns a list of transfer to pms charges for selected pos for current day
        /// </summary>
        /// <param name="db"> DB connection </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <returns> List of end of day transfer to pms models. See: <seealso cref="Symposium.Models.Models.EodTransferToPmsModel"/> </returns>
        public List<EodTransferToPmsModel> EodTransferToPms(IDbConnection db, long posInfo)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("PosInfoId", posInfo);

            string query = @"select PmsDepartmentId,PmsDepartmentDescription, hotelId,roomDescription,
                                (select top 1 description from EODAccountToPmsTransfer where posinfoid=@posinfoid and PmsRoom=RoomId order by accountid) as Profilename,
                                sum(total) as total 
                                from TransferToPms 
                                where EndOfDayid is null and TransferToPms.PosInfoid=@posinfoid and sendtopms=0 and isdeleted is null
                                group by PmsDepartmentId, hotelId,PmsDepartmentDescription,roomDescription,RoomId";

            //string query = @"select PmsDepartmentId,PmsDepartmentDescription, hotelId,roomDescription,
            //                    (select top 1 description from EODAccountToPmsTransfer where posinfoid=@posinfoid and PmsRoom=roomDescription order by accountid) as Profilename,
            //                    sum(total) as total 
            //                    from TransferToPms 
            //                    where EndOfDayid is null and TransferToPms.PosInfoid=@posinfoid and sendtopms=0 and isdeleted is null
            //                    group by PmsDepartmentId, hotelId,PmsDepartmentDescription,roomDescription";

            return db.Query<EodTransferToPmsModel>(query, queryParameters).ToList<EodTransferToPmsModel>();
        }

        public long EodDeleteSign(IDbConnection db)
        {
            string query = @"Delete From DigitalSignature";

            int a = db.Execute(query);
            return a;
        }

    }
}
