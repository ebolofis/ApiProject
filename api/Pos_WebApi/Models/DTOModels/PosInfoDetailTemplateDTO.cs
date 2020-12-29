using System.Collections.Generic;

namespace Pos_WebApi.Models.DTOModels
{
    public class PosInfoDetailTemplateDTO
    {
        public PosInfoDetailTemplateDTO()
        {
        }

        public InvoiceTypes InvoiceType { get; set; }
        public short TransactionType { get; set; }

        private PosInfoDetail SetDefaultValues(InvoiceTypes template, bool createTransaction)
        {
            var model = new PosInfoDetail();
            model.Abbreviation = template.Abbreviation;
            model.Description = template.Description;
            model.Counter = 0;
            model.FiscalType = 0;
            model.GroupId = template.Type;
            model.InvoicesTypeId = template.Id;
            model.ResetsAfterEod = false;
            model.Status = 1;
            model.SendsVoidToKitchen = 0;
            model.IsPdaHidden = false;
            model.CreateTransaction = createTransaction;

            switch (model.GroupId)
            {
                case 1:
                    model.ButtonDescription = createTransaction ? "Payment" : "Receipt";
                    model.InvoiceId = 1;
                    model.IsCancel = false;
                    model.IsInvoice = true;
                    model.SendsVoidToKitchen = 0;
                    model.IsPdaHidden = false;
                    break;
                case 2:
                    model.InvoiceId = 2;
                    model.Counter = 0;
                    model.IsInvoice = false;
                    model.IsCancel = false;
                    model.IsPdaHidden = false;
                    model.CreateTransaction = false;
                    model.ButtonDescription = "Captains Order";
                    break;

                case 3:
                    model.InvoiceId = 3;
                    model.Counter = 0;
                    model.IsInvoice = false;
                    model.IsCancel = true;
                    model.IsPdaHidden = false;
                    createTransaction = true;
                    model.ButtonDescription = "Void Receipt";

                    break;
                case 4:
                    model.InvoiceId = 1;
                    model.Counter = 0;
                    model.IsInvoice = true;
                    model.IsCancel = false;
                    model.IsPdaHidden = false;
                    createTransaction = true;
                    model.ButtonDescription = "Complimentary Receipt";

                    break;
                case 5:
                    model.InvoiceId = 1;
                    model.Counter = 0;
                    model.IsInvoice = true;
                    model.IsCancel = false;
                    model.IsPdaHidden = false;
                    createTransaction = true;
                    model.ButtonDescription = "ROOM PACKAGE";

                    break;
                case 7:
                    model.InvoiceId = 4;
                    model.Counter = 0;
                    model.IsInvoice = true;
                    model.IsCancel = false;
                    model.IsPdaHidden = true;
                    model.ButtonDescription = "Invoice";
                    break;
                case 8:
                    model.InvoiceId = 2;
                    model.Counter = 0;
                    model.IsInvoice = false;
                    model.IsCancel = true;
                    model.IsPdaHidden = false;
                    model.CreateTransaction = false;
                    model.SendsVoidToKitchen = 1;
                    model.ButtonDescription = "Void Order";
                    break;
                case 11:
                    model.InvoiceId = 5;
                    model.Counter = 0;
                    model.IsInvoice = false;
                    model.IsCancel = false;
                    model.CreateTransaction = true;
                    model.IsPdaHidden = true;
                    model.ButtonDescription = "Απόδειξη είσπραξης";

                    break;
                case 12:
                    model.InvoiceId = 5;
                    model.Counter = 0;
                    model.IsInvoice = false;
                    model.IsCancel = true;
                    model.CreateTransaction = true;
                    model.IsPdaHidden = true;
                    model.ButtonDescription = "Απόδειξη πληρωμής";
                    break;
            }
            return model;

        }

        public ICollection<PosInfoDetail> CreateDetails()
        {
            var models = new List<PosInfoDetail>();

            models.Add(SetDefaultValues(InvoiceType, true));
            if (TransactionType == 1 && InvoiceType.Type < 8)
            {
                models.Add(SetDefaultValues(InvoiceType, false));
            }

            return models;
        }
    }

}