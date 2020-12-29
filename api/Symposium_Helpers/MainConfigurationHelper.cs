using Symposium.Models.Models.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers
{
    public static class MainConfigurationHelper
    {
        public static Dictionary<string, Dictionary<string, dynamic>> MainConfigurationDictionary;
        public static Dictionary<string, Dictionary<string, List<DescriptorsModel>>> DescriptorsDictionary;
        public static string apiBaseConfiguration = "api";
        public static string apiDeliveryConfiguration = "da";
        public static string agentConfiguration = "agent";

        /// <summary>
        /// Initialize and fill main configuration dictionary and check for basic configuration
        /// </summary>
        public static void InitializeConfiguration()
        {
            MainConfigurationDictionary = new Dictionary<string, Dictionary<string, dynamic>>();
            string configurationsPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "MainConfiguration";
            List<string> configurationFiles = ReadJsonFileNamesFromPath(configurationsPath);
            FillMainDictionary(MainConfigurationDictionary, configurationFiles);
            bool basicConfigurationChecked = CheckBasicConfiguration();
            if (!basicConfigurationChecked)
            {
                throw new Exception("FILE api.json IS MISSING FROM FOLDER MainConfiguration!");
            }
            DescriptorsDictionary = new Dictionary<string, Dictionary<string, List<DescriptorsModel>>>();
            string descriptorsPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "Descriptors";
            List<string> descriptorFiles = ReadJsonFileNamesFromPath(descriptorsPath);
            FillDescriptorsDictionary(DescriptorsDictionary, descriptorFiles);
            return;
        }

        /// <summary>
        /// Get main configuration based on key
        /// </summary>
        /// <param name="key">Key of main configuration</param>
        /// <returns>Configuration as dictionary</returns>
        public static Dictionary<string, dynamic> GetMainConfiguration(string key)
        {
            Dictionary<string, dynamic> value;
            bool keyExists = MainConfigurationDictionary.TryGetValue(key, out value);
            if (!keyExists)
            {
                throw new BusinessException("File " + key + ".json does not exist!");
            }
            return value;
        }

        /// <summary>
        /// Get sub configuration based on keys
        /// </summary>
        /// <param name="mainKey">Key of main configuration</param>
        /// <param name="subKey">Key of sub configuration</param>
        /// <returns>Configuration as dynamic</returns>
        public static dynamic GetSubConfiguration(string mainKey, string subKey)
        {
            dynamic subValue = null;
            Dictionary<string, dynamic> mainValue = GetMainConfiguration(mainKey);
            if (mainValue != null)
            {
                bool subKeyExists = mainValue.TryGetValue(subKey, out subValue);
                if (!subKeyExists)
                {
                    throw new Exception("Field " + subKey + " does not exist in file " + mainKey + ".json!");
                }
            }
            return subValue;
        }

        /// <summary>
        /// Find available json files in given folder path and return a list containing each json file's full pathname
        /// </summary>
        /// <param name="path">Folder path to find json files</param>
        /// <returns>List of json files' full pathnames</returns>
        private static List<string> ReadJsonFileNamesFromPath(string folderpath)
        {
            if (!Directory.Exists(folderpath))
            {
                DirectoryInfo di = Directory.CreateDirectory(folderpath);
            }
            List<string> fullFilenames = new List<string>();
            foreach (string fullFilename in Directory.EnumerateFiles(folderpath, "*.json", SearchOption.AllDirectories))
            {
                fullFilenames.Add(fullFilename);
            }
            return fullFilenames;
        }

        /// <summary>
        /// Fill main dictionary
        /// </summary>
        /// <param name="dictionary">Dictionary to fill</param>
        /// <param name="jsonFiles">List of configuration file pathnames to read and store</param>
        private static void FillMainDictionary(Dictionary<string, Dictionary<string, dynamic>> dictionary, List<string> jsonFiles)
        {
            lock (dictionary)
            {
                foreach (string jsonFile in jsonFiles)
                {
                    string configurationKey = GetFileNameFromFilePath(jsonFile);
                    Dictionary<string, dynamic> configurationValue = FillSubDictionary(jsonFile);
                    foreach(string s in configurationValue.Keys.ToList())
                    {
                        if(configurationValue[s] == null)
                        {
                            configurationValue[s] = "";
                        }
                    }
                    dictionary.Add(configurationKey, configurationValue);
                }
            }
            return;
        }

        /// <summary>
        /// Fill sub dictionary
        /// </summary>
        /// <param name="jsonFile">Configuration file pathname to read and store</param>
        /// <returns>Filled dictionary</returns>
        private static Dictionary<string, dynamic> FillSubDictionary(string jsonFile)
        {
            Dictionary<string, dynamic> dictionary;
            string rawData = File.ReadAllText(jsonFile, Encoding.Default);
            dictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(rawData);
            return dictionary;
        }

        /// <summary>
        /// Check if basic configuration json files exist
        /// </summary>
        /// <returns>Passed check or no</returns>
        private static bool CheckBasicConfiguration()
        {
            bool configurationsExist = true;
            Dictionary<string, dynamic> configuration;
            bool apiConfigurationExists = MainConfigurationDictionary.TryGetValue(apiBaseConfiguration, out configuration);
            if (!apiConfigurationExists)
            {
                configurationsExist = false;
            }
            bool daConfigurationExists = MainConfigurationDictionary.TryGetValue(apiDeliveryConfiguration, out configuration);
            if (!daConfigurationExists)
            {
                MainConfigurationDictionary.Add(apiDeliveryConfiguration, null);
            }
            bool agentConfigurationExists = MainConfigurationDictionary.TryGetValue(agentConfiguration, out configuration);
            if (!agentConfigurationExists)
            {
                MainConfigurationDictionary.Add(agentConfiguration, null);
            }
            return configurationsExist;
        }

        /// <summary>
        /// Fill descriptor dictionary
        /// </summary>
        /// <param name="dictionary">Descriptors dictionary to fill</param>
        /// <param name="jsonFiles">List of configuration file pathnames to read and store</param>
        private static void FillDescriptorsDictionary(Dictionary<string, Dictionary<string, List<DescriptorsModel>>> dictionary, List<string> jsonFiles)
        {
            foreach (string jsonFile in jsonFiles)
            {
                string configurationKey = GetFileNameFromFilePath(jsonFile);
                Dictionary<string, List<DescriptorsModel>> configurationValue = FillDescriptorsSubDictionary(jsonFile);
                dictionary.Add(configurationKey, configurationValue);
            }
            return;
        }

        /// <summary>
        /// Fill descriptor sub dictionary
        /// </summary>
        /// <param name="jsonFile">Descriptor file pathname to read and store</param>
        /// <returns>Filled descriptor dictionary</returns>
        private static Dictionary<string, List<DescriptorsModel>> FillDescriptorsSubDictionary(string jsonFile)
        {
            Dictionary<string, List<DescriptorsModel>> dictionary;
            string rawData = File.ReadAllText(jsonFile, Encoding.Default);
            dictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<DescriptorsModel>>>(rawData);
            return dictionary;
        }

        /// <summary>
        /// Get file name from file path
        /// </summary>
        /// <param name="pathName">File path</param>
        /// <returns>File name</returns>
        private static string GetFileNameFromFilePath(string pathName)
        {
            string[] pathNameComponents = pathName.Split('\\');
            string fileName = pathNameComponents[pathNameComponents.Length - 1];
            string[] fileNameComponents = fileName.Split('.');
            string name = fileNameComponents[0];
            return name;
        }

    }
}
