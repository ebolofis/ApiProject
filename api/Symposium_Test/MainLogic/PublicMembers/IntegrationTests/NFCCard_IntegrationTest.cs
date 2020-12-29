using NUnit.Framework;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium_DTOs.PosModel_Info;
using Symposium_Test.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Symposium_Test.MainLogic.PublicMembers.IntegrationTests
{
    [TestFixture]
    class NFCCard_IntegrationTest : IoCSupportedTest<TestModule>
    {
        DBInfoModel store;
        string connectionString;
        INFCcardFlows nfcCard;
        IUsersToDatabasesXML usersToDatabases;
        IGenericITableDAO<AccountDTO> accountTableDao;
        IGenericITableDAO<DepartmentDTO> departmentTableDao;
        IGenericITableDAO<PosInfoDTO> posinfoTableDao;
        IGenericITableDAO<InvoiceTypesDTO> invoicetypesTableDao;
        IGenericITableDAO<PosInfoDetailDTO> posinfodetailsTableDao;
        IGenericITableDAO<StaffDTO> staffTableDao;
        IGenericITableDAO<EndOfDayDTO> endOFDayDao;
        IGenericITableDAO<NFCcardDTO> nfcCardDao;
        IGenericDAO<NFCcardDTO> nfcCardDao2;
        IGenericITableDAO<VatDTO> vatDao;
        IGenericITableDAO<UnitsDTO> unitDao;
        IGenericITableDAO<KdsDTO> kdsDao;
        IGenericITableDAO<KitchenDTO> kitchenDao;
        IGenericITableDAO<KitchenRegionDTO> kitchenregionDao;

        public NFCCard_IntegrationTest()
        {
            usersToDatabases = Resolve<IUsersToDatabasesXML>();
            nfcCard = Resolve<INFCcardFlows>();
            accountTableDao = Resolve<IGenericITableDAO<AccountDTO>>();
            departmentTableDao = Resolve<IGenericITableDAO<DepartmentDTO>>();
            posinfoTableDao = Resolve<IGenericITableDAO<PosInfoDTO>>();
            invoicetypesTableDao = Resolve<IGenericITableDAO<InvoiceTypesDTO>>();
            posinfodetailsTableDao = Resolve<IGenericITableDAO<PosInfoDetailDTO>>();
            staffTableDao = Resolve<IGenericITableDAO<StaffDTO>>();
            endOFDayDao = Resolve<IGenericITableDAO<EndOfDayDTO>>();
            nfcCardDao = Resolve<IGenericITableDAO<NFCcardDTO>>();
            nfcCardDao2 = Resolve<IGenericDAO<NFCcardDTO>>();
            vatDao = Resolve<IGenericITableDAO<VatDTO>>();
            unitDao = Resolve<IGenericITableDAO<UnitsDTO>>();
            kdsDao = Resolve<IGenericITableDAO<KdsDTO>>();
            kitchenDao = Resolve<IGenericITableDAO<KitchenDTO>>();
            kitchenregionDao = Resolve<IGenericITableDAO<KitchenRegionDTO>>();
        }

        [OneTimeSetUp]
        public void Init()
        {
            //Initialize mappings
            AutoMapperConfig.RegisterMappings();
            //Create Store from config file 
            string storeJson = Properties.Settings.Default.Store_NikkiBeach;
            CustomJsonDeserializers jdes = new CustomJsonDeserializers();
            store = jdes.JsonToStore(storeJson);
            connectionString = usersToDatabases.ConfigureConnectionString(store);
        }

        [Test, Order(1)]
        public void Successful_NFCCard_Get()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    EstablishTestEnvironment establish = new EstablishTestEnvironment(accountTableDao, departmentTableDao, posinfoTableDao, invoicetypesTableDao, posinfodetailsTableDao, staffTableDao, endOFDayDao, vatDao, unitDao, kdsDao, kitchenDao, kitchenregionDao, nfcCardDao, nfcCardDao2);
                    PosEnvironment environment = establish.EstablishNFCCardEnvironment(db);

                    NFCcardModel card = nfcCard.SelectNFCcardTable(store);
                    Assert.NotNull(card);
                    Assert.That<int>(card.Type, Is.EqualTo(environment.NFCCard.Type));
                    Assert.That<int>(card.RoomSector, Is.EqualTo(environment.NFCCard.RoomSector));
                    Assert.That<int>(card.FirstDateSector, Is.EqualTo(environment.NFCCard.FirstDateSector));
                    Assert.That<int>(card.SecondDateSector, Is.EqualTo(environment.NFCCard.SecondDateSector));
                    Assert.That<bool>(card.ValidateDate, Is.EqualTo(environment.NFCCard.ValidateDate));
                }
            }
        }

        [Test, Order(2)]
        public void Successful_Update_NFCCard()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    EstablishTestEnvironment establish = new EstablishTestEnvironment(accountTableDao, departmentTableDao, posinfoTableDao, invoicetypesTableDao, posinfodetailsTableDao, staffTableDao, endOFDayDao, vatDao, unitDao, kdsDao, kitchenDao, kitchenregionDao, nfcCardDao, nfcCardDao2);
                    PosEnvironment environment = establish.EstablishNFCCardEnvironment(db);

                    NFCcardModel card = AutoMapper.Mapper.Map<NFCcardModel>(environment.NFCCard);
                    bool success = nfcCard.UpdateNFCcardTable(store, card);
                    Assert.That<bool>(success, Is.EqualTo(true));
                }
            }
        }

        [Test, Order(3)]
        public void Successful_Unsuccessful_Update_NFCCard()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    EstablishTestEnvironment establish = new EstablishTestEnvironment(accountTableDao, departmentTableDao, posinfoTableDao, invoicetypesTableDao, posinfodetailsTableDao, staffTableDao, endOFDayDao, vatDao, unitDao, kdsDao, kitchenDao, kitchenregionDao, nfcCardDao, nfcCardDao2);
                    PosEnvironment environment = establish.EstablishNFCCardEnvironment(db);

                    NFCcardModel card = AutoMapper.Mapper.Map<NFCcardModel>(environment.NFCCard);

                    card.ValidateDate = true;
                    card.FirstDateSector = 666;
                    card.SecondDateSector = 666;
                    Assert.Throws<BusinessException>(() => nfcCard.UpdateNFCcardTable(store, card));
                    card.ValidateDate = false;
                    bool success = nfcCard.UpdateNFCcardTable(store, card);
                    Assert.That<bool>(success, Is.EqualTo(true));
                }
            }
        }

        [Test, Order(4)]
        public void Unsuccessful_Update_NFCCard_InvalidType()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    EstablishTestEnvironment establish = new EstablishTestEnvironment(accountTableDao, departmentTableDao, posinfoTableDao, invoicetypesTableDao, posinfodetailsTableDao, staffTableDao, endOFDayDao, vatDao, unitDao, kdsDao, kitchenDao, kitchenregionDao, nfcCardDao, nfcCardDao2);
                    PosEnvironment environment = establish.EstablishNFCCardEnvironment(db);

                    NFCcardModel card = AutoMapper.Mapper.Map<NFCcardModel>(environment.NFCCard);
                    card.Type = 2;
                    Assert.Throws<BusinessException>(() => nfcCard.UpdateNFCcardTable(store, card));
                }
            }
        }

        [Test, Order(5)]
        public void Unsuccessful_Update_NFCCard_InvalidRoomSectorMifareStandard()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    EstablishTestEnvironment establish = new EstablishTestEnvironment(accountTableDao, departmentTableDao, posinfoTableDao, invoicetypesTableDao, posinfodetailsTableDao, staffTableDao, endOFDayDao, vatDao, unitDao, kdsDao, kitchenDao, kitchenregionDao, nfcCardDao, nfcCardDao2);
                    PosEnvironment environment = establish.EstablishNFCCardEnvironment(db);

                    NFCcardModel card = AutoMapper.Mapper.Map<NFCcardModel>(environment.NFCCard);
                    card.Type = 0;

                    card.RoomSector = -1;
                    Assert.Throws<BusinessException>(() => nfcCard.UpdateNFCcardTable(store, card));
                    card.RoomSector = 64;
                    Assert.Throws<BusinessException>(() => nfcCard.UpdateNFCcardTable(store, card));
                }
            }
        }

        [Test, Order(6)]
        public void Unsuccessful_Update_NFCCard_InvalidDateSectorMifareStandard()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    EstablishTestEnvironment establish = new EstablishTestEnvironment(accountTableDao, departmentTableDao, posinfoTableDao, invoicetypesTableDao, posinfodetailsTableDao, staffTableDao, endOFDayDao, vatDao, unitDao, kdsDao, kitchenDao, kitchenregionDao, nfcCardDao, nfcCardDao2);
                    PosEnvironment environment = establish.EstablishNFCCardEnvironment(db);

                    NFCcardModel card = AutoMapper.Mapper.Map<NFCcardModel>(environment.NFCCard);
                    card.Type = 0;
                    card.ValidateDate = true;

                    card.FirstDateSector = -1;
                    Assert.Throws<BusinessException>(() => nfcCard.UpdateNFCcardTable(store, card));
                    card.FirstDateSector = 64;
                    Assert.Throws<BusinessException>(() => nfcCard.UpdateNFCcardTable(store, card));
                    card.SecondDateSector = -1;
                    Assert.Throws<BusinessException>(() => nfcCard.UpdateNFCcardTable(store, card));
                    card.SecondDateSector = 64;
                    Assert.Throws<BusinessException>(() => nfcCard.UpdateNFCcardTable(store, card));
                }
            }
        }

        [Test, Order(7)]
        public void Unsuccessful_Update_NFCCard_InvalidRoomSectorMifareUltraLight()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    EstablishTestEnvironment establish = new EstablishTestEnvironment(accountTableDao, departmentTableDao, posinfoTableDao, invoicetypesTableDao, posinfodetailsTableDao, staffTableDao, endOFDayDao, vatDao, unitDao, kdsDao, kitchenDao, kitchenregionDao, nfcCardDao, nfcCardDao2);
                    PosEnvironment environment = establish.EstablishNFCCardEnvironment(db);

                    NFCcardModel card = AutoMapper.Mapper.Map<NFCcardModel>(environment.NFCCard);
                    card.Type = 1;
                    card.ValidateDate = true;

                    card.FirstDateSector = -1;
                    Assert.Throws<BusinessException>(() => nfcCard.UpdateNFCcardTable(store, card));
                    card.FirstDateSector = 16;
                    Assert.Throws<BusinessException>(() => nfcCard.UpdateNFCcardTable(store, card));
                    card.SecondDateSector = -1;
                    Assert.Throws<BusinessException>(() => nfcCard.UpdateNFCcardTable(store, card));
                    card.SecondDateSector = 16;
                    Assert.Throws<BusinessException>(() => nfcCard.UpdateNFCcardTable(store, card));
                }
            }
        }

        [Test, Order(8)]
        public void Unsuccessful_Update_NFCCard_InvalidDateSectorMifareUltraLight()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    EstablishTestEnvironment establish = new EstablishTestEnvironment(accountTableDao, departmentTableDao, posinfoTableDao, invoicetypesTableDao, posinfodetailsTableDao, staffTableDao, endOFDayDao, vatDao, unitDao, kdsDao, kitchenDao, kitchenregionDao, nfcCardDao, nfcCardDao2);
                    PosEnvironment environment = establish.EstablishNFCCardEnvironment(db);

                    NFCcardModel card = AutoMapper.Mapper.Map<NFCcardModel>(environment.NFCCard);
                    card.Type = 1;
                    card.ValidateDate = true;

                    card.FirstDateSector = -1;
                    Assert.Throws<BusinessException>(() => nfcCard.UpdateNFCcardTable(store, card));
                    card.FirstDateSector = 16;
                    Assert.Throws<BusinessException>(() => nfcCard.UpdateNFCcardTable(store, card));
                    card.SecondDateSector = -1;
                    Assert.Throws<BusinessException>(() => nfcCard.UpdateNFCcardTable(store, card));
                    card.SecondDateSector = 16;
                    Assert.Throws<BusinessException>(() => nfcCard.UpdateNFCcardTable(store, card));
                }
            }
        }
    }
}
