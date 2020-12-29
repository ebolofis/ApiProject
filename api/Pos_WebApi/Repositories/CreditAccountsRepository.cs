using Pos_WebApi.Helpers;
using Pos_WebApi.Models;
using System;
using System.Linq;
using Pos_WebApi.Models.DTOModels;
using System.Data.Entity;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ServiceStack;
using Newtonsoft.Json;
using log4net;
using Symposium.Models.Models;
using Symposium.Models.Enums;

namespace Pos_WebApi.Repositories
{
    public class CreditAccountsRepository
    {
        protected PosEntities DbContext;
        protected InvoiceRepository repo;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public CreditAccountsRepository(PosEntities db)
        {
            this.DbContext = db;
            repo = new InvoiceRepository(DbContext);
        }

        public CreditAccountsDTO GetAccount(string code, long? creditaccountid = null)
        {
            var ca = CheckIfCodeExists(code);
            if (ca == null)
            {
                var cc = CreateCreditCodes(code, creditaccountid);
                if (creditaccountid == null)
                {
                    ca = ActivateAccount(cc);
                }
                else
                {
                    DbContext.SaveChanges();
                    ca = CheckIfCodeExists(code);
                }
            }
            else
            {
                if (creditaccountid != null)
                {
                    return null;
                }
            }
            FindConnectedAccounts(ca);
            return ca;
        }

        public CreditAccounts GetAccount(string code)
        {
            var creditAccounts = from q in DbContext.CreditCodes
                                 join qq in DbContext.CreditAccounts on q.CreditAccountId equals qq.Id
                                 where q.Code == code
                                 select q.CreditAccounts;

            return creditAccounts.SingleOrDefault();
        }

        public CreditAccountsDTO CheckIfCodeExists(string code)
        {
            var creditAccounts = (from q in DbContext.CreditCodes
                                  join qq in DbContext.CreditAccounts on q.CreditAccountId equals qq.Id
                                  join qqq in DbContext.CreditTransactions on q.CreditAccountId equals qqq.CreditAccountId into f
                                  from ct in f.DefaultIfEmpty()
                                  join qqqq in DbContext.Transactions on ct.TransactionId equals qqqq.Id into ff
                                  from t in ff.DefaultIfEmpty()
                                  where q.Code == code && qq.EndOfDayId == null
                                  select new //
                                  {
                                      Id = qq.Id,
                                      Description = qq.Description,
                                      ActivateTS = qq.ActivateTS,
                                      DeactivateTS = qq.DeactivateTS,
                                      CreditAccountId = qq.Id,
                                      CreditCodesCode = q.Code,
                                      CreditCodesId = q.Id,
                                      CreditCodesDescription = q.Description,
                                      AccountId = t.AccountId,
                                      PosInfoId = ct != null ? ct.PosInfoId : null,
                                      Amount = ct != null ? ct.Amount : 0,
                                      CreationTS = ct != null ? ct.CreationTS : null,
                                      Type = ct != null ? ct.Type : null,
                                      StaffId = ct != null ? ct.StaffId : null,
                                      CreditTransactionsDescription = ct != null ? ct.Description : "",
                                      TransactionId = ct != null ? ct.TransactionId : null,
                                      CreditTransactionId = ct != null ? ct.Id : 0
                                  }).ToList().GroupBy(g => g.Id).Select(s => new CreditAccountsDTO
                                  {
                                      Id = s.Key,
                                      Description = s.FirstOrDefault().Description,
                                      ActivateTS = s.FirstOrDefault().ActivateTS,
                                      DeactivateTS = s.FirstOrDefault().DeactivateTS,
                                      CreditCodesId = s.FirstOrDefault(f => f.CreditCodesCode == code).CreditCodesId,
                                      CreditCodesCode = s.FirstOrDefault(f => f.CreditCodesCode == code).CreditCodesCode,
                                      CreditCodesDescription = s.FirstOrDefault(f => f.CreditCodesCode == code).CreditCodesDescription,
                                      HasConnectedAccounts = s.Count() > 1,
                                      Balance = s.Sum(sm => sm.Amount),
                                      CreditAccountTransactions = s.Select(ss => new CreditAccountTransactions
                                      {
                                          AccountId = ss.AccountId,
                                          TransactionId = ss.TransactionId,
                                          CreditTransactionId = ss.CreditTransactionId,
                                          CreditAccountTransactionsDescription = ss.CreditTransactionsDescription,
                                          Type = ss.Type,
                                          Amount = ss.Amount,
                                          CreationTS = ss.CreationTS
                                      }).Where(sss => sss.CreditTransactionId != 0).ToList()
                                  });
            if (creditAccounts == null)
                return null;
            return creditAccounts.FirstOrDefault();
        }

