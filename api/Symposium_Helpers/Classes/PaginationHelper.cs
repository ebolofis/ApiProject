
using Symposium.Helpers.Interfaces;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Classes
{
    /// <summary>
    /// Helper class that performs pagination activities
    /// </summary>
    /// <typeparam name="T"></typeparam>
   public  class PaginationHelper<T>: IPaginationHelper<T> //where T:class
    {
        /// <summary>
        /// Get the whole list of T items and return the items of the current page
        /// </summary>
        /// <param name="list">the whole list</param>
        /// <param name="currentPage">the number of current page. Start from 1. For currentPage 0 return the whole list</param>
        /// <param name="pageLength">the number of items per page</param>
        /// <returns>a PaginationModel containing the items of the page, the current page, the total number of items, the page size.  </returns>
        public PaginationModel<T> GetPage(List<T> list, int currentPage, int pageSize)
        {
            if (currentPage < 0) throw new Exception(Symposium.Resources.Errors.GREATERNUMBER);
            if (pageSize <= 0) throw new Exception(Symposium.Resources.Errors.GREATERPAGELENGTH);

            PaginationModel<T> page = new PaginationModel<T>();
            if (list == null || list.Count==0)
            {
                page.PageList = new List<T>();
                return page;
            }
            if (currentPage == 0)//return list as one page
            { 
                page.PageList = list;
                page.PageLength = page.PageList.Count();
                page.ItemsCount = list.Count;
                page.CurrentPage = 1;
                page.PagesCount = 1;
                return page;
            }

            //create page
            page.PageList= list.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList<T>();
            page.PageLength = page.PageList.Count();
            page.CurrentPage = currentPage;
            page.ItemsCount = list.Count;
            page.PagesCount = page.ItemsCount / pageSize;
            if (page.ItemsCount % pageSize > 0) page.PagesCount++;
            return page;
        }

    }
}
