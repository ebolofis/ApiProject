using NUnit.Framework;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_Test.Helpers.UnitTests
{
    /// <summary>
    /// Test Symposium.Helpers.PaginationHelper class
    /// </summary>
    [TestFixture]
    class Pagination_UnitTest
    {
        public Pagination_UnitTest()
        {
        }

        List<String> list;
       

        [OneTimeSetUp]
        public void Init()
        {
            //create a list with 999 items with values 1...999. WE will page this list...
            list = new List<String>();
            for (int i = 1; i <= 999; i++)
            {
                list.Add(i.ToString());
            }    
        }

        [Test, Order(1)]
        public void Pagination_PagesShouldBe100()
        {
            PaginationHelper<String> pgh = new PaginationHelper<String>();
            int curPage = 1;
            int pageSize = 10;
            PaginationModel<String> page = pgh.GetPage(list, curPage, pageSize);
            Assert.That<int>(page.PagesCount, Is.EqualTo(100));
        }

        [Test, Order(2)]
        public void Pagination_TheListItemOfTheFirstPageShouldBe10()
        {
            PaginationHelper<String> pgh = new PaginationHelper<String>();
            int curPage = 1;
            int pageSize = 10;
            PaginationModel<String> page = pgh.GetPage(list, curPage, pageSize);
            Assert.That<String>(page.PageList[9], Is.EqualTo("10"));
        }


        [Test, Order(3)]
        public void Pagination_TheLastPageShouldHave9Items()
        {
            PaginationHelper<String> pgh = new PaginationHelper<String>();
            int curPage = 100;
            int pageSize = 10;
            PaginationModel<String> page = pgh.GetPage(list, curPage, pageSize);
            Assert.That<int>(page.PageLength, Is.EqualTo(9));
        }

        [Test, Order(4)]
        public void Pagination_WhenAllItemsReturnInOnePageTheLastItemShouldBe999()
        {
            PaginationHelper<String> pgh = new PaginationHelper<String>();
            int curPage = 0;
            int pageSize = 10;
            PaginationModel<String> page = pgh.GetPage(list, curPage, pageSize);
            Assert.That<int>(page.PageLength, Is.EqualTo(999));
            Assert.That<String>(page.PageList[998], Is.EqualTo("999"));
            Assert.That<int>(page.CurrentPage, Is.EqualTo(1));
            Assert.That<int>(page.PagesCount, Is.EqualTo(1));
        }

        [Test, Order(5)]
        public void Pagination_WhenCurrPageIsLessThan0ShouldThrowException()
        {
            PaginationHelper<String> pgh = new PaginationHelper<String>();
            int curPage = -1;
            int pageSize = 10;
            Exception ex=  Assert.Throws<Exception>(delegate { pgh.GetPage(list, curPage, pageSize); });
            StringAssert.StartsWith( "The number of current page must be 0 or greater", ex.Message);
        }

        [Test, Order(6)]
        public void Pagination_WhenPageSizeIsLessOrEqualTo0ShouldThrowException()
        {
            PaginationHelper<String> pgh = new PaginationHelper<String>();
            int curPage = 5;
            int pageSize = 0;
            Exception ex = Assert.Throws<Exception>(delegate { pgh.GetPage(list, curPage, pageSize); });
            StringAssert.StartsWith("The page length must be 1 or greater", ex.Message);
        }

        [Test, Order(7)]
        public void Pagination_WhenListIsNullShouldReturnAnEmptyPage()
        {
            PaginationHelper<String> pgh = new PaginationHelper<String>();
            int curPage = 5;
            int pageSize = 10;
            PaginationModel<String> page = pgh.GetPage(null, curPage, pageSize);
            Assert.That<int>(page.PagesCount,Is.EqualTo(0));
            Assert.That<int>(page.ItemsCount, Is.EqualTo(0));
        }

        [OneTimeTearDown]
        public void TestTearDown()
        {
        }

    }

}
