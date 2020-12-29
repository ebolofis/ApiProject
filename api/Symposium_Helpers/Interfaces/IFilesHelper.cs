using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Interfaces
{
  public  interface IFilesHelper
    {


        /// <summary>
        /// Enumerate files into directory
        /// </summary>
        /// <param name="path">directory path</param>
        /// <param name="searchPattern">searchPattern, default value: *.*</param>
        /// <param name="searchOption">searchOption, default: TopDirectoryOnly</param>
        /// <returns></returns>
        List<string> EnumerateFiles(string path, string searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly);
    }
}
