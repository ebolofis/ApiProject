using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{
    public class OrderDetailInvoicesTasks : IOrderDetailInvoicesTasks
    {
        IOrderDetailInvoicesDT dt;
        IProductTasks prodTask;
        IPricelistTasks priceListTask;
        IPricelistDetailTasks prLstDetTask;
        IIngredientsTasks ingTask;
        ICategoriesTasks categTask;
        IProductCategoriesTasks prodCategTask;
        IPosInfoDetailTasks posInfoDetTask;
        ITableDT tableDt;
        IVatTasks vatTask;
        IOrderDetailInvoicesDT orderDetailInvoicesDT;

        public OrderDetailInvoicesTasks(IOrderDetailInvoicesDT dt, IProductTasks prodTask,
            IPricelistTasks priceListTask, IPricelistDetailTasks prLstDetTask,
            IIngredientsTasks ingTask, ICategoriesTasks categTask,
            IProductCategoriesTasks prodCategTask, IPosInfoDetailTasks posInfoDetTask,
            ITableDT tableDt, IVatTasks vatTask, IOrderDetailInvoicesDT orderDetailInvoicesDT)
        {
            this.dt = dt;
            this.prodTask = prodTask;
            this.priceListTask = priceListTask;
            this.prLstDetTask = prLstDetTask;
            this.ingTask = ingTask;
            this.categTask = categTask;
            this.prodCategTask = prodCategTask;
            this.posInfoDetTask = posInfoDetTask;
            this.tableDt = tableDt;
            this.vatTask = vatTask;
            this.orderDetailInvoicesDT = orderDetailInvoicesDT;
        }

        /// <summary>
        /// Return's a List Of Order Detail Invoices and 
        /// a list Of Order Detail included exrtas
        /// </summary>
        /// <param name="receiptDetails"></param>
        /// <param name="invoice"></param>
        /// <param name="orderType"></param>
        /// <param name="Abbrev"></param>
        /// <returns></returns>
        public List<OrderDetailWithExtrasModel> CreateListOfOrderDetailInvoiceFromReceipt(ICollection<ReceiptDetails> receiptDetails,
            InvoiceModel invoice, ModifyOrderDetailsEnum orderType, string Abbrev, bool isPrinted, int? Counter)
        {
            List<OrderDetailWithExtrasModel> ret = new List<OrderDetailWithExtrasModel>();

            OrderDetailWithExtrasModel lastProduce = new OrderDetailWithExtrasModel();

            foreach (ReceiptDetails item in receiptDetails)
            {
                switch (orderType)
                {
                    case ModifyOrderDetailsEnum.FromScratch:
                        if (item.IsExtra == 0)
                        {
                            //Create's new OrderDetail Model
                            OrderDetailWithExtrasModel ordDet = AutoMapper.Mapper.Map<OrderDetailWithExtrasModel>(item);
                            ordDet.Discount = item.ItemDiscount;
                            ordDet.Qty = item.ItemQty;
                            ordDet.TotalAfterDiscount = item.ItemGross;
                            ordDet.StatusTS = DateTime.Now;

                            //List Of OrderDetailIngredients
                            ordDet.OrderDetIngrendients = new List<OrderDetailIngredientsModel>();
                            //List Of OrderDetailInvoices
                            ordDet.OrderDetailInvoices = new List<OrderDetailInvoicesModel>();

                            //New OrderDetailInvoices. for each item in ReceiptDetails
                            OrderDetailInvoicesModel tmpObj = AutoMapper.Mapper.Map<OrderDetailInvoicesModel>(item);
                            tmpObj.CreationTS = DateTime.Now;
                            tmpObj.Counter = invoice == null ? (long?)null : invoice.Counter;
                            tmpObj.IsPrinted = isPrinted;
                            tmpObj.Counter = Counter;
                            tmpObj.Qty = item.ItemQty;
                            tmpObj.VatAmount = item.ItemVatValue;
                            tmpObj.VatRate = item.ItemVatRate;
                            tmpObj.TaxAmount = item.ItemTaxAmount;
                            tmpObj.Total = item.ItemGross;
                            tmpObj.Net = item.ItemNet;
                            tmpObj.Discount = item.ItemDiscount;
                            tmpObj.Description = item.ItemDescr;
                            tmpObj.EndOfDayId = invoice == null ? (long?)null : invoice.EndOfDayId;
                            tmpObj.Abbreviation = Abbrev;
                            ordDet.OrderDetailInvoices.Add(tmpObj);

                            ret.Add(ordDet);
                            lastProduce = ordDet;
                        }
                        else
                        {
                            //Item is extra add addided to last line
                            OrderDetailIngredientsModel odi = new OrderDetailIngredientsModel
                            {
                                IngredientId = item.ProductId,
                                PriceListDetailId = item.PriceListDetailId,
                                Price = item.Price,
                                Qty = item.ItemQty,
                                TotalAfterDiscount = item.ItemGross,
                                Discount = item.ItemDiscount
                            };
                            if (!(orderType == ModifyOrderDetailsEnum.FromScratch))
                                odi.OrderDetailId = null; 
                            else
                                //{TODO: Find Id}
                                odi.OrderDetailId = 0;
                            lastProduce.OrderDetIngrendients.Add(odi);

                            //New OrderDetailInvoices. for each item in ReceiptDetails Add it to last line
                            OrderDetailInvoicesModel tmpObj = AutoMapper.Mapper.Map<OrderDetailInvoicesModel>(item);
                            tmpObj.CreationTS = DateTime.Now;
                            tmpObj.Counter = invoice == null ? (long?)null : invoice.Counter;
                            tmpObj.IsPrinted = isPrinted;
                            tmpObj.Qty = item.ItemQty;
                            tmpObj.VatAmount = item.ItemVatValue;
                            tmpObj.VatRate = item.ItemVatRate;
                            tmpObj.TaxAmount = item.ItemTaxAmount;
                            tmpObj.Total = item.ItemGross;
                            tmpObj.Net = item.ItemNet;
                            tmpObj.Discount = item.ItemDiscount;
                            tmpObj.Description = item.ItemDescr;
                            tmpObj.EndOfDayId = invoice == null ? (long?)null : invoice.EndOfDayId;
                            tmpObj.Abbreviation = Abbrev;
                            tmpObj.IngredientId = item.ProductId;
                            lastProduce.OrderDetailInvoices.Add(tmpObj);

                        }
                        break;
                    case ModifyOrderDetailsEnum.FromOtherUnmodified:
                        //odi.OrderDetailId = receiptDetails.OrderDetailId;
                        //if (receiptDetails.IsExtra == 1)
                        //    odi.OrderDetailIgredientsId = receiptDetails.OrderDetailIgredientsId;
                        break;
                    default: break;
                }


                if(item.SelectedQuantity != null && item.SelectedQuantity != 0)
                {
                    /*{TODO: Σπάω το Αρχικό ΔΠ. 
                            έστω 5 coca cola στο αρχικό και πληρώνω 2
                            στον OrderDetail 2 εγγραφές μια το 5-2 = 3 εναπομείναντα προϊόντα
                                                    και μια με 2 νέα προϊόντα
                           Αντίστοιχα στον OrderDetailInvoices 2 εγγραφές.
                           Στον OrderDetailInvoices επιπλέον μια εγγραφή με την ποσότητα των νέων προϊόντων για την Απόδειξη
                    }
                    */
                }
            }

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewOrderDetailInvoice(DBInfoModel Store, OrderDetailInvoicesModel item)
        {
            return dt.AddNewOrderDetailInvoice(Store, item);
        }

        /// <summary>
        /// Return's a list of orderdetails, erdetdetailingredients and orderdetailextras
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Order"></param>
        /// <param name="orderDet"></param>
        /// <param name="extras"></param>
        /// <param name="orderType"></param>
        /// <param name="GuestId"></param>
        /// <param name="IsDA"></param>
        /// <param name="SalesType"></param>
        /// <param name="TableId"></param>
        /// <param name="StaffId"></param>
        /// <param name="Error"></param>
        /// <param name="db"></param>
        /// <param name="dbTransact"></param>
        /// <returns></returns>
        public List<OrderDetailWithExtrasModel> ConvertDA_OrderDetailsToOrderDetails(DBInfoModel Store, DA_OrderModel Order,
            List<DA_OrderDetails> orderDet, List<DA_OrderDetailsExtrasModel> extras, 
            ModifyOrderDetailsEnum orderType, long? GuestId, bool IsDA, long? SalesType, 
            long? TableId, DeliveryCustomerModel customer, long?StaffId, out string Error, IDbConnection db = null, IDbTransaction dbTransact = null)
        {
            Error = "";
            List<OrderDetailWithExtrasModel> ret = new List<OrderDetailWithExtrasModel>();

            OrderDetailWithExtrasModel ordDet = new OrderDetailWithExtrasModel();

            PosInfoDetailModel posInfoDet = posInfoDetTask.GetSinglePosInfoDetail(Store, Order.StorePosInfoDetail ?? 0, db, dbTransact);
            if(posInfoDet == null)
            {
                Error = "Pos Info Detail for Pos " + Order.PosId.ToString() + " not found";
                return null;
            }

            decimal PstDisc = 0;
            if (Order.Discount != 0)
                PstDisc = Math.Round(Order.Discount / Order.Price, 4);
            //PstDisc = Math.Round((100 * Order.Discount) / (Order.Discount + Order.Total), 2);

            TablesModel table = tableDt.GetModelById(Store, TableId ?? 0, db, dbTransact);
            List<string> tableLabels = orderDetailInvoicesDT.GetTableLabelsInTable(Store, TableId);
            string tableLabel = DetermineTableLabel(TableId, tableLabels, customer);
            List<VatModel> vats = vatTask.GetAllVats(Store, db, dbTransact);

            ProductModel mainProd;
            IngredientsModel ingModel;
            PricelistDetailModel prlDet;
            ProductCategoriesModel prodCategory;
            ProductCategoriesModel inredCategory;
            CategoriesModel category;
            VatModel vatModel = new VatModel();

            long? priceListId;

            int ItemPos = 0;

            foreach (DA_OrderDetails item in orderDet)
            {
                if (IsDA)
                    mainProd = prodTask.GetModelByDAIs(Store, item.ProductId, db, dbTransact);
                else
                    mainProd = prodTask.GetModelById(Store, item.ProductId, db, dbTransact);
                if (mainProd == null)
                {
                    Error = "Product not found. " + (IsDA ? "DAId : " : "Id : ") + item.ProductId.ToString();
                    return null;
                }
                priceListId = priceListTask.GetIdByDAIs(Store, item.PriceListId ?? 0, db, dbTransact);
                prlDet = prLstDetTask.SelectPricelistDetailForProductAndPricelist(Store, mainProd.Id, priceListId ?? 0, db, dbTransact);
                prodCategory = prodCategTask.GetModelById(Store, mainProd.ProductCategoryId ?? 0, db, dbTransact);
                if (prodCategory != null)
                    category = categTask.GetModelById(Store, prodCategory.CategoryId ?? 0, db, dbTransact);
                else
                    category = null;

                vatModel = vats.Find(f => f.Percentage == item.RateVat);

                //Create new model foreac item in List of orderDetails. Add's it to ret for return;
                ordDet = new OrderDetailWithExtrasModel();


                switch (orderType)
                {
                    case ModifyOrderDetailsEnum.FromScratch:
                      
                        //Create's new OrderDetail Model
                        ordDet.Couver = 0;
                        ordDet.Discount = item.Discount;
                        ordDet.GuestId = GuestId;
                        ordDet.Guid = Guid.NewGuid();
                        ordDet.IsDeleted = false;
                        ordDet.KdsId = mainProd.KdsId;
                        ordDet.KitchenId = mainProd.KitchenId;
                        ordDet.PaidStatus = Order.OrderType == OrderTypeStatus.DineIn && 
                                                               (Order.AccountType == (short)AccountTypeEnum.CreditCard || Order.AccountType == (short)AccountTypeEnum.TicketCompliment) && 
                                                               Order.IsPaid ? (byte)1 : (byte)0;
                        ordDet.PendingQty = null;
                        ordDet.PreparationTime = mainProd.PreparationTime;
                        ordDet.Price = item.Price;
                        ordDet.PriceListDetailId = prlDet.Id;
                        ordDet.ProductId = mainProd.Id;
                        ordDet.Qty = (double)item.Qnt;
                        ordDet.SalesTypeId = SalesType;
                        ordDet.Status = (byte)Order.Status;
                        ordDet.StatusTS = DateTime.Now;
                        ordDet.TableId = table?.Id;
                        ordDet.TotalAfterDiscount = item.Total;
                        ordDet.OtherDiscount = item.OtherDiscount;
                        //όλα τα OrderDetail που δεν ανήκουν σε κουζίνα (KdsId = null) πάνε KitchenStatus έτοιμα τα υπόλοιπα pending  
                        if (ordDet.KdsId == null)
                        {
                            ordDet.KitchenStatus = (int)KdsKitchenStatusEnums.Ready;
                        }
                        else
                        {
                            ordDet.KitchenStatus = (int)KdsKitchenStatusEnums.Pending;
                        }

                        //List Of OrderDetailIngredients
                        ordDet.OrderDetIngrendients = new List<OrderDetailIngredientsModel>();
                        
                        //List Of OrderDetailInvoices
                        ordDet.OrderDetailInvoices = new List<OrderDetailInvoicesModel>();



                        //Add all Record's OrderDetail to OrderDetailInvoicesModel
                        decimal totalAmount = item.Total;
                        decimal totalDiscount = 0;
                        decimal totalVatAmount = item.TotalVat;
                        decimal totalTaxAmount = item.TotalTax;

                        if (PstDisc != 0)
                        {
                            totalDiscount = Math.Round(totalAmount * PstDisc, 4); // Math.Round((decimal)(totalAmount - (totalAmount / ((100 + PstDisc) / 100))), 2);
                            totalAmount -= totalDiscount;
                            if (vatModel.Percentage != 0)
                            {
                                decimal tempNet = Math.Round(totalAmount / (1 + ((vatModel.Percentage ?? 0) / 100)), 4);
                                totalVatAmount = totalAmount - tempNet;
                            }
                        }


                        OrderDetailInvoicesModel ordInv = new OrderDetailInvoicesModel();
                        ordInv.Abbreviation = "";
                        ordInv.CategoryId = category == null ? (long?)null : category.Id;
                        ordInv.CreationTS = DateTime.Now;
                        ordInv.Description = item.Description;
                        ordInv.Discount = item.Discount;
                        ordInv.IsDeleted = false;
                        ordInv.IsExtra = false;
                        ordInv.IsPrinted = false;
                        ordInv.ItemBarcode = 0;
                        ordInv.ItemCode = mainProd.Code;
                        ordInv.ItemPosition = 0;
                        ordInv.ItemRegion = "";
                        ordInv.ItemRemark = item.ItemRemark;
                        ItemPos++;
                        ordInv.ItemSort = ItemPos;
                        ordInv.PosInfoDetailId = posInfoDet.Id;
                        ordInv.Price = item.Price;
                        ordInv.PricelistId = priceListId;
                        ordInv.ProductCategoryId = prodCategory.Id;
                        ordInv.ProductId = mainProd.Id;
                        ordInv.Qty = (double?)item.Qnt;
                        ordInv.SalesTypeId = SalesType;
                        ordInv.StaffId = StaffId;
                        ordInv.TableId = table?.Id;
                        ordInv.TableCode = table?.Code;
                        ordInv.TableLabel = tableLabel;
                        ordInv.RegionId = table?.RegionId;
                        ordInv.TaxId = (long?)item.RateTax;
                        ordInv.ReceiptSplitedDiscount = totalDiscount;// Math.Round((decimal)(PstDisc == 0 ? 0 : (item.Total - (item.Total / ((100 + PstDisc) / 100)))), 2);
                        ordInv.Total = totalAmount;
                        ordInv.TotalAfterDiscount = totalAmount;
                        ordInv.TotalBeforeDiscount = totalAmount + totalDiscount + item.Discount;
                        ordInv.VatAmount = totalVatAmount;
                        ordInv.TaxAmount = totalTaxAmount;
                        ordInv.Net = totalAmount - totalVatAmount - totalTaxAmount;

                        ordInv.VatCode = vatModel == null ? (int?)null : vatModel.Code;
                        ordInv.VatId = vatModel == null ? (long?)null : vatModel.Id;
                        ordInv.VatRate = vatModel == null ? (decimal?)null : vatModel.Percentage;
                        ordDet.OrderDetailInvoices.Add(ordInv);

                        //Find all extras for order detail record
                        List<DA_OrderDetailsExtrasModel> ordExtras = extras.FindAll(f => f.OrderDetailId == item.Id);
                        foreach (DA_OrderDetailsExtrasModel ext in ordExtras)
                        {
                            OrderDetailIngredientsModel ordIng = new OrderDetailIngredientsModel();
                            vatModel = vats.Find(f => f.Percentage == ext.RateVat);

                            ingModel = ingTask.GetModelByDAIs(Store, ext.ExtrasId, db, dbTransact);
                            if(ingModel == null && ext.ExtrasId > 0) //For Goodys extras id equals -1 and make different deployment
                            {
                                Error = "Ingredient not found. " + (IsDA ? "DAId : " : "Id : ") + ext.ExtrasId.ToString();
                                return null;
                            }
                            if (ingModel == null)
                                inredCategory = null;
                            else
                                inredCategory = prodCategTask.GetModelById(Store, ingModel.IngredientCategoryId ?? 0, db, dbTransact);


                            ordIng.IngredientId = ingModel == null ? ext.ExtrasId : ingModel.Id; //Implementation for goodys
                            ordIng.IsDeleted = false;
                            ordIng.Price = ext.Price;
                            ordIng.PriceListDetailId = prlDet.Id;
                            ordIng.Qty = (double?)(ext.Qnt * item.Qnt);
                            ordIng.TotalAfterDiscount = ext.NetAmount + ext.TotalVat + ext.TotalTax;
                            ordIng.UnitId = ingModel == null ? -1 : ingModel.UnitId; //Implementation for goodys
                            ordIng.PendingQty = ordIng.Qty;
                            ordDet.OrderDetIngrendients.Add(ordIng);

                            //Add all Record's Extras to OrderDetailInvoicesModel

                            totalAmount = (ext.Qnt * ext.Price) * item.Qnt;
                            totalDiscount = 0;
                            totalVatAmount = (ext.Qnt * ext.TotalVat) * item.Qnt;
                            totalTaxAmount = (ext.Qnt * ext.TotalTax) * item.Qnt;

                            if (PstDisc != 0)
                            {
                                totalDiscount = Math.Round(totalAmount * PstDisc, 4); //Math.Round((decimal)(totalAmount - (totalAmount / ((100 + PstDisc) / 100))), 2);
                                totalAmount -= totalDiscount;
                                if (vatModel != null && vatModel.Percentage != 0)
                                {
                                    decimal tempNet = Math.Round(totalAmount / (1 + ((vatModel.Percentage ?? 0) / 100)), 4);
                                    totalVatAmount = totalAmount - tempNet;
                                }
                            }

                            ordInv = new OrderDetailInvoicesModel();
                            ordInv.Abbreviation = "";
                            ordInv.CategoryId = category == null ? (long?)null : category.Id;
                            ordInv.CreationTS = DateTime.Now;
                            ordInv.Description = ingModel == null ? ext.Description : ingModel.Description; //Implementation for goodys
                            ordInv.Discount = 0;
                            ordInv.IsDeleted = false;
                            ordInv.IsExtra = true;
                            ordInv.IsPrinted = false;
                            ordInv.ItemBarcode = 0;
                            ordInv.ItemCode = ingModel == null ? ext.ExtraCode : ingModel.Code; //Implementation for goodys
                            ordInv.ItemPosition = 0;
                            ordInv.ItemRegion = "";
                            ordInv.ItemRemark = item.ItemRemark;
                            ItemPos++;
                            ordInv.ItemSort = ItemPos;
                            ordInv.IngredientId = ingModel == null ? null : ingModel.Id; //Implementation for goodys.Any Other value not in Ingredients table raise Foreign Key Error
                            ordInv.ProductId = ingModel == null ? null : ingModel.Id; //Implementation for goodys.Any Other value not in Ingredients table raise Foreign Key Error
                            ordInv.Net = totalAmount - totalVatAmount - totalTaxAmount;
                            ordInv.PosInfoDetailId = posInfoDet.Id;
                            ordInv.Price = ext.Price;
                            ordInv.PricelistId = priceListId;
                            ordInv.ProductCategoryId = inredCategory != null ? inredCategory.Id : prodCategory.Id;
                            ordInv.Qty = (double?)(ext.Qnt * item.Qnt);
                            ordInv.ReceiptSplitedDiscount = totalDiscount;
                            ordInv.SalesTypeId = SalesType;
                            ordInv.StaffId = StaffId;
                            ordInv.TableId = table?.Id;
                            ordInv.TableCode = table?.Code;
                            ordInv.TableLabel = tableLabel;
                            ordInv.RegionId = table?.RegionId;
                            ordInv.TaxAmount = totalTaxAmount;
                            ordInv.TaxId = (long?)ext.RateTax;
                            ordInv.Total = totalAmount;
                            ordInv.TotalAfterDiscount = totalAmount;
                            ordInv.TotalBeforeDiscount = totalAmount + totalDiscount + item.Discount;
                            ordInv.VatAmount = totalVatAmount;

                            ordInv.VatCode = vatModel == null ? (int?)null : vatModel.Code;
                            ordInv.VatId = vatModel == null ? (long?)null : vatModel.Id;
                            ordInv.VatRate = vatModel == null ? (decimal?)null : vatModel.Percentage;
                            ordDet.OrderDetailInvoices.Add(ordInv);
                        }
                        

                        break;
                    case ModifyOrderDetailsEnum.FromOtherUnmodified:
                        //odi.OrderDetailId = receiptDetails.OrderDetailId;
                        //if (receiptDetails.IsExtra == 1)
                        //    odi.OrderDetailIgredientsId = receiptDetails.OrderDetailIgredientsId;
                        break;
                    default: break;
                }


                //if (item.SelectedQuantity != null && item.SelectedQuantity != 0)
                //{
                //    /*{TODO: Σπάω το Αρχικό ΔΠ. 
                //            έστω 5 coca cola στο αρχικό και πληρώνω 2
                //            στον OrderDetail 2 εγγραφές μια το 5-2 = 3 εναπομείναντα προϊόντα
                //                                    και μια με 2 νέα προϊόντα
                //           Αντίστοιχα στον OrderDetailInvoices 2 εγγραφές.
                //           Στον OrderDetailInvoices επιπλέον μια εγγραφή με την ποσότητα των νέων προϊόντων για την Απόδειξη
                //    }
                //    */
                //}
                ret.Add(ordDet);
            }

            return ret;
        }

        /// <summary>
        /// Gets order detail invoices of selected invoice based on InvoicesId
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="InvoiceId"> Invoice to get order detail invoices from </param>
        /// <returns> List of order detail invoices. See: <seealso cref="Symposium.Models.Models.OrderDetailInvoicesModel"/> </returns>
        public List<OrderDetailInvoicesModel> GetOrderDetailInvoicesOfSelectedInvoice(DBInfoModel Store, long InvoiceId)
        {
            return dt.GetOrderDetailInvoicesOfSelectedInvoice(Store, InvoiceId);
        }

        /// <summary>
        /// Determines table tabel of order according to customer's email and already inserted table labels
        /// </summary>
        /// <param name="tableLabels"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public string DetermineTableLabel(long? tableId, List<string> tableLabels, DeliveryCustomerModel customer)
        {
            if (tableId == null)
            {
                return null;
            }
            if (customer == null)
            {
                return null;
            }
            string customerFirstName = customer.FirstName ?? "";
            string customerLastName = customer.LastName ?? "";
            string labelToInsert = String.Concat(customerLastName, " ", customerFirstName);
            bool labelFound = false;
            foreach(string label in tableLabels)
            {
                if (label.Equals(labelToInsert))
                {
                    labelFound = true;
                    break;
                }
            }
            if (labelFound)
            {
                for (int x = 1; x < 100; x++)
                {
                    string tempLabel = String.Concat(labelToInsert, " (", x.ToString(), ")");
                    labelFound = false;
                    foreach (string label in tableLabels)
                    {
                        if (label.Equals(tempLabel))
                        {
                            labelFound = true;
                            break;
                        }
                    }
                    if (!labelFound)
                    {
                        labelToInsert = tempLabel;
                        break;
                    }
                }
            }
            return labelToInsert;
        }


        /// <summary>
        /// Return's a record from OrderDetailInvoices using Parameters can use
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderDetailId"></param>
        /// <param name="ProductIngredId"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="PosInfoId"></param>
        /// <param name="PosInfoDetailId"></param>
        /// <param name="IsExtra"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public OrderDetailInvoicesModel GetOrderDtailByOrderDetailId(DBInfoModel Store, long OrderDetailId, long ProductIngredId, long? InvoiceId,
            long? PosInfoId, long? PosInfoDetailId, bool IsExtra, out string Error)
        {
            return dt.GetOrderDtailByOrderDetailId(Store, OrderDetailId, ProductIngredId, InvoiceId, PosInfoId, PosInfoDetailId, IsExtra, out Error);
        }


        /// <summary>
        /// Return's a list of orderdetails, erdetdetailingredients and orderdetailextras
        /// Uses Transaction
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dbTransact"></param>
        /// <param name="Order"></param>
        /// <param name="orderDet"></param>
        /// <param name="extras"></param>
        /// <param name="orderType"></param>
        /// <param name="GuestId"></param>
        /// <param name="IsDA"></param>
        /// <param name="SalesType"></param>
        /// <param name="TableId"></param>
        /// <param name="StaffId"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        //public List<OrderDetailWithExtrasModel> ConvertDA_OrderDetailsToOrderDetails(IDbConnection db, IDbTransaction dbTransact, 
        //    DA_OrderModel Order, List<DA_OrderDetails> orderDet, List<DA_OrderDetailsExtrasModel> extras,
        //    ModifyOrderDetailsEnum orderType, long? GuestId, bool IsDA, long? SalesType,
        //    long? TableId, long? StaffId, out string Error)
        //{
        //    Error = "";
        //    List<OrderDetailWithExtrasModel> ret = new List<OrderDetailWithExtrasModel>();

        //    OrderDetailWithExtrasModel ordDet = new OrderDetailWithExtrasModel();
            
        //    string SQL = "SELECT TOP 1 * FROM PosInfoDetail WHERE Id = @PosInfoId";

        //    PosInfoDetailModel posInfoDet = db.Query<PosInfoDetailModel>(SQL, new { PosInfoId = Order.StorePosInfoDetail }, dbTransact).FirstOrDefault();
        //    if (posInfoDet == null)
        //    {
        //        Error = "Pos Info Detail for Pos " + Order.PosId.ToString() + " not found";
        //        return null;
        //    }

        //    decimal PstDisc = 0;
        //    if (Order.Discount != 0)
        //        PstDisc = Math.Round(Order.Discount / Order.Price, 4);

        //    SQL = "SELECT * FROM [Table] WHERE Id = @Id";
        //    TablesModel table = db.Query<TablesModel>(SQL, new { Id = TableId ?? 0 }, dbTransact).FirstOrDefault();
        //    SQL = "SELECT * FROM Vat";
        //    List<VatModel> vats = db.Query<VatModel>(SQL, null, dbTransact).ToList();

        //    ProductModel mainProd;
        //    IngredientsModel ingModel;
        //    PricelistDetailModel prlDet;
        //    ProductCategoriesModel prodCategory;
        //    ProductCategoriesModel inredCategory;
        //    CategoriesModel category;
        //    VatModel vatModel = new VatModel();

        //    long? priceListId;

        //    int ItemPos = 0;

        //    foreach (DA_OrderDetails item in orderDet)
        //    {
        //        if (IsDA)
        //            SQL = "SELECT * FROM Product WHERE DAId = @ProductId";
        //        else
        //            SQL = "SELECT * FROM Product WHERE Id = @ProductId";
        //        mainProd = db.Query<ProductModel>(SQL, new { ProductId = item.ProductId }, dbTransact).FirstOrDefault();

        //        if (mainProd == null)
        //        {
        //            Error = "Product not found. " + (IsDA ? "DAId : " : "Id : ") + item.ProductId.ToString();
        //            return null;
        //        }
        //        SQL = "SELECT Id FROM Pricelist WHERE DAId = @PriceListId";
        //        priceListId = db.Query<long>(SQL, new { PriceListId = item.PriceListId }, dbTransact).FirstOrDefault();
        //        SQL = "SELECT * FROM PricelistDetail WHERE PricelistId = @PricelistId AND ProductId = @ProductId";
        //        prlDet = db.Query<PricelistDetailModel>(SQL, new { PricelistId = priceListId, ProductId = mainProd.Id }, dbTransact).FirstOrDefault();
        //        SQL = "SELECT * FROM ProductCategories WHERE Id = @Id";
        //        prodCategory = db.Query<ProductCategoriesModel>(SQL, new { Id = mainProd.ProductCategoryId }, dbTransact).FirstOrDefault();
        //        if (prodCategory != null)
        //        {
        //            SQL = "SELECT * FROM Categories WHERE Id = @Id";
        //            category = db.Query<CategoriesModel>(SQL, new { Id = prodCategory.CategoryId }, dbTransact).FirstOrDefault();
        //        }
        //        else
        //            category = null;

        //        vatModel = vats.Find(f => f.Percentage == item.RateVat);

        //        //Create new model foreac item in List of orderDetails. Add's it to ret for return;
        //        ordDet = new OrderDetailWithExtrasModel();

        //        switch (orderType)
        //        {
        //            case ModifyOrderDetailsEnum.FromScratch:

        //                //Create's new OrderDetail Model

        //                ordDet.Couver = 0;
        //                ordDet.Discount = item.Discount;
        //                ordDet.GuestId = GuestId;
        //                ordDet.Guid = Guid.NewGuid();
        //                ordDet.IsDeleted = false;
        //                ordDet.KdsId = mainProd.KdsId;
        //                ordDet.KitchenId = mainProd.KitchenId;
        //                ordDet.PaidStatus = 0;
        //                ordDet.PendingQty = null;
        //                ordDet.PreparationTime = mainProd.PreparationTime;
        //                ordDet.Price = item.Price;
        //                ordDet.PriceListDetailId = prlDet.Id;
        //                ordDet.ProductId = mainProd.Id;
        //                ordDet.Qty = (double)item.Qnt;
        //                ordDet.SalesTypeId = SalesType;
        //                ordDet.Status = 1;
        //                ordDet.StatusTS = DateTime.Now;
        //                ordDet.TableId = TableId;
        //                ordDet.TotalAfterDiscount = item.Total;

        //                //List Of OrderDetailIngredients
        //                ordDet.OrderDetIngrendients = new List<OrderDetailIngredientsModel>();

        //                //List Of OrderDetailInvoices
        //                ordDet.OrderDetailInvoices = new List<OrderDetailInvoicesModel>();

        //                //Add all Record's OrderDetail to OrderDetailInvoicesModel
        //                decimal totalAmount = item.Total;
        //                decimal totalDiscount = 0;
        //                decimal totalVatAmount = item.TotalVat;
        //                decimal totalTaxAmount = item.TotalTax;

        //                if (PstDisc != 0)
        //                {
        //                    totalDiscount = Math.Round(totalAmount * PstDisc, 4);  //Math.Round((decimal)(totalAmount - (totalAmount / ((100 + PstDisc) / 100))), 2);
        //                    totalAmount -= totalDiscount;
        //                    if (vatModel.Percentage != 0)
        //                    {
        //                        decimal tempNet = Math.Round(totalAmount / (1 + ((vatModel.Percentage ?? 0) / 100)), 4);
        //                        totalVatAmount = totalAmount - tempNet;
        //                    }
        //                }

        //                OrderDetailInvoicesModel ordInv = new OrderDetailInvoicesModel();
        //                ordInv.Abbreviation = "";
        //                ordInv.CategoryId = category == null ? (long?)null : category.Id;
        //                ordInv.CreationTS = DateTime.Now;
        //                ordInv.Description = item.Description;
        //                ordInv.Discount = item.Discount;
        //                ordInv.IsDeleted = false;
        //                ordInv.IsExtra = false;
        //                ordInv.IsPrinted = false;
        //                ordInv.ItemBarcode = 0;
        //                ordInv.ItemCode = mainProd.Code;
        //                ordInv.ItemPosition = 0;
        //                ordInv.ItemRegion = "";
        //                ordInv.ItemRemark = item.ItemRemark;
        //                ItemPos++;
        //                ordInv.ItemSort = ItemPos;
        //                ordInv.PosInfoDetailId = posInfoDet.Id;
        //                ordInv.Price = item.Price;
        //                ordInv.PricelistId = priceListId;
        //                ordInv.ProductCategoryId = prodCategory.Id;
        //                ordInv.ProductId = mainProd.Id;
        //                ordInv.Qty = (double?)item.Qnt;
        //                ordInv.SalesTypeId = SalesType;
        //                ordInv.StaffId = StaffId;
        //                ordInv.TableCode = table == null ? (string)null : table.Code;
        //                ordInv.TaxId = (long?)item.RateTax;
        //                ordInv.ReceiptSplitedDiscount = totalDiscount;
        //                ordInv.Total = totalAmount;
        //                ordInv.TotalAfterDiscount = totalAmount;
        //                ordInv.TotalBeforeDiscount = totalAmount + totalDiscount + item.Discount;
        //                ordInv.VatAmount = totalVatAmount;
        //                ordInv.TaxAmount = totalTaxAmount;
        //                ordInv.Net = totalAmount - totalVatAmount - totalTaxAmount;
        //                ordInv.VatCode = vatModel == null ? (int?)null : vatModel.Code;
        //                ordInv.VatId = vatModel == null ? (long?)null : vatModel.Id;
        //                ordInv.VatRate = vatModel == null ? (decimal?)null : vatModel.Percentage;
        //                ordDet.OrderDetailInvoices.Add(ordInv);

        //                //Find all extras for order detail record
        //                List<DA_OrderDetailsExtrasModel> ordExtras = extras.FindAll(f => f.OrderDetailId == item.Id);
        //                foreach (DA_OrderDetailsExtrasModel ext in ordExtras)
        //                {
        //                    OrderDetailIngredientsModel ordIng = new OrderDetailIngredientsModel();
        //                    SQL = "SELECT * FROM Ingredients WHERE DAId = @DAId";
        //                    ingModel = db.Query<IngredientsModel>(SQL, new { DAId = ext.ExtrasId }, dbTransact).FirstOrDefault();
        //                    if (ingModel == null)
        //                    {
        //                        Error = "Ingredient not found. " + (IsDA ? "DAId : " : "Id : ") + ext.ExtrasId.ToString();
        //                        return null;
        //                    }
        //                    SQL = "SELECT * FROM ProductCategories WHERE Id = @Id";
        //                    inredCategory = db.Query<ProductCategoriesModel>(SQL, new { Id = ingModel.IngredientCategoryId }, dbTransact).FirstOrDefault();

        //                    ordIng.IngredientId = ingModel.Id;
        //                    ordIng.IsDeleted = false;
        //                    ordIng.Price = ext.Price;
        //                    ordIng.PriceListDetailId = prlDet.Id;
        //                    ordIng.Qty = (double?)(ext.Qnt * item.Qnt);
        //                    ordIng.TotalAfterDiscount = ext.NetAmount + ext.TotalVat + ext.TotalTax;
        //                    ordIng.UnitId = ingModel.UnitId;
        //                    ordDet.OrderDetIngrendients.Add(ordIng);

        //                    //Add all Record's Extras to OrderDetailInvoicesModel

        //                    totalAmount = (ext.Price * ext.Qnt) * item.Qnt;
        //                    totalDiscount = 0;
        //                    totalVatAmount = (ext.TotalVat * ext.Qnt) * item.Qnt;
        //                    totalAmount = (ext.TotalTax * ext.Qnt) * item.Qnt;

        //                    if (PstDisc != 0)
        //                    {
        //                        totalDiscount = Math.Round(totalAmount * PstDisc, 4); //Math.Round((decimal)(totalAmount - (totalAmount / ((100 + PstDisc) / 100))), 2);
        //                        totalTaxAmount -= totalDiscount;
        //                        if (vatModel.Percentage != 0)
        //                        {
        //                            decimal tempNet = Math.Round(totalAmount / (1 + ((vatModel.Percentage ?? 0) / 100)), 4);
        //                            totalVatAmount = totalAmount - tempNet;
        //                        }
        //                    }

        //                    ordInv = new OrderDetailInvoicesModel();
        //                    ordInv.Abbreviation = "";
        //                    ordInv.CategoryId = category == null ? (long?)null : category.Id;
        //                    ordInv.CreationTS = DateTime.Now;
        //                    ordInv.Description = ingModel.Description;
        //                    ordInv.Discount = 0;
        //                    ordInv.IsDeleted = false;
        //                    ordInv.IsExtra = true;
        //                    ordInv.IsPrinted = false;
        //                    ordInv.ItemBarcode = 0;
        //                    ordInv.ItemCode = ingModel.Code;
        //                    ordInv.ItemPosition = 0;
        //                    ordInv.ItemRegion = "";
        //                    ordInv.ItemRemark = item.ItemRemark;
        //                    ItemPos++;
        //                    ordInv.ItemSort = ItemPos;
        //                    ordInv.IngredientId = ingModel.Id;
        //                    ordInv.ProductId = ingModel.Id;
        //                    ordInv.Net = totalAmount - totalVatAmount - totalTaxAmount;
        //                    ordInv.PosInfoDetailId = posInfoDet.Id;
        //                    ordInv.Price = ext.Price;
        //                    ordInv.PricelistId = priceListId;
        //                    ordInv.ProductCategoryId = inredCategory != null ? inredCategory.Id : prodCategory.Id;
        //                    ordInv.Qty = (double?)(ext.Qnt * item.Qnt);
        //                    ordInv.ReceiptSplitedDiscount = totalDiscount;
        //                    ordInv.SalesTypeId = SalesType;
        //                    ordInv.StaffId = StaffId;
        //                    ordInv.TableCode = table == null ? (string)null : table.Code;
        //                    ordInv.TaxAmount = totalTaxAmount;
        //                    ordInv.TaxId = (long?)ext.RateTax;
        //                    ordInv.Total = totalAmount;
        //                    ordInv.TotalAfterDiscount = totalAmount;
        //                    ordInv.TotalBeforeDiscount = totalAmount + totalDiscount + item.Discount;
        //                    ordInv.VatAmount = totalVatAmount;
        //                    vatModel = vats.Find(f => f.Percentage == ext.RateVat);
        //                    ordInv.VatCode = vatModel == null ? (int?)null : vatModel.Code;
        //                    ordInv.VatId = vatModel == null ? (long?)null : vatModel.Id;
        //                    ordInv.VatRate = vatModel == null ? (decimal?)null : vatModel.Percentage;
        //                    ordDet.OrderDetailInvoices.Add(ordInv);
        //                }
        //                break;
        //            case ModifyOrderDetailsEnum.FromOtherUnmodified:
        //                //odi.OrderDetailId = receiptDetails.OrderDetailId;
        //                //if (receiptDetails.IsExtra == 1)
        //                //    odi.OrderDetailIgredientsId = receiptDetails.OrderDetailIgredientsId;
        //                break;
        //            default: break;
        //        }

        //        //if (item.SelectedQuantity != null && item.SelectedQuantity != 0)
        //        //{
        //        //    /*{TODO: Σπάω το Αρχικό ΔΠ. 
        //        //            έστω 5 coca cola στο αρχικό και πληρώνω 2
        //        //            στον OrderDetail 2 εγγραφές μια το 5-2 = 3 εναπομείναντα προϊόντα
        //        //                                    και μια με 2 νέα προϊόντα
        //        //           Αντίστοιχα στον OrderDetailInvoices 2 εγγραφές.
        //        //           Στον OrderDetailInvoices επιπλέον μια εγγραφή με την ποσότητα των νέων προϊόντων για την Απόδειξη
        //        //    }
        //        //    */
        //        //}
        //        ret.Add(ordDet);
        //    }

        //    return ret;
        //}
    }
}
