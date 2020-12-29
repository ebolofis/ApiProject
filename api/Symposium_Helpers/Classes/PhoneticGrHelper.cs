using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Symposium.Helpers.Classes
{
    public class PhoneticGrHelper : PhoneticAbstHelper
    {

        private List<string> results;
        private Dictionary<string, List<string>> shorthands;

        public PhoneticGrHelper()
        {
            results = new List<string>();
            CreateShorthands();
        }

        /// <summary>
        /// Return the list of the phonetic words for the given word(s) 
        /// </summary>
        /// <param name="original">words(s) ex: "Λεωφόρος Ανδρέα Συγγρού 166" </param>
        /// <param name="shorthands">true: add shorthands to the result list (ex: add λ for Λεωφόρος, π for πλατεια) </param>
        /// <param name="diconstructAddrNumber">true: if address number does not contain only numbers (ex:3-5, 15β) then add to thesaurous only the numbers as well. </param>
        /// <returns> return the list of the phonetic words ex: "λεοφοροσ λ ανδρεα σιγγρου 166" (λ is shorthand)</returns>
        public override List<string> CreatePhonetics(string original, bool shorthands = true, bool diconstructAddrNumber = true)
        {
            results = new List<string>();
            if (string.IsNullOrWhiteSpace(original)) return results;

            //1.prepare the word
            original = original.ToLower().Replace(".", " ").Replace(",", " ");
            original = RemoveTonoi(original);

            //2.split words by space ' '
            List<string> parts = new List<string>();
            parts = original.Split(' ').ToList();
            parts.RemoveAll(x => string.IsNullOrWhiteSpace(x));

            //3. create phonetic for every word
            foreach (string part in parts)
            {
                if (!string.IsNullOrEmpty(part))
                {
                    string prt = part.Trim();
                    ConstructPhonetics(prt, shorthands, diconstructAddrNumber);
                }

            }
            return results;
        }

        /// <summary>
        /// create pronetic words from a list of real words
        /// </summary>
        /// <param name="words">real words</param>
        /// <returns></returns>
        public override string CreatePhonetics(params string[] words)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string word in words)
            {
                if (!string.IsNullOrWhiteSpace(word))
                {
                    if (sb.Length > 0) sb.Append(" ");
                    sb.Append(word);
                }
            }
            return CreatePhoneticsToString(sb.ToString().Replace(",", " "));
        }

        /// <summary>
        /// return true if str is a Shorthand (ex: λ, σ, παρ)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public override bool IsShorthand(string str)
        {
            return shorthands.ContainsKey(str);
        }

        /// <summary>
        /// Create the phonetic word by a real word
        /// </summary>
        /// <param name="part">the word</param>
        /// <param name="shorthands">true: add shorthands to the result list (ex: add λ for Λεωφόρος, π for πλατεια)</param>
        /// <param name="diconstructAddrNumber">true: if address number does not contain only numbers (ex:3-5, 15β) then add to thesaurous only the numbers as well. </param>
        private void ConstructPhonetics(string part, bool shorthands, bool diconstructAddrNumber)
        {
            part = part.ToLower().Trim();

            //add to phonetic words the original english value
            if ((part[0] >= 'A' && part[0] <= 'Z') || (part[0] >= 'a' && part[0] <= 'z'))
                AddToThesaurus(part);


            //greekglish --> greek
            string res = FromGreeklish(part);

            //αυ --> αβ
            res = Replace(res, new string[] { "αυ" }, "αβ");
            //ευ --> εβ
            res = Replace(res, new string[] { "ευ" }, "εφ");
            //αι --> ε
            res = Replace(res, new string[] { "αι" }, "ε");
            // -- preserve "ου"
            res = Replace(res, new string[] { "ου" }, "==^==");
            // οι, ει υ, η --> ι
            res = Replace(res, new string[] { "οι", "ει", "υ", "η" }, "ι");
            // -- restore "ου"
            res = Replace(res, new string[] { "==^==" }, "ου");
            //ω --> ο
            res = Replace(res, new string[] { "ω" }, "ο");
            //σσ --> σ
            res = Replace(res, new string[] { "σσ" }, "σ");
            //ρρ --> ρ
            res = Replace(res, new string[] { "ρρ" }, "ρ");
            //νν --> ν
            res = Replace(res, new string[] { "νν" }, "ν");
            //λλ --> λ
            res = Replace(res, new string[] { "λλ" }, "λ");
            //μμ --> μ
            res = Replace(res, new string[] { "μμ" }, "μ");
            //κκ --> κ
            res = Replace(res, new string[] { "κκ" }, "κ");
            //ππ --> π
            res = Replace(res, new string[] { "ππ" }, "π");
            //ββ --> β
            res = Replace(res, new string[] { "ββ" }, "β");
            //ττ --> τ
            res = Replace(res, new string[] { "ττ" }, "τ");
            //ντζ --> τζ
            res = Replace(res, new string[] { "ντζ" }, "τζ");
            //γγ --> γκ
            res = Replace(res, new string[] { "γγ" }, "γκ");
            //ς --> σ
            res = Replace(res, new string[] { "ς" }, "σ");



            AddToThesaurus(res);


            //Add Ahorthands
            if (shorthands) AddShorthands(res);

            //diconstruct address number
            if (diconstructAddrNumber) DeconstructAddrNumber(res);

            //νδ --> ντ (ex: ανδρεασ --> αντρεασ)
            if (res.Contains("νδ")) AddToThesaurus(Replace(res, new string[] { "νδ" }, "ντ"));

            //Ιnsert Αdditional Εntries...
            //add "και" and "&"
            if (res == "κε") AddToThesaurus("&");
            if (res == "&") AddToThesaurus("κε");
            if (res == "and" || res == "κ")
            {
                AddToThesaurus("&");
                AddToThesaurus("κε");
            }

            //Endings...
            //-του --> -τι (ex: αιγινιτου --> αιγινιτι)
            if (res.EndsWith("του"))
            {
                AddToThesaurus(res.Remove(res.Length - 3, 3) + "τι");
            }
            //-δου --> -δι  
            if (res != "παροδου" && res.EndsWith("δου"))
            {
                AddToThesaurus(res.Remove(res.Length - 3, 3) + "δι");
            }
            //-νου --> -νι (ex: ιοαννου --> ιοαννι)
            if (res.EndsWith("νου"))
            {
                AddToThesaurus(res.Remove(res.Length - 3, 3) + "νι");
            }
            //-ριου --> -ρι (ex: Νοεμβρίου --> Νοέμβρη)
            if (res.EndsWith("ριου"))
            {
                AddToThesaurus(res.Remove(res.Length - 4, 4) + "ρι");
            }

            //-ος --> -ας (ex: ελπίδος --> ελπιδας)
            if (res != "λεοφοροσ" && res != "παροδοσ" && res.EndsWith("οσ"))
            {
                AddToThesaurus(res.Remove(res.Length - 2, 2) + "ασ");
            }
            //-εως --> -ησ (ex: αναγενησεως --> ανναγενησης)
            if (res.EndsWith("εοσ"))
            {
                AddToThesaurus(res.Remove(res.Length - 3, 3) + "ισ");
            }
            //-εουσ --> -η (ex: θεμιστοκλεους --> θεμιστοκλη)
            if (res.EndsWith("εουσ"))
            {
                AddToThesaurus(res.Remove(res.Length - 4, 4) + "ι");
            }
            //-ουσ --> -η (ex: Διοφάντους  --> Διοφάντη)
            if (res.EndsWith("ουσ") && !res.EndsWith("εουσ"))
            {
                AddToThesaurus(res.Remove(res.Length - 3, 3) + "ι");
            }
            //-λουσ --> -λη     (ex: Αριστοτέλους --> Αριστοτέλη)
            if (res.EndsWith("λουσ"))
            {
                AddToThesaurus(res.Remove(res.Length - 4, 4) + "λι");
            }
            //-ης --> -ας (ex: κονιτσης --> κονιτσας)
            if (part.EndsWith("ης"))
            {
                AddToThesaurus(res.Remove(res.Length - 2, 2) + "ασ");
            }
        }

        /// <summary>
        /// create the list of shorthand
        /// </summary>
        private void CreateShorthands()
        {
            shorthands = new Dictionary<string, List<string>>();
            //greek keys
            shorthands.Add("λ", new List<string>() { "λεοφοροσ", "λεοφορου", "λεοφ", "λεo", "λ" });
            shorthands.Add("σ", new List<string>() { "στρατιγου", "στρ", "στρατ" });
            shorthands.Add("π", new List<string>() { "πλατια", "πλατιασ", "πλατ", "πλ", "πλα" });
            shorthands.Add("παρ", new List<string>() { "παροδοσ", "παρ", "παροδου" });
            shorthands.Add("ν", new List<string>() { "νεοσ", "νεο", "νεα" });
            shorthands.Add("α", new List<string>() { "ανο" });
            shorthands.Add("κ", new List<string>() { "κατο" });
            shorthands.Add("πα", new List<string>() { "παλιο", "παλεο" });
            shorthands.Add("αγ", new List<string>() { "αγιοσ", "αγια", "αγιου", "αγιασ", "αγιον" });
            shorthands.Add("β", new List<string>() { "βασιλεοσ", "βασιλισισ", "βασιλισασ" });
            shorthands.Add("&", new List<string>() { "κε", "κ", "and" });

            //english keys
            shorthands.Add("l", new List<string>() { "λεοφοροσ", "λεοφορου", "λεοφ", "λεo", "λ" });
            shorthands.Add("s", new List<string>() { "στρατιγου", "στρ", "στρατ" });
            shorthands.Add("p", new List<string>() { "πλατια", "πλατιασ", "πλατ", "πλ", "πλα" });
            shorthands.Add("par", new List<string>() { "παροδοσ", "παρ", "παροδου" });
            shorthands.Add("n", new List<string>() { "νεοσ", "νεο", "νεα" });
            shorthands.Add("a", new List<string>() { "ανο" });
            shorthands.Add("k", new List<string>() { "κατο" });
            shorthands.Add("pa", new List<string>() { "παλιο", "παλεο" });
            shorthands.Add("ag", new List<string>() { "αγιοσ", "αγια", "αγιου", "αγιασ", "αγιον" });
            shorthands.Add("b", new List<string>() { "βασιλεοσ", "βασιλισισ", "βασιλισασ" });


        }



        private string FromGreeklish(string res)
        {
            //english a --> greek α
            res = Replace(res, new string[] { "a" }, "α");
            //english e --> greek ε
            res = Replace(res, new string[] { "e" }, "ε");
            //english o,w --> greek ο
            res = Replace(res, new string[] { "o", "w" }, "ο");
            //english i --> greek ι
            res = Replace(res, new string[] { "i" }, "ι");
            //english u,y --> greek υ
            res = Replace(res, new string[] { "u", "y" }, "υ");
            //english x,ch --> greek χ
            res = Replace(res, new string[] { "x", "ch" }, "χ");
            //english ps --> greek ψ
            res = Replace(res, new string[] { "pc" }, "ψ");
            //english ks --> greek ξ
            res = Replace(res, new string[] { "ks" }, "ξ");
            //english k --> greek κ
            res = Replace(res, new string[] { "k" }, "κ");
            //english l --> greek λ
            res = Replace(res, new string[] { "l" }, "λ");
            //english f --> greek φ
            res = Replace(res, new string[] { "f" }, "φ");
            //english th --> greek θ
            res = Replace(res, new string[] { "th" }, "θ");
            //english r --> greek ρ
            res = Replace(res, new string[] { "r" }, "ρ");
            //english t --> greek τ
            res = Replace(res, new string[] { "t" }, "τ");
            //english p --> greek π
            res = Replace(res, new string[] { "p" }, "π");
            //english s --> greek σ
            res = Replace(res, new string[] { "s" }, "σ");
            //english d --> greek δ
            res = Replace(res, new string[] { "d" }, "δ");
            //english g --> greek γ
            res = Replace(res, new string[] { "g" }, "γ");
            //english z --> greek ζ
            res = Replace(res, new string[] { "z" }, "ζ");
            //english b --> greek β
            res = Replace(res, new string[] { "b", "v" }, "β");
            //english n --> greek ν
            res = Replace(res, new string[] { "n" }, "ν");
            //english m --> greek μ
            res = Replace(res, new string[] { "m" }, "μ");

            return res;
        }

        private string RemoveTonoi(string str)
        {
            str = str.Replace("ά", "α").Replace("έ", "ε").Replace("ό", "ο").Replace("ώ", "ω").Replace("ί", "ι").Replace("ύ", "υ").Replace("ή", "η")
                .Replace("ϊ", "ι").Replace("ΐ", "ι").Replace("ϋ", "υ").Replace("ΰ", "υ");
            return str;
        }

        /// <summary>
        /// replase into a word a list of chars to char
        /// </summary>
        /// <param name="word"> the word</param>
        /// <param name="from">list of chars to be replaced</param>
        /// <param name="to">the char to replece the from list</param>
        /// <returns></returns>
        private string Replace(string word, string[] from, string to)
        {
            string res = word;
            foreach (string f in from)
            {
                res = res.Replace(f, to); //replace all
            }
            return res;
        }

        private void AddToThesaurus(string original, string result)
        {
            if (original != result && !results.Contains(result)) results.Add(result);
        }

        private void AddToThesaurus(string result)
        {
            if (!results.Contains(result)) results.Add(result);
        }

        private void AddShorthands(string res)
        {
            foreach (string key in shorthands.Keys)
            {
                if (shorthands[key].Contains(res)) AddToThesaurus(key);
            }
        }

        /// <summary>
        /// if address number does not contain only numbers (ex:3-5, 15β) then add to thesaurous only the numbers as well.
        ///  ex: for res="3-5" values "3" and "5" will be added to Thesaurus
        ///  ex: for res="15β" value "15" will be added to Thesaurus
        /// </summary>
        /// <param name="res"></param>
        private void DeconstructAddrNumber(string res)
        {
            //if res does not start with number then return
            if (!Regex.IsMatch(res, @"^\d+")) return;

            //if res is numeric then return (nothing special to do there)
            if (int.TryParse(res, out _)) return;

            //separate all numerics from res and add them to Thesaurus. 
            var sb = new StringBuilder();
            var goodChars = "0123456789".ToCharArray();
            foreach (var c in res)
            {
                if (Array.IndexOf(goodChars, c) >= 0)
                    sb.Append(c);
                else
                {
                    var output = sb.ToString();
                    if (!string.IsNullOrWhiteSpace(output)) AddToThesaurus(output);
                    sb.Clear();
                }
            }
            var output2 = sb.ToString();
            if (!string.IsNullOrWhiteSpace(output2)) AddToThesaurus(output2);

        }


    }
}
