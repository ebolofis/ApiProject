using Symposium.Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Classes
{
   public class FilesHelper: IFilesHelper
    {

        /// <summary>
        /// Enumerate files into directory
        /// </summary>
        /// <param name="path">directory path</param>
        /// <param name="searchPattern">searchPattern, default value: *.*</param>
        /// <param name="searchOption">searchOption, default: TopDirectoryOnly</param>
        /// <returns></returns>
        public List<string> EnumerateFiles(string path, string searchPattern="*.*", SearchOption searchOption= SearchOption.TopDirectoryOnly)
        {
            return Directory.EnumerateFiles(path, searchPattern, searchOption).ToList();
        }



    }
}
