using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pos_WebApi.Helpers
{
    public class Economic
    {
        public Transactions SetEconomicNumbers(Transactions transactions, Order order, PosEntities db, List<long> orderDetails)
        {
            if (transactions.TransactionType == (int)TransactionTypesEnum.Sale || transactions.TransactionType == (int)TransactionTypesEnum.Cancel)
            {
                transactions.Gross = transactions.Amount;

                decimal totalvat = 0;
                decimal totaltax = 0;
                decimal totalgross = 0;
                decimal totalnet = 0;
                if (order == null)
                {
                    order = db.Order.Include("OrderDetail").Include("OrderDetail.OrderDetailIgredients").Where(w => w.Id == transactions.OrderId).FirstOrDefault();

                }
                if (order != null)
                {
                    orderDetails = order.OrderDetail.Select(s => s.Id).ToList();
                    foreach (var d in order.OrderDetail.Where(w => orderDetails.Contains(w.Id)))
                    {
                        PricelistDetail prdet = db.PricelistDetail.Find(d.PriceListDetailId);
                        Vat vat = db.Vat.Find(prdet.VatId);
                        Tax tax = db.Tax.Find(prdet.TaxId);
                        decimal tempprice = d.Price != null ? d.Price.Value : 0;//prdet.Price != null ? prdet.Price.Value : 0;
                        tempprice = d.Qty != null && d.Qty > 0 ? (decimal)(d.Qty.Value) * tempprice : tempprice;
                        tempprice = d.Discount != null ? tempprice - (decimal)d.Discount.Value : tempprice;
                        decimal tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, tempprice) : tempprice;
                        decimal tempgross = tempprice;
                        decimal tempvat = (decimal)(tempprice - tempnetbyvat);

                        decimal tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
                        decimal temptax = (decimal)(tempnetbyvat - tempnetbytax);

                        decimal tempnet = (decimal)(tempprice - tempvat - temptax);
                        totalvat += tempvat;
                        totaltax += temptax;
                        totalgross += tempgross;
                        totalnet += tempnet;
                        foreach (var odi in d.OrderDetailIgredients)
                        {
                            //PricelistDetail prdet2 = db.PricelistDetail.Find(odi.PriceListDetailId);
                            Vat vat2 = db.PricelistDetail.Include("Vat").Where(w => w.Id == odi.PriceListDetailId).FirstOrDefault().Vat;
                            Tax tax2 = db.PricelistDetail.Include("Tax").Where(w => w.Id == odi.PriceListDetailId).FirstOrDefault().Tax;
                            decimal tempprice2 = odi.Price != null ? odi.Price.Value : 0;
                            tempprice2 = odi.Discount != null ? tempprice2 - (decimal)odi.Discount.Value : tempprice2;
                            //tempprice2 = odi.Qty != null ? (decimal)(odi.Qty.Value) * tempprice2 : tempprice2; Pairnw etoimh thn timh apo to pos
                            decimal tempnetbyvat2 = vat2 != null && vat2.Percentage != null ? DeVat(vat2.Percentage.Value, tempprice2) : tempprice2;
                            decimal tempgross2 = tempprice2;
                            decimal tempvat2 = (decimal)(tempprice2 - tempnetbyvat2);

                            decimal tempnetbytax2 = tax2 != null && tax2.Percentage != null ? DeVat(tax2.Percentage.Value, tempnetbyvat2) : tempnetbyvat2;
                            decimal temptax2 = (decimal)(tempnetbyvat2 - tempnetbytax2);
                            decimal tempnet2 = (decimal)(tempprice2 - tempvat2 - temptax2);
                            totalvat += tempvat2;
                            totaltax += temptax2;
                            totalgross += tempgross2;
                            totalnet += tempnet2;
                        }
                    }
                }
                transactions.Vat = transactions.TransactionType == (int)TransactionTypesEnum.Sale ? totalvat : totalvat * -1;
                transactions.Tax = transactions.TransactionType == (int)TransactionTypesEnum.Sale ? totaltax : totaltax * -1;
                transactions.Net = transactions.TransactionType == (int)TransactionTypesEnum.Sale ? totalnet : totalnet * -1;
                transactions.Gross = transactions.TransactionType == (int)TransactionTypesEnum.Sale ? totalgross : totalgross * -1;
            }
            return transactions;
        }

        public Transactions SetEconomicNumbersOrderDetails(Transactions transactions, IEnumerable<OrderDetail> orderdetails, PosEntities db)
        {
            if (transactions.TransactionType == (int)TransactionTypesEnum.Sale || transactions.TransactionType == (int)TransactionTypesEnum.Cancel)
            {
                transactions.Gross = transactions.Amount;

                decimal totalvat = 0;
                decimal totaltax = 0;
                decimal totalgross = 0;
                decimal totalnet = 0;
                if (orderdetails != null)
                {
                    foreach (var d in orderdetails)
                    {
                        PricelistDetail prdet = db.PricelistDetail.Find(d.PriceListDetailId);
                        Vat vat = db.Vat.Find(prdet.VatId);
                        Tax tax = db.Tax.Find(prdet.TaxId);
                        decimal tempprice = d.Price != null ? d.Price.Value : 0;//prdet.Price != null ? prdet.Price.Value : 0;
                        tempprice = d.Qty != null && d.Qty > 0 ? (decimal)(d.Qty.Value) * tempprice : tempprice;
                        tempprice = d.Discount != null ? tempprice - (decimal)d.Discount.Value : tempprice;
                        tempprice = d.TotalAfterDiscount != null ? d.TotalAfterDiscount.Value : tempprice;
                        decimal tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, tempprice) : tempprice;
                        decimal tempgross = tempprice;
                        decimal tempvat = (decimal)(tempprice - tempnetbyvat);

                        decimal tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
                        decimal temptax = (decimal)(tempnetbyvat - tempnetbytax);

                        decimal tempnet = (decimal)(tempprice - tempvat - temptax);
                        totalvat += tempvat;
                        totaltax += temptax;
                        totalgross += tempgross;
                        totalnet += tempnet;
                        foreach (var odi in d.OrderDetailIgredients)
                        {
                            //PricelistDetail prdet2 = db.PricelistDetail.Find(odi.PriceListDetailId);
                            Vat vat2 = db.PricelistDetail.Include("Vat").Where(w => w.Id == odi.PriceListDetailId).FirstOrDefault().Vat;
                            Tax tax2 = db.PricelistDetail.Include("Tax").Where(w => w.Id == odi.PriceListDetailId).FirstOrDefault().Tax;
                            decimal tempprice2 = odi.Price != null ? odi.Price.Value : 0;
                            tempprice2 = odi.Discount != null ? tempprice2 - (decimal)odi.Discount.Value : tempprice2;
                            //tempprice2 = odi.Qty != null ? (decimal)(odi.Qty.Value) * tempprice2 : tempprice2; Pairnw etoimh thn timh apo to pos
                            decimal tempnetbyvat2 = vat2 != null && vat2.Percentage != null ? DeVat(vat2.Percentage.Value, tempprice2) : tempprice2;
                            decimal tempgross2 = tempprice2;
                            decimal tempvat2 = (decimal)(tempprice2 - tempnetbyvat2);

                            decimal tempnetbytax2 = tax2 != null && tax2.Percentage != null ? DeVat(tax2.Percentage.Value, tempnetbyvat2) : tempnetbyvat2;
                            decimal temptax2 = (decimal)(tempnetbyvat2 - tempnetbytax2);
                            decimal tempnet2 = (decimal)(tempprice2 - tempvat2 - temptax2);
                            totalvat += tempvat2;
                            totaltax += temptax2;
                            totalgross += tempgross2;
                            totalnet += tempnet2;
                        }
                    }
                }
                transactions.Vat = transactions.TransactionType == (int)TransactionTypesEnum.Sale ? totalvat : totalvat * -1;
                transactions.Tax = transactions.TransactionType == (int)TransactionTypesEnum.Sale ? totaltax : totaltax * -1;
                transactions.Net = transactions.TransactionType == (int)TransactionTypesEnum.Sale ? totalnet : totalnet * -1;
                transactions.Gross = transactions.TransactionType == (int)TransactionTypesEnum.Sale ? totalgross : totalgross * -1;
            }
            return transactions;
        }

        private static decimal DeVat(decimal perc, decimal tempnetbyvat)
        {
            return (decimal)(tempnetbyvat / (decimal)(1 + (decimal)(perc / 100)));
        }

        public ICollection<OrderDetail> EpimerismosEkptwshs(decimal? totalDiscount, decimal? totalBeforeDiscount, ICollection<OrderDetail> orderDetails)
        {
            if (totalDiscount != null && totalBeforeDiscount > 0 /* && orderDetails.Sum(s=> s.TotalAfterDiscount) > 0*/ )
            {
                int count = 1;
                decimal? percentage = totalDiscount / totalBeforeDiscount ;
                decimal? remainingDiscount = totalDiscount;
                foreach (var od in orderDetails)
                {
                    foreach (var odi in od.OrderDetailIgredients)
                    {
                        var extraTOT = odi.TotalAfterDiscount * percentage;
                        odi.TotalAfterDiscount = odi.TotalAfterDiscount - extraTOT;
                        odi.Discount = odi.Discount != null ? odi.Discount + extraTOT : extraTOT;
                        remainingDiscount = remainingDiscount - extraTOT;
                    }
                    if (count < orderDetails.Count)
                    {
                        var tot = od.TotalAfterDiscount * percentage;
                        od.TotalAfterDiscount = od.TotalAfterDiscount - tot;
                        od.Discount = od.Discount != null ? od.Discount + tot : tot;
                        remainingDiscount = remainingDiscount - tot;
                    }
                    else
                    {
                        od.TotalAfterDiscount = od.TotalAfterDiscount - remainingDiscount;
                        od.Discount = od.Discount != null ? od.Discount + remainingDiscount : remainingDiscount; ;
                    }
                    //Epimerismos kai tou pinaka TablePaySuggestion
                    if (od.TablePaySuggestion != null && od.TablePaySuggestion.Count > 0 && od.TotalAfterDiscount != 0)
                    {
                        int TPScount = 1;
                        decimal ExtrasTotal = od.OrderDetailIgredients.Sum(sm => sm.TotalAfterDiscount) != null ? od.OrderDetailIgredients.Sum(sm => sm.TotalAfterDiscount).Value : 0;
                        decimal? CurAmount = od.TotalAfterDiscount + ExtrasTotal;
                        decimal? TotalAmounts = od.TablePaySuggestion.Sum(sm => sm.Amount);
                        decimal? TPStotalDiscount = TotalAmounts - CurAmount;
                        decimal? TPSpercentage = 1 - (CurAmount / TotalAmounts == 0 ? 1 : TotalAmounts);
                        decimal? totalEpim = 0;
                        decimal? TPSremainingDiscount = TPStotalDiscount;
                        foreach (var tps in od.TablePaySuggestion)
                        {
                            if (TPScount < od.TablePaySuggestion.Count)
                            {
                                decimal? subtotal = tps.Amount;
                                decimal? percSub = Math.Round((decimal)(subtotal * TPSpercentage), 2);
                                totalEpim += subtotal - percSub;
                                tps.Amount = subtotal - percSub;
                                TPSremainingDiscount = TPSremainingDiscount - percSub;
                            }
                            else
                            {
                                decimal? subtotal = tps.Amount;
                                tps.Amount = subtotal - TPSremainingDiscount;
                                totalEpim += subtotal - TPSremainingDiscount;
                            }
                            TPScount++;
                        }
                    }
                    count++;
                }
            }
            return orderDetails;
        }

        public decimal EpimerisiAccountTotal(IEnumerable<OrderDetail> orderDetail, decimal GroupTotal)
        {
            orderDetail = orderDetail.OrderBy(f => f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount);
            decimal StartTotal = orderDetail.Sum(f => f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount);
            decimal totalDiscount = StartTotal - GroupTotal;
            decimal percentage = (decimal)(GroupTotal / StartTotal == 0 ? 1 : StartTotal);
            decimal total = 0;
            decimal remainingDiscount = totalDiscount;
            decimal counter = 1;
            foreach (var f in orderDetail)
            {
                if (counter < orderDetail.Count())
                {
                    decimal subtotal = f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount;
                    decimal percSub = Math.Round((decimal)(subtotal * percentage), 2);
                    total += subtotal - percSub;
                    remainingDiscount = remainingDiscount - percSub;
                }
                else
                {
                    decimal subtotal = f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount;
                    total += subtotal - remainingDiscount;
                }
                counter++;
            }
            return total;
        }



        public static  List<AccountsObj> AmountSharing(Decimal TotalAmount, IEnumerable<AccountsObj> accObjs)
        {
            var result = new List<AccountsObj>();

            decimal sums = TotalAmount;
            decimal accobjsums = accObjs.Sum(sm => sm.Amount);
            decimal percentageEpim = accobjsums == 0 ? 0 : 1 - (decimal)(sums / accobjsums);

            decimal remainingDiscount = accobjsums - sums;
            // decimal ctr = 1;
            decimal subtotal = sums;
            foreach (var epimItem in accObjs)
            {
                epimItem.Amount -= Math.Round((decimal)(epimItem.Amount * percentageEpim), 2);
                remainingDiscount = remainingDiscount - epimItem.Amount;
                //("Account Id = " + epimItem.AccountId + " Amount: " + epimItem.Amount).Dump();
                result.Add(new AccountsObj
                    {
                        AccountId = epimItem.AccountId,
                        Amount = Math.Round(epimItem.Amount, 2),
                        GuestId = epimItem.GuestId
                    });
            }
            decimal diff = TotalAmount - result.Sum(sm => sm.Amount);
            result.FirstOrDefault().Amount += diff;
            return result;
        }


        public static List<decimal> AmountSharing(Decimal total,  int divider)
        {
            var res = new List<decimal>();
            while (divider > 0)
            {
                decimal amount = Math.Round(total / divider, 2);
                res.Add(amount);
                
                total -= amount;
                divider--;
            }
            return res;
        }

    }

}