using Pos_WebApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;


namespace Pos_WebApi.Patches {
    public class FixWaiterUnpaidInvoices : DataIntegrityPatch {
        public override void ApplyPatch( PosEntities dbContext ) {
            var invoiceRepo = new InvoiceRepository(dbContext);
            var unpaidInvoices = dbContext.Invoices
                .Include(x => x.OrderDetailInvoices)
                .Include(x => x.PosInfo)
                .Include(x => x.Transactions)
                .Where(x => x.IsPaid == 0 &&
                        ( !x.EndOfDayId.HasValue ) &&
                         x.Transactions.Sum(y => y.Amount) == x.Total).ToArray();
            foreach ( var currentInvoice in unpaidInvoices ) {
                var prodCatId = currentInvoice.OrderDetailInvoices.First().ProductCategoryId;
                var posDeptId = currentInvoice.PosInfo.DepartmentId;
                var priceListId = currentInvoice.OrderDetailInvoices.First().PricelistId;

                var pmsDept = dbContext.TransferMappings.Where(x => x.PosDepartmentId == posDeptId &&
                            x.PriceListId == priceListId &&
                            x.ProductCategoryId == prodCatId)
                   .Select(x => new {
                       PmsDepartmentId = x.PmsDepartmentId,
                       PmsDepartmentDescription = x.PmsDepDescription,
                       PosDepartmentId = x.PosDepartmentId,
                       HotelId = x.HotelId,
                   }).First();

                var orderTotal = currentInvoice.OrderDetailInvoices.Sum(x => x.Total);
                var guestId = currentInvoice.GuestId;
                var regNo = 0;
                var profileNo = 0;
                var profileName = "Cash";
                var roomId = 0;
                var roomDescription = "9610";
                if ( guestId.HasValue ) {

                    var guest = dbContext.Guest.Find(guestId.Value);
                    regNo = guest.ReservationId.Value;
                    profileNo = guest.ProfileNo.Value;
                    profileName = guest.LastName;
                    roomId = guest.RoomId.Value;
                    roomDescription = guest.Room;
                }
                var transactionId = currentInvoice.Transactions.First().Id;
                short transferType = 0;
                var desc = string.Format("{0}:{1}, Pos: {2}, {3}", "Rec", currentInvoice.Counter, currentInvoice.PosInfoId, pmsDept.PosDepartmentId);
                var receiptNo = currentInvoice.Counter;
                var posInfoDetailId = currentInvoice.PosInfoDetailId;
                var sendToPMS = profileName == "Cash" ? 0 : 1;


                var transferToPMSRecord = new TransferToPms() {
                    Total = orderTotal,
                    Description = desc,
                    PmsDepartmentDescription = pmsDept.PmsDepartmentDescription,
                    PmsDepartmentId = pmsDept.PmsDepartmentId,
                    PosInfoDetailId = posInfoDetailId,
                    EndOfDayId = null,
                    HotelId = pmsDept.HotelId,
                    PosInfoId = currentInvoice.PosInfoId,
                    ProfileId = profileNo.ToString(),
                    ProfileName = profileName,
                    ReceiptNo = receiptNo.ToString(),
                    RegNo = regNo.ToString(),
                    RoomDescription = roomDescription,
                    RoomId = roomId.ToString(),
                    TransactionId = transactionId,
                    Status = 2,
                    TransferIdentifier = Guid.NewGuid(),
                    TransferType = transferType,
                };

                currentInvoice.IsPaid = 2;
                currentInvoice.PaidTotal = currentInvoice.Total;
                dbContext.Entry(currentInvoice).State = EntityState.Modified;

                dbContext.TransferToPms.Add(transferToPMSRecord);
                dbContext.SaveChanges();

            }
        }
    }
}