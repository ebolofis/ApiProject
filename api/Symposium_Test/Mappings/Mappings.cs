using NUnit.Framework;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_Test.Mappings
{
    [TestFixture]
    class Mappings
    {

        [OneTimeSetUp]
        public void Init()
        {
            AutoMapperConfig.RegisterMappings();
        }

        [Test, Order(1)]
        public void PosInfoDto_To_PosInfoModel()
        {
            PosInfoDTO posinfodto = new PosInfoDTO() {
                                                        Id = 4,
                                                        Code = "01",
                                                        Description = "Cafe Nikki 1",
                                                        FODay = Convert.ToDateTime("2017 - 06 - 22 00:00:00.000"),
                                                        CloseId = 154,
                                                        IPAddress = "6.6.6.6",
                                                        Type = 1,
                                                        wsIP = "192.168.15.8",
                                                        wsPort = "4502",
                                                        DepartmentId = 3,
                                                        FiscalName = "ECR-666",
                                                        FiscalType = 1,
                                                        IsOpen = true,
                                                        ReceiptCount = 11854,
                                                        ResetsReceiptCounter = null,
                                                        Theme = "Light",
                                                        AccountId = 1,
                                                        LogInToOrder = false,
                                                        ClearTableManually = false,
                                                        ViewOnly = null,
                                                        IsDeleted = null,
                                                        InvoiceSumType = null,
                                                        LoginToOrderMode = 1,
                                                        KeyboardType = 3,
                                                        CustomerDisplayGif = "view1.gif",
                                                        DefaultHotelId = null,
                                                        NfcDevice = "NFCTestCom"
                                                    };

            PosInfoModel posinfo = AutoMapper.Mapper.Map<PosInfoModel>(posinfodto);

            Assert.That<long>(posinfo.Id, Is.EqualTo(posinfodto.Id));
            Assert.That<string>(posinfo.Code, Is.EqualTo(posinfodto.Code));
            Assert.That<string>(posinfo.Description, Is.EqualTo(posinfodto.Description));
            Assert.That<DateTime?>(posinfo.FODay, Is.EqualTo(posinfodto.FODay));
            Assert.That<long?>(posinfo.CloseId, Is.EqualTo(posinfodto.CloseId));
            Assert.That<string>(posinfo.IPAddress, Is.EqualTo(posinfodto.IPAddress));
            Assert.That<byte?>(posinfo.Type, Is.EqualTo(posinfodto.Type));
            Assert.That<long?>(posinfo.DepartmentId, Is.EqualTo(posinfodto.DepartmentId));
            Assert.That<string>(posinfo.DepartmentDescription, Is.EqualTo(null));
            Assert.That<string>(posinfo.FiscalName, Is.EqualTo(posinfodto.FiscalName));
            Assert.That<Int16?>(posinfo.FiscalType, Is.EqualTo(posinfodto.FiscalType));
            Assert.That<bool?>(posinfo.IsOpen, Is.EqualTo(posinfodto.IsOpen));
            Assert.That<long?>(posinfo.ReceiptCount, Is.EqualTo(posinfodto.ReceiptCount));
            Assert.That<bool?>(posinfo.ResetsReceiptCounter, Is.EqualTo(posinfodto.ResetsReceiptCounter));
            Assert.That<long?>(posinfo.AccountId, Is.EqualTo(posinfodto.AccountId));
            Assert.That<bool?>(posinfo.LogInToOrder, Is.EqualTo(posinfodto.LogInToOrder));
            Assert.That<bool?>(posinfo.ClearTableManually, Is.EqualTo(posinfodto.ClearTableManually));
            Assert.That<bool?>(posinfo.IsDeleted, Is.EqualTo(posinfodto.IsDeleted));
            Assert.That<int?>(posinfo.InvoiceSumType, Is.EqualTo(posinfodto.InvoiceSumType));
            Assert.That<short?>(posinfo.LoginToOrderMode, Is.EqualTo(posinfodto.LoginToOrderMode));
            Assert.That<short?>(posinfo.KeyboardType, Is.EqualTo(posinfodto.KeyboardType));
            Assert.That<string>(posinfo.CustomerDisplayGif, Is.EqualTo(posinfodto.CustomerDisplayGif));
            Assert.That<int?>(posinfo.DefaultHotelId, Is.EqualTo(posinfodto.DefaultHotelId));
            Assert.That<string>(posinfo.NfcDevice, Is.EqualTo(posinfodto.NfcDevice));

        }


    }
}
