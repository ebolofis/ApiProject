using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Dapper;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_Test.MockupObjs
{
    class EndOfDayDaoMock : IEndOfDayDAO
    {
        /// <summary>
        /// Return a face list of EndofDay Totals per Type of Totals.
        /// <seealso cref="Symposium.Models.Enums.EndOfDayReceiptTypes"/>
        /// <para>call the sp EndOfDayTotal</para>
        /// </summary>
        /// <param name="posInfo">posInfo</param>
        /// <param name="db">DB Connection</param>
        /// <returns>List of EndOfDayTotalModel</returns>
        public List<EndOfDayTotalModel> EndOfDayTotal(long posInfo, IDbConnection db)
        {
            List<EndOfDayTotalModel> result = new List<EndOfDayTotalModel>();
            result.Add(new EndOfDayTotalModel() { Id= EndOfDayReceiptTypes.Default, Description= "Cash", ReceiptCount= 15, Amount=221.5M,AccountId=1,AccountType=1 });//0
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.Default, Description = "Room Charge-C.C", ReceiptCount = 14, Amount = 22.678M, AccountId = 3, AccountType = 3 });//0
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.Default, Description = "MASTERCARD", ReceiptCount = 13, Amount = 22.15M, AccountId = 8, AccountType = 4 });//0
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.NotPaid, Description = "Not Paid", ReceiptCount = 12, Amount = 12.15M, AccountId = 0, AccountType = 0 });//-99
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.ReceiptTotal, Description = "Receipt Total", ReceiptCount = 11, Amount = 0.15M, AccountId = 0, AccountType = 0 });//-100
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.NotInvoiced, Description = "Not Invoiced", ReceiptCount = 10, Amount = 123M, AccountId = 0, AccountType = 0 });//-101
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.Canceled, Description = "Canceled", ReceiptCount = 9, Amount = 0, AccountId = 0, AccountType = 0 });//-102
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.NotPrinted, Description = "Not Printed", ReceiptCount = 8, Amount = 6.4M, AccountId = 0, AccountType = 0 });//-103
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.DiscountTotal, Description = "Discount Total", ReceiptCount = 7, Amount = 0.12M, AccountId = 0, AccountType = 0 });//-104
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.Couvers, Description = "Couvers", ReceiptCount = 6, Amount = 2345.001M, AccountId = 0, AccountType = 0 });//-105
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.OpenCashier, Description = "Open Cashier", ReceiptCount = 5, Amount = 123M, AccountId = 0, AccountType = 0 });//-106
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.CloseCashier, Description = "Close Cashier", ReceiptCount = 4, Amount = 456.0M, AccountId = 0, AccountType = 0 });//-107
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.AccountReturn, Description = "Account Return", ReceiptCount = 3, Amount = 8M, AccountId = 0, AccountType = 0 });//-108
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.TotalToReturn, Description = "Total Return", ReceiptCount = 2, Amount = 9.12M, AccountId = 0, AccountType = 0 });//-109

            return result;
           // return db.Query<EndOfDayTotalModel>("EndOfDayTotal", new { posInfo = posInfo }, commandType: CommandType.StoredProcedure).ToList<EndOfDayTotalModel>();
        }

        /// <summary>
        /// Return a face list of EndofDay Totals per Type of Totals.
        /// <seealso cref="Symposium.Models.Enums.EndOfDayReceiptTypes"/>
        /// <para>call the sp EndOfDayTotal</para>
        /// </summary>
        /// <param name="posInfo">posInfo</param>
        /// <param name="db">DB Connection</param>
        /// <returns>List of EndOfDayTotalModel</returns>
        public List<EndOfDayTotalModel> PreviewSalesType(IDbConnection db, long posInfo)
        {
            List<EndOfDayTotalModel> result = new List<EndOfDayTotalModel>();
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.Default, Description = "Cash", ReceiptCount = 15, Amount = 221.5M, AccountId = 1, AccountType = 1 });//0
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.Default, Description = "Room Charge-C.C", ReceiptCount = 14, Amount = 22.678M, AccountId = 3, AccountType = 3 });//0
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.Default, Description = "MASTERCARD", ReceiptCount = 13, Amount = 22.15M, AccountId = 8, AccountType = 4 });//0
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.NotPaid, Description = "Not Paid", ReceiptCount = 12, Amount = 12.15M, AccountId = 0, AccountType = 0 });//-99
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.ReceiptTotal, Description = "Receipt Total", ReceiptCount = 11, Amount = 0.15M, AccountId = 0, AccountType = 0 });//-100
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.NotInvoiced, Description = "Not Invoiced", ReceiptCount = 10, Amount = 123M, AccountId = 0, AccountType = 0 });//-101
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.Canceled, Description = "Canceled", ReceiptCount = 9, Amount = 0, AccountId = 0, AccountType = 0 });//-102
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.NotPrinted, Description = "Not Printed", ReceiptCount = 8, Amount = 6.4M, AccountId = 0, AccountType = 0 });//-103
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.DiscountTotal, Description = "Discount Total", ReceiptCount = 7, Amount = 0.12M, AccountId = 0, AccountType = 0 });//-104
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.Couvers, Description = "Couvers", ReceiptCount = 6, Amount = 2345.001M, AccountId = 0, AccountType = 0 });//-105
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.OpenCashier, Description = "Open Cashier", ReceiptCount = 5, Amount = 123M, AccountId = 0, AccountType = 0 });//-106
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.CloseCashier, Description = "Close Cashier", ReceiptCount = 4, Amount = 456.0M, AccountId = 0, AccountType = 0 });//-107
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.AccountReturn, Description = "Account Return", ReceiptCount = 3, Amount = 8M, AccountId = 0, AccountType = 0 });//-108
            result.Add(new EndOfDayTotalModel() { Id = EndOfDayReceiptTypes.TotalToReturn, Description = "Total Return", ReceiptCount = 2, Amount = 9.12M, AccountId = 0, AccountType = 0 });//-109

            return result;
            // return db.Query<EndOfDayTotalModel>("EndOfDayTotal", new { posInfo = posInfo }, commandType: CommandType.StoredProcedure).ToList<EndOfDayTotalModel>();
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
            List<EndOfDayByStaffModel> result = new List<EndOfDayByStaffModel>();
            result.Add(new EndOfDayByStaffModel() { Id = EndOfDayReceiptTypes.Default, Description = "Cash", ReceiptCount = 12, Amount = 22.5M, AccountId = 1, AccountType = 1,StaffId=1 ,StaffName="staff1"});//0
            result.Add(new EndOfDayByStaffModel() { Id = EndOfDayReceiptTypes.Default, Description = "Room Charge-C.C", ReceiptCount = 14, Amount = 22.678M, AccountId = 3, AccountType = 3, StaffId = 1, StaffName = "staff1" });//0
            result.Add(new EndOfDayByStaffModel() { Id = EndOfDayReceiptTypes.Default, Description = "MASTERCARD", ReceiptCount = 13, Amount = 22.15M, AccountId = 8, AccountType = 4, StaffId = 1, StaffName = "staff1" });//0
            result.Add(new EndOfDayByStaffModel() { Id = EndOfDayReceiptTypes.NotPaid, Description = "Not Paid", ReceiptCount = 12, Amount = 12.15M, AccountId = 0, AccountType = 0, StaffId = 1, StaffName = "staff1" });//-99
            result.Add(new EndOfDayByStaffModel() { Id = EndOfDayReceiptTypes.ReceiptTotal, Description = "Receipt Total", ReceiptCount = 11, Amount = 0.15M, AccountId = 0, AccountType = 0, StaffId = 1, StaffName = "staff1" });//-100
            result.Add(new EndOfDayByStaffModel() { Id = EndOfDayReceiptTypes.NotInvoiced, Description = "Not Invoiced", ReceiptCount = 10, Amount = 123M, AccountId = 0, AccountType = 0, StaffId = 1, StaffName = "staff1" });//-101
            result.Add(new EndOfDayByStaffModel() { Id = EndOfDayReceiptTypes.Canceled, Description = "Canceled", ReceiptCount = 9, Amount = 0, AccountId = 0, AccountType = 0, StaffId = 1, StaffName = "staff1" });//-102
            result.Add(new EndOfDayByStaffModel() { Id = EndOfDayReceiptTypes.NotPrinted, Description = "Not Printed", ReceiptCount = 8, Amount = 6.4M, AccountId = 0, AccountType = 0, StaffId = 1, StaffName = "staff1" });//-103
            result.Add(new EndOfDayByStaffModel() { Id = EndOfDayReceiptTypes.DiscountTotal, Description = "Discount Total", ReceiptCount = 7, Amount = 0.12M, AccountId = 0, AccountType = 0, StaffId = 1, StaffName = "staff1" });//-104
            result.Add(new EndOfDayByStaffModel() { Id = EndOfDayReceiptTypes.Couvers, Description = "Couvers", ReceiptCount = 6, Amount = 2345.001M, AccountId = 0, AccountType = 0, StaffId = 1, StaffName = "staff1" });//-105
            result.Add(new EndOfDayByStaffModel() { Id = EndOfDayReceiptTypes.OpenCashier, Description = "Open Cashier", ReceiptCount = 5, Amount = 123M, AccountId = 0, AccountType = 0, StaffId = 1, StaffName = "staff1" });//-106
            result.Add(new EndOfDayByStaffModel() { Id = EndOfDayReceiptTypes.CloseCashier, Description = "Close Cashier", ReceiptCount = 4, Amount = 456.0M, AccountId = 0, AccountType = 0, StaffId = 1, StaffName = "staff1" });//-107
            result.Add(new EndOfDayByStaffModel() { Id = EndOfDayReceiptTypes.AccountReturn, Description = "Account Return", ReceiptCount = 3, Amount = 8M, AccountId = 0, AccountType = 0, StaffId = 1, StaffName = "staff1" });//-108
            result.Add(new EndOfDayByStaffModel() { Id = EndOfDayReceiptTypes.TotalToReturn, Description = "Total Return", ReceiptCount = 2, Amount = 9.12M, AccountId = 0, AccountType = 0, StaffId = 1, StaffName = "staff1" });//-109
            result.Add(new EndOfDayByStaffModel() { Id = EndOfDayReceiptTypes.Default, Description = "Cash", ReceiptCount = 5, Amount = 51.5M, AccountId = 1, AccountType = 1, StaffId = 2, StaffName = "staff2" });//0
            result.Add(new EndOfDayByStaffModel() { Id = EndOfDayReceiptTypes.Default, Description = "Room Charge-C.C", ReceiptCount = 14, Amount = 7.678M, AccountId = 3, AccountType = 3, StaffId = 2, StaffName = "staff2" });//0

            return result;
            // return db.Query<EndOfDayByStaffModel>("EndOfDayByStaff", new { posInfo = posInfo }, commandType: CommandType.StoredProcedure).ToList<EndOfDayByStaffModel>();
        }

        public EndOfDayBarcodesModel DayBarcodeTotals(long posInfo, IDbConnection db)
        {
            return null;
        }

        public long DayLockerTotals(long posInfo, IDbConnection db)
        {
            return 0;
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
            return getMockupData();
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
            return getMockupData();
        }

        /// <summary>
        /// Return a list of all printed and not voiced invoices for a specific POS and Staff (EndOfDayReceiptTypes = -100). 
        /// If staffId=0 then return results for all staffs accosiated to the specific POS 
        /// </summary>
        /// <param name="db">DB Connection</param>
        /// <param name="posInfo">posInfo</param>
        /// <param name="staffId">staffId</param>
        /// <returns></returns>
        public List<EODAnalysisReceiptModel> AnalysisReceiptTotal(IDbConnection db, long posInfo, long staffId = 0, int all = 0)
        {
            return getMockupData();
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
            return getMockupData();
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
            return getMockupData();
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
            return getMockupData();
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
            return getMockupData();
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
            return getMockupData();
        }

        public List<EodVatAnalysisModel> VatAnalysisForCurrentDay(IDbConnection db, long posInfo)
        {
            List<EodVatAnalysisModel> result = new List<EodVatAnalysisModel>();
            result.Add(new EodVatAnalysisModel() { VatId = 1, VatRate = 23, Gross = 18, Net = 14.64M, Tax = 0, Discount = 0, ItemCount = 2, TicketCount = 2 });
            result.Add(new EodVatAnalysisModel() { VatId = 2, VatRate = 13, Gross = 194.68M, Net = 172.28M, Tax = 0, Discount = 0, ItemCount = 9, TicketCount = 3 });
            return result;
        }

        public List<EodVatAnalysisModel> VatAnalysisForSelectedDay(IDbConnection db, long posInfo, long endOfDayId)
        {
            List<EodVatAnalysisModel> result = new List<EodVatAnalysisModel>();
            result.Add(new EodVatAnalysisModel() { VatId = 1, VatRate = 23, Gross = 18, Net = 14.64M, Tax = 0, Discount = 0, ItemCount = 2, TicketCount = 2 });
            result.Add(new EodVatAnalysisModel() { VatId = 2, VatRate = 13, Gross = 194.68M, Net = 172.28M, Tax = 0, Discount = 0, ItemCount = 9, TicketCount = 3 });
            return result;
        }

        public List<EodAccountAnalysisModel> PaymentAnalysisForCurrentDay(IDbConnection db, long posInfo)
        {
            List<EodAccountAnalysisModel> result = new List<EodAccountAnalysisModel>();
            result.Add(new EodAccountAnalysisModel() { AccountId = 1, Description = "Cash", AccountType = 1, Amount = 0 });
            result.Add(new EodAccountAnalysisModel() { AccountId = 3, Description = "Room Charge-C.C", AccountType = 3, Amount = 9 });
            result.Add(new EodAccountAnalysisModel() { AccountId = 8, Description = "MASTERCARD", AccountType = 4, Amount = 0 });
            result.Add(new EodAccountAnalysisModel() { AccountId = 9, Description = "VISA", AccountType = 4, Amount = 95 });
            return result;
        }

        public List<EodAccountAnalysisModel> PaymentAnalysisForSelectedDay(IDbConnection db, long posInfo, long endOfDayId)
        {
            List<EodAccountAnalysisModel> result = new List<EodAccountAnalysisModel>();
            result.Add(new EodAccountAnalysisModel() { AccountId = 1, Description = "Cash", AccountType = 1, Amount = 0 });
            result.Add(new EodAccountAnalysisModel() { AccountId = 3, Description = "Room Charge-C.C", AccountType = 3, Amount = 9 });
            result.Add(new EodAccountAnalysisModel() { AccountId = 8, Description = "MASTERCARD", AccountType = 4, Amount = 0 });
            result.Add(new EodAccountAnalysisModel() { AccountId = 9, Description = "VISA", AccountType = 4, Amount = 95 });
            return result;
        }

        public List<EodAccountAnalysisModel> VoidAnalysisForCurrentDay(IDbConnection db, long posInfo)
        {
            List<EodAccountAnalysisModel> result = new List<EodAccountAnalysisModel>();
            result.Add(new EodAccountAnalysisModel() { AccountId = 1, Description = "Cash", AccountType = 1, Amount = -29.18M });
            result.Add(new EodAccountAnalysisModel() { AccountId = 8, Description = "MASTERCARD", AccountType = 4, Amount = -79.50M });
            return result;
        }

        public List<EodAccountAnalysisModel> VoidAnalysisForSelectedDay(IDbConnection db, long posInfo, long endOfDayId)
        {
            List<EodAccountAnalysisModel> result = new List<EodAccountAnalysisModel>();
            result.Add(new EodAccountAnalysisModel() { AccountId = 1, Description = "Cash", AccountType = 1, Amount = -29.18M });
            result.Add(new EodAccountAnalysisModel() { AccountId = 8, Description = "MASTERCARD", AccountType = 4, Amount = -79.50M });
            return result;
        }

        public List<EodAccountAnalysisModel> BarcodeAnalysisForCurrentDay(IDbConnection db, long posInfo)
        {
            List<EodAccountAnalysisModel> result = new List<EodAccountAnalysisModel>();
            return result;
        }

        public List<EodAccountAnalysisModel> BarcodeAnalysisForSelectedDay(IDbConnection db, long posInfo, long endOfDayId)
        {
            List<EodAccountAnalysisModel> result = new List<EodAccountAnalysisModel>();
            return result;
        }

        public List<ProductForEodAnalysisModel> ProductForEodAnalysisForCurrentDay(IDbConnection db, long posInfo)
        {
            List<ProductForEodAnalysisModel> result = new List<ProductForEodAnalysisModel>();
            return result;
        }

        public List<ProductForEodAnalysisModel> ProductForEodAnalysisForSelectedDay(IDbConnection db, long posInfo, long endOfDayId)
        {
            List<ProductForEodAnalysisModel> result = new List<ProductForEodAnalysisModel>();
            return result;
        }

        public List<EodTransferToPmsModel> EodTransferToPms(IDbConnection db, long posInfo)
        {
            List<EodTransferToPmsModel> result = new List<EodTransferToPmsModel>();
            result.Add(new EodTransferToPmsModel() { PmsDepartmentId = 1, PmsDepartmentDescription = "Room Charge 6 %", hotelId = 1, roomDescription = "9601", Profilename = "Μετρητά", total = 0 });
            result.Add(new EodTransferToPmsModel() { PmsDepartmentId = 14, PmsDepartmentDescription = "Cafe Nikki Beverage 13%", hotelId = 1, roomDescription = "9601", Profilename = "Μετρητά", total = 0 });
            return result;
        }

        /// <summary>
        /// use CrvHelper to read data from csv file
        /// </summary>
        /// <returns></returns>
        private List<EODAnalysisReceiptModel> getMockupData()
        {
            CsvReader csv = null;
            using (TextReader reader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + @"\MockupData\EndOfDayAnalysis.csv"))
            {

                var dateTimeOptions = new TypeConverterOptions
                {
                    Format = "yyyy-MM-dd HH:mm:ss,fff",
                };
                CsvHelper.TypeConversion.TypeConverterOptionsFactory.AddOptions<DateTime>(dateTimeOptions);

                csv = new CsvReader(reader);
                csv.Configuration.Delimiter = ";";
                return csv.GetRecords<EODAnalysisReceiptModel>().ToList<EODAnalysisReceiptModel>();
            }
        }

        public long EodDeleteSign(IDbConnection db)
        {
            throw new NotImplementedException();
        }
    }




  


}
