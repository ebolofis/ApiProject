using Dapper;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT
{
    public class SalesPosLookupsDT : ISalesPosLookupsDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;

        public SalesPosLookupsDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        public enum CustomerPolicyEnum { NoCustomers = 0, HotelInfo = 1, Other = 2, Delivery = 3, PmsInterface = 4 }

        /// <summary>
        /// Get the set of data POS needs:
        /// <para>1. get communication info from every pms </para> 
        /// <para>2. create sp ReservationInfo and ProtelDepartments for every pms </para>
        /// <para>3. get Posinfo and the related data based on client type </para>
        /// <para>4. get salesTypes (Τύποι πώλησης) </para>
        /// <para>5. get pricelists (και τα SalesType) που αφορούν το συγκεκριμένο pos </para>
        /// <para>6. get the active staff for the specific pos  </para>
        /// <para>7. get storeinfoid (not needed, client already got store.) </para>
        /// <para>8. get all active Accounts(Οι δυνατοί τρόποι πληρωμής) except Credit Cards (Type=4) </para>
        /// <para>9. get Credit Cards and assign them with pms's rooms </para>
        /// <para>10. get hotelInfo (Πληροφορίες επικοινωνίας με το pms) </para>
        /// <para>11. get TransferMappings (αντιστοιχίες μεταξύ τμημάτων του PMS και των δευτερευόντων κατηγοριών των προϊόντων (ProductCategory) για αποστολή χρεώσεων στα τμήματα του PMS) </para>
        /// <para>12. get KitchenInstructions (Μηνύματα προς τη κουζίνα) for specific POS.  </para>
        /// <para>13. determine CustomerPolicy based on HotelInfo.Type of the first record in table HotelInfo </para>
        /// <para>14. set hasCustomers=true if customerpolicy is HotelInfo or Other or PmsInterface </para>
        /// <para>15. get delivery's url for searching customer </para>
        /// <para>16. get RegionLockerProduct (το product ‘Locker’ )included Product table, Vats </para>
        /// <para>17. get allowedboardMeals (δικαιωμένα) </para>
        /// <para>18. get Vats </para>
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="ipAddress">ip η οποία χαρακτηρίζει μοναδικά ένα POS σε μία DB. Για Client POS (type=11) έχει δομή ip,clientPosCode  ex: 1.1.1.1,23</param>
        /// <param name="type">τύπος client. 1: POS, 11: client POS, 10: PDA</param>
        /// <returns>Create new anonymous onbect to return the aquired data</returns>
        public SalesPosLookupsModelsPreview GetPosByIp(DBInfoModel Store, string storeid, string ipAddress, int type = 1)
        {
            // get the results
            SalesPosLookupsModelsPreview getPosByIpModel = new SalesPosLookupsModelsPreview();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //storeinfoId
                string storeIdQuery = "SELECT s.Id FROM Store AS s";
                long storeinfoId = db.Query<long>(storeIdQuery).FirstOrDefault();
                getPosByIpModel.storeinfoid = storeinfoId;

                //posInfo
                if (type == 10) // PDA 
                {
                    long posid = Convert.ToInt64(ipAddress);

                    string posInfoIdQuery = "SELECT distinct pi1.*  \n"
                                      + "FROM PosInfo AS pi1  \n"
                                      + "INNER JOIN PosInfoDetail AS pid ON pid.PosInfoId = pi1.Id  \n"
                                      + "LEFT OUTER JOIN PosInfoDetail_Pricelist_Assoc AS pidpa ON pidpa.PosInfoDetailId = pid.Id \n"
                                      + "LEFT OUTER JOIN PosInfoDetail_Excluded_Accounts AS pidea ON pidea.PosInfoDetailId = pidpa.PosInfoDetailId \n"
                                      + "LEFT OUTER JOIN Department AS d ON d.Id = pi1.DepartmentId \n"
                                      + "WHERE pi1.Id =@ID";

                    string posinfodeatailsQuery = "SELECT * FROM PosInfoDetail AS pid WHERE pid.PosInfoId =@pId";

                    string pid_Excluded_AccQuery = "SELECT pidea.*  \n"
                                                 + "FROM PosInfoDetail AS pid  \n"
                                                 + "LEFT OUTER JOIN PosInfoDetail_Excluded_Accounts AS pidea ON pidea.PosInfoDetailId = pid.Id \n"
                                                 + "WHERE pid.id =@ID";

                    string posInfoDetail_Pricelist_AssocQuery = "SELECT * FROM PosInfoDetail_Pricelist_Assoc AS pidpa WHERE pidpa.PosInfoDetailId =@PosInfoDetailId";

                    PosInfoModel posinfoModel = db.Query<PosInfoModel>(posInfoIdQuery, new { ID = posid }).FirstOrDefault();
                    getPosByIpModel.posinfo = posinfoModel;

                    List<PosInfoDetail> posinfodet = db.Query<PosInfoDetail>(posinfodeatailsQuery, new { pId = posinfoModel.Id }).ToList();
                    foreach (PosInfoDetail posDetail in posinfodet)
                    {
                        List<PosInfoDetail_Excluded_AccountsModel> pid_Excluded_AccModel = db.Query<PosInfoDetail_Excluded_AccountsModel>(pid_Excluded_AccQuery, new { ID = posDetail.Id }).ToList();
                        posDetail.PosInfoDetail_Excluded_Accounts = pid_Excluded_AccModel;
                        List<PosInfoDetail_Pricelist_Assoc_Model> posInfoDetail_Pricelist_Assoc_model = db.Query<PosInfoDetail_Pricelist_Assoc_Model>(posInfoDetail_Pricelist_AssocQuery, new { PosInfoDetailId = posDetail.Id }).ToList();
                        posDetail.PosInfoDetail_Pricelist_Assoc = posInfoDetail_Pricelist_Assoc_model;
                    }
                    posinfoModel.PosInfoDetail = posinfodet;
                }
                else if (type == 11) //Client POS
                {
                    var ipandcode = ipAddress.Split(',');  //ex: ipAddress=1.1.1.1,23
                    ipAddress = ipandcode[0];
                    string code = ipandcode[1]; // to clientPosCode που δόθηκε από τον χρήστη κατά το registration

                    string posInfoIdQuery = "SELECT distinct pi1.*  \n"
                                      + "FROM PosInfo AS pi1  \n"
                                      + "INNER JOIN PosInfoDetail AS pid ON pid.PosInfoId = pi1.Id  \n"
                                      + "LEFT OUTER JOIN PosInfoDetail_Pricelist_Assoc AS pidpa ON pidpa.PosInfoDetailId = pid.Id \n"
                                      + "LEFT OUTER JOIN PosInfoDetail_Excluded_Accounts AS pidea ON pidea.PosInfoDetailId = pidpa.PosInfoDetailId \n"
                                      + "LEFT OUTER JOIN Department AS d ON d.Id = pi1.DepartmentId \n"
                                      + "WHERE pi1.Code =@Code";

                    string posinfodeatailsQuery = "SELECT * FROM PosInfoDetail AS pid WHERE pid.PosInfoId =@pId";

                    string pid_Excluded_AccQuery = "SELECT pidea.*  \n"
                                                 + "FROM PosInfoDetail AS pid  \n"
                                                 + "LEFT OUTER JOIN PosInfoDetail_Excluded_Accounts AS pidea ON pidea.PosInfoDetailId = pid.Id \n"
                                                 + "WHERE pid.id =@ID";

                    string posInfoDetail_Pricelist_AssocQuery = "SELECT * FROM PosInfoDetail_Pricelist_Assoc AS pidpa WHERE pidpa.PosInfoDetailId =@PosInfoDetailId";


                    PosInfoModel posinfoModel = db.Query<PosInfoModel>(posInfoIdQuery, new { Code = code }).FirstOrDefault();
                    getPosByIpModel.posinfo = posinfoModel;

                    List<PosInfoDetail> posinfodet = db.Query<PosInfoDetail>(posinfodeatailsQuery, new { pId = posinfoModel.Id }).ToList();
                    foreach (PosInfoDetail posDetail in posinfodet)
                    {
                        List<PosInfoDetail_Excluded_AccountsModel> pid_Excluded_AccModel = db.Query<PosInfoDetail_Excluded_AccountsModel>(pid_Excluded_AccQuery, new { ID = posDetail.Id }).ToList();
                        posDetail.PosInfoDetail_Excluded_Accounts = pid_Excluded_AccModel;
                        List<PosInfoDetail_Pricelist_Assoc_Model> posInfoDetail_Pricelist_Assoc_model = db.Query<PosInfoDetail_Pricelist_Assoc_Model>(posInfoDetail_Pricelist_AssocQuery, new { PosInfoDetailId = posDetail.Id }).ToList();
                        posDetail.PosInfoDetail_Pricelist_Assoc = posInfoDetail_Pricelist_Assoc_model;
                    }
                    posinfoModel.PosInfoDetail = posinfodet;
                }
                else
                {
                    string posInfoIdQuery = "SELECT distinct pi1.*  \n"
                                      + "FROM PosInfo AS pi1  \n"
                                      + "INNER JOIN PosInfoDetail AS pid ON pid.PosInfoId = pi1.Id  \n"
                                      + "LEFT OUTER JOIN PosInfoDetail_Pricelist_Assoc AS pidpa ON pidpa.PosInfoDetailId = pid.Id \n"
                                      + "LEFT OUTER JOIN PosInfoDetail_Excluded_Accounts AS pidea ON pidea.PosInfoDetailId = pidpa.PosInfoDetailId \n"
                                      + "LEFT OUTER JOIN Department AS d ON d.Id = pi1.DepartmentId \n"
                                      + "WHERE pi1.IPAddress =@IPAddress";

                    string posinfodeatailsQuery = "SELECT * FROM PosInfoDetail AS pid WHERE pid.PosInfoId =@pId";

                    string pid_Excluded_AccQuery = "SELECT pidea.*  \n"
                                                 + "FROM PosInfoDetail AS pid  \n"
                                                 + "LEFT OUTER JOIN PosInfoDetail_Excluded_Accounts AS pidea ON pidea.PosInfoDetailId = pid.Id \n"
                                                 + "WHERE pid.id =@ID";

                    string posInfoDetail_Pricelist_AssocQuery = "SELECT * FROM PosInfoDetail_Pricelist_Assoc AS pidpa WHERE pidpa.PosInfoDetailId =@PosInfoDetailId";

                    PosInfoModel posinfoModel = db.Query<PosInfoModel>(posInfoIdQuery, new { IPAddress = ipAddress }).FirstOrDefault();
                    getPosByIpModel.posinfo = posinfoModel;

                    List<PosInfoDetail> posinfodet = db.Query<PosInfoDetail>(posinfodeatailsQuery, new { pId = posinfoModel.Id }).ToList();
                    foreach (PosInfoDetail posDetail in posinfodet)
                    {
                        List<PosInfoDetail_Excluded_AccountsModel> pid_Excluded_AccModel = db.Query<PosInfoDetail_Excluded_AccountsModel>(pid_Excluded_AccQuery, new { ID = posDetail.Id }).ToList();
                        posDetail.PosInfoDetail_Excluded_Accounts = pid_Excluded_AccModel;
                        List<PosInfoDetail_Pricelist_Assoc_Model> posInfoDetail_Pricelist_Assoc_model = db.Query<PosInfoDetail_Pricelist_Assoc_Model>(posInfoDetail_Pricelist_AssocQuery, new { PosInfoDetailId = posDetail.Id }).ToList();
                        posDetail.PosInfoDetail_Pricelist_Assoc = posInfoDetail_Pricelist_Assoc_model;
                    }
                    posinfoModel.PosInfoDetail = posinfodet;
                }
                //Get salesTypes (Τύποι πώλησης)
                string salestypeQuery = "SELECT * FROM SalesType AS st";

                string transferMappingsQuery = "SELECT * FROM TransferMappings AS tm WHERE tm.SalesTypeId =@sId";

                string SalesType_PricelistQuery = "SELECT * FROM SalesType_PricelistMaster_Assoc AS stpma WHERE stpma.SalesTypeId =@sId";

                List<OrderDetailModel> orderDet = new List<OrderDetailModel>();

                List<SalesTypesModels> salestypeModel = db.Query<SalesTypesModels>(salestypeQuery).ToList();
                foreach(SalesTypesModels sTypes in salestypeModel)
                {
                    List<TransferMappings_Model> transferMappingsModel = db.Query<TransferMappings_Model>(transferMappingsQuery, new { sId = sTypes.Id }).ToList();
                    sTypes.TransferMappings = transferMappingsModel;
                    List<SalesType_PricelistMaster_Assoc_Model> salesType_PricelistMaster_Assoc_Model = db.Query<SalesType_PricelistMaster_Assoc_Model>(SalesType_PricelistQuery, new { sId = sTypes.Id }).ToList();
                    sTypes.SalesType_PricelistMaster_Assoc = salesType_PricelistMaster_Assoc_Model;
           
                    sTypes.OrderDetail = orderDet;
                }
                getPosByIpModel.salesTypes = salestypeModel;

                //Vats
                string vatQuery = "SELECT * FROM Vat AS v";

                List<VatModel> vatModel = db.Query<VatModel>(vatQuery).ToList();
                getPosByIpModel.vats = vatModel;

                //pricelists (και τα SalesType) που αφορούν το συγκεκριμένο pos
                if (type == 10)
                {
                    long posid = Convert.ToInt64(ipAddress);

                    string pricelistQuery = "SELECT distinct p.* FROM Pricelist AS p \n"
                                          + "LEFT OUTER JOIN SalesType AS st ON p.SalesTypeId = st.Id \n"
                                          + "LEFT OUTER JOIN PosInfoDetail_Pricelist_Assoc AS pidpa ON pidpa.PricelistId = p.Id \n"
                                          + "LEFT OUTER JOIN PosInfoDetail AS pid ON pidpa.PosInfoDetailId = pid.Id \n"
                                          + "LEFT OUTER JOIN PosInfo AS pi1 ON pi1.Id = pid.PosInfoId \n"
                                          + "WHERE pi1.Id =@ID";

                    List<pricelistModels> pricelistModel = db.Query<pricelistModels>(pricelistQuery, new { ID = posid }).ToList();
                    getPosByIpModel.pricelist = pricelistModel;
                }
                else if(type == 11)
                {
                    var ipandcode = ipAddress.Split(',');  //ex: ipAddress=1.1.1.1,23
                    ipAddress = ipandcode[0];
                    string code = ipandcode[1]; // to clientPosCode που δόθηκε από τον χρήστη κατά το registration

                    string pricelistQuery = "SELECT distinct p.* FROM Pricelist AS p \n"
                                          + "LEFT OUTER JOIN SalesType AS st ON p.SalesTypeId = st.Id \n"
                                          + "LEFT OUTER JOIN PosInfoDetail_Pricelist_Assoc AS pidpa ON pidpa.PricelistId = p.Id \n"
                                          + "LEFT OUTER JOIN PosInfoDetail AS pid ON pidpa.PosInfoDetailId = pid.Id \n"
                                          + "LEFT OUTER JOIN PosInfo AS pi1 ON pi1.Id = pid.PosInfoId \n"
                                          + "WHERE pi1.Code =@Code";

                    List<pricelistModels> pricelistModel = db.Query<pricelistModels>(pricelistQuery, new { Code = code }).ToList();
                    getPosByIpModel.pricelist = pricelistModel;
                }
                else
                {
                    string pricelistQuery = "SELECT distinct p.* FROM Pricelist AS p \n"
                                          + "LEFT OUTER JOIN SalesType AS st ON p.SalesTypeId = st.Id \n"
                                          + "LEFT OUTER JOIN PosInfoDetail_Pricelist_Assoc AS pidpa ON pidpa.PricelistId = p.Id \n"
                                          + "LEFT OUTER JOIN PosInfoDetail AS pid ON pidpa.PosInfoDetailId = pid.Id \n"
                                          + "LEFT OUTER JOIN PosInfo AS pi1 ON pi1.Id = pid.PosInfoId \n"
                                          + "WHERE pi1.IPAddress =@IPAddress";

                    List<pricelistModels> pricelistModel = db.Query<pricelistModels>(pricelistQuery, new { IPAddress = ipAddress }).ToList();
                    getPosByIpModel.pricelist = pricelistModel;
                }
                //get the active staff for the specific pos 
                if (type == 10)
                {
                    long posid = Convert.ToInt64(ipAddress);

                    string staffQuery = "SELECT distinct s.* FROM Staff AS s \n"
                                      + "LEFT OUTER JOIN AssignedPositions AS ap ON ap.StaffId = s.Id \n"
                                      + "LEFT OUTER JOIN PosInfo_StaffPositin_Assoc AS pispa ON pispa.StaffPositionId = ap.StaffPositionId \n"
                                      + "LEFT OUTER JOIN PosInfo AS pi1 ON pispa.PosInfoId = pi1.Id \n"
                                      + "WHERE pi1.Id =@ID AND (s.IsDeleted IS NULL OR s.IsDeleted = 0)";
                    List<StaffModels> staffModel = db.Query<StaffModels>(staffQuery, new { ID = posid }).ToList();
                    getPosByIpModel.staff = staffModel;
                }
                else if(type == 11)
                {
                    var ipandcode = ipAddress.Split(',');  //ex: ipAddress=1.1.1.1,23
                    ipAddress = ipandcode[0];
                    string code = ipandcode[1]; // to clientPosCode που δόθηκε από τον χρήστη κατά το registration

                    string staffQuery = "SELECT distinct s.* FROM Staff AS s \n"
                                      + "LEFT OUTER JOIN AssignedPositions AS ap ON ap.StaffId = s.Id \n"
                                      + "LEFT OUTER JOIN PosInfo_StaffPositin_Assoc AS pispa ON pispa.StaffPositionId = ap.StaffPositionId \n"
                                      + "LEFT OUTER JOIN PosInfo AS pi1 ON pispa.PosInfoId = pi1.Id \n"
                                      + "WHERE pi1.Code =@Code AND (s.IsDeleted IS NULL OR s.IsDeleted = 0)";
                    List<StaffModels> staffModel = db.Query<StaffModels>(staffQuery, new { Code = code }).ToList();
                    getPosByIpModel.staff = staffModel;
                }
                else
                {
                    string staffQuery = "SELECT distinct s.* FROM Staff AS s \n"
                                      + "LEFT OUTER JOIN AssignedPositions AS ap ON ap.StaffId = s.Id \n"
                                      + "LEFT OUTER JOIN PosInfo_StaffPositin_Assoc AS pispa ON pispa.StaffPositionId = ap.StaffPositionId \n"
                                      + "LEFT OUTER JOIN PosInfo AS pi1 ON pispa.PosInfoId = pi1.Id \n"
                                      + "WHERE pi1.IPAddress =@IPAddress AND (s.IsDeleted IS NULL OR s.IsDeleted = 0)";
                    List<StaffModels> staffModel = db.Query<StaffModels>(staffQuery, new { IPAddress = ipAddress }).ToList();
                    getPosByIpModel.staff = staffModel;
                }
                //get all active Accounts(Οι δυνατοί τρόποι πληρωμής) except Credit Cards(Type = 4)
                string accountQuery = "SELECT * FROM Accounts AS a WHERE (a.IsDeleted IS NULL OR a.IsDeleted = 0) AND a.[Type] != 4";
                List<AccountModel> accountModel = db.Query<AccountModel>(accountQuery).ToList();
                //getPosByIpModel.Accounts = accountModel;

                string PID_Excluded_AccountsQuery = "SELECT pidea.*  \n"
                                                  + "FROM Accounts AS a \n"
                                                  + "LEFT OUTER JOIN PosInfoDetail_Excluded_Accounts AS pidea ON pidea.AccountId = a.Id  \n"
                                                  + "WHERE (a.IsDeleted IS NULL OR a.IsDeleted = 0) AND a.[Type] != 4 AND a.Id =@accountId";
                foreach (AccountModel ac in accountModel)
                {
                    List<PosInfoDetail_Excluded_AccountsModel> Pid_Excluded_Acc = db.Query<PosInfoDetail_Excluded_AccountsModel>(PID_Excluded_AccountsQuery, new { accountId = ac.Id }).ToList();
                    ac.PosInfoDetail_Excluded_Accounts = Pid_Excluded_Acc;
                }
                getPosByIpModel.Accounts = accountModel;

                //set hasCustomers=true if customerpolicy is HotelInfo or Other or PmsInterface
                string hQuery = "SELECT * FROM HotelInfo AS hi";
                List<availableHotelsModels> avHotels = db.Query<availableHotelsModels>(hQuery).ToList();

                if (avHotels.Count > 0)
                {
                    getPosByIpModel.hasCustomers = true;
                }
                else
                {
                    getPosByIpModel.hasCustomers = false;
                }
                //determine CustomerPolicy based on HotelInfo.Type of the first record in table HotelInfo
                int hType = 0;
                List<availableHotelsModels> hotelsInfo = db.Query<availableHotelsModels>("SELECT * FROM HotelInfo AS hi").ToList();
                if (hotelsInfo.Count > 0)
                {
                    hType = db.Query<int>("SELECT HI.[Type] FROM HotelInfo AS hi").FirstOrDefault();
                }
                byte CustomerPolicy = 0;
                switch (hType)
                {
                    case 0:
                    case 10:
                        CustomerPolicy = (byte)CustomerPolicyEnum.HotelInfo;
                        getPosByIpModel.CustomerPolicy = CustomerPolicy;
                        break;
                    case 2:
                        CustomerPolicy = (byte)CustomerPolicyEnum.Other;
                        getPosByIpModel.CustomerPolicy = CustomerPolicy;
                        break;
                    case 3:
                        CustomerPolicy = (byte)CustomerPolicyEnum.Delivery;
                        getPosByIpModel.CustomerPolicy = CustomerPolicy;
                        break;
                    case 4:
                        CustomerPolicy = (byte)CustomerPolicyEnum.PmsInterface;
                        getPosByIpModel.CustomerPolicy = CustomerPolicy;

                        break;
                    default:
                        CustomerPolicy = (byte)CustomerPolicyEnum.NoCustomers;
                        getPosByIpModel.CustomerPolicy = CustomerPolicy;
                        break;
                };

                //CustomerServiceProviderUrl (not used)

                getPosByIpModel.CustomerServiceProviderUrl = "";

                //RedirectToCustomerCard (get delivery's url for searching customer) 

                getPosByIpModel.RedirectToCustomerCard = "";

                //get Credit Cards and assign them with pms's rooms
                string accountsQuery = "SELECT distinct a.* \n"
                                        + "FROM Accounts AS a \n"
                                        + "LEFT OUTER JOIN EODAccountToPmsTransfer AS etpt ON etpt.AccountId = a.Id \n"
                                        + "LEFT OUTER JOIN PosInfo AS pi1 ON etpt.PosInfoId = pi1.Id \n"
                                        + "WHERE a.[Type] = 4 AND etpt.PosInfoId = pi1.Id";

                string roomsQuery = "SELECT distinct etpt.PmsRoom FROM EODAccountToPmsTransfer AS etpt \n"
                                  + "WHERE etpt.AccountId =@accountId \n";
                List<CreditCardsModels> credCardsModel = new List<CreditCardsModels>();
                List<AccountModel> accountsModel = db.Query<AccountModel>(accountsQuery).ToList();
                foreach (AccountModel acc in accountsModel)
                {
                    long room = db.Query<long>(roomsQuery, new { accountId = acc.Id }).FirstOrDefault();
                    CreditCardsModels n = new CreditCardsModels();
                    n.Account = acc; n.Room = room;
                    credCardsModel.Add(n);
                }
                getPosByIpModel.CreditCards = credCardsModel;

                //get KitchenInstructions (Μηνύματα προς τη κουζίνα) for specific POS. 
                if (type == 10)
                {
                    long posid = Convert.ToInt64(ipAddress);

                    string kitInstructionsQuery = "SELECT ki.* \n"
                                               + "FROM KitchenInstruction AS ki \n"
                                               + "LEFT OUTER JOIN PosInfo_KitchenInstruction_Assoc AS pikia ON pikia.KitchenInstructionId = ki.Id \n"
                                               + "LEFT OUTER JOIN PosInfo AS pi1 ON pikia.PosInfoId = pi1.Id \n"
                                               + "WHERE pikia.PosInfoId = pi1.Id AND pi1.Id =@ID";

                    List<KitchenInstructionsModels> kitInstructionModel = db.Query<KitchenInstructionsModels>(kitInstructionsQuery, new { ID = posid }).ToList();
                    getPosByIpModel.KitchenInstructions = kitInstructionModel;
                }
                else if (type == 11)
                {
                    var ipandcode = ipAddress.Split(',');  //ex: ipAddress=1.1.1.1,23
                    ipAddress = ipandcode[0];
                    string code = ipandcode[1]; // to clientPosCode που δόθηκε από τον χρήστη κατά το registration

                    string kitInstructionsQuery = "SELECT ki.* \n"
                                                + "FROM KitchenInstruction AS ki \n"
                                                + "LEFT OUTER JOIN PosInfo_KitchenInstruction_Assoc AS pikia ON pikia.KitchenInstructionId = ki.Id \n"
                                                + "LEFT OUTER JOIN PosInfo AS pi1 ON pikia.PosInfoId = pi1.Id \n"
                                                + "WHERE pikia.PosInfoId = pi1.Id AND pi1.Code =@Code";

                    List<KitchenInstructionsModels> kitInstructionModel = db.Query<KitchenInstructionsModels>(kitInstructionsQuery, new { Code = code }).ToList();
                    getPosByIpModel.KitchenInstructions = kitInstructionModel;
                }
                else
                {
                    string kitInstructionsQuery = "SELECT ki.* \n"
                                                + "FROM KitchenInstruction AS ki \n"
                                                + "LEFT OUTER JOIN PosInfo_KitchenInstruction_Assoc AS pikia ON pikia.KitchenInstructionId = ki.Id \n"
                                                + "LEFT OUTER JOIN PosInfo AS pi1 ON pikia.PosInfoId = pi1.Id \n"
                                                + "WHERE pikia.PosInfoId = pi1.Id AND pi1.IPAddress =@IPAddress";

                    List<KitchenInstructionsModels> kitInstructionModel = db.Query<KitchenInstructionsModels>(kitInstructionsQuery, new { IPAddress = ipAddress }).ToList();
                    getPosByIpModel.KitchenInstructions = kitInstructionModel;
                }
                //get KitchenRegions
                string kitchenRegionQuery = "SELECT * FROM KitchenRegion AS kr";

                List<ItemRegionModels> itemRegModel = db.Query<ItemRegionModels>(kitchenRegionQuery).ToList();
                getPosByIpModel.ItemRegions = itemRegModel;

                //get InvoiceTypes (Τύποι παραστατικών)
                string InvoiceTypesQuery = "SELECT * FROM InvoiceTypes AS it";

                List<InvoiceTypeModel> InvoiceTypesModel = db.Query<InvoiceTypeModel>(InvoiceTypesQuery).ToList();
                getPosByIpModel.InvoiceTypes = InvoiceTypesModel;

                //get RegionLockerProduct (το product ‘Locker’ )included Product table, Vats
                if (type == 10)
                {
                    long posid = Convert.ToInt64(ipAddress);

                    string posinfoIdQuery = "SELECT pi1.Id FROM PosInfo AS pi1 WHERE pi1.Id =@ID";
                    string lockerProductsQuery = "SELECT rlp.PosInfoId, rlp.Discount, rlp.ReturnPaymentpId, rlp.PaymentId, rlp.SaleId \n"
                                               + "FROM RegionLockerProduct AS rlp \n"
                                               + "WHERE rlp.PosInfoId =@posInfoId";
                    string pageButtonQuery = "SELECT rlp.ProductId, rlp.SalesDescription AS [DESCRIPTION], 0 AS PreparationTime, 0 AS Sort,  \n"
                                           + "rlp.PriceListId AS SetDefaultPriceListId,	10 AS [TYPE], p.Code   \n"
                                           + "FROM RegionLockerProduct AS rlp \n"
                                           + "LEFT OUTER JOIN Product AS p ON rlp.ProductId = p.Id \n"
                                           + "WHERE rlp.PosInfoId =@posInfoId";
                    string priceListQuery = "SELECT distinct pd.Id, pd.VatId, pd.TaxId, pd.Price, rlp.PosInfoId \n"
                                          + "FROM RegionLockerProduct AS rlp \n"
                                          + "LEFT OUTER JOIN PricelistDetail AS pd ON pd.PricelistId = rlp.PriceListId \n"
                                          + "LEFT OUTER JOIN Product AS p ON rlp.ProductId = p.Id \n"
                                          + "WHERE rlp.PosInfoId =@posInfoId AND pd.ProductId =@productId";
                    string vatsQuery = "SELECT v.Id, v.[Description], v.Percentage, v.Code \n"
                                    + "FROM RegionLockerProduct AS rlp \n"
                                    + "LEFT OUTER JOIN PricelistDetail AS pd ON pd.PricelistId = rlp.PriceListId \n"
                                    + "LEFT OUTER JOIN Product AS p ON rlp.ProductId = p.Id \n"
                                    + "LEFT OUTER JOIN Vat AS v ON pd.VatId = v.Id \n"
                                    + "WHERE rlp.PosInfoId =@posInfoId AND pd.ProductId =@productId AND pd.VatId =@vatId";

                    long posInfId = db.Query<long>(posinfoIdQuery, new { ID = posid }).FirstOrDefault();
                    List<lockerProductsModels> productsLockerModel = db.Query<lockerProductsModels>(lockerProductsQuery, new { posInfoId = posInfId }).ToList();
                    foreach (lockerProductsModels pLocker in productsLockerModel)
                    {
                        List<PageButtonModel> pButtonModel = db.Query<PageButtonModel>(pageButtonQuery, new { posInfoId = posInfId }).ToList();
                        pLocker.PageButton = pButtonModel;
                        foreach (PageButtonModel pd in pButtonModel)
                        {
                            List<PricelistDetailsModel> priceListModel = db.Query<PricelistDetailsModel>(priceListQuery, new { posInfoId = posInfId, productId = pd.ProductId }).ToList();
                            pd.PricelistDetails = priceListModel;
                            foreach (PricelistDetailsModel vat in priceListModel)
                            {
                                VatDetailModel vatsModel = db.Query<VatDetailModel>(vatsQuery, new { posInfoId = posInfId, productId = pd.ProductId, vatId = vat.VatId }).FirstOrDefault();
                                vat.Vat = vatsModel;
                            }
                        }
                    }
                    getPosByIpModel.lockerProducts = productsLockerModel;
                }
                else if (type == 11)
                {
                    var ipandcode = ipAddress.Split(',');  //ex: ipAddress=1.1.1.1,23
                    ipAddress = ipandcode[0];
                    string code = ipandcode[1]; // to clientPosCode που δόθηκε από τον χρήστη κατά το registration

                    string posinfoIdQuery = "SELECT pi1.Id FROM PosInfo AS pi1 WHERE pi1.Code =@Code";
                    string lockerProductsQuery = "SELECT rlp.PosInfoId, rlp.Discount, rlp.ReturnPaymentpId, rlp.PaymentId, rlp.SaleId \n"
                                               + "FROM RegionLockerProduct AS rlp \n"
                                               + "WHERE rlp.PosInfoId =@posInfoId";
                    string pageButtonQuery = "SELECT rlp.ProductId, rlp.SalesDescription AS [DESCRIPTION], 0 AS PreparationTime, 0 AS Sort,  \n"
                                           + "rlp.PriceListId AS SetDefaultPriceListId,	10 AS [TYPE], p.Code   \n"
                                           + "FROM RegionLockerProduct AS rlp \n"
                                           + "LEFT OUTER JOIN Product AS p ON rlp.ProductId = p.Id \n"
                                           + "WHERE rlp.PosInfoId =@posInfoId";
                    string priceListQuery = "SELECT distinct pd.Id, pd.VatId, pd.TaxId, pd.Price, rlp.PosInfoId \n"
                                          + "FROM RegionLockerProduct AS rlp \n"
                                          + "LEFT OUTER JOIN PricelistDetail AS pd ON pd.PricelistId = rlp.PriceListId \n"
                                          + "LEFT OUTER JOIN Product AS p ON rlp.ProductId = p.Id \n"
                                          + "WHERE rlp.PosInfoId =@posInfoId AND pd.ProductId =@productId";
                    string vatsQuery = "SELECT v.Id, v.[Description], v.Percentage, v.Code \n"
                                    + "FROM RegionLockerProduct AS rlp \n"
                                    + "LEFT OUTER JOIN PricelistDetail AS pd ON pd.PricelistId = rlp.PriceListId \n"
                                    + "LEFT OUTER JOIN Product AS p ON rlp.ProductId = p.Id \n"
                                    + "LEFT OUTER JOIN Vat AS v ON pd.VatId = v.Id \n"
                                    + "WHERE rlp.PosInfoId =@posInfoId AND pd.ProductId =@productId AND pd.VatId =@vatId";

                    long posInfId = db.Query<long>(posinfoIdQuery, new { Code = code }).FirstOrDefault();
                    List<lockerProductsModels> productsLockerModel = db.Query<lockerProductsModels>(lockerProductsQuery, new { posInfoId = posInfId }).ToList();
                    foreach (lockerProductsModels pLocker in productsLockerModel)
                    {
                        List<PageButtonModel> pButtonModel = db.Query<PageButtonModel>(pageButtonQuery, new { posInfoId = posInfId }).ToList();
                        pLocker.PageButton = pButtonModel;
                        foreach (PageButtonModel pd in pButtonModel)
                        {
                            List<PricelistDetailsModel> priceListModel = db.Query<PricelistDetailsModel>(priceListQuery, new { posInfoId = posInfId, productId = pd.ProductId }).ToList();
                            pd.PricelistDetails = priceListModel;
                            foreach (PricelistDetailsModel vat in priceListModel)
                            {
                                VatDetailModel vatsModel = db.Query<VatDetailModel>(vatsQuery, new { posInfoId = posInfId, productId = pd.ProductId, vatId = vat.VatId }).FirstOrDefault();
                                vat.Vat = vatsModel;
                            }
                        }
                    }
                    getPosByIpModel.lockerProducts = productsLockerModel;
                }
                else
                {
                    string posinfoIdQuery = "SELECT pi1.Id FROM PosInfo AS pi1 WHERE pi1.IPAddress =@IPAddress";
                    string lockerProductsQuery = "SELECT rlp.PosInfoId, rlp.Discount, rlp.ReturnPaymentpId, rlp.PaymentId, rlp.SaleId \n"
                                               + "FROM RegionLockerProduct AS rlp \n"
                                               + "WHERE rlp.PosInfoId =@posInfoId";
                    string pageButtonQuery = "SELECT rlp.ProductId, rlp.SalesDescription AS [DESCRIPTION], 0 AS PreparationTime, 0 AS Sort,  \n"
                                           + "rlp.PriceListId AS SetDefaultPriceListId,	10 AS [TYPE], p.Code   \n"
                                           + "FROM RegionLockerProduct AS rlp \n"
                                           + "LEFT OUTER JOIN Product AS p ON rlp.ProductId = p.Id \n"
                                           + "WHERE rlp.PosInfoId =@posInfoId";
                    string priceListQuery = "SELECT distinct pd.Id, pd.VatId, pd.TaxId, pd.Price, rlp.PosInfoId \n"
                                          + "FROM RegionLockerProduct AS rlp \n"
                                          + "LEFT OUTER JOIN PricelistDetail AS pd ON pd.PricelistId = rlp.PriceListId \n"
                                          + "LEFT OUTER JOIN Product AS p ON rlp.ProductId = p.Id \n"
                                          + "WHERE rlp.PosInfoId =@posInfoId AND pd.ProductId =@productId";
                    string vatsQuery = "SELECT v.Id, v.[Description], v.Percentage, v.Code \n"
                                    + "FROM RegionLockerProduct AS rlp \n"
                                    + "LEFT OUTER JOIN PricelistDetail AS pd ON pd.PricelistId = rlp.PriceListId \n"
                                    + "LEFT OUTER JOIN Product AS p ON rlp.ProductId = p.Id \n"
                                    + "LEFT OUTER JOIN Vat AS v ON pd.VatId = v.Id \n"
                                    + "WHERE rlp.PosInfoId =@posInfoId AND pd.ProductId =@productId AND pd.VatId =@vatId";

                    long posInfId = db.Query<long>(posinfoIdQuery, new { IPAddress = ipAddress }).FirstOrDefault();
                    List<lockerProductsModels> productsLockerModel = db.Query<lockerProductsModels>(lockerProductsQuery, new { posInfoId = posInfId }).ToList();
                    foreach (lockerProductsModels pLocker in productsLockerModel)
                    {
                        List<PageButtonModel> pButtonModel = db.Query<PageButtonModel>(pageButtonQuery, new { posInfoId = posInfId }).ToList();
                        pLocker.PageButton = pButtonModel;
                        foreach (PageButtonModel pd in pButtonModel)
                        {
                            List<PricelistDetailsModel> priceListModel = db.Query<PricelistDetailsModel>(priceListQuery, new { posInfoId = posInfId, productId = pd.ProductId }).ToList();
                            pd.PricelistDetails = priceListModel;
                            foreach (PricelistDetailsModel vat in priceListModel)
                            {
                                VatDetailModel vatsModel = db.Query<VatDetailModel>(vatsQuery, new { posInfoId = posInfId, productId = pd.ProductId, vatId = vat.VatId }).FirstOrDefault();
                                vat.Vat = vatsModel;
                            }
                        }
                    }
                    getPosByIpModel.lockerProducts = productsLockerModel;
                }
                //get allowedboardMeals (δικαιωμένα)
                string AllowedMealsPerBoardQuery = "SELECT * FROM AllowedMealsPerBoard AS ampb";

                string AllowedMealsPerBoardDetailsQuery = "SELECT * FROM AllowedMealsPerBoardDetails AS ampbd WHERE ampbd.AllowedMealsPerBoardId =@allowedMealsPerBoardId";


                List<allowedboardMealsModels> allowMealsPerBoardModel = db.Query<allowedboardMealsModels>(AllowedMealsPerBoardQuery).ToList();

                foreach (allowedboardMealsModels item in allowMealsPerBoardModel)
                {
                    List<AllowedMealsPerBoardDetailsModel> details = db.Query<AllowedMealsPerBoardDetailsModel>(AllowedMealsPerBoardDetailsQuery, new { allowedMealsPerBoardId = item.Id }).ToList();
                    item.AllowedMealsPerBoardDetails = details;
                }
                getPosByIpModel.allowedboardMeals = allowMealsPerBoardModel;

                //get TransferMappings(αντιστοιχίες μεταξύ τμημάτων του PMS και των δευτερευόντων κατηγοριών των προϊόντων (ProductCategory) για αποστολή χρεώσεων στα τμήματα του PMS)
                if (type == 10)
                {
                    long posid = Convert.ToInt64(ipAddress);

                    string hotelQuery = "SELECT * FROM HotelInfo AS hi";
                    string posQuery = "SELECT pi1.DefaultHotelId FROM PosInfo AS pi1 WHERE pi1.Id =@ID";
                    string hotelTypeQuery = "SELECT hi.[Type] FROM HotelInfo AS hi";
                    string departmentIdQuery = "SELECT pi1.DepartmentId FROM PosInfo AS pi1 WHERE pi1.Id =@ID";
                    string TransferMappingsQuery = "SELECT distinct tm.HotelId, tm.ProductCategoryId  \n"
                                                 + "FROM TransferMappings AS tm  \n"
                                                 + "WHERE tm.ProductCategoryId IS NOT NULL AND tm.PosDepartmentId =@posDepartmentId \n"
                                                 + "ORDER BY tm.HotelId, tm.ProductCategoryId";
                    string priceListIdQuery = "SELECT DISTINCT tm.PriceListId \n"
                                            + "FROM TransferMappings AS tm \n"
                                            + "WHERE tm.HotelId =@hotelId AND tm.ProductCategoryId =@productCategoryId AND tm.ProductCategoryId IS NOT NULL \n"
                                            + "AND tm.PosDepartmentId =@posDepartmentId \n";

                    List<availableHotelsModels> hotels = db.Query<availableHotelsModels>(hotelQuery).ToList();
                    int? defaultHId = db.Query<int?>(posQuery, new { ID = posid }).FirstOrDefault();
                    int hotelType = db.Query<int>(hotelTypeQuery).FirstOrDefault();
                    long departmentId = db.Query<int>(departmentIdQuery, new { ID = posid }).FirstOrDefault();

                    int? defaultHotelId = 1;
                    if (hotels.Count() > 0 && defaultHId != null)
                    {
                        defaultHotelId = defaultHId;
                    }

                    if (hotels.Count() > 0 && hotelType == 4)
                    {
                        List<transferMappingsModels> transfMappingModel = db.Query<transferMappingsModels>(TransferMappingsQuery, new { posDepartmentId = departmentId }).ToList();
                        foreach (transferMappingsModels transfer in transfMappingModel)
                        {
                            List<long> priceLists = db.Query<long>(priceListIdQuery, new { hotelId = transfer.HotelId, productCategoryId = transfer.ProductCategoryId, posDepartmentId = departmentId }).ToList();
                            transfer.Pricelists = priceLists;
                        }
                        getPosByIpModel.transferMappings = transfMappingModel;
                    }
                    else
                    {
                        //  for type <> 4 (one hotel or no hotel at all),  λαμβάνεται υπόψη η εγγραφή του πίνακα HotelInfo που θεωρήται ως default
                        //  every returned object contains:  ProductCategoryId, list of Pricelists. Objects are grouped by  ProductCategoryId
                        string TransferMappingsQuery2 = "SELECT distinct tm.HotelId, tm.ProductCategoryId  \n"
                                                + "FROM TransferMappings AS tm  \n"
                                                + "WHERE tm.ProductCategoryId IS NOT NULL AND tm.PosDepartmentId =@posDepartmentId AND tm.HotelId=@DefaultHotelId \n"
                                                + "ORDER BY tm.HotelId, tm.ProductCategoryId";
                        string priceListIdQuery2 = "SELECT DISTINCT tm.PriceListId \n"
                                                + "FROM TransferMappings AS tm \n"
                                                + "WHERE tm.HotelId =@hotelId AND tm.ProductCategoryId =@productCategoryId AND tm.ProductCategoryId IS NOT NULL \n"
                                                + "AND tm.PosDepartmentId =@posDepartmentId AND tm.HotelId=@DefaultHotelId\n";
                        List<transferMappingsModels> transfMappingModel = db.Query<transferMappingsModels>(TransferMappingsQuery2, new { posDepartmentId = departmentId, DefaultHotelId = defaultHotelId }).ToList();
                        foreach (transferMappingsModels transfer in transfMappingModel)
                        {
                            List<long> priceLists = db.Query<long>(priceListIdQuery2, new { hotelId = transfer.HotelId, productCategoryId = transfer.ProductCategoryId, posDepartmentId = departmentId, DefaultHotelId = defaultHotelId }).ToList();
                            transfer.Pricelists = priceLists;
                        }
                        getPosByIpModel.transferMappings = transfMappingModel;
                    }
                }
                else if (type == 11)
                {
                    var ipandcode = ipAddress.Split(',');  //ex: ipAddress=1.1.1.1,23
                    ipAddress = ipandcode[0];
                    string code = ipandcode[1]; // to clientPosCode που δόθηκε από τον χρήστη κατά το registration

                    string hotelQuery = "SELECT * FROM HotelInfo AS hi";
                    string posQuery = "SELECT pi1.DefaultHotelId FROM PosInfo AS pi1 WHERE pi1.Code =@Code";
                    string hotelTypeQuery = "SELECT hi.[Type] FROM HotelInfo AS hi";
                    string departmentIdQuery = "SELECT pi1.DepartmentId FROM PosInfo AS pi1 WHERE pi1.Code =@Code";
                    string TransferMappingsQuery = "SELECT distinct tm.HotelId, tm.ProductCategoryId  \n"
                                                 + "FROM TransferMappings AS tm  \n"
                                                 + "WHERE tm.ProductCategoryId IS NOT NULL AND tm.PosDepartmentId =@posDepartmentId \n"
                                                 + "ORDER BY tm.HotelId, tm.ProductCategoryId";
                    string priceListIdQuery = "SELECT DISTINCT tm.PriceListId \n"
                                            + "FROM TransferMappings AS tm \n"
                                            + "WHERE tm.HotelId =@hotelId AND tm.ProductCategoryId =@productCategoryId AND tm.ProductCategoryId IS NOT NULL \n"
                                            + "AND tm.PosDepartmentId =@posDepartmentId \n";

                    List<availableHotelsModels> hotels = db.Query<availableHotelsModels>(hotelQuery).ToList();
                    int? defaultHId = db.Query<int?>(posQuery, new { Code = code }).FirstOrDefault();
                    int hotelType = db.Query<int>(hotelTypeQuery).FirstOrDefault();
                    long departmentId = db.Query<int>(departmentIdQuery, new { Code = code }).FirstOrDefault();

                    int? defaultHotelId = 1;
                    if (hotels.Count() > 0 && defaultHId != null)
                    {
                        defaultHotelId = defaultHId;
                    }

                    if (hotels.Count() > 0 && hotelType == 4)
                    {
                        List<transferMappingsModels> transfMappingModel = db.Query<transferMappingsModels>(TransferMappingsQuery, new { posDepartmentId = departmentId }).ToList();
                        foreach (transferMappingsModels transfer in transfMappingModel)
                        {
                            List<long> priceLists = db.Query<long>(priceListIdQuery, new { hotelId = transfer.HotelId, productCategoryId = transfer.ProductCategoryId, posDepartmentId = departmentId }).ToList();
                            transfer.Pricelists = priceLists;
                        }
                        getPosByIpModel.transferMappings = transfMappingModel;
                    }
                    else
                    {
                        //  for type <> 4 (one hotel or no hotel at all),  λαμβάνεται υπόψη η εγγραφή του πίνακα HotelInfo που θεωρήται ως default
                        //  every returned object contains:  ProductCategoryId, list of Pricelists. Objects are grouped by  ProductCategoryId
                        string TransferMappingsQuery2 = "SELECT distinct tm.HotelId, tm.ProductCategoryId  \n"
                                                + "FROM TransferMappings AS tm  \n"
                                                + "WHERE tm.ProductCategoryId IS NOT NULL AND tm.PosDepartmentId =@posDepartmentId AND tm.HotelId=@DefaultHotelId \n"
                                                + "ORDER BY tm.HotelId, tm.ProductCategoryId";
                        string priceListIdQuery2 = "SELECT DISTINCT tm.PriceListId \n"
                                                + "FROM TransferMappings AS tm \n"
                                                + "WHERE tm.HotelId =@hotelId AND tm.ProductCategoryId =@productCategoryId AND tm.ProductCategoryId IS NOT NULL \n"
                                                + "AND tm.PosDepartmentId =@posDepartmentId AND tm.HotelId=@DefaultHotelId\n";
                        List<transferMappingsModels> transfMappingModel = db.Query<transferMappingsModels>(TransferMappingsQuery2, new { posDepartmentId = departmentId, DefaultHotelId = defaultHotelId }).ToList();
                        foreach (transferMappingsModels transfer in transfMappingModel)
                        {
                            List<long> priceLists = db.Query<long>(priceListIdQuery2, new { hotelId = transfer.HotelId, productCategoryId = transfer.ProductCategoryId, posDepartmentId = departmentId, DefaultHotelId = defaultHotelId }).ToList();
                            transfer.Pricelists = priceLists;
                        }
                        getPosByIpModel.transferMappings = transfMappingModel;
                    }

                }
                else
                {
                    string hotelQuery = "SELECT * FROM HotelInfo AS hi";
                    string posQuery = "SELECT pi1.DefaultHotelId FROM PosInfo AS pi1 WHERE pi1.IPAddress =@IPAddress";
                    string hotelTypeQuery = "SELECT hi.[Type] FROM HotelInfo AS hi";
                    string departmentIdQuery = "SELECT pi1.DepartmentId FROM PosInfo AS pi1 WHERE pi1.IPAddress =@IPAddress";
                    string TransferMappingsQuery = "SELECT distinct tm.HotelId, tm.ProductCategoryId  \n"
                                                 + "FROM TransferMappings AS tm  \n"
                                                 + "WHERE tm.ProductCategoryId IS NOT NULL AND tm.PosDepartmentId =@posDepartmentId \n"
                                                 + "ORDER BY tm.HotelId, tm.ProductCategoryId";
                    string priceListIdQuery = "SELECT DISTINCT tm.PriceListId \n"
                                            + "FROM TransferMappings AS tm \n"
                                            + "WHERE tm.HotelId =@hotelId AND tm.ProductCategoryId =@productCategoryId AND tm.ProductCategoryId IS NOT NULL \n"
                                            + "AND tm.PosDepartmentId =@posDepartmentId \n";

                    List<availableHotelsModels> hotels = db.Query<availableHotelsModels>(hotelQuery).ToList();
                    int? defaultHId = db.Query<int?>(posQuery, new { IPAddress = ipAddress }).FirstOrDefault();
                    int hotelType = db.Query<int>(hotelTypeQuery).FirstOrDefault();
                    long departmentId = db.Query<int>(departmentIdQuery, new { IPAddress = ipAddress }).FirstOrDefault();

                    int? defaultHotelId = 1;
                    if (hotels.Count() > 0 && defaultHId != null)
                    {
                        defaultHotelId = defaultHId;
                    }

                    if (hotels.Count() > 0 && hotelType == 4)
                    {
                        List<transferMappingsModels> transfMappingModel = db.Query<transferMappingsModels>(TransferMappingsQuery, new { posDepartmentId = departmentId }).ToList();
                        foreach (transferMappingsModels transfer in transfMappingModel)
                        {
                            List<long> priceLists = db.Query<long>(priceListIdQuery, new { hotelId = transfer.HotelId, productCategoryId = transfer.ProductCategoryId, posDepartmentId = departmentId }).ToList();
                            transfer.Pricelists = priceLists;
                        }
                        getPosByIpModel.transferMappings = transfMappingModel;
                    }
                    else
                    {
                        //  for type <> 4 (one hotel or no hotel at all),  λαμβάνεται υπόψη η εγγραφή του πίνακα HotelInfo που θεωρήται ως default
                        //  every returned object contains:  ProductCategoryId, list of Pricelists. Objects are grouped by  ProductCategoryId
                        string TransferMappingsQuery2 = "SELECT distinct tm.HotelId, tm.ProductCategoryId  \n"
                                                + "FROM TransferMappings AS tm  \n"
                                                + "WHERE tm.ProductCategoryId IS NOT NULL AND tm.PosDepartmentId =@posDepartmentId AND tm.HotelId=@DefaultHotelId \n"
                                                + "ORDER BY tm.HotelId, tm.ProductCategoryId";
                        string priceListIdQuery2 = "SELECT DISTINCT tm.PriceListId \n"
                                                + "FROM TransferMappings AS tm \n"
                                                + "WHERE tm.HotelId =@hotelId AND tm.ProductCategoryId =@productCategoryId AND tm.ProductCategoryId IS NOT NULL \n"
                                                + "AND tm.PosDepartmentId =@posDepartmentId AND tm.HotelId=@DefaultHotelId\n";
                        List<transferMappingsModels> transfMappingModel = db.Query<transferMappingsModels>(TransferMappingsQuery2, new { posDepartmentId = departmentId, DefaultHotelId = defaultHotelId }).ToList();
                        foreach (transferMappingsModels transfer in transfMappingModel)
                        {
                            List<long> priceLists = db.Query<long>(priceListIdQuery2, new { hotelId = transfer.HotelId, productCategoryId = transfer.ProductCategoryId, posDepartmentId = departmentId, DefaultHotelId = defaultHotelId }).ToList();
                            transfer.Pricelists = priceLists;
                        }
                        getPosByIpModel.transferMappings = transfMappingModel;
                    }
                }
                //get hotelInfo(Πληροφορίες επικοινωνίας με το pms)
                string hotelInfoQuery = "SELECT hi.HotelId, hi.HotelName AS [Description], hi.[Type], hi.MPEHotel FROM HotelInfo AS hi";

                List<availableHotelsModels> hotelInfoModel = db.Query<availableHotelsModels>(hotelInfoQuery).ToList();
                getPosByIpModel.availableHotels = hotelInfoModel;
            }

            return getPosByIpModel;
        }
    }
}
