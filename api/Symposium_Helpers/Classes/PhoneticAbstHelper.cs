using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Classes
{
    /// <summary>
    /// abstract class for creating phonetics. see also PhoneticGrHelper class 
    /// </summary>
    abstract public class PhoneticAbstHelper
    {
        /// <summary>
        /// Return the list of the phonetic words for the given word(s) 
        /// </summary>
        /// <param name="original">words(s) ex: "Λεωφόρος Ανδρέα Συγγρού 166" </param>
        /// <param name="shorthands">true: add shorthands to the result list (ex: add λ for Λεωφόρος, π for πλατεια) </param>
        /// <param name="diconstructAddrNumber">true: if address number does not contain only numbers (ex:3-5, 15β) then add to thesaurous only the numbers as well. </param>
        /// <returns> return the list of the phonetic words ex: "λεοφοροσ λ ανδρεα σιγγρου 166" (λ is shorthand)</returns>
        public abstract List<string> CreatePhonetics(string original, bool shorthands = true, bool diconstructAddrNumber = true);


        /// <summary>
        /// return true if str is a Shorthand (ex: λ, σ, παρ)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public abstract bool IsShorthand(string str);


        /// <summary>
        /// Return the list of the phonetic words as string, list's items are separated by " ".
        /// </summary>
        /// <param name="original">words(s) ex: "Λεωφόρος Ανδρέα Συγγρού 166" </param>
        /// <param name="shorthands">true: add shorthands to the result list (ex: add λ for Λεωφόρος, π for πλατεια) </param>
        /// <param name="diconstructAddrNumber">true: if address number does not contain only numbers (ex:3-5, 15β) then add to thesaurous only the numbers as well. </param>
        /// <returns> return the list of the phonetic words ex: "λεοφοροσ λ ανδρεα σιγγρου 166" (λ is shorthand)</returns>
        public string CreatePhoneticsToString(string original, bool shorthands = true, bool diconstructAddrNumber = true)
        {
            List<string> results = this.CreatePhonetics(original, shorthands, diconstructAddrNumber);
            if (results == null || results.Count == 0) return null;
            return string.Join(" ", results);
        }

        /// <summary>
        /// create pronetic words from a list of real words
        /// </summary>
        /// <param name="words">real words</param>
        /// <returns></returns>
        public abstract string CreatePhonetics(params string[] words);

    }
}
