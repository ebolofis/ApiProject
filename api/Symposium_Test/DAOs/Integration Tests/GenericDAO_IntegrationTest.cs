using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.DAOs;
using Symposium_DTOs.PosModel_Info;
using Symposium.Helpers.Classes;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using System.Data;
using System.Data.SqlClient;

namespace Symposium_Test.DAOs.Integration_Tests
{
    [TestFixture]
    class GenericDAO_IntegrationTest : IoCSupportedTest<TestModule>//<-- the module to register
    {

        DBInfoModel store;
        long posInfo;
        string connectionString;
        IGenericDAO<EndOfYearDTO> endOfYearDao;
        IGenericDAO<ProductDTO> productDao;
        IGenericDAO<PagesDTO> pagesDao;
        ICustomJsonDeserializers jdes;
        IUsersToDatabasesXML usersToDatabases;
        List<EndOfYearDTO> endOfYearList;
        EndOfYearDTO eoy, eoy2, eoy3;
        DateTime closedDate;

        IGenericITableDAO<EndOfYearDTO> endOfYear2Dao;

        public GenericDAO_IntegrationTest()
        {
            endOfYearDao = Resolve<IGenericDAO<EndOfYearDTO>>();//IoC
            productDao = Resolve<IGenericDAO<ProductDTO>>();
            pagesDao = Resolve<IGenericDAO<PagesDTO>>();
            jdes = Resolve<ICustomJsonDeserializers>();
            usersToDatabases = Resolve<IUsersToDatabasesXML>();

            endOfYear2Dao = Resolve<IGenericITableDAO<EndOfYearDTO>>();
        }

        [OneTimeSetUp]
        public void Init()
        {
            //Create Store from config file 
            string storeJson = Properties.Settings.Default.Store_NikkiBeach;
            store = jdes.JsonToStore(storeJson);
            posInfo = Properties.Settings.Default.PosInfo;

            connectionString = usersToDatabases.ConfigureConnectionString(store);
            closedDate = DateTime.Now;
            eoy = new EndOfYearDTO() { ClosedYear = 1900, ClosedDate = closedDate, Description = "This is a Test" };
        }


