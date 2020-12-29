using log4net;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.DAOs;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium.WebApi.DataAccess.XMLs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symposium.Helpers.Interfaces;
using Symposium_DTOs.PosModel_Info;
using System.Transactions;
using Dapper;

namespace Symposium.WebApi.DataAccess.DT
{
    /// <summary>
    /// Class that handles data related to End Of Day Procedure
    /// </summary>
    public class EndOfDayDT : IEndOfDayDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IEndOfDayDAO eodDao;
        IGenericDAO<EndOfDayDTO> eodGenericDao;
        IGenericDAO<PosInfoDTO> posInfoGenericDao;
        IGenericDAO<InvoicesDTO> invoicesGenericDao;
        IGenericDAO<OrderDetailInvoicesDTO> orderDetailInvoicesGenericDao;
        IGenericDAO<TransactionsDTO> transactionsGenericDao;
        IGenericDAO<OrderDTO> orderGenericDao;
        IGenericDAO<TransferToPmsDTO> transferToPmsGenericDao;
        IGenericDAO<MealConsumptionDTO> mealConsumptionGenericDao;
        IGenericDAO<CreditTransactionsDTO> creditTransactionsGenericDao;
        IGenericDAO<KitchenInstructionLoggerDTO> kitchenInstructionLoggerGenericDao;
        IGenericDAO<LockersDTO> lockersGenericDao;
        IGenericITableDAO<EndOfDayDTO> eodGenericTableDao;
        IGenericITableDAO<EndOfDayDetailDTO> eodDetailGenericTableDao;
        IGenericITableDAO<EndOfDayPaymentAnalysisDTO> eodPaymentAnalysisGenericTableDao;
        IGenericITableDAO<EndOfDayVoidsAnalysisDTO> eodVoidsAnalysisGenericTableDao;
        IGenericITableDAO<PosInfoDTO> posInfoGenericTableDao;
        IGenericITableDAO<TransferToPmsDTO> transferToPmsGenericTableDao;
        IGenericITableDAO<InvoicesDTO> invoicesGenericTableDao;
        IGenericITableDAO<OrderDetailInvoicesDTO> orderDetailInvoicesGenericTableDao;
        IGenericITableDAO<TransactionsDTO> transactionsGenericTableDao;
        IGenericITableDAO<OrderDTO> orderGenericTableDao;
        IGenericITableDAO<MealConsumptionDTO> mealConsumptionGenericTableDao;
        IGenericITableDAO<CreditTransactionsDTO> creditTransactionsGenericTableDao;
        IGenericITableDAO<KitchenInstructionLoggerDTO> kitchenInstructionLoggerGenericTableDao;
        IGenericITableDAO<LockersDTO> lockersGenericTableDao;
        //  IPaginationHelper<EODAnalysisReceiptModel> pageHlp;


        public EndOfDayDT(IUsersToDatabasesXML usersToDatabases, IEndOfDayDAO eodDao, IGenericDAO<EndOfDayDTO> eodGenericDao, IGenericDAO<PosInfoDTO> posInfoGenericDao, IGenericDAO<InvoicesDTO> invoicesGenericDao, IGenericDAO<OrderDetailInvoicesDTO> orderDetailInvoicesGenericDao, IGenericDAO<TransactionsDTO> transactionsGenericDao, IGenericDAO<OrderDTO> orderGenericDao, IGenericDAO<TransferToPmsDTO> transferToPmsGenericDao, IGenericDAO<MealConsumptionDTO> mealConsumptionGenericDao, IGenericDAO<CreditTransactionsDTO> creditTransactionsGenericDao, IGenericDAO<KitchenInstructionLoggerDTO> kitchenInstructionLoggerGenericDao, IGenericDAO<LockersDTO> lockersGenericDao, IGenericITableDAO<EndOfDayDTO> eodGenericTableDao, IGenericITableDAO<EndOfDayDetailDTO> eodDetailGenericTableDao, IGenericITableDAO<EndOfDayPaymentAnalysisDTO> eodPaymentAnalysisGenericTableDao, IGenericITableDAO<EndOfDayVoidsAnalysisDTO> eodVoidsAnalysisGenericTableDao, IGenericITableDAO<PosInfoDTO> posInfoGenericTableDao, IGenericITableDAO<TransferToPmsDTO> transferToPmsGenericTableDao, IGenericITableDAO<InvoicesDTO> invoicesGenericTableDao, IGenericITableDAO<OrderDetailInvoicesDTO> orderDetailInvoicesGenericTableDao, IGenericITableDAO<TransactionsDTO> transactionsGenericTableDao, IGenericITableDAO<OrderDTO> orderGenericTableDao, IGenericITableDAO<MealConsumptionDTO> mealConsumptionGenericTableDao, IGenericITableDAO<CreditTransactionsDTO> creditTransactionsGenericTableDao, IGenericITableDAO<KitchenInstructionLoggerDTO> kitchenInstructionLoggerGenericTableDao, IGenericITableDAO<LockersDTO> lockersGenericTableDao)
        {
            this.usersToDatabases = usersToDatabases;
            this.eodDao = eodDao;
            this.eodGenericDao = eodGenericDao;
            this.posInfoGenericDao = posInfoGenericDao;
            this.invoicesGenericDao = invoicesGenericDao;
            this.orderDetailInvoicesGenericDao = orderDetailInvoicesGenericDao;
            this.transactionsGenericDao = transactionsGenericDao;
            this.orderGenericDao = orderGenericDao;
            this.transferToPmsGenericDao = transferToPmsGenericDao;
            this.mealConsumptionGenericDao = mealConsumptionGenericDao;
            this.creditTransactionsGenericDao = creditTransactionsGenericDao;
            this.kitchenInstructionLoggerGenericDao = kitchenInstructionLoggerGenericDao;
            this.lockersGenericDao = lockersGenericDao;
            this.eodGenericTableDao = eodGenericTableDao;
            this.eodDetailGenericTableDao = eodDetailGenericTableDao;
            this.eodPaymentAnalysisGenericTableDao = eodPaymentAnalysisGenericTableDao;
            this.eodVoidsAnalysisGenericTableDao = eodVoidsAnalysisGenericTableDao;
            this.posInfoGenericTableDao = posInfoGenericTableDao;
            this.transferToPmsGenericTableDao = transferToPmsGenericTableDao;
            this.invoicesGenericTableDao = invoicesGenericTableDao;
            this.orderDetailInvoicesGenericTableDao = orderDetailInvoicesGenericTableDao;
            this.transactionsGenericTableDao = transactionsGenericTableDao;
            this.orderGenericTableDao = orderGenericTableDao;
            this.mealConsumptionGenericTableDao = mealConsumptionGenericTableDao;
            this.creditTransactionsGenericTableDao = creditTransactionsGenericTableDao;
            this.kitchenInstructionLoggerGenericTableDao = kitchenInstructionLoggerGenericTableDao;
            this.lockersGenericTableDao = lockersGenericTableDao;
            //    this.pageHlp = pageHlp;
        }