        public List<CreditAccountsDTO> CreateCreditAcc(string str)
        {
            List<CreditAccountsDTO> res = new List<CreditAccountsDTO>();
            CreditAccountsDTO tempCred = new CreditAccountsDTO();
            List<CredAccounts> tmpJson = JsonConvert.DeserializeObject<List<CredAccounts>>(str);
            foreach (var item in tmpJson)
            {
                if (!item.Codes.Contains(','))
                {
                    res.Add(CheckIfCodeExists(item.Codes));
                }
                else
                {
                    string[] tempStr = item.Codes.Split(',');
                    foreach (string element in tempStr)
                    {
                        res.Add(CheckIfCodeExists(element));
                    }
                    
                }
            }


            return res;
        }

        private void FindConnectedAccounts(CreditAccountsDTO account)
        {
            account.ConnectedCreditCodes = (from q in DbContext.CreditCodes
                                            where q.CreditAccountId == account.Id
                                            select q.Description).ToList();
            return;
        }

        /// <summary>
        /// Gets All Credits
        /// </summary>
        public string GetAllCred() {
            
            var CCodes = DbContext.CreditCodes
                 .GroupBy(x => x.CreditAccountId)
                 .ToList()
                 .Select(x => new
                 {
                     CreditAccountId = x.Key,
                     Codes = string.Join(", ", x.Select(n => n.Code))
                 }).OrderBy(x => x.CreditAccountId);

            var CTransactions = DbContext.CreditTransactions
                                .GroupBy(x => x.CreditAccountId)
                                .Select(x => new
                                {
                                    CreditAccountId = x.Key,
                                    Amount = x.Sum(i => i.Amount)
                                }).OrderBy(x => x.CreditAccountId);


            var result2 = from ca in DbContext.CreditAccounts.AsEnumerable()
                          join cc in CCodes on ca.Id equals cc.CreditAccountId
                          join ct in CTransactions on ca.Id equals ct.CreditAccountId
                          where ca.DeactivateTS == null && ca.EndOfDayId == null                      
                          select new {
                                     Id = ca.Id,
                                     ActivateTS = string.Format("{0: MM/dd/yyyy}", ca.ActivateTS),
                                     Codes = cc.Codes,
                                     Amount = ct.Amount,
                          };

            return result2.ToList().ToJson();
        }

        private CreditAccountsDTO ActivateAccount(CreditCodes code)
        {
            CreditAccounts cc = new CreditAccounts()
            {
                ActivateTS = DateTime.Now,
                Description = code.Description,

            };
            cc.CreditCodes.Add(code);
            DbContext.CreditAccounts.Add(cc);
            DbContext.SaveChanges();

            CreditAccountsDTO dto = new CreditAccountsDTO()
            {
                Id = cc.Id,
                Description = cc.Description,
                ActivateTS = cc.ActivateTS,
                DeactivateTS = cc.DeactivateTS,
                CreditCodesId = code.Id,
                CreditCodesCode = code.Code,
                CreditCodesDescription = code.Description,
                HasConnectedAccounts = false,
            };
            return dto;
        }

        private bool DeActivateAccount(long creditAccountId)
        {
            CreditAccounts cc = DbContext.CreditAccounts.Where(w => w.Id == creditAccountId).SingleOrDefault();
            cc.DeactivateTS = DateTime.Now;
            DbContext.Entry(cc).State = EntityState.Modified;
            return true;
        }

        private CreditCodes CreateCreditCodes(string code, long? creditAccountId)
        {
            CreditCodes cc = new CreditCodes();
            cc.Code = code;
            cc.Description = code;
            cc.Type = 1;
            cc.CreditAccountId = creditAccountId;
            DbContext.CreditCodes.Add(cc);
            return cc;

        }


