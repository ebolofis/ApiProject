using Pos_WebApi.Helpers;
using Pos_WebApi.Models;
using System;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using Pos_WebApi.Models.DTOModels;
using log4net;
using Symposium.Models.Models;
using Symposium.Models.Enums;

namespace Pos_WebApi.Repositories
{
    public class LockerRepository
    {
        protected PosEntities DBContext;
        protected InvoiceRepository repo;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public LockerRepository(PosEntities db)
        {
            DBContext = db;
            repo = new InvoiceRepository(DBContext);
        }

        //public IEnumerable<Lockers> GetLockers()
        //{

        //}
        public async Task<Lockers> GetLockers(long posInfoId)
        {
            return await DBContext.Lockers.FirstOrDefaultAsync(f => f.PosInfoId == posInfoId && f.EndOfDayId == null);
        }

        public async Task<dynamic> AddLocker(long posInfoId, LockerDBO lockersToAdd)
        {
            var lockerProduct = DBContext.RegionLockerProduct.Include(i => i.Product).FirstOrDefault(f => f.ProductId == lockersToAdd.RegionLockerProductId && f.PosInfoId == lockersToAdd.PosInfoId/*&& f.RegionId == lockersToAdd.RegionId*/);
            if (lockerProduct == null)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("CreateInvoiceForLocker : " + lockersToAdd.RegionLockerProductId + " |  RegionLockerProduct not found."));
                logger.Error("CreateInvoiceForLocker : " + lockersToAdd.RegionLockerProductId + " |  RegionLockerProduct not found.");
                return null;

            }
            bool externalCommunication = true;
            var result = repo.InsertInvoice(CreateInvoiceForLocker(LockerActionEnum.Open, CreditTransactionType.PayLocker, lockersToAdd, lockerProduct, (lockersToAdd.UnitPrice * lockersToAdd.Quantity), 0), out externalCommunication);
            var currentLocker = DBContext.Lockers.FirstOrDefault(f => f.PosInfoId == posInfoId && f.EndOfDayId == null);
            if (currentLocker == null)
            {
                currentLocker = new Lockers();
                currentLocker.Id = 0;
                currentLocker.HasLockers = true;
                currentLocker.PosInfoId = posInfoId;
            }

            var amount = lockersToAdd.Quantity * (double)lockersToAdd.UnitPrice;
            
            currentLocker.TotalLockers += lockersToAdd.Quantity;
            currentLocker.TotalLockersAmount += amount;
            currentLocker.OccLockers += lockersToAdd.Quantity;
            currentLocker.OccLockersAmount += (decimal)amount;
            if (lockersToAdd.CreditAccountId == 0)
            {
                currentLocker.TotalCash += lockersToAdd.Quantity;
            }
            else
            {
                currentLocker.TotalSplashCash += lockersToAdd.Quantity;
            }
            if (currentLocker.Id == 0)
            {              
                DBContext.Lockers.Add(currentLocker);
            }
            else
            {
                DBContext.Entry(currentLocker).State = EntityState.Modified;
            }

