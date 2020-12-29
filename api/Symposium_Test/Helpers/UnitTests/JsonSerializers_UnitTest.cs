using System;
using NUnit.Framework;
using Symposium.Helpers;
using Symposium.Models.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symposium.Helpers.Classes;

namespace Symposium_Test.Helpers.UnitTests
{
    /// <summary>
    /// Test Symposium.Helpers.CustomJsonSerializers & Symposium.Helpers.CustomJsonDeserializers classes
    /// </summary>
    [TestFixture]
    class JsonSerializers_UnitTest
    {
        DBInfoModel store;

       [OneTimeSetUp]
        public void Init()
        {
           store = new DBInfoModel()
            {
                DataBase = "DataBase1",
                DataBasePassword = "pass1",
                DataBaseUsername = "user1",
                DataSource = "DataSource1",
                Id = new Guid("dd07eeec-752a-45cf-a219-2a868731f089"),
                IsIntegrated = "true",
                Password = "pass2",
                Role = "role1",
                Username = "username1"
            };
        }

        [Test, Order(1)]
        public void Store_DeforeAndAfterSerializationDeserializationStoreShouldBeTheSame()
        {
            CustomJsonSerializers jSer = new CustomJsonSerializers();
            string json = jSer.StoreToJson(store);
            CustomJsonDeserializers jDeser = new CustomJsonDeserializers();
            DBInfoModel store2 = jDeser.JsonToStore(json);

            Assert.That<String>(json, Is.EqualTo( "{\"DataBase\":\"DataBase1\",\"DataBasePassword\":\"pass1\",\"DataBaseUsername\":\"user1\",\"DataSource\":\"DataSource1\",\"Id\":\"dd07eeec-752a-45cf-a219-2a868731f089\",\"IsIntegrated\":\"true\",\"Password\":\"pass2\",\"Role\":\"role1\",\"Username\":\"username1\"}"));
           // StringAssert.EndsWith( "\"Username\":\"username1\"}", json);
            Assert.That<int>(json.Length, Is.EqualTo(228));

            Assert.That<String>(store2.DataBase, Is.EqualTo(store.DataBase));
            Assert.That<String>(store2.DataBasePassword, Is.EqualTo(store.DataBasePassword));
            Assert.That<String>(store2.DataBaseUsername, Is.EqualTo(store.DataBaseUsername));
            Assert.That<String>(store2.DataSource, Is.EqualTo(store.DataSource));
            Assert.That<String>(store2.Id.ToString(), Is.EqualTo(store.Id.ToString()));
            Assert.That<String>(store2.IsIntegrated, Is.EqualTo(store.IsIntegrated));
            Assert.That<String>(store2.Password, Is.EqualTo(store.Password));
            Assert.That<String>(store2.Role, Is.EqualTo(store.Role));
            Assert.That<String>(store2.Username, Is.EqualTo(store.Username));
        }

        [OneTimeTearDown]
        public void TestTearDown()
        {
        }

    }
}