        public dynamic AddCredit(CreditTransactionDTO model)
        {
            var res = CreateInvoiceForCredit(CreditTransactionType.AddCredit,
                                            CreditTransactionEnum.Add,
                                            model.PosInfoId,
                                            model.StaffId,
                                            model.CreditAccountId,
                                            model.CreditAccountDescription,
                                            //"Add amount on Barcode Credit Account",
                                            model.Amount,
                                            model.AccountId,
                                            model.CreditCodeId);


            bool externalCommunication = true;
            return repo.InsertInvoice(res, out externalCommunication);// CheckIfCodeExists(creditAccountDescription);
        }

        public int SaveChanges()
        {
            return DbContext.SaveChanges();
        }

        public dynamic ReturnCredit(CreditTransactionDTO model, bool deactivate = false)
        {
            var res = CreateInvoiceForCredit(deactivate ? CreditTransactionType.ReturnAllCredits : CreditTransactionType.ReturnCredit,
                                           CreditTransactionEnum.Return,
                                           model.PosInfoId,
                                           model.StaffId,
                                           model.CreditAccountId,
                                           model.CreditAccountDescription,
                                           //"Add amount on Barcode Credit Account",
                                           model.Amount,
                                           model.AccountId,
                                           model.CreditCodeId);
            if (deactivate)
                DeActivateAccount(model.CreditAccountId);
            bool externalCommunication = true;
            return repo.InsertInvoice(res, out externalCommunication);// CheckIfCodeExists(creditAccountDescription);

        }


        private PosInfoDetail GetPosInfoDetail(CreditTransactionType ctt, long posInfoId)
        {
            PosInfoDetail posInfoDetail;
            switch (ctt)
            {
                case CreditTransactionType.AddCredit:
                    posInfoDetail = DbContext.PosInfoDetail.FirstOrDefault(f => f.PosInfoId == posInfoId && f.GroupId == (short)InvoiceTypesEnum.PaymentReceipt);
                    if (posInfoDetail == null)
                    {
                        //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("CreditTransaction  action : " + ctt.ToString() + " |  posInfoDetail not found."));
                        logger.Error("CreditTransaction  action : " + ctt.ToString() + " |  posInfoDetail not found. Check if GroupId = " + ((short)InvoiceTypesEnum.PaymentReceipt).ToString()+" ("+ InvoiceTypesEnum.PaymentReceipt.ToString() + ")" + Environment.NewLine);
                        return null;
                    }
                    return posInfoDetail;
                case CreditTransactionType.RemoveCredit:
                    posInfoDetail = DbContext.PosInfoDetail.FirstOrDefault(f => f.PosInfoId == posInfoId && f.GroupId == (short)InvoiceTypesEnum.RefundReceipt);
                    if (posInfoDetail == null)
                    {
                        //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("CreditTransaction : " + ctt.ToString() + " |  posInfoDetail not found."));
                        logger.Error("CreditTransaction : " + ctt.ToString() + " |  posInfoDetail not found. Check if GroupId = " + ((short)InvoiceTypesEnum.RefundReceipt).ToString() + " (" + InvoiceTypesEnum.RefundReceipt.ToString() + ")" + Environment.NewLine);
                        return null;
                    }
                    return posInfoDetail;
                case CreditTransactionType.ReturnAllCredits:
                    posInfoDetail = DbContext.PosInfoDetail.FirstOrDefault(f => f.PosInfoId == posInfoId && f.GroupId == (short)InvoiceTypesEnum.RefundReceipt);
                    if (posInfoDetail == null)
                    {
                        //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("CreditTransaction : " + ctt.ToString() + " |  posInfoDetail not found."));
                        logger.Error("CreditTransaction : " + ctt.ToString() + " |  posInfoDetail not found. Check if GroupId = " + ((short)InvoiceTypesEnum.RefundReceipt).ToString() + " (" + InvoiceTypesEnum.RefundReceipt.ToString() + ")" + Environment.NewLine);
                        return null;
                    }
                    return posInfoDetail;
                case CreditTransactionType.ReturnCredit:
                    posInfoDetail = DbContext.PosInfoDetail.FirstOrDefault(f => f.PosInfoId == posInfoId && f.GroupId == (short)InvoiceTypesEnum.RefundReceipt);
                    if (posInfoDetail == null)
                    {
                        //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("CreditTransaction : " + ctt.ToString() + " |  posInfoDetail not found."));
                        logger.Error("CreditTransaction : " + ctt.ToString() + " |  posInfoDetail not found. Check if GroupId = " + ((short)InvoiceTypesEnum.RefundReceipt).ToString() + " (" + InvoiceTypesEnum.RefundReceipt.ToString() + ")" + Environment.NewLine);
                        return null;
                    }
                    return posInfoDetail;
                default:
                    break;
            }
            return null;
        }
        private Receipts CreateInvoiceForCredit(CreditTransactionType ctt, CreditTransactionEnum creditTransactionType, long posInfoId, long staffId, long creditAccountId, string salesDescription, decimal amount, long accountId, long creditCodeId)
        {
            //GEO many changes here same for locker
            var posInfoDetail = GetPosInfoDetail(ctt, posInfoId);
            if (posInfoDetail == null)
                return null;

            var posInfo = DbContext.PosInfo.Include(i => i.Department).FirstOrDefault(pi => pi.Id == posInfoId);
            if (posInfo == null)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("CreateInvoiceForLocker : " + posInfoId.ToString() + " |  PosInfo not found."));
                logger.Error("CreateInvoiceForLocker : " + posInfoId.ToString() + " |  PosInfo not found.");
                return null;
            }

