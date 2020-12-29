using AutoMapper;
using log4net;
using Newtonsoft.Json;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
using Symposium.Models.Models;
using Symposium.Models.Models.Configuration;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Plugins;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Symposium.WebApi.MainLogic.Tasks.Configuration
{
    public class ConfigurationTasks : IConfigurationTasks
    {
        public ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IPosInfoDT posInfoDT;
        PluginHelper pluginHelper = new PluginHelper();

        List<string> configurationList = new List<string>();


        public ConfigurationTasks(IPosInfoDT _posInfoDT)
        {
            this.posInfoDT = _posInfoDT;
        }

        /// <summary>
        /// Refresh main configuration
        /// </summary>
        public void RefreshMainConfiguration()
        {
            MainConfigurationHelper.InitializeConfiguration();
            return;
        }

        /// <summary>
        /// Append plugin configuration
        /// </summary>
        public void AppendPluginConfiguration()
        {
            pluginHelper.AppendWithPluginConfiguration();
            return;
        }

        /// <summary>
        /// Get configuration based on key from dictionary
        /// </summary>
        /// <param name="key">Key of configuration in dictionary</param>
        /// <returns>Configuration as dictionary</returns>
        public Dictionary<string, dynamic> GetConfiguration(string key)
        {
            Dictionary<string, dynamic> configuration = MainConfigurationHelper.GetMainConfiguration(key);
            return configuration;
        }

        /// <summary>
        /// Get All configuration (MainConfigurationDictionary)
        /// </summary>
        /// <returns>MainConfigurationDictionary</returns>
        public Dictionary<string, Dictionary<string, dynamic>> GetAllConfiguration()
        {
            Dictionary<string, Dictionary<string, dynamic>> configuration = MainConfigurationHelper.MainConfigurationDictionary;
            return configuration;
        }

        /// <summary>
        /// Get All Descriptors configuration
        /// </summary>
        /// <returns>All Descriptors Configuration</returns>
        public Dictionary<string, Dictionary<string, List<DescriptorsModel>>> GetAllDescriptors()
        {
            Dictionary<string, Dictionary<string, List<DescriptorsModel>>> descriptors = MainConfigurationHelper.DescriptorsDictionary;
            return descriptors;
        }

        /// <summary>
        /// Get All Configuration From BO
        /// </summary>
        /// <param name="NewMainConfigurationDictionary">New Configuration Model</param>
        /// <returns>true for successful update or false for fail</returns>
        public bool SaveAllChanges(Dictionary<string, Dictionary<string, dynamic>> NewMainConfigurationDictionary)
        {
            bool res = false;
            //1. Save Configuration Json files
            string basePath = System.Web.HttpContext.Current.Server.MapPath("~/") + "MainConfiguration";
            List<string> configurationFiles = ReadJsonFileNamesFromPath(basePath);
            foreach (string jsonFile in configurationFiles)
            {
                string[] pathNameComponents = jsonFile.Split('\\');
                string fileName = pathNameComponents[pathNameComponents.Length - 1];
                string[] fieNameComponents = fileName.Split('.');
                string configurationKey = fieNameComponents[0];
                foreach (string fName in NewMainConfigurationDictionary.Keys)
                {
                    if (configurationKey == fName)
                    {
                        try
                        {
                            string json = JsonConvert.SerializeObject(NewMainConfigurationDictionary[fName]);
                            System.IO.File.WriteAllText(jsonFile, json, Encoding.Default);
                            res = true;
                        }
                        catch
                        {
                            res = false;
                        }
                    }
                }
            }

            //2.Save Plugin Configuration
            pluginHelper.SaveConfiguration(NewMainConfigurationDictionary);

            //3.Refresh Main Configuration
            RefreshMainConfiguration();

            //4.Refresh Plugin Configuration
            AppendPluginConfiguration();

            return res;
        }

        /// <summary>
        /// Save Specific Pos Configuration
        /// </summary>
        /// <param name="PosJsonConfigurationDictionary"></param>
        /// <returns>true for successful update or false for fail</returns>
        public bool SaveSpecificPos(Dictionary<string, Dictionary<string, dynamic>> PosJsonConfigurationDictionary)
        {
            bool res = false;
            //1. Save Configuration Json files
            string basePath = System.Web.HttpContext.Current.Server.MapPath("~/") + "MainConfiguration";
            List<string> configurationFiles = ReadJsonFileNamesFromPath(basePath);
            foreach (string jsonFile in configurationFiles)
            {
                string[] pathNameComponents = jsonFile.Split('\\');
                string fileName = pathNameComponents[pathNameComponents.Length - 1];
                string[] fieNameComponents = fileName.Split('.');
                string configurationKey = fieNameComponents[0];
                foreach (string fName in PosJsonConfigurationDictionary.Keys)
                {
                    if (configurationKey == fName)
                    {
                        try
                        {
                            string json = JsonConvert.SerializeObject(PosJsonConfigurationDictionary[fName]);
                            System.IO.File.WriteAllText(jsonFile, json, Encoding.Default);
                            res = true;
                        }
                        catch
                        {
                            res = false;
                        }
                    }
                }
            }

            //3.Refresh Main Configuration
            RefreshMainConfiguration();

            //4.Refresh Plugin Configuration
            AppendPluginConfiguration();

            return res;
        }

        /// <summary>
        /// Get All Configuration Files from PosInfo
        /// </summary>
        /// <returns>List of Pos Info Configuration Files</returns>
        public List<string> GetAllPosInfoConfig(DBInfoModel DBInfo)
        {
            return posInfoDT.GetAllPosInfoConfig(DBInfo);
        }

        /// <summary>
        /// Add Pos Configiguration File
        /// </summary>
        /// <param name="posJsonFile"></param>
        /// <returns>true for success or false for fail</returns>
        public bool AddPosConfig(string posJsonFile)
        {
            bool res = false;

            //Add File
            try
            {
                List<string> fileNames = new List<string>();
                string basePath = System.Web.HttpContext.Current.Server.MapPath("~/") + "MainConfiguration\\Pos";
                string pathWithFileName = basePath + "\\" + posJsonFile + ".json";
                System.IO.File.Create(pathWithFileName).Dispose();
                object emptyObj = new object();
                string json = JsonConvert.SerializeObject(emptyObj);
                System.IO.File.WriteAllText(pathWithFileName, json, Encoding.Default);
                res = true;
            }
            catch
            {
                res = false;
            }

            return res;
        }


        /// <summary>
        /// Delete Selected Pos Configiguration File
        /// </summary>
        /// <param name="PosConfigFile"></param>
        /// <returns>true for success or false for fail</returns>
        public bool DeletePosConfig(DBInfoModel DBInfo, string PosConfigFile)
        {
            bool res = false;
            bool isConfigInUse = false;
            //Check if file is in use
            configurationList = GetAllPosInfoConfig(DBInfo);
            foreach (string config in configurationList)
            {
                if (config == PosConfigFile)
                {
                    isConfigInUse = true;
                }
            }
            if (isConfigInUse == false)
            {
                //Delete File
                string basePath = System.Web.HttpContext.Current.Server.MapPath("~/") + "MainConfiguration";
                List<string> configurationFiles = ReadJsonFileNamesFromPath(basePath);
                foreach (string jsonFile in configurationFiles)
                {
                    string[] pathNameComponents = jsonFile.Split('\\');
                    string fileName = pathNameComponents[pathNameComponents.Length - 1];
                    string[] fieNameComponents = fileName.Split('.');
                    string configurationKey = fieNameComponents[0];
                    if (configurationKey == PosConfigFile)
                    {
                        try
                        {
                            System.IO.File.Delete(jsonFile);
                            res = true;
                        }
                        catch
                        {
                            res = false;
                        }
                    }
                }

                //3.Refresh Main Configuration
                RefreshMainConfiguration();

                //4.Refresh Plugin Configuration
                AppendPluginConfiguration();
            }
            else
            {
                res = false;
            }
            return res;
        }

        /// <summary>
        /// Get pos configuration file names
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllPosFiles()
        {
            List<string> fileNames = new List<string>();
            string basePath = System.Web.HttpContext.Current.Server.MapPath("~/") + "MainConfiguration\\Pos";
            List<string> configurationFiles = ReadJsonFileNamesFromPath(basePath);
            foreach (string file in configurationFiles)
            {
                string[] pathNameComponents = file.Split('\\');
                string fullFileName = pathNameComponents[pathNameComponents.Length - 1];
                string[] fileNameComponents = fullFileName.Split('.');
                string fileName = fileNameComponents[0];
                fileNames.Add(fileName);
            }
            return fileNames;
        }

        /// <summary>
        /// Get store id and first pos id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        public DA_GetStorePosModel GetStorePos(DBInfoModel dbInfo)
        {
            DA_GetStorePosModel getModel = new DA_GetStorePosModel();

            string StoreIdRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_StoreId");
            getModel.StoreId = StoreIdRaw.Trim().ToLower();
            getModel.PosId = posInfoDT.GetFirstPosInfoId(dbInfo);

            return getModel;
        }

        /// <summary>
        ///  Get cancel statuses
        /// </summary>
        /// <returns></returns>
        public List<string> GetCancelStatus()
        {
            string DACancelStatusesRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_Cancel");
            string DACancelStatuses = DACancelStatusesRaw.Trim();
            List<string> DACancelStatusesList = DACancelStatuses.Split(new char[] { ',' }).ToList();
            return DACancelStatusesList;
        }


        /// <summary>
        /// Find available json files in given folder path and return a list containing each json file's full pathname
        /// </summary>
        /// <param name="path">Folder path to find json files</param>
        /// <returns>List of json files' full pathnames</returns>
        private List<string> ReadJsonFileNamesFromPath(string folderpath)
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

    }
}
