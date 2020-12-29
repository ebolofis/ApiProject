using Symposium.Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Classes
{
    /// <summary>
    /// Prepare Greek words for sql searching...
    /// </summary>
    public class GreekVowelsHelper: IGreekVowelsHelper
    {
        private char[] fonientaMeTonoi = new char[] { 'ά', 'Ά', 'έ', 'Έ', 'ό', 'Ό', 'ί', 'Ί', 'ή', 'Ή', 'ύ', 'Ύ', 'ώ', 'Ώ' };


        /// <summary>
        /// Remove Tonoi from Greek Vowels
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public string RemoveTonoi(string search="")
        {
            if (FirstTonos(search) != -1)
            {
                return search.Replace("ά", "α").Replace("Ά", "Α")
                    .Replace("έ", "ε").Replace("Έ", "Ε")
                    .Replace("ό", "ο").Replace("Ό", "Ο")
                    .Replace("ί", "ι").Replace("Ί", "Ι")
                    .Replace("ή", "η").Replace("Ή", "Η")
                    .Replace("ύ", "υ").Replace("Ύ", "Υ")
                    .Replace("ώ", "ω").Replace("Ώ", "Ω");
            }
            return search;
        }

        /// <summary>
        /// Return the position of the fist tonos. If no tonos exist return -1.
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public int FirstTonos(string search)
        {
            return search.IndexOfAny(fonientaMeTonoi);
        }

        /// <summary>
        /// return sql part for replacing tonoi from sql column
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string ReplaceSql(string columnName)
        {
            return $"REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE({columnName},'ά','α'),'έ','ε'),'ή','η'),'ί','ι'),'ό','ο'),'ύ','υ'),'ώ','ω'),'Ά','Α'),'Έ','Ε'),'Ή','Η'),'Ί','Ι'),'Ό','Ο'),'Ύ','Υ'),'Ώ','Ω')";
        }
    }
}
