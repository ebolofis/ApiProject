using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Interfaces
{
    public interface IPaginationHelper<T> //where T : class
    {

        /// <summary>
        /// Get the whole list of T items and return the items of the current page
        /// </summary>
        /// <param name="list">the whole list</param>
        /// <param name="currentPage">the number of current page. Start from 1. For currentPage 0 return the whole list</param>
        /// <param name="pageLength">the number of items per page</param>
        /// <returns>a PaginationModel containing the items of the page, the current page, the total number of items, the page size.  </returns>
        PaginationModel<T> GetPage(List<T> list, int currentPage, int pageSize);

    }
}
