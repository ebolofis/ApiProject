using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Interfaces
{
   public interface IGreekVowelsHelper
    {
        /// <summary>
        /// Remove Tonoi from Greek Vowels
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        string RemoveTonoi(string search);

        /// <summary>
        /// Return the position of the fist tonos. If no tonos exist return -1.
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        int FirstTonos(string search);

        /// <summary>
        /// return sql part for replacing tonoi from sql column
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        string ReplaceSql(string columnName);
    }
}