            var creditAcccount = DbContext.CreditAccounts.FirstOrDefault(ca => ca.Id == creditAccountId);
            if (creditAcccount == null)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("CreateInvoiceForLocker : " + creditAccountId.ToString() + " |  CreditAcccount not found."));
                logger.Error("CreateInvoiceForLocker : " + creditAccountId.ToString() + " |  CreditAcccount not found.");
                return null;
            }

            var pfee = (from q in DbContext.ProductForBarcodeEod
                        join qq in DbContext.Product on q.ProductId equals qq.Id
                        join qqq in DbContext.PricelistDetail on q.ProductId equals qqq.ProductId
                        join qqqq in DbContext.Vat on qqq.VatId equals qqqq.Id
                        select new
                        {
                            ProductId = q.ProductId,
                            ProductCode = qq.Code,
                            PricelistDetailId = qqq.Id,
                            PricelistId = qqq.PricelistId,
                            VatId = qqqq.Id,
                            VatCode = qqqq.Code,
                            VatPercentage = qqqq.Percentage
                        }).FirstOrDefault();
            if (pfee == null)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("CreateInvoiceForLocker : " + pfee.PricelistDetailId.ToString() + " |  PricelistDetail not found."));
                logger.Error("CreateInvoiceForLocker : " + pfee.PricelistDetailId.ToString() + " |  PricelistDetail not found.");
                return null;
            }

            var pricelistDetail = DbContext.PricelistDetail.Include(i => i.Vat).FirstOrDefault(f => f.Id == pfee.PricelistDetailId);
            if (pricelistDetail == null)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("CreateInvoiceForLocker : " + pfee.PricelistDetailId.ToString() + " |  PricelistDetail not found."));
                logger.Error("CreateInvoiceForLocker : " + pfee.PricelistDetailId.ToString() + " |  PricelistDetail not found.");
                return null;
            }

            var receipt = new Receipts();
            receipt.Abbreviation = posInfoDetail.Abbreviation;
            receipt.BuzzerNumber = null;
            receipt.CashAmount = null;
            receipt.Cover = 0;
            receipt.CreateTransactions = posInfoDetail.CreateTransaction ?? false;
            receipt.Day = DateTime.Now;
            receipt.DepartmentDescription = posInfo.Department.Description;
            receipt.DepartmentId = posInfo.DepartmentId;
            receipt.Discount = 0;
            receipt.DiscountRemark = null;
            receipt.InvoiceDescription = posInfoDetail.Description;
            receipt.InvoiceIndex = posInfoDetail.InvoiceId;
            receipt.InvoiceTypeId = posInfoDetail.InvoicesTypeId;
            receipt.InvoiceTypeType = (short)posInfoDetail.GroupId;
            receipt.ModifyOrderDetails = (int)Helpers.ModifyOrderDetailsEnum.FromScratch;
            receipt.OrderNo = posInfo.ReceiptCount != null ? (posInfo.ReceiptCount + 1).ToString() : "0";
            receipt.OrderOrigin = (int)OrderOriginEnum.Local;
            receipt.PosInfoDescription = posInfo.Description;
            receipt.PosInfoDetailId = posInfoDetail.Id;
            receipt.PosInfoId = posInfoId;
            receipt.PrintType = Symposium.Models.Enums.PrintTypeEnum.PrintWhole;
            receipt.IsPrinted = true;
            receipt.ReceiptNo = null;
            receipt.StaffId = staffId;
            //receipt.StaffCode
            //receipt.StaffName
            receipt.TableId = null;
            receipt.TableCode = null;
            receipt.TableSum = null;
            receipt.Total = amount;
            receipt.Net = DeVat(pricelistDetail.Vat.Percentage.Value, amount);
            receipt.Vat = receipt.Total - receipt.Net;

            var rd1 = new ReceiptDetails()
            {
                Guid = Guid.NewGuid(),
                TableCode = null,
                TableId = null,
                InvoiceType = (short)posInfoDetail.GroupId,
                IsExtra = 0,
                ProductId = pfee.ProductId,
                //ProductCategoryId = pfee.ProductCategoryId,
                //CategoryId = pfee.CategoryId,
                ItemCode = pfee.ProductCode,
                ItemDescr = salesDescription,
                ItemQty = 1,
                ItemGross = amount,
                ItemNet = DeVat(pfee.VatPercentage.Value, amount),
                ItemVatRate = pfee.VatPercentage,
                ItemVatValue = amount - DeVat(pfee.VatPercentage.Value, amount),
                ItemDiscount = 0,
                ItemRemark = null,
                ItemSort = 1,
                VatCode = pfee.VatCode,
                VatId = pfee.VatId,
                KdsId = null,
                PreparationTime = null,
                KitchenId = null,
                OrderNo = Convert.ToInt64(receipt.OrderNo),
                PaidStatus = 2,
                PosInfoDetailId = posInfoDetail.Id,
                PosInfoId = posInfoId,
                Price = amount,
                PriceListDetailId = pfee.PricelistDetailId,
                PricelistId = pfee.PricelistId,
                TotalBeforeDiscount = 1 * amount,
                ReceiptSplitedDiscount = 0,
                RegionId = null,
                StaffId = staffId,
                Status = 0,
                OrderDetailIgredientsId = null,
                Abbreviation = posInfoDetail.Abbreviation
                //ItemRegion, RegionPosition, TaxId, ItemTaxAmount, SalesTypeId
            };
            //rd1.ItemVatValue = rd1.ItemGross - rd1.ItemNet;

            receipt.ReceiptDetails.Add(rd1);

            var account = DbContext.Accounts.FirstOrDefault(f => f.Id == accountId);


            var cashPayment = new ReceiptPayments()
            {
                AccountId = account.Id,
                AccountType = account.Type,
                AccountDescription = account.Description,
                SendsTransfer = account.SendsTransfer,
                Amount = amount,
                InvoiceType = (short)posInfoDetail.GroupId,
                PosInfoId = posInfoId,
                TransactionType = 7,
                CreditAccountId = creditAccountId,
                CreditAccountDescription = creditAcccount.Description,
                CreditCodeId = creditCodeId,
                CreditTransactionAction = (short)ctt
            };
            receipt.ReceiptPayments.Add(cashPayment);

            posInfo.ReceiptCount = posInfo.ReceiptCount + 1;
            DbContext.Entry(posInfo).State = EntityState.Modified;

            return receipt;
        }

        private static decimal DeVat(decimal perc, decimal? tempnetbyvat)
        {
            return (decimal)(tempnetbyvat / (decimal)(1 + (decimal)(perc / 100)));
        }

    }
}