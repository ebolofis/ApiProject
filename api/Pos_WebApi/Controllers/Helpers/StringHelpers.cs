using System.Text.RegularExpressions;

namespace Pos_WebApi.Controllers.Helpers
{

    public static class StringHelpers
    {
        /// <summary>
        /// Removes special characters from a string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveSpecialCharacters(string str)
        {
            if (!string.IsNullOrEmpty(str))
                return Regex.Replace(str, "[^α-ωΑ-ωa-zA-Z0-9_+-.:() ]+", " ", RegexOptions.Compiled);
            else
                return "";
        }
    }
}