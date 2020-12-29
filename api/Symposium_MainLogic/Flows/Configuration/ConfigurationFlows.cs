using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.Models.Models.Configuration;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Configuration;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.Configuration
{
    public class ConfigurationFlows : IConfigurationFlows
    {
        IConfigurationTasks configurationTasks;

        public ConfigurationFlows(IConfigurationTasks _configurationTasks)
        {
            this.configurationTasks = _configurationTasks;
        }

        /// <summary>
        /// Refresh configuration
        /// </summary>
        public void RefreshConfiguration()
        {
            configurationTasks.RefreshMainConfiguration();
            configurationTasks.AppendPluginConfiguration();
            return;
        }

        /// <summary>
        /// Get configuration based on key from dictionary
        /// </summary>
        /// <param name="key">Key of configuration in dictionary</param>
        /// <returns>Configuration as dictionary</returns>
        public Dictionary<string, dynamic> GetConfiguration(string key)
        {
            Dictionary<string, dynamic> configuration = configurationTasks.GetConfiguration(key);
            return configuration;
        }

        /// <summary>
        /// Get All configuration (MainConfigurationDictionary)
        /// </summary>
        /// <returns>MainConfigurationDictionary</returns>
        public Dictionary<string, Dictionary<string, dynamic>> GetAllConfiguration()
        {
            Dictionary<string, Dictionary<string, dynamic>> configuration = configurationTasks.GetAllConfiguration();
            return configuration;
        }

        /// <summary>
        /// Get All Descriptors configuration
        /// </summary>
        /// <returns>All Descriptors Configuration</returns>
        public Dictionary<string, Dictionary<string, List<DescriptorsModel>>> GetAllDescriptors()
        {
            Dictionary<string, Dictionary<string, List<DescriptorsModel>>> descriptors = configurationTasks.GetAllDescriptors();
            return descriptors;
        }

        /// <summary>
        /// Get All Configuration From BO
        /// </summary>
        /// <param name="NewMainConfigurationDictionary">New Configuration Model</param>
        /// <returns>true for successful update or false for fail</returns>
        public bool SaveAllChanges(Dictionary<string, Dictionary<string, dynamic>> NewMainConfigurationDictionary)
        {
            return configurationTasks.SaveAllChanges(NewMainConfigurationDictionary);
        }

        /// <summary>
        /// Save Specific Pos Configuration
        /// </summary>
        /// <param name="PosJsonConfigurationDictionary"></param>
        /// <returns>true for successful update or false for fail</returns>
        public bool SaveSpecificPos(Dictionary<string, Dictionary<string, dynamic>> PosJsonConfigurationDictionary)
        {
            return configurationTasks.SaveSpecificPos(PosJsonConfigurationDictionary);
        }

        /// <summary>
        /// Get All Configuration Files from PosInfo
        /// </summary>
        /// <returns>List of Pos Info Configuration Files</returns>
        public List<string> GetAllPosInfoConfig(DBInfoModel DBInfo)
        {
            return configurationTasks.GetAllPosInfoConfig(DBInfo);
        }

        /// <summary>
        /// Add Pos Configiguration File
        /// </summary>
        /// <param name="posJsonFile"></param>
        /// <returns>true for success or false for fail</returns>
        public bool AddPosConfig(string posJsonFile)
        {
            return configurationTasks.AddPosConfig(posJsonFile);
        }

        /// <summary>
        /// Delete Selected Pos Configiguration File
        /// </summary>
        /// <param name="PosConfigFile"></param>
        /// <returns>true for success or false for fail</returns>
        public bool DeletePosConfig(DBInfoModel DBInfo, string PosConfigFile)
        {
            return configurationTasks.DeletePosConfig(DBInfo, PosConfigFile);
        }

        /// <summary>
        /// Get pos configuration file names
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllPosFiles()
        {
            List<string> posFiles = configurationTasks.GetAllPosFiles();
            return posFiles;
        }

        /// <summary>
        /// Get store id and first pos id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        public DA_GetStorePosModel GetStorePos(DBInfoModel dbInfo)
        {
            DA_GetStorePosModel model = configurationTasks.GetStorePos(dbInfo);
            if (model.StoreId != null && model.PosId > 0)
                return model;
            else
                throw new BusinessException(Symposium.Resources.Errors.NOSTOREIDORPOSID);
        }

        /// <summary>
        ///  Get cancel statuses
        /// </summary>
        /// <returns></returns>
        public List<string> GetCancelStatus()
        {
            return configurationTasks.GetCancelStatus();
        }

    }
}