        /// <summary>
        /// Get Cashier Analytic Statistics For Cash
        /// </summary>
        /// <param name="StaffId"></param>
        /// <returns>
        /// </returns>
        public List<CashierStatisticsModel> GetCashCashierStatistics(DBInfoModel Store, long StaffId, long PosInfo)
        {
            List<CashierStatisticsModel> CashModel = new List<CashierStatisticsModel>();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            string SqlData = @"SELECT  a.[Description] AS PaymentMethod, t.Amount,  t.[Description] AS PaymentType, t.[Day] AS [Date]
	                            FROM Transactions AS t 
	                            INNER JOIN Accounts AS a ON a.Id = t.AccountId
	                            WHERE t.StaffId =@staffId AND t.EndOfDayId IS NULL AND a.[Type] = 1 AND t.PosInfoId =@posInfoId AND t.IsDeleted IS NULL";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                CashModel = db.Query<CashierStatisticsModel>(SqlData, new { staffId = StaffId, posInfoId = PosInfo }).ToList();
            }
            return CashModel;
        }

        /// <summary>
        /// Get Cashier Analytic Statistics For Cash
        /// </summary>
        /// <param name="StaffId"></param>
        /// <returns>
        /// </returns>
        public List<CashierStatisticsModel> GetCreditCashierStatistics(DBInfoModel Store, long StaffId, long PosInfo)
        {
            List<CashierStatisticsModel> CreditModel = new List<CashierStatisticsModel>();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            string SqlData = @"SELECT  a.[Description] AS PaymentMethod, t.Amount,  t.[Description] AS PaymentType, t.[Day] AS [Date]
	                            FROM Transactions AS t 
	                            INNER JOIN Accounts AS a ON a.Id = t.AccountId
	                            WHERE t.StaffId =@staffId AND t.EndOfDayId IS NULL AND a.[Type] = 4 AND t.PosInfoId =@posInfoId AND t.IsDeleted IS NULL";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                CreditModel = db.Query<CashierStatisticsModel>(SqlData, new { staffId = StaffId, posInfoId = PosInfo }).ToList();
            }
            return CreditModel;
        }

        /// <summary>
        /// Get Cashier Analytic Statistics For Cash
        /// </summary>
        /// <param name="StaffId"></param>
        /// <returns>
        /// </returns>
        public decimal GetCashierTotalCash(DBInfoModel Store, long StaffId, long PosInfo)
        {
            decimal resCash = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            string SqlData = @"SELECT ISNULL(SUM(t.Amount), 0) AS CashTotal
	                            FROM Transactions AS t 
	                            INNER JOIN Accounts AS a ON a.Id = t.AccountId
	                            WHERE t.StaffId =@staffId AND t.EndOfDayId IS NULL AND a.[Type] = 1 AND t.PosInfoId =@posInfoId AND t.IsDeleted IS NULL";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                resCash = db.Query<decimal>(SqlData, new { staffId = StaffId, posInfoId = PosInfo }).FirstOrDefault();
            }
            return resCash;
        }

        /// <summary>
        /// Get Cashier Analytic Statistics For Cash
        /// </summary>
        /// <param name="StaffId"></param>
        /// <returns>
        /// </returns>
        public decimal GetCashierTotalCredit(DBInfoModel Store, long StaffId, long PosInfo)
        {
            decimal resCredit = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            string SqlData = @"SELECT ISNULL(SUM(t.Amount), 0) AS CreditTotal
	                            FROM Transactions AS t 
	                            INNER JOIN Accounts AS a ON a.Id = t.AccountId
	                            WHERE t.StaffId =@staffId AND t.EndOfDayId IS NULL AND a.[Type] = 4 AND t.PosInfoId =@posInfoId AND t.IsDeleted IS NULL";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                resCredit = db.Query<decimal>(SqlData, new { staffId = StaffId, posInfoId = PosInfo }).FirstOrDefault();
            }
            return resCredit;
        }

        /// <summary>
        /// Get Cashier Income,Outcome and Total Amounts for Every Account Type 
        /// </summary>
        public List<CashierTotalAmountsModel> GetCashierAmounts(DBInfoModel dbInfo, long PosInfo)
        {
            List<CashierTotalAmountsModel> model = new List<CashierTotalAmountsModel>();
            List<AccountModel> accountsModel = new List<AccountModel>();

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            string SqlDataAccounts = @"SELECT * FROM Accounts AS a";
            string SqlIncomeAmount = @"SELECT ISNULL(SUM(-t.Amount), 0) AS IncomeAmount
                                        FROM Transactions AS t 
                                        INNER JOIN Accounts AS a ON a.Id = t.AccountId
                                        WHERE t.EndOfDayId IS NULL AND t.TransactionType = 1 AND a.Id =@ID AND t.PosInfoId =@posInfoId AND t.IsDeleted IS NULL";
            string SqlOutcomeAmount = @"SELECT ISNULL(SUM(-t.Amount), 0) AS OutcomeAmount
                                        FROM Transactions AS t 
                                        INNER JOIN Accounts AS a ON a.Id = t.AccountId
                                        WHERE t.EndOfDayId IS NULL AND t.TransactionType = 2 AND a.Id =@ID AND t.PosInfoId =@posInfoId AND t.IsDeleted IS NULL";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                accountsModel = db.Query<AccountModel>(SqlDataAccounts).ToList();

                foreach (AccountModel acc in accountsModel)
                {
                    CashierTotalAmountsModel tmpCashier = new CashierTotalAmountsModel();
                    tmpCashier.IncomeAmount = db.Query<decimal>(SqlIncomeAmount, new { ID = acc.Id, posInfoId = PosInfo }).FirstOrDefault();
                    tmpCashier.OutcomeAmount = db.Query<decimal>(SqlOutcomeAmount, new { ID = acc.Id, posInfoId = PosInfo }).FirstOrDefault();
                    tmpCashier.AccountId = acc.Id;
                    tmpCashier.AccountDescr = acc.Description;
                    tmpCashier.TotalAmount = tmpCashier.IncomeAmount + tmpCashier.OutcomeAmount;
                    model.Add(tmpCashier);
                }
            }

            return model;
        }

        /// <summary>
        /// Get Cashier Total Model  
        /// </summary>
        public CashierTotals GetCashierTotalAmounts(DBInfoModel dbInfo, long PosInfo)
        {
            CashierTotals model = new CashierTotals();
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            string SqlDataIncome = @"SELECT ISNULL(SUM(-t.Amount), 0) AS TotalIncomeAmount
                                     FROM Transactions AS t
                                     WHERE t.EndOfDayId IS NULL AND t.TransactionType = 1 AND t.PosInfoId =@posInfoId AND t.IsDeleted IS NULL";
            string SqlDataOutcome = @"SELECT ISNULL(SUM(-t.Amount), 0) AS TotalOutcomeAmount
                                      FROM Transactions AS t
                                      WHERE t.EndOfDayId IS NULL AND t.TransactionType = 2 AND t.PosInfoId =@posInfoId AND t.IsDeleted IS NULL";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                model.TotalIncomeAmount = db.Query<decimal>(SqlDataIncome, new { posInfoId = PosInfo }).FirstOrDefault();
                model.TotalOutcomeAmount = db.Query<decimal>(SqlDataOutcome, new { posInfoId = PosInfo }).FirstOrDefault();
                model.TotalAmount = model.TotalIncomeAmount + model.TotalOutcomeAmount;
            }
            return model;
        }

        /// <summary>
        /// Get Cashier Income,Outcome and Total Amounts for Every Account Type 
        /// </summary>
        public List<CashierTotalAmountsModel> GetCashierAmountsByStaff(DBInfoModel dbInfo, long StaffId, long PosInfo)
        {
            List<CashierTotalAmountsModel> model = new List<CashierTotalAmountsModel>();
            List<AccountModel> accountsModel = new List<AccountModel>();

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            string SqlDataAccounts = @"SELECT * FROM Accounts AS a";
            string SqlIncomeAmount = @"SELECT ISNULL(SUM(t.Amount), 0) AS IncomeAmount
                                        FROM Transactions AS t 
                                        INNER JOIN Accounts AS a ON a.Id = t.AccountId
                                        WHERE t.EndOfDayId IS NULL AND t.StaffId =@staffId AND t.TransactionType = 1 AND a.Id =@ID AND t.PosInfoId =@posInfoId AND t.IsDeleted IS NULL";
            string SqlOutcomeAmount = @"SELECT ISNULL(SUM(-t.Amount), 0) AS OutcomeAmount
                                        FROM Transactions AS t 
                                        INNER JOIN Accounts AS a ON a.Id = t.AccountId
                                        WHERE t.EndOfDayId IS NULL AND t.StaffId =@staffId AND t.TransactionType = 2 AND a.Id =@ID AND t.PosInfoId =@posInfoId AND t.IsDeleted IS NULL";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                accountsModel = db.Query<AccountModel>(SqlDataAccounts).ToList();

                foreach (AccountModel acc in accountsModel)
                {
                    CashierTotalAmountsModel tmpCashier = new CashierTotalAmountsModel();
                    tmpCashier.IncomeAmount = db.Query<decimal>(SqlIncomeAmount, new { ID = acc.Id, staffId = StaffId, posInfoId = PosInfo }).FirstOrDefault();
                    tmpCashier.OutcomeAmount = db.Query<decimal>(SqlOutcomeAmount, new { ID = acc.Id, staffId = StaffId, posInfoId = PosInfo }).FirstOrDefault();
                    tmpCashier.AccountId = acc.Id;
                    tmpCashier.AccountDescr = acc.Description;
                    tmpCashier.TotalAmount = tmpCashier.IncomeAmount - tmpCashier.OutcomeAmount;
                    model.Add(tmpCashier);
                }
            }

            return model;
        }

        /// <summary>
        /// Get Cashier Total Model  
        /// </summary>
        public CashierTotals GetCashierTotalAmountsByStaff(DBInfoModel dbInfo, long StaffId, long PosInfo)
        {
            CashierTotals model = new CashierTotals();
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            string SqlDataIncome = @"SELECT ISNULL(SUM(t.Amount), 0) AS TotalIncomeAmount
                                     FROM Transactions AS t
                                     WHERE t.EndOfDayId IS NULL  AND t.StaffId =@staffId AND t.TransactionType = 1 AND t.PosInfoId =@posInfoId AND t.IsDeleted IS NULL";
            string SqlDataOutcome = @"SELECT ISNULL(SUM(-t.Amount), 0) AS TotalOutcomeAmount
                                      FROM Transactions AS t
                                      WHERE t.EndOfDayId IS NULL  AND t.StaffId =@staffId AND t.TransactionType = 2 AND t.PosInfoId =@posInfoId AND t.IsDeleted IS NULL";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                model.TotalIncomeAmount = db.Query<decimal>(SqlDataIncome, new { staffId = StaffId, posInfoId = PosInfo }).FirstOrDefault();
                model.TotalOutcomeAmount = db.Query<decimal>(SqlDataOutcome, new { staffId = StaffId, posInfoId = PosInfo }).FirstOrDefault();
                model.TotalAmount = model.TotalIncomeAmount - model.TotalOutcomeAmount;
            }
            return model;
        }

        /// <summary>
        /// Get Count From Transactions paid with Credit Card that have DA_IsPaid = false
        /// <param name="PosInfo"></param>
        /// </summary>
        public long GetExpectedPaidCount(DBInfoModel dbInfo, long PosInfo)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            string SqlData = @"SELECT DISTINCT ISNULL(COUNT(*) , 0 ) AS PaidInvoices
                                FROM [Order] AS o 
                                CROSS APPLY(
	                                SELECT TOP 1 id 
	                                FROM OrderDetail AS od 
	                                WHERE od.OrderId = o.Id
                                ) od
                                CROSS APPLY(
	                                SELECT DISTINCT odi.InvoicesId
	                                FROM OrderDetailInvoices AS odi 
	                                WHERE odi.OrderDetailId = od.id 
                                ) odi
                                INNER JOIN Transactions AS t ON t.InvoicesId = odi.InvoicesId
                                INNER JOIN Accounts AS a ON a.Id = t.AccountId AND a.[Type] = 4
                                WHERE o.DA_IsPaid = 0 AND t.EndOfDayId IS NULL AND t.PosInfoId =@posInfoId";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                res = db.Query<long>(SqlData, new { posInfoId = PosInfo }).FirstOrDefault();
            }
            return res;
        }

        public decimal GetExpectedPaidTotalAmount(DBInfoModel dbInfo, long PosInfo)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            string SqlData = @"SELECT DISTINCT ISNULL(SUM(t.Amount), 0) AS TotalAmount
                                FROM [Order] AS o 
                                CROSS APPLY(
	                                SELECT TOP 1 id 
	                                FROM OrderDetail AS od 
	                                WHERE od.OrderId = o.Id
                                ) od
                                CROSS APPLY(
	                                SELECT DISTINCT odi.InvoicesId
	                                FROM OrderDetailInvoices AS odi 
	                                WHERE odi.OrderDetailId = od.id 
                                ) odi
                                INNER JOIN Transactions AS t ON t.InvoicesId = odi.InvoicesId
                                INNER JOIN Accounts AS a ON a.Id = t.AccountId AND a.[Type] = 4
                                WHERE o.DA_IsPaid = 0 AND t.EndOfDayId IS NULL AND t.PosInfoId =@posInfoId";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                res = db.Query<long>(SqlData, new { posInfoId = PosInfo }).FirstOrDefault();
            }
            return res;
        }

        public decimal GetAlreadyPaidTotalAmount(DBInfoModel dbInfo, long PosInfo)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            string SqlData = @"SELECT DISTINCT ISNULL(SUM(t.Amount), 0) AS TotalAmount
                                FROM [Order] AS o 
                                CROSS APPLY(
	                                SELECT TOP 1 id 
	                                FROM OrderDetail AS od 
	                                WHERE od.OrderId = o.Id
                                ) od
                                CROSS APPLY(
	                                SELECT DISTINCT odi.InvoicesId
	                                FROM OrderDetailInvoices AS odi 
	                                WHERE odi.OrderDetailId = od.id 
                                ) odi
                                INNER JOIN Transactions AS t ON t.InvoicesId = odi.InvoicesId
                                INNER JOIN Accounts AS a ON a.Id = t.AccountId AND a.[Type] = 4
                                WHERE o.DA_IsPaid = 1 AND t.EndOfDayId IS NULL AND t.PosInfoId =@posInfoId";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                res = db.Query<long>(SqlData, new { posInfoId = PosInfo }).FirstOrDefault();
            }
            return res;
        }

        /// <summary>
        /// Get Count From Transactions paid with Credit Card that have DA_IsPaid = true
        /// <param name="PosInfo"></param>
        /// </summary>
        public long GetAlreadyPaidCount(DBInfoModel dbInfo, long PosInfo)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            string SqlData = @"SELECT DISTINCT ISNULL(COUNT(*) , 0 ) AS PaidInvoices
                                FROM [Order] AS o 
                                CROSS APPLY(
	                                SELECT TOP 1 id 
	                                FROM OrderDetail AS od 
	                                WHERE od.OrderId = o.Id
                                ) od
                                CROSS APPLY(
	                                SELECT DISTINCT odi.InvoicesId
	                                FROM OrderDetailInvoices AS odi 
	                                WHERE odi.OrderDetailId = od.id 
                                ) odi
                                INNER JOIN Transactions AS t ON t.InvoicesId = odi.InvoicesId
                                INNER JOIN Accounts AS a ON a.Id = t.AccountId AND a.[Type] = 4
                                WHERE o.DA_IsPaid = 1 AND t.EndOfDayId IS NULL AND t.PosInfoId =@posInfoId";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                res = db.Query<long>(SqlData, new { posInfoId = PosInfo }).FirstOrDefault();
            }
            return res;
        }

        /// <summary>
        /// Get Count From Transactions paid with Credit Card that have DA_IsPaid = false By StaffId
        /// <param name="PosInfo"></param>
        /// </summary>
        public long GetExpectedPaidCountByStaff(DBInfoModel dbInfo, long PosInfo, long StaffId)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            string SqlData = @"SELECT DISTINCT ISNULL(COUNT(*) , 0 ) AS PaidInvoices
                                FROM [Order] AS o 
                                CROSS APPLY(
	                                SELECT TOP 1 id 
	                                FROM OrderDetail AS od 
	                                WHERE od.OrderId = o.Id
                                ) od
                                CROSS APPLY(
	                                SELECT DISTINCT odi.InvoicesId
	                                FROM OrderDetailInvoices AS odi 
	                                WHERE odi.OrderDetailId = od.id 
                                ) odi
                                INNER JOIN Transactions AS t ON t.InvoicesId = odi.InvoicesId
                                INNER JOIN Accounts AS a ON a.Id = t.AccountId AND a.[Type] = 4
                                WHERE o.DA_IsPaid = 0 AND t.EndOfDayId IS NULL AND t.PosInfoId =@posInfoId AND o.StaffId =@staffId";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                res = db.Query<long>(SqlData, new { posInfoId = PosInfo, staffId = StaffId }).FirstOrDefault();
            }
            return res;
        }

        public decimal GetExpectedPaidTotalAmountByStaff(DBInfoModel dbInfo, long PosInfo, long StaffId)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            string SqlData = @"SELECT DISTINCT ISNULL(SUM(t.Amount), 0) AS TotalAmount
                                FROM [Order] AS o 
                                CROSS APPLY(
	                                SELECT TOP 1 id 
	                                FROM OrderDetail AS od 
	                                WHERE od.OrderId = o.Id
                                ) od
                                CROSS APPLY(
	                                SELECT DISTINCT odi.InvoicesId
	                                FROM OrderDetailInvoices AS odi 
	                                WHERE odi.OrderDetailId = od.id 
                                ) odi
                                INNER JOIN Transactions AS t ON t.InvoicesId = odi.InvoicesId
                                INNER JOIN Accounts AS a ON a.Id = t.AccountId AND a.[Type] = 4
                                WHERE o.DA_IsPaid = 0 AND t.EndOfDayId IS NULL AND t.PosInfoId =@posInfoId AND o.StaffId =@staffId";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                res = db.Query<long>(SqlData, new { posInfoId = PosInfo, staffId = StaffId }).FirstOrDefault();
            }
            return res;
        }

        public decimal GetAlreadyPaidTotalAmountByStaff(DBInfoModel dbInfo, long PosInfo, long StaffId)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            string SqlData = @"SELECT DISTINCT ISNULL(SUM(t.Amount), 0) AS TotalAmount
                                FROM [Order] AS o 
                                CROSS APPLY(
	                                SELECT TOP 1 id 
	                                FROM OrderDetail AS od 
	                                WHERE od.OrderId = o.Id
                                ) od
                                CROSS APPLY(
	                                SELECT DISTINCT odi.InvoicesId
	                                FROM OrderDetailInvoices AS odi 
	                                WHERE odi.OrderDetailId = od.id 
                                ) odi
                                INNER JOIN Transactions AS t ON t.InvoicesId = odi.InvoicesId
                                INNER JOIN Accounts AS a ON a.Id = t.AccountId AND a.[Type] = 4
                                WHERE o.DA_IsPaid = 1 AND t.EndOfDayId IS NULL AND t.PosInfoId =@posInfoId AND o.StaffId =@staffId";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                res = db.Query<long>(SqlData, new { posInfoId = PosInfo, staffId = StaffId }).FirstOrDefault();
            }
            return res;
        }

        /// <summary>
        /// Get Count From Transactions paid with Credit Card that have DA_IsPaid = true
        /// <param name="PosInfo"></param>
        /// </summary>
        public long GetAlreadyPaidCountByStaff(DBInfoModel dbInfo, long PosInfo, long StaffId)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            string SqlData = @"SELECT DISTINCT ISNULL(COUNT(*) , 0 ) AS PaidInvoices
                                FROM [Order] AS o 
                                CROSS APPLY(
	                                SELECT TOP 1 id 
	                                FROM OrderDetail AS od 
	                                WHERE od.OrderId = o.Id
                                ) od
                                CROSS APPLY(
	                                SELECT DISTINCT odi.InvoicesId
	                                FROM OrderDetailInvoices AS odi 
	                                WHERE odi.OrderDetailId = od.id 
                                ) odi
                                INNER JOIN Transactions AS t ON t.InvoicesId = odi.InvoicesId
                                INNER JOIN Accounts AS a ON a.Id = t.AccountId AND a.[Type] = 4
                                WHERE o.DA_IsPaid = 1 AND t.EndOfDayId IS NULL AND t.PosInfoId =@posInfoId AND o.StaffId =@staffId";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                res = db.Query<long>(SqlData, new { posInfoId = PosInfo, staffId = StaffId }).FirstOrDefault();
            }
            return res;
        }

        public long UpdateStaffStatus(DBInfoModel Store)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            string SqlData = @"UPDATE Staff SET IsOnRoad =0, StatusTimeChanged = GETDATE()";
            string SqlUpdate = @"UPDATE odi SET odi.EndOfDayId = i.EndOfDayId 
                                FROM orderdetailinvoices AS odi
                                INNER JOIN Invoices AS i ON i.Id = odi.InvoicesId
                                INNER JOIN [Order] AS o ON odi.OrderId = o.Id
                                WHERE odi.EndOfDayId IS NULL AND o.ExtType = 5";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Query<long>(SqlData).FirstOrDefault();
                db.Query<long>(SqlUpdate).FirstOrDefault();
                res = 1;
            }
            return res;
        }

        public decimal GetTipsTotal(DBInfoModel dbInfo, long PosInfo)
        {
            decimal res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            string SqlData = @"SELECT ISNULL(SUM(t.Amount), 0) AS TipsTotal FROM Transactions AS t WHERE t.EndOfDayId IS NULL AND t.TransactionType = 10 AND t.PosInfoId =@posInfoId";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                res = db.Query<decimal>(SqlData, new { posInfoId = PosInfo }).FirstOrDefault();
            }
            return res;
        }

        public decimal GetTipsTotalByStaff(DBInfoModel dbInfo, long PosInfo, long StaffId)
        {
            decimal res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            string SqlData = @"SELECT ISNULL(SUM(t.Amount), 0) AS TipsTotal FROM Transactions AS t WHERE t.EndOfDayId IS NULL AND t.TransactionType = 10 AND t.PosInfoId =@posInfoId AND t.StaffId =@staffId";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                res = db.Query<decimal>(SqlData, new { posInfoId = PosInfo, staffId = StaffId }).FirstOrDefault();
            }
            return res;
        }

        /// <summary>
        /// Return the list of receipt's totals per type for a POS for the current day. 
        /// </summary>
        /// <param name="posInfo">posInfo</param>
        public List<EndOfDayTotalModel> DayTotals(DBInfoModel Store, long posInfo)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return eodDao.EndOfDayTotal(posInfo, db);
            }
        }

        public List<EndOfDayTotalModel> PreviewSalesType(DBInfoModel dbInfo, long PosInfo)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return eodDao.PreviewSalesType(db, PosInfo);
            }
        }

        /// <summary>
        /// Return the list of receipt's totals per type per Staff for a POS for the current day. 
        /// </summary>
        /// <param name="posInfo">posInfo</param>
        public List<EndOfDayByStaffModel> DayTotalsPerStaff(DBInfoModel Store, long posInfo)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return eodDao.EndOfDayPerStaffTotal(posInfo, db);
            }
        }

        /// <summary>
        /// Return the sum of cash and credit cards of barcode transaction types 7 
        /// </summary>
        /// <param name="PosInfo"></param>
        /// <param name="Store"></param>
        /// <returns></returns>
        public EndOfDayBarcodesModel DayBarcodeTotals(long PosInfo, DBInfoModel Store)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return eodDao.DayBarcodeTotals(PosInfo, db);
            }
        }

        /// <summary>
        /// Return the sum of locker transaction types 8, 9
        /// </summary>
        /// <param name="PosInfo"></param>
        /// <param name="Store"></param>
        /// <returns></returns>
        public long DayLockerTotals(long PosInfo, DBInfoModel Store)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return eodDao.DayLockerTotals(PosInfo, db);
            }
        }

        /// <summary>
        /// Return the list of invoises for a specific type for a specific POS.
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <param name="staffId">StaffId.If StaffId is less than 1 then return data for all staffs </param>
        /// <param name="accountId">accountId. Used only for AnalysisByAccount (FOR ID 0)</param>
        /// <param name="type">the type of invoices to return see<seealso cref="Symposium.Models.Enums.EndOfDayReceiptTypes"/></param>
        /// <returns></returns>
        public List<EODAnalysisReceiptModel> GetDayAnalysis(DBInfoModel Store, long posInfo, long staffId, long accountId, EndOfDayReceiptTypes type)
        {
            List<EODAnalysisReceiptModel> list = null;

            //1. get data from DB  PaginationModel<EODAnalysisReceiptModel> 
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                switch (type)
                {
                    case EndOfDayReceiptTypes.Default://AnalysisByAccount
                        list = eodDao.AnalysisByAccount(db, posInfo, accountId, staffId);
                        break;
                    case EndOfDayReceiptTypes.NotPaid:
                        list = eodDao.AnalysisNotPaid(db, posInfo, staffId);
                        break;
                    case EndOfDayReceiptTypes.ReceiptTotal:
                        list = eodDao.AnalysisReceiptTotal(db, posInfo, staffId, 0);
                        break;
                    case EndOfDayReceiptTypes.NotInvoiced:
                        list = eodDao.AnalysisNotInvoiced(db, posInfo, staffId);
                        break;
                    case EndOfDayReceiptTypes.Canceled:
                        list = eodDao.AnalysisCanceled(db, posInfo, staffId);
                        break;
                    case EndOfDayReceiptTypes.NotPrinted:
                        list = eodDao.AnalysisNotPrinted(db, posInfo, staffId);
                        break;
                    case EndOfDayReceiptTypes.DiscountTotal:
                        list = eodDao.AnalysisDiscount(db, posInfo, staffId);
                        break;
                    case EndOfDayReceiptTypes.LoyaltyDiscountTotal:
                        list = eodDao.AnalysisLoyaltyDiscount(db, posInfo, staffId);
                        break;
                }
            }
            return list;

        }

        /// <summary>
        /// Return a list of all printed and not canseled invoices (not ΔΠ) for a specific POS and Staff (EndOfDayReceiptTypes = -100). 
        /// If staffId=0 then return results for all staffs accosiated to the specific POS 
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="posInfo">Pos</param>
        /// <param name="staffId">StaffId.If StaffId is less than 1 then return data for all staffs </param>
        /// <returns></returns>
        public List<EODAnalysisReceiptModel> GetTotalReportAnalysis(DBInfoModel Store, long posInfo, long staffId)
        {
            List<EODAnalysisReceiptModel> list = null;

            //1. get data from DB
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                list = eodDao.AnalysisReceiptTotal(db, posInfo, staffId, 1);
            }
            return list;
        }

        /// <summary>
        /// Creates vat analysis for current or selected day
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <param name="endOfDayId"> Id of end of day </param>
        /// <returns> List of end of day vat analysis models. See: <seealso cref="Symposium.Models.Models.EodVatAnalysisModel"/> </returns>
        public List<EodVatAnalysisModel> GetVatAnalysis(DBInfoModel Store, long posInfo, long? endOfDayId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                if (endOfDayId == null)
                {
                    return eodDao.VatAnalysisForCurrentDay(db, posInfo);
                }
                else
                {
                    return eodDao.VatAnalysisForSelectedDay(db, posInfo, endOfDayId ?? 0);
                }
            }
        }

        /// <summary>
        /// Creates payment analysis for current or selected day
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <param name="endOfDayId"> Id of end of day </param>
        /// <returns> List of end of day account analysis models. See: <seealso cref="Symposium.Models.Models.EodAccountAnalysisModel"/> </returns>
        public List<EodAccountAnalysisModel> GetPaymentAnalysis(DBInfoModel Store, long posInfo, long? endOfDayId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                if (endOfDayId == null)
                {
                    return eodDao.PaymentAnalysisForCurrentDay(db, posInfo);
                }
                else
                {
                    return eodDao.PaymentAnalysisForSelectedDay(db, posInfo, endOfDayId ?? 0);
                }
            }
        }

        /// <summary>
        /// Creates void analysis for current or selected day
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <param name="endOfDayId"> Id of end of day </param>
        /// <returns> List of end of day account analysis models. See: <seealso cref="Symposium.Models.Models.EodAccountAnalysisModel"/> </returns>
        public List<EodAccountAnalysisModel> GetVoidAnalysis(DBInfoModel Store, long posInfo, long? endOfDayId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                if (endOfDayId == null)
                {
                    return eodDao.VoidAnalysisForCurrentDay(db, posInfo);
                }
                else
                {
                    return eodDao.VoidAnalysisForSelectedDay(db, posInfo, endOfDayId ?? 0);
                }
            }
        }

        /// <summary>
        /// Creates barcode analysis for current or selected day
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <param name="endOfDayId"> Id of end of day </param>
        /// <returns> List of end of day account analysis models. See: <seealso cref="Symposium.Models.Models.EodAccountAnalysisModel"/> </returns>
        public List<EodAccountAnalysisModel> GetBarcodeAnalysis(DBInfoModel Store, long posInfo, long? endOfDayId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                if (endOfDayId == null)
                {
                    return eodDao.BarcodeAnalysisForCurrentDay(db, posInfo);
                }
                else
                {
                    return eodDao.BarcodeAnalysisForSelectedDay(db, posInfo, endOfDayId ?? 0);
                }
            }
        }

        /// <summary>
        /// Creates product for eod analysis for current or selected day
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="endOfDayId"> Id of end of day </param>
        /// <returns> List of end of day product analysis models. See: <seealso cref="Symposium.Models.Models.ProductForEodAnalysisModel"/> </returns>
        public List<ProductForEodAnalysisModel> GetProductForEodAnalysis(DBInfoModel Store, long posInfo, long? endOfDayId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                if (endOfDayId == null)
                {
                    return eodDao.ProductForEodAnalysisForCurrentDay(db, posInfo);
                }
                else
                {
                    return eodDao.ProductForEodAnalysisForSelectedDay(db, posInfo, endOfDayId ?? 0);
                }
            }
        }


        /// <summary>
        /// Selects single end of day according to Id
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="endOfDayId"> Id of end of day </param>
        /// <returns> End of day model. See: <seealso cref="Symposium.Models.Models.EndOfDayModel"/> </returns>
        public EndOfDayModel GetSingleEndOfDay(DBInfoModel Store, long endOfDayId)
        {
            EndOfDayDTO endOfDay;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                endOfDay = eodGenericDao.Select(db, endOfDayId);
            }
            return AutoMapper.Mapper.Map<EndOfDayModel>(endOfDay);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"> Store </param>
        /// <returns></returns>
        public long GetLastEndOfDayId(DBInfoModel store)
        {
            long lastEndOfDayId;
            connectionString = usersToDatabases.ConfigureConnectionString(store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                lastEndOfDayId = eodGenericDao.GetMaxId(db, "EndOfDay");
            }
            return lastEndOfDayId;
        }

        /// <summary>
        /// Selects transfers to pms for selected pos
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="posInfo"> Id of posinfo </param>
        /// <returns> Transfer to pms model. See: <seealso cref="Symposium.Models.Models.EodTransferToPmsModel"/> </returns>
        public List<EodTransferToPmsModel> GetListEodTransferToPms(DBInfoModel Store, long posInfo)
        {
            List<EodTransferToPmsModel> list = null;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                list = eodDao.EodTransferToPms(db, posInfo);
            }
            return list;
        }

        /// <summary>
        /// Updates database after closing day and printing Z report
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="posInfoId"> Id of posinfo </param>
        /// <param name="staffId"> Id of staff </param>
        /// <param name="eodModel"> End of day model </param>
        /// <param name="endOfDayDetailModel"> End of day details model </param>
        /// <param name="endOfDayPaymentAnalysisModel"> End of day payments analysis model </param>
        /// <param name="endOfDayVoidsAnalysisModel"> End of day voids analysis model </param>
        /// <param name="eodTransferToPmsModel"> End of day transfer to pms model </param>
        public long GetUpdateDatabaseAfterEod(DBInfoModel Store, long posInfoId, long staffId, EndOfDayModel eodModel, List<EndOfDayDetailModel> endOfDayDetailModel, List<EndOfDayPaymentAnalysisModel> endOfDayPaymentAnalysisModel, List<EndOfDayVoidsAnalysisModel> endOfDayVoidsAnalysisModel, List<EndOfDayTransferToPmsModel> eodTransferToPmsModel)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            int i, rowsAffected;
            long eodAfterInsertId;
            List<EndOfDayDetailDTO> eodDetailDTO = new List<EndOfDayDetailDTO>();
            List<EndOfDayPaymentAnalysisDTO> eodPaymentAnalysisDTO = new List<EndOfDayPaymentAnalysisDTO>();
            List<EndOfDayVoidsAnalysisDTO> eodVoidsAnalysisDTO = new List<EndOfDayVoidsAnalysisDTO>();
            List<TransferToPmsDTO> transferPmsDTO = new List<TransferToPmsDTO>();
            List<PosInfoDTO> posInfoDTO = new List<PosInfoDTO>();

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //TransactionOptions transactionOptions = new TransactionOptions();
                //transactionOptions.Timeout = new TimeSpan(0, 0, 1);
                //using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
                using (var scope = new TransactionScope())
                {
                    eodAfterInsertId = eodGenericDao.Insert(db, AutoMapper.Mapper.Map<EndOfDayDTO>(eodModel));
                    for (i = 0; i < endOfDayDetailModel.Count(); i++)
                    {
                        endOfDayDetailModel[i].EndOfdayId = eodAfterInsertId;
                        eodDetailDTO.Add(AutoMapper.Mapper.Map<EndOfDayDetailDTO>(endOfDayDetailModel[i]));
                    }
                    eodDetailGenericTableDao.InsertList(db, eodDetailDTO);
                    for (i = 0; i < endOfDayPaymentAnalysisModel.Count(); i++)
                    {
                        endOfDayPaymentAnalysisModel[i].EndOfdayId = eodAfterInsertId;
                        eodPaymentAnalysisDTO.Add(AutoMapper.Mapper.Map<EndOfDayPaymentAnalysisDTO>(endOfDayPaymentAnalysisModel[i]));
                    }
                    eodPaymentAnalysisGenericTableDao.InsertList(db, eodPaymentAnalysisDTO);
                    for (i = 0; i < endOfDayVoidsAnalysisModel.Count(); i++)
                    {
                        endOfDayVoidsAnalysisModel[i].EndOfdayId = eodAfterInsertId;
                        eodVoidsAnalysisDTO.Add(AutoMapper.Mapper.Map<EndOfDayVoidsAnalysisDTO>(endOfDayVoidsAnalysisModel[i]));
                    }
                    eodVoidsAnalysisGenericTableDao.InsertList(db, eodVoidsAnalysisDTO);
                    for (i = 0; i < eodTransferToPmsModel.Count(); i++)
                    {
                        eodTransferToPmsModel[i].EndOfDayId = eodAfterInsertId;
                        transferPmsDTO.Add(AutoMapper.Mapper.Map<TransferToPmsDTO>(eodTransferToPmsModel[i]));
                    }
                    transferToPmsGenericTableDao.InsertList(db, transferPmsDTO);
                    posInfoDTO = posInfoGenericDao.Select("select *from PosInfo where Id=@id", new { id = posInfoId }, db);
                    posInfoDTO[0].CloseId++;
                    posInfoDTO[0].IsOpen = false;
                    posInfoGenericTableDao.Upsert(db, posInfoDTO[0]);
                    rowsAffected = invoicesGenericDao.Execute(db, "update Invoices set EndOfDayId = @eodId where EndOfDayId is null and PosInfoId = @id", new { eodId = eodAfterInsertId, id = posInfoId });
                    rowsAffected = orderDetailInvoicesGenericDao.Execute(db, "update orderDetailInvoices set EndOfDayId = @eodId where EndOfDayId is null and PosInfoId = @id", new { eodId = eodAfterInsertId, id = posInfoId });
                    rowsAffected = transactionsGenericDao.Execute(db, "update transactions set EndOfDayId = @eodId where EndOfDayId is null and PosInfoId = @id", new { eodId = eodAfterInsertId, id = posInfoId });
                    rowsAffected = orderGenericDao.Execute(db, "update [order] set EndOfDayId = @eodId where EndOfDayId is null and PosId = @id", new { eodId = eodAfterInsertId, id = posInfoId });
                    rowsAffected = transferToPmsGenericDao.Execute(db, "update transferToPms set EndOfDayId = @eodId where EndOfDayId is null and PosInfoId = @id", new { eodId = eodAfterInsertId, id = posInfoId });
                    rowsAffected = mealConsumptionGenericDao.Execute(db, "update mealConsumption set EndOfDayId = @eodId where EndOfDayId is null and PosInfoId = @id", new { eodId = eodAfterInsertId, id = posInfoId });
                    rowsAffected = creditTransactionsGenericDao.Execute(db, "update creditTransactions set EndOfDayId = @eodId where EndOfDayId is null and PosInfoId = @id", new { eodId = eodAfterInsertId, id = posInfoId });
                    rowsAffected = kitchenInstructionLoggerGenericDao.Execute(db, "update kitchenInstructionLogger set EndOfDayId = @eodId where EndOfDayId is null and PosInfoId = @id", new { eodId = eodAfterInsertId, id = posInfoId });
                    rowsAffected = lockersGenericDao.Execute(db, "update lockers set EndOfDayId = @eodId where PosInfoId = @posinfoid and EndOfDayId is null", new { eodId = eodAfterInsertId, posinfoid = posInfoId });

                    scope.Complete();
                }
            }
            return eodAfterInsertId;
        }


        /// <summary>
        /// Delete All data from Digital Signature after closing day and printing Z report
        /// </summary>
        /// <param name="Store"> Store </param>
        public long ExecuteDeleteDigitalSignatureAfterEod(DBInfoModel Store)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return eodDao.EodDeleteSign(db);
            }
        }


        /// <summary>
        /// Get Lockers Statistics
        /// </summary>
        /// <param name="posInfo">Pos</param>
        /// <returns></returns>
        public LockersStatisticsModel GetLockersStatistics(DBInfoModel Store, long posInfo)
        {
            List<LockersDTO> lockersDTO = new List<LockersDTO>();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    lockersDTO = lockersGenericDao.Select("SELECT TotalLockers, TotalCash, ReturnCash, CloseCash, TotalSplashCash, ReturnSplashCash, CloseSplashCash FROM Lockers WHERE PosInfoId = @posinfoid AND EndOfDayId IS NULL", new { posinfoid = posInfo }, db);
                    if (lockersDTO.Count > 0)
                        return AutoMapper.Mapper.Map<LockersStatisticsModel>(lockersDTO[0]);
                    else
                        return null;
                }
            }
        }

        /// <summary>
        /// Insert zlogger to endofday table
        /// </summary>
        /// <param name="zlogger">Pos</param>
        /// <returns></returns>
        public void InsertZlogger(DBInfoModel Store, string zlogger)
        {
            List<EndOfDayDTO> endOfDayDTO = new List<EndOfDayDTO>();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    endOfDayDTO = eodGenericDao.Select("SELECT TOP 1 *FROM EndOfDay ORDER BY id desc", null, db);
                    zlogger = zlogger.Replace("_", "/");
                    zlogger = zlogger.Replace("$", ".");
                    eodGenericDao.Execute(db, "update EndOfDay set zlogger = '" + zlogger + "' where id = " + endOfDayDTO[0].Id, null);
                    scope.Complete();
                }
            }
        }

        /// <summary>
        /// Staff Insert Cash Amount
        /// </summary>
        /// <param name="StaffId"></param>
        /// <param name="CashAmount"></param>
        /// <returns>
        /// </returns>
        public long InsertStaffCash(DBInfoModel dbInfo, long StaffId, decimal CashAmount)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            string SqlData = @"INSERT INTO StaffCash
                                (
	                                StaffId,
	                                CashAmount,
	                                [Date]
                                )
                                VALUES
                                (
	                                @staffId,
                                    @cashAmount,
                                    @date
                                )";
           
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Execute(SqlData, new { staffId = StaffId, cashAmount = CashAmount, date = DateTime.Now });
                res = 1;
            }
            return res;
        }

        /// <summary>
        ///  Set EndOfDayId to StaffCash Where EndOfDayId is Null
        /// </summary>
        /// <param name="EndOfDayId"></param>
        /// <returns>
        /// </returns>
        public void SetEndOfDayId(DBInfoModel dbInfo, long EndOfDayId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            string SqlData = @"UPDATE StaffCash SET EndOfDayId =@endOfDayId WHERE EndOfDayId IS NULL";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Execute(SqlData, new { endOfDayId = EndOfDayId });
            }
            return;
        }

        /// <summary>
        /// Delete Everything from OrderDetailIngredientsKDS
        /// </summary>
        /// <param name="dbInfo"></param>
        public void DeleteOrderDetailIngredientsKDS(DBInfoModel dbInfo)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            string SqlData = @"DELETE FROM OrderDetailIgredientsKDS";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Execute(SqlData);
            }
            return;
        }
    }
}
