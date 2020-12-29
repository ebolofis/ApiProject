using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    /// <summary>
    /// Model that contains the items of a single page (Pagination) .
    ///<para> see: <seealso cref="Symposium.Helpers.PaginationHelper"/></para>
    /// </summary>
    /// <typeparam name="T">an object or premitive type</typeparam>
    public class PaginationModel<T> 
    {

        /// <summary>
        /// Items of the current page
        /// </summary>
        public List<T> PageList { get; set; }

        /// <summary>
        /// The number og the current page (min value is 1)
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// number of total pages
        /// </summary>
        public int PagesCount { get; set; }

        /// <summary>
        /// the number of items in the current page
        /// </summary>
        public int PageLength { get; set; }
        /// <summary>
        /// the total number of items from all pages
        /// </summary>
        public int ItemsCount { get; set; }

    }
}