        [Test, Order(1)]
        public void GenericDAO_Select_EndOfYear()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                endOfYearList = endOfYearDao.Select(db, "EndOfYear");
            }
            Assert.True(endOfYearList.Count >= 0);
        }

        [Test, Order(2)]
        public void GenericDAO_Product_Pages()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string conditions = "where ProductCategoryId=@ProductCategoryId";
                var parameters = new { ProductCategoryId = 2 };
                string orderby = "Description desc";

                //get the number and the length of the last page
                int c = productDao.RecordCount(db, conditions, parameters);
                int pages = c / 10;
                int md = c % 10;
                if (md > 0) pages++;

                //get the last page
                List<ProductDTO> products = productDao.GetPage(db, pages, 10, conditions, orderby, parameters);//where ProductCategoryId=2
                Assert.That<int>(products.Count, Is.EqualTo(md));
            }
        }

        [Test, Order(3)]
        public void GenericDAO_Product_Select2Queries()
        {
            using (IDbConnection db = new SqlConnection(connectionString))

            {
                //get some products and some pages seperately
                var parameters = new { ProductCategoryId = 2, PagesetId = 4 };
                int prod = productDao.RecordCount(db, "where ProductCategoryId=@ProductCategoryId", parameters);
                int pgs = pagesDao.RecordCount(db, "where PagesetId=@PagesetId", parameters);

                //get some products and some pages together
                Tuple<List<ProductDTO>, List<PagesDTO>> tuple = productDao.Select2Queries<PagesDTO>(db, "select * from Product where ProductCategoryId=@ProductCategoryId", "select * from Pages where  PagesetId=@PagesetId", parameters);

                Assert.That<int>(tuple.Item1.Count, Is.EqualTo(prod));
                Assert.That<int>(tuple.Item2.Count, Is.EqualTo(pgs));
            }
        }

        [Test, Order(4)]
        public void GenericDAO_Products_UpdateList()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //1. get some products from db
                List<ProductDTO> products = productDao.Select("select top 5 * from Product ", null, db);

                //2. CHANGE THE DESCRIPTION FOR THE SELECTED PRODUCTS
                foreach(ProductDTO P in products)
                {
                    P.Description = P.Description + P.Id.ToString();
                }


                using (var scope = new TransactionScope())
                {
                    //3. update the changed products to db
                    int i = productDao.UpdateList(db, products);
                    Assert.That<int>(i, Is.EqualTo(products.Count));

                    //4. select again the same products from db
                    List<ProductDTO> products2 = productDao.Select("select top 5 * from Product ", null, db);

                    //5. check that updates did well
                    for(int k=0;k< products2.Count;k++)
                    {
                        ProductDTO p= products[k];
                        ProductDTO p2 = products2[k];
                        StringAssert.EndsWith(p.Id.ToString(), p2.Description);
                        Assert.That<string>(p2.ExtraDescription, Is.EqualTo((p.ExtraDescription)));
                    }

                    //6. DO NOT commit changes...
                }
            }
        }

        [Test, Order(5)]
        public void GenericITableDAO_EndOfYear_InsertList()
        {

            using (IDbConnection db = new SqlConnection(connectionString))
            {
               

                //1. get the current max Id from table
                long maxId = endOfYearDao.GetMaxId(db, "EndOfYear");

                //2. create 3 new objects to insert
                List<EndOfYearDTO> list = new List<EndOfYearDTO>();
                list.Add(new EndOfYearDTO() { ClosedYear = 1900, ClosedDate = closedDate, Description = "This is 1st Test" });
                list.Add(new EndOfYearDTO() { ClosedYear = 1901, ClosedDate = closedDate, Description = "This is 2nd Test" });
                list.Add(new EndOfYearDTO() { ClosedYear = 1902, ClosedDate = closedDate, Description = "This is 3rd Test" });

               
                using (var scope = new TransactionScope())
                {
                    //3. Insert the list
                    List<EndOfYearDTO> newList = endOfYear2Dao.InsertList(db, list);

                    //4. in the  newList Ids must be greater than maxId
                    foreach (EndOfYearDTO e in newList)
                    {
                        Assert.That<long>(e.Id, Is.GreaterThan(maxId));
                    }

                    //5. Get the inserted list directly from DB, should be 3 items
                    List<EndOfYearDTO> insertList =endOfYearDao.Select(db, "where Id>@Id", new { @Id = maxId });
                    Assert.That<int>(insertList.Count, Is.EqualTo(3));
                    Assert.That<int?>(insertList[0].ClosedYear, Is.EqualTo(newList[0].ClosedYear));
                    Assert.That<int?>(insertList[1].ClosedYear, Is.EqualTo(newList[1].ClosedYear));
                    Assert.That<int?>(insertList[2].ClosedYear, Is.EqualTo(newList[2].ClosedYear));
                    Assert.That<string>(insertList[0].Description, Is.EqualTo(newList[0].Description));
                    Assert.That<string>(insertList[1].Description, Is.EqualTo(newList[1].Description));
                    Assert.That<string>(insertList[2].Description, Is.EqualTo(newList[2].Description));

                    //6. DO NOT commit changes...
                }


            }
        }

        [Test, Order(5)]
        public void GenericITableDAO_EndOfYear_Upsert()
        {

            using (IDbConnection db = new SqlConnection(connectionString))
            {

                //1. get the last record from table
                long maxId = endOfYearDao.GetMaxId(db, "EndOfYear");
                EndOfYearDTO exist = endOfYearDao.Select(db, maxId);

                //2. change the existing object
                if(exist!=null) exist.Description = exist.Description + exist.Id.ToString();

                //3. create 2 new objects to insert
                List<EndOfYearDTO> list = new List<EndOfYearDTO>();
                list.Add(new EndOfYearDTO() { ClosedYear = 1900, ClosedDate = closedDate, Description = "This is 1st Test" });
                list.Add(new EndOfYearDTO() { ClosedYear = 1901, ClosedDate = closedDate, Description = "This is 2nd Test" });

                //4. add the existing object to the list
                if (exist != null) list.Add(exist);

                using (var scope = new TransactionScope())
                {
                    //3. Upsert the list
                    List<EndOfYearDTO> newList = endOfYear2Dao.UpsertList(db, list);

                    //4. in the  newList Ids must be greater or equal than maxId
                    foreach (EndOfYearDTO e in newList)
                    {
                        Assert.That<long>(e.Id, Is.GreaterThanOrEqualTo(maxId));
                    }

                    //5. Get the inserted list directly from DB, should be 3 items
                    List<EndOfYearDTO> insertList = endOfYearDao.Select(db, "where Id>=@Id", new { @Id = maxId });
                    Assert.That<int>(insertList.Count, Is.EqualTo(3));
                    Assert.That<int?>(insertList[1].ClosedYear, Is.EqualTo(newList[0].ClosedYear));
                    Assert.That<int?>(insertList[2].ClosedYear, Is.EqualTo(newList[1].ClosedYear));
                    Assert.That<int?>(insertList[0].ClosedYear, Is.EqualTo(newList[2].ClosedYear));
                    Assert.That<string>(insertList[1].Description, Is.EqualTo(newList[0].Description));
                    Assert.That<string>(insertList[2].Description, Is.EqualTo(newList[1].Description));
                    Assert.That<string>(insertList[0].Description, Is.EqualTo(newList[2].Description));

                    //6. DO NOT commit changes...
                }


            }
        }

        [Test, Order(7)]
        public void GenericDAO_EndOfYear_CRUD()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            { 
                using (var scope = new TransactionScope())
                {
                    //1. insert a record
                    eoy.Id= endOfYearDao.Insert(db, eoy);
                    Assert.That<long>(eoy.Id, Is.GreaterThan(0));

                    //2. select all data from table
                    endOfYearList = endOfYearDao.Select(db, "EndOfYear");
                    Assert.True(endOfYearList.Count >= 1);

                    //3. select the inserted record and test the selected data, should match the original ones
                    endOfYearList = endOfYearDao.Select("select * from EndOfYear where Id=@Id",new {Id=eoy.Id}, db);
                    Assert.True(endOfYearList.Count == 1);
                    Assert.That<long>(endOfYearList[0].Id, Is.EqualTo(eoy.Id));
                    Assert.That<int?>(endOfYearList[0].ClosedYear, Is.EqualTo(eoy.ClosedYear));
                    Assert.That<string>(endOfYearList[0].ClosedDate.ToString(), Is.EqualTo(eoy.ClosedDate.ToString()));
                    Assert.That<string>(endOfYearList[0].Description, Is.EqualTo(eoy.Description));

                    //4. get the number of records with the inserted id, should be 1
                   int c= endOfYearDao.RecordCount(db, "where Id=@Id", new { Id = eoy.Id });
                    Assert.That<int>(c, Is.EqualTo(1));

                    //5. Update the inserted record,  1 record should be updated
                    eoy.Description = "This is a new Test";
                    eoy.ClosedYear = 1901;
                    int i= endOfYearDao.Update(db, eoy);
                    Assert.That<int>(i, Is.EqualTo(1));

                    //6. select the inserted and updated record, should read the updates
                    eoy2 =  endOfYearDao.Select(db, eoy.Id);
                    Assert.That<long>(eoy2.Id, Is.EqualTo(eoy.Id));
                    Assert.That<int?>(eoy2.ClosedYear, Is.EqualTo(eoy.ClosedYear));
                    Assert.That<string>(eoy2.ClosedDate.ToString(), Is.EqualTo(eoy.ClosedDate.ToString()));
                    Assert.That<string>(eoy2.Description, Is.EqualTo(eoy.Description));


                    //7. Greate a list with an existining object (to DB) and a fake object. Run UpdateList, only one record will be updated
                    eoy2 = new EndOfYearDTO() { Id=999999999,ClosedYear = 1800, ClosedDate = closedDate, Description = "This is a 2nd Test" };
                    eoy.Description = "This is a 3rd Test";
                    eoy.ClosedYear = 1902;
                    int p= endOfYearDao.UpdateList(db, new List<EndOfYearDTO>() { eoy, eoy2 });
                    Assert.That<int>(p, Is.EqualTo(1));

                    eoy3 = endOfYearDao.Select(db, eoy.Id);
                    Assert.That<long>(eoy3.Id, Is.EqualTo(eoy.Id));
                    Assert.That<int?>(eoy3.ClosedYear, Is.EqualTo(eoy.ClosedYear));
                    Assert.That<string>(eoy3.ClosedDate.ToString(), Is.EqualTo(eoy.ClosedDate.ToString()));
                    Assert.That<string>(eoy3.Description, Is.EqualTo(eoy.Description));


                    //8. Delete the inserted record, 1 record should be deleted
                    int k = endOfYearDao.Delete(db,eoy.Id);
                   Assert.That<int>(k, Is.EqualTo(1));

                   
                }

                //9. select the deleted item, should return 0 records
                endOfYearList = endOfYearDao.Select(db, "where Id=@Id", new { Id = eoy.Id });
                Assert.True(endOfYearList.Count == 0);
            }
        }

     
    }
}