            await DBContext.SaveChangesAsync();
            return result;
        }

        private PosInfoDetail GetPosInfoDetail(LockerActionEnum lockerAction, RegionLockerProduct lockerProduct)
        {
            PosInfoDetail posInfoDetail;
            switch (lockerAction)
            {
                case LockerActionEnum.Open:
                    posInfoDetail = DBContext.PosInfoDetail.FirstOrDefault(f => f.Id == lockerProduct.PaymentId);
                    if (posInfoDetail == null)
                    {
                        //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("CreateInvoiceForLocker : " + lockerProduct.PaymentId.ToString() + " |  posInfoDetail not found."));
                        logger.Error("CreateInvoiceForLocker : " + lockerProduct.PaymentId.ToString() + " |  posInfoDetail not found.");
                        return null;
                    }
                    return posInfoDetail;
                case LockerActionEnum.CloseNoDiscount:
                    posInfoDetail = DBContext.PosInfoDetail.FirstOrDefault(f => f.Id == lockerProduct.SaleId);
                    if (posInfoDetail == null)
                    {
                        //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("CreateInvoiceForLocker : " + lockerProduct.SaleId.ToString() + " |  posInfoDetail not found."));
                        logger.Error("CreateInvoiceForLocker : " + lockerProduct.SaleId.ToString() + " |  posInfoDetail not found.");
                        return null;
                    }
                    return posInfoDetail;
                case LockerActionEnum.CloseWithDiscount:
                    posInfoDetail = DBContext.PosInfoDetail.FirstOrDefault(f => f.Id == lockerProduct.SaleId);
                    if (posInfoDetail == null)
                    {
                        //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("CreateInvoiceForLocker : " + lockerProduct.ReturnPaymentpId.ToString() + " |  posInfoDetail not found."));
                        logger.Error("CreateInvoiceForLocker : " + lockerProduct.ReturnPaymentpId.ToString() + " |  posInfoDetail not found.");
                        return null;
                    }
                    return posInfoDetail;
                case LockerActionEnum.ReturnAmount:
                    posInfoDetail = DBContext.PosInfoDetail.FirstOrDefault(f => f.Id == lockerProduct.ReturnPaymentpId);
                    if (posInfoDetail == null)
                    {
                        //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("CreateInvoiceForLocker : " + lockerProduct.SaleId.ToString() + " |  posInfoDetail not found."));
                        logger.Error("CreateInvoiceForLocker : " + lockerProduct.SaleId.ToString() + " |  posInfoDetail not found.");
                        return null;
                    }
                    return posInfoDetail;
                default:
                    break;
            }
            return null;
        }
        private Receipts CreateInvoiceForLocker(LockerActionEnum lockerAction, CreditTransactionType ctt, LockerDBO locker, RegionLockerProduct lockerProduct, decimal amountToCharge, decimal amountDiscount)
        {

            var posInfoDetail = GetPosInfoDetail(lockerAction, lockerProduct);
            if (posInfoDetail == null)
                return null;

            var posInfo = DBContext.PosInfo.Include(i => i.Department).FirstOrDefault(pi => pi.Id == lockerProduct.PosInfoId);
            if (posInfo == null)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("CreateInvoiceForLocker : " + lockerProduct.PosInfoId.ToString() + " |  PosInfo not found."));
                logger.Error("CreateInvoiceForLocker : " + lockerProduct.PosInfoId.ToString() + " |  PosInfo not found.");
                return null;
            }


            var pricelistDetail = DBContext.PricelistDetail.Include(i => i.Vat).FirstOrDefault(f => f.ProductId == lockerProduct.ProductId && f.PricelistId == lockerProduct.PriceListId);
            if (pricelistDetail == null)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("CreateInvoiceForLocker : " + lockerProduct.PriceListId.ToString() + " |  PricelistDetail not found."));
                logger.Error("CreateInvoiceForLocker : " + lockerProduct.PriceListId.ToString() + " |  PricelistDetail not found.");
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
            receipt.DiscountRemark = "";// amountDiscount != 0 ? "Return Locker" : null;
            receipt.InvoiceDescription = posInfoDetail.Description;
            receipt.InvoiceIndex = posInfoDetail.InvoiceId;
            receipt.InvoiceTypeId = posInfoDetail.InvoicesTypeId;
            receipt.InvoiceTypeType = (short)posInfoDetail.GroupId;
            receipt.ModifyOrderDetails = (int)Helpers.ModifyOrderDetailsEnum.FromScratch;
            receipt.OrderNo = posInfo.ReceiptCount != null ? (posInfo.ReceiptCount + 1).ToString() : "0";
            receipt.OrderOrigin = (int)OrderOriginEnum.Local;
            receipt.PosInfoDescription = posInfo.Description;
            receipt.PosInfoDetailId = posInfoDetail.Id;
            receipt.PosInfoId = locker.PosInfoId;
            receipt.PrintType = PrintTypeEnum.PrintWhole;
            receipt.IsPrinted = true;
            receipt.ReceiptNo = null;
            receipt.StaffId = locker.StaffId;
            //receipt.StaffCode
            //receipt.StaffName
            receipt.TableId = null;
            receipt.TableCode = null;
            receipt.TableSum = null;
            receipt.Total = amountToCharge;
            receipt.Net = DeVat(pricelistDetail.Vat.Percentage.Value, amountToCharge);
            receipt.Vat = receipt.Total - receipt.Net;

            var rd1 = new ReceiptDetails()
            {
                Guid = Guid.NewGuid(),
                TableCode = null,
                TableId = null,
                InvoiceType = (short)posInfoDetail.GroupId,
                IsExtra = 0,
                ProductId = lockerProduct.ProductId,
                //ProductCategoryId = lockerProduct.Product.ProductCategoryId,
                //CategoryId = lockerProduct.Product.ProductCategories.CategoryId,
                ItemCode = lockerProduct.Product.Code,
                ItemDescr = lockerProduct.SalesDescription,
                ItemQty = locker.Quantity,
                ItemGross = lockerProduct.Price,
                ItemNet = DeVat(pricelistDetail.Vat.Percentage.Value, amountToCharge),
                ItemVatRate = pricelistDetail.Vat.Percentage,
                ItemVatValue = amountToCharge - DeVat(pricelistDetail.Vat.Percentage.Value, amountToCharge),
                ItemDiscount = 0,
                ItemRemark = null,
                ItemSort = 1,
                VatCode = pricelistDetail.Vat.Code,
                VatId = pricelistDetail.VatId,
                KdsId = null,
                PreparationTime = null,
                KitchenId = null,
                OrderNo = Convert.ToInt64(receipt.OrderNo),
                PaidStatus = 2,
                PosInfoDetailId = posInfoDetail.Id,
                PosInfoId = locker.PosInfoId,
                Price = locker.UnitPrice,
                PriceListDetailId = pricelistDetail.Id,
                PricelistId = lockerProduct.PriceListId,
                TotalBeforeDiscount = locker.UnitPrice * locker.Quantity,
                ReceiptSplitedDiscount = 0,
                RegionId = null,//lockerProduct.RegionId,
                StaffId = locker.StaffId,
                Status = 0,
                OrderDetailIgredientsId = null,
                Abbreviation = posInfoDetail.Abbreviation
                //ItemRegion, RegionPosition, TaxId, ItemTaxAmount, SalesTypeId
            };
            //rd1.ItemVatValue = rd1.ItemGross - rd1.ItemNet;

            receipt.ReceiptDetails.Add(rd1);

            //the returned item
            if (lockerAction == LockerActionEnum.CloseWithDiscount)
            {
                var rdret = new ReceiptDetails()
                {

                    Guid = Guid.NewGuid(),
                    TableCode = null,
                    TableId = null,
                    InvoiceType = (short)posInfoDetail.GroupId,
                    IsExtra = 0,
                    ProductId = lockerProduct.ProductId,
                    //ProductCategoryId = lockerProduct.Product.ProductCategoryId,
                    //CategoryId = lockerProduct.Product.ProductCategories.CategoryId,
                    ItemCode = lockerProduct.Product.Code,
                    ItemDescr = lockerProduct.SalesDescription,
                    ItemQty = locker.Quantity,
                    ItemGross = -lockerProduct.Discount,
                    ItemNet = -DeVat(pricelistDetail.Vat.Percentage.Value, amountToCharge),
                    ItemVatRate = pricelistDetail.Vat.Percentage,
                    ItemVatValue = -amountToCharge - DeVat(pricelistDetail.Vat.Percentage.Value, amountToCharge),
                    ItemDiscount = 0,
                    ItemRemark = null,
                    ItemSort = 2,
                    VatCode = pricelistDetail.Vat.Code,
                    VatId = pricelistDetail.VatId,
                    KdsId = null,
                    PreparationTime = null,
                    KitchenId = null,
                    OrderNo = Convert.ToInt64(receipt.OrderNo),
                    PaidStatus = 2,
                    PosInfoDetailId = posInfoDetail.Id,
                    PosInfoId = locker.PosInfoId,
                    Price = lockerProduct.Discount,
                    PriceListDetailId = pricelistDetail.Id,
                    PricelistId = lockerProduct.PriceListId,
                    TotalBeforeDiscount = (amountToCharge - lockerProduct.Price) * locker.Quantity,
                    ReceiptSplitedDiscount = 0,
                    RegionId = null,//lockerProduct.RegionId,
                    StaffId = locker.StaffId,
                    Status = 0,
                    OrderDetailIgredientsId = null,
                    Abbreviation = posInfoDetail.Abbreviation
                    //ItemRegion, RegionPosition, TaxId, ItemTaxAmount, SalesTypeId
                };
                //rd1.ItemVatValue = rd1.ItemGross - rd1.ItemNet;

                receipt.ReceiptDetails.Add(rdret);
            }
            var account = DBContext.Accounts.FirstOrDefault(f => f.Id == locker.AccountId);


            var cashPayment = new ReceiptPayments()
            {
                AccountId = account.Id,
                AccountType = account.Type,
                AccountDescription = account.Description,
                SendsTransfer = account.SendsTransfer,
                Amount = amountToCharge,// - amountDiscount,
                InvoiceType = (short)posInfoDetail.GroupId,
                PosInfoId = locker.PosInfoId,
                TransactionType = lockerAction == LockerActionEnum.Open ? (short)TransactionTypesEnum.OpenLocker : (short)TransactionTypesEnum.CloseLocker,
                CreditAccountId = locker.CreditAccountId,
                CreditAccountDescription = locker.CreditAccountDescription,
                CreditCodeId = locker.CreditCodeId,
                CreditTransactionAction = (short)ctt

            };
            receipt.ReceiptPayments.Add(cashPayment);

            posInfo.ReceiptCount = posInfo.ReceiptCount + 1;
            DBContext.Entry(posInfo).State = EntityState.Modified;

            return receipt;
        }

        private static decimal DeVat(decimal perc, decimal? tempnetbyvat)
        {
            return (decimal)(tempnetbyvat / (decimal)(1 + (decimal)(perc / 100)));
        }

        public async Task<IEnumerable<dynamic>> RemoveLocker(long posInfoId, LockerDBO lockersToRemove, bool withDiscount)
        {
            List<dynamic> res = new List<dynamic>();
            var lockerProduct = DBContext.RegionLockerProduct.Include(i => i.Product).FirstOrDefault(f => f.ProductId == lockersToRemove.RegionLockerProductId && f.PosInfoId == lockersToRemove.PosInfoId/*&& f.RegionId == lockersToRemove.RegionId*/);
            if (lockerProduct == null)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("CreateInvoiceForLocker : " + lockersToRemove.RegionLockerProductId + " |  RegionLockerProduct not found."));
                logger.Error("CreateInvoiceForLocker : " + lockersToRemove.RegionLockerProductId + " |  RegionLockerProduct not found.");
                return null;
            }
            if (withDiscount)
            {
                var amountToClose = ((lockersToRemove.UnitPrice - (lockerProduct.Discount??0)) * lockersToRemove.Quantity);
                var discountAmount = (decimal)(lockerProduct.Discount * lockersToRemove.Quantity);
                bool externalCommunication = true;
                var result = repo.InsertInvoice(CreateInvoiceForLocker(LockerActionEnum.CloseWithDiscount, CreditTransactionType.ReturnLocker,
                                                                       lockersToRemove,
                                                                       lockerProduct,
                                                                       amountToClose,
                                                                       discountAmount), out externalCommunication);
                res.Add(result);
                externalCommunication = true;
                repo.InsertInvoice(CreateInvoiceForLocker(LockerActionEnum.ReturnAmount, CreditTransactionType.ReturnLocker,
                                                                        lockersToRemove,
                                                                        lockerProduct,
                                                                        discountAmount + amountToClose,
                                                                        0), out externalCommunication);
            }
            else
            {
                bool externalCommunication = true;
                var result2 = repo.InsertInvoice(CreateInvoiceForLocker(LockerActionEnum.CloseNoDiscount, CreditTransactionType.ReturnLocker,
                                                                        lockersToRemove,
                                                                        lockerProduct,
                                                                        (lockersToRemove.UnitPrice * lockersToRemove.Quantity),
                                                                        0), out externalCommunication);
                res.Add(result2);
            }

            var currentLocker = DBContext.Lockers.FirstOrDefault(f => f.PosInfoId == posInfoId && f.EndOfDayId == null);

            if (currentLocker == null)
            {
                currentLocker = new Lockers();
                currentLocker.Id = 0;
                currentLocker.HasLockers = true;
                currentLocker.PosInfoId = posInfoId;
            }

            var amount = lockersToRemove.Quantity * (double)lockersToRemove.UnitPrice;
            currentLocker.OccLockers -= lockersToRemove.Quantity;
            currentLocker.OccLockersAmount -= (decimal)amount;
            currentLocker.Paidlockers += lockersToRemove.Quantity;
            currentLocker.PaidlockersAmount += (decimal)amount;
            if (withDiscount)
            {
                if (lockersToRemove.CreditAccountId == 0)
                {
                    currentLocker.ReturnCash += lockersToRemove.Quantity;
                }
                else
                {
                    currentLocker.ReturnSplashCash += lockersToRemove.Quantity;
                }
            }
            else
            {
                if (lockersToRemove.CreditAccountId == 0)
                {
                    currentLocker.CloseCash += lockersToRemove.Quantity;
                }
                else
                {
                    currentLocker.CloseSplashCash += lockersToRemove.Quantity;
                }
            }
            if (currentLocker.Id == 0)
            {
                DBContext.Lockers.Add(currentLocker);
            }
            else
            {
                DBContext.Entry(currentLocker).State = EntityState.Modified;
            }
            await DBContext.SaveChangesAsync();
            return res;
        }

        public int SaveChanges()
        {
            return DBContext.SaveChanges();
        }
    }





}
