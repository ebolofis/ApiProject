using Dapper;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Symposium.WebApi.DataAccess.DAOs {
    public class ReportDAO : IReportDAO {
        /// <summary>
        /// Get general filters by caller with a database connection constucts a dapper string query
        /// Then on dupper split handle result function groups list of invoices and vats , constuct totals for groups then returns data to primary Task.
        /// Applied function to results produce from flat model
        /// Totals by VatId foreach Invoice of its invoicedetails and Totals by VatId of invoices by AccountId from InvoiceDetails Price by % payment Total of transaction by accountid Total
        /// This % total purpose is dynamically created for each invoice payment cause there is an inipendent by items % of cost and payment
        /// </summary>
        /// <param name="PosInfoId">Filter invoices with endofdayid null and posinfoid if filter is >0 else it is not applied</param>
        /// <param name="DepartmentId">Filter invoices with endofdayid null and DepartmentId if filter is >0 else it is not applied</param>
        /// <param name="StaffId">Filter invoices with endofdayid null and StaffId if filter is >0 else it is not applied</param>
        /// <param name="db">Database instance of DT connection string provided to </param>
        /// <returns>List of Receipt Analysis by AccountId Total Models</returns>
        public List<ReportByAccountTotalsModel> OpenEndOfDayStats(long PosInfoId, long DepartmentId, long StaffId, IDbConnection db) {
            List<ReportByAccountTotalsModel> flatRecAnalysis;
            string query = @"select Distinct 
                                tr.AccountId as AccountId, tr.Description as [Description], 0 as ReceiptCount, Amount,                               
                                inv.Id, inv.Day, inv.Description, deps.Description as DepartmentDescription,
                                inv.PosInfoId, '' as Abbreviation, 
                                inv.Cover as Couver, stf.FirstName as StaffName, inv.StaffId,
                                inv.Total , inv.Discount , tr.Amount, 0 as ItemsCount,
                                inv.OrderNo, inv.Counter as ReceiptNo, inv.InvoiceTypeId as InvoiceType,
                                inv.IsInvoiced, inv.IsVoided, inv.IsPaid, inv.Rooms, inv.TableId,
                                tbl.Code as TableCode,
                                invd.Id, invd.VatId, invd.Total , invd.PosInfoDetailId
                                from Transactions as tr
                                inner join PosInfo as pif on pif.Id = tr.PosInfoId 
                                inner join Invoices as inv on inv.Id = tr.InvoicesId
                                cross apply (
                	                select 
                	                    CASE
                		                WHEN inv.Total = 0 THEN 0
                		                WHEN inv.Total != 0 THEN (tr.Amount / inv.Total) * od.Total
                		                ELSE 0
                		                END AS Total, od.Id , od.VatId , od.InvoiceType , od.PosInfoDetailId
                	                from OrderDetailInvoices as od  
                	                where od.InvoicesId = inv.Id and (od.IsDeleted = 0 or od.IsDeleted is null )
                	                group by Total, Id , VatId, InvoiceType , PosInfoDetailId
                                )invd
                                inner join Staff as stf on stf.Id = inv.StaffId
                                left outer join [Table] as tbl on tbl.Id = inv.TableId
                                INNER JOIN Department deps on deps.Id = tr.DepartmentId
                                where
                                tr.EndOfDayId is null and inv.endOfDayId is null and
                                inv.IsPrinted = 1 and
                                (inv.IsDeleted = 0 or inv.IsDeleted is null) and
                                invd.InvoiceType != 8 ";
            //and inv.Id = 64290 ";

            //Append input filters to query string
            DynamicParameters paramobj = new DynamicParameters();
            if (PosInfoId > 0) {
                paramobj.Add("posfilter", PosInfoId);
                query += " and tr.PosInfoId  = @posfilter ";
            }
            if (DepartmentId > 0) {
                paramobj.Add("departmentfilter", DepartmentId);
                query += " and tr.DepartmentId  = @departmentfilter ";
            }
            if (StaffId > 0) {
                paramobj.Add("staffFilter", StaffId);
                query += " and stf.Id  = @staffFilter ";
            }
            query += " order by inv.Id";
            //lookup lists that do not need to include on dapper q 
            string poid = @"Select  pid.Id as Id , pid.Abbreviation from posinfodetail as pid";
            string accs = @"Select  ac.Id as Id , ac.Description as Description , Type  from accounts as ac";
            string vatq = @"Select  v.Id as Id , v.Description as Description , 0.0  as TotalAmount  from vat as v";
            string vattq = @"Select  v.Id as Id , v.Description as Description , 0.0  as TotalAmountByVat  from vat as v";
            List<PosInfoDetailModel> posinfodetails = db.Query<PosInfoDetailModel>(poid).ToList();
            List<AccountModel> accounts = db.Query<AccountModel>(accs).ToList();
            List<VatDetailsReceiptModel> vats = db.Query<VatDetailsReceiptModel>(vatq).ToList();
            List<VatDetailsAcountTotalModel> vatqs = db.Query<VatDetailsAcountTotalModel>(vattq).ToList();
            var lookup = new Dictionary<long, ReportByAccountTotalsModel>();//Create a data structure to store products uniquely  

            var result = db.Query<ReportByAccountTotalsModel, ReportAnalysisReceiptModel, OrderDetailInvoicesModel, ReportByAccountTotalsModel>( // <TFirst, TSecond, TReturn>
                query, (byacc, invoice, invoicedetail) => {
                    ReportByAccountTotalsModel group;

                    decimal p = invoicedetail.Total ?? new decimal(0.0);
                    //if entitity is not included on dictionary
                    if (!lookup.TryGetValue(byacc.AccountId, out group)) {
                        //append empty vat total models on group 
                        //sqlquery applied to correct auto map of dapper to same model as vatqs was applying to model and cast continued to append results to same obj 
                        //if (byacc.VatDetailsTotal == null)
                        byacc.VatDetailsTotal = db.Query<VatDetailsAcountTotalModel>(vattq).ToList();
                        //append empty vat total models on invoice
                        //if (invoice.VatDetails == null)
                        invoice.VatDetails = new List<VatDetailsReceiptModel>(vats);
                        //initiallize if List of invoices is empty
                        //if (byacc.List == null)
                            byacc.List = new List<ReportAnalysisReceiptModel>();

                        int indv = invoice.VatDetails.IndexOf(invoice.VatDetails.Where(ss => ss.Id == invoicedetail.VatId).FirstOrDefault());
                        if (indv != -1) {
                            invoice.VatDetails[indv].TotalAmount = p;
                        } else {
                            throw new System.InvalidOperationException(Symposium.Resources.Errors.VATDETAILTOTAL);
                        }


                        int indg = byacc.VatDetailsTotal.IndexOf(byacc.VatDetailsTotal.Where(ss => ss.Id == invoicedetail.VatId).FirstOrDefault());
                        if (indg != -1) {
                            byacc.VatDetailsTotal[indg].TotalAmountByVat = p;
                        } else {
                            throw new System.InvalidOperationException(Symposium.Resources.Errors.VATDETAILTOTAL);
                        }
                        invoice.Abbreviation = posinfodetails.Where(d => d.Id == invoicedetail.PosInfoDetailId).Select(s => s.Abbreviation).FirstOrDefault();

                        //the product does not exit into lookup dictionary add the invoice to its List, manage  Amount of group and Count of list
                        byacc.List.Add(invoice);
                        byacc.ReceiptCount = byacc.List.Count();
                        byacc.Description = accounts.Where(d => d.Id == byacc.AccountId).Select(s => s.Description).FirstOrDefault();
                        group = byacc;
                        lookup.Add(byacc.AccountId, byacc);
                    } else {
                        //group (Account Object) exists on dictionary 
                        int invindex = group.List.IndexOf(group.List.Where(iinv => iinv.Id == invoice.Id).FirstOrDefault());
                        //invoice found 
                        //update vat details of invoice[j] **found on indx=j**
                        if (invindex != -1) {
                            int indv = group.List[invindex].VatDetails.IndexOf(group.List[invindex].VatDetails.Where(ss => ss.Id == invoicedetail.VatId).FirstOrDefault());
                            if (indv != -1) {
                                //on invoice[j].Vatdetails[k] = update price by p 
                                group.List[invindex].VatDetails[indv].TotalAmount = group.List[invindex].VatDetails[indv].TotalAmount + p;
                            } else {
                                //vat detail not found 
                                throw new System.InvalidOperationException(Symposium.Resources.Errors.VATDETAILTOTAL);
                            }
                        } else {
                            //invoice not been registered
                            //search invoice loaded to GroupList
                            if (invoice.VatDetails == null) {
                                invoice.VatDetails = new List<VatDetailsReceiptModel>(vats);
                            }
                            //search InvoiceVatDetails[k] on equal vatid and 
                            int indv = invoice.VatDetails.IndexOf(invoice.VatDetails.Where(ss => ss.Id == invoicedetail.VatId).FirstOrDefault());
                            if (indv != -1) {
                                //update invoicedetails[j] totalamount by p 
                                invoice.VatDetails[indv].TotalAmount = p;
                            } else {
                                throw new System.InvalidOperationException(Symposium.Resources.Errors.VATDETAILTOTAL);
                            }
                            invoice.Abbreviation = posinfodetails.Where(d => d.Id == invoicedetail.PosInfoDetailId).Select(s => s.Abbreviation).FirstOrDefault();
                            group.List.Add(invoice);
                            group.Amount = group.Amount + invoice.Total;
                            group.ReceiptCount = group.List.Count();
                        }

                        //Group Vat Details has to update even if invoice is found or not 
                        int groupVindx = group.VatDetailsTotal.IndexOf(group.VatDetailsTotal.Where(ss => ss.Id == invoicedetail.VatId).FirstOrDefault());
                        if (groupVindx != -1) {
                            group.VatDetailsTotal[groupVindx].TotalAmountByVat = group.VatDetailsTotal[groupVindx].TotalAmountByVat + p;
                        } else {
                            //invoice not been registered
                            throw new System.InvalidOperationException(Symposium.Resources.Errors.VATDETAILTOTAL);
                        }
                    }
                    return group;
                }, paramobj);
            flatRecAnalysis = lookup.Select(x => x.Value).ToList<ReportByAccountTotalsModel>();

            return flatRecAnalysis;
        }
    }
}
