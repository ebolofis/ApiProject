using Dapper;
using log4net;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{
    public class ReceiptDetailsForExtecrTasks : IReceiptDetailsForExtecrTasks
    {
        IReceiptDetailsForExtecrDT receiptDetailsForExtecrDB;
        IUsersToDatabasesXML usersToDatabases;
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ReceiptDetailsForExtecrTasks(IReceiptDetailsForExtecrDT resDT, IUsersToDatabasesXML usersToDatabases)
        {
            this.receiptDetailsForExtecrDB = resDT;
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Get Receipt Details For Extecr
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns>
        public ExtecrReceiptModel GetReceiptDetailsForExtecr(DBInfoModel Store, Int64 invoiceId, bool groupReceiptItems, bool isForKitchen)
        {
            // get the results
            ExtecrReceiptModel ReceiptModelPreview = receiptDetailsForExtecrDB.GetReceiptDetailsForExtecr(Store, invoiceId, groupReceiptItems, isForKitchen);

            return ReceiptModelPreview;
        }

        /// <summary>
        /// Calculate Vat Of Price
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        public ExtecrReceiptModel CalculateVatPrice(ExtecrReceiptModel model, DBInfoModel Store)
        {
            if(model == null)
            {
                logger.Warn("Cannot Calculate Vat Prices Because Receipt Model for ExtECR was NULL!");
                return null;
            }
            List<string> vats = new List<string>();
            List<CalculateVatPrice> calculateList = new List<CalculateVatPrice>();
            List<VatPercentageModel> vatpercentagemodel = receiptDetailsForExtecrDB.VatPercentage(Store);

            decimal vat1 = 0; decimal netvat1 = 0;
            decimal vat2 = 0; decimal netvat2 = 0;
            decimal vat3 = 0; decimal netvat3 = 0;
            decimal vat4 = 0; decimal netvat4 = 0;
            decimal vat5 = 0; decimal netvat5 = 0;
            foreach (EXTECR_ReceiptItemsModel item in model.Details)
            {
                switch (item.ItemVatRate)
                {
                    case 1:
                        decimal temp1 = item.ItemNet;
                        netvat1 += temp1;
                        vat1 += item.ItemVatValue;
                        break;
                    case 2:
                        decimal temp2 = item.ItemNet;
                        netvat2 += temp2;
                        vat2 += item.ItemVatValue;
                        break;
                    case 3:
                        decimal temp3 = item.ItemNet;
                        netvat3 += temp3;
                        vat3 += item.ItemVatValue;
                        break;
                    case 4:
                        decimal temp4 = item.ItemNet;
                        vat4 += temp4;
                        netvat4 += item.ItemVatValue;
                        break;
                    case 5:
                        decimal temp5 = item.ItemNet;
                        vat5 += temp5;
                        netvat5 += item.ItemVatValue;
                        break;
                    default:
                        Console.WriteLine("Default case");
                        break;
                }

                int index = vats.LastIndexOf(item.ItemVatDesc);
                if (!vats.Contains(item.ItemVatDesc))
                    vats.Add(item.ItemVatDesc);
                CalculateVatPrice c = new CalculateVatPrice();
                c.VatRate = item.ItemVatDesc;
                if (index < 0)
                {
                    c.Total = item.ItemNet;
                    c.VatPrice = item.ItemVatValue;
                    calculateList.Add(c);
                }
                else
                {
                    c.Total = calculateList[index].Total + item.ItemTotal;
                    c.VatPrice = calculateList[index].VatPrice + item.ItemVatValue;
                    calculateList[index] = c;
                }
                foreach (EXTECR_ReceiptExtrasModel extra in item.Extras)
                {
                    int indexExtra = vats.LastIndexOf(extra.ItemVatDesc);
                    switch (extra.ItemVatRate)
                    {
                        case 1:
                            decimal tempextranetvat1 = (decimal)extra.ItemNet;
                            vat1 += extra.ItemVatValue;
                            netvat1 += tempextranetvat1;
                            break;
                        case 2:
                            decimal tempextranetvat2 = (decimal)extra.ItemNet;
                            vat2 += extra.ItemVatValue;
                            netvat2 += tempextranetvat2;
                            break;
                        case 3:
                            decimal tempextranetvat3 = (decimal)extra.ItemNet;
                            vat3 += extra.ItemVatValue;
                            netvat3 += tempextranetvat3;
                            break;
                        case 4:
                            decimal tempextranetvat4 = (decimal)extra.ItemNet;
                            vat4 += extra.ItemVatValue;
                            netvat4 += tempextranetvat4;
                            break;
                        case 5:
                            decimal tempextranetvat5 = (decimal)extra.ItemNet;
                            vat5 += extra.ItemVatValue;
                            netvat5 += tempextranetvat5;
                            break;
                        default:
                            Console.WriteLine("Default case");
                            break;
                    }
                    if (!vats.Contains(extra.ItemVatDesc))
                        vats.Add(extra.ItemVatDesc);
                    CalculateVatPrice cc = new CalculateVatPrice();
                    cc.VatRate = extra.ItemVatDesc;
                    if (indexExtra < 0)
                    {
                        cc.Total = extra.ItemNet;
                        cc.VatPrice = extra.ItemVatValue;
                        calculateList.Add(cc);
                    }
                    else
                    {
                        cc.Total = calculateList[indexExtra].Total + extra.ItemNet;
                        cc.VatPrice = calculateList[indexExtra].VatPrice + extra.ItemVatValue;
                        calculateList[indexExtra] = cc;
                    }
                }
            }

            model.CalculateVatPrice = calculateList;

            model.TotalVat1 = decimal.Round(vat1, 2, MidpointRounding.ToEven);
            model.TotalVat2 = decimal.Round(vat2, 2, MidpointRounding.ToEven);
            model.TotalVat3 = decimal.Round(vat3, 2, MidpointRounding.ToEven);
            model.TotalVat4 = decimal.Round(vat4, 2, MidpointRounding.ToEven);
            model.TotalVat5 = decimal.Round(vat5, 2, MidpointRounding.ToEven);

            model.TotalNetVat1 = decimal.Round(netvat1, 2, MidpointRounding.ToEven);
            model.TotalNetVat2 = decimal.Round(netvat2, 2, MidpointRounding.ToEven);
            model.TotalNetVat3 = decimal.Round(netvat3, 2, MidpointRounding.ToEven);
            model.TotalNetVat4 = decimal.Round(netvat4, 2, MidpointRounding.ToEven);
            model.TotalNetVat5 = decimal.Round(netvat5, 2, MidpointRounding.ToEven);

            return model;
        }
    }
}
