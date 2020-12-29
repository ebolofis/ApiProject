using Symposium.Models.Models;
using Symposium.Models.Models.Configuration;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.Configuration
{
    public interface IConfigurationTasks
    {
        /// <summary>
        /// Refresh main configuration
        /// </summary>
        void RefreshMainConfiguration();

        /// <summary>
        /// Append plugin configuration
        /// </summary>
        void AppendPluginConfiguration();

        /// <summary>
        /// Get configuration based on key from dictionary
        /// </summary>
        /// <param name="key">Key of configuration in dictionary</param>
        /// <returns>Configuration as dictionary</returns>
        Dictionary<string, dynamic> GetConfiguration(string key);

        /// <summary>
        /// Get All configuration (MainConfigurationDictionary)
        /// </summary>
        /// <returns>MainConfigurationDictionary</returns>
        Dictionary<string, Dictionary<string, dynamic>> GetAllConfiguration();

        /// <summary>
        /// Get All Descriptors configuration
        /// </summary>
        /// <returns>All Descriptors Configuration</returns>
        Dictionary<string, Dictionary<string, List<DescriptorsModel>>> GetAllDescriptors();


        /// <summary>
        /// Get All Configuration From BO
        /// </summary>
        /// <param name="NewMainConfigurationDictionary">New Configuration Model</param>
        /// <returns>true for successful update or false for fail</returns>
        bool SaveAllChanges(Dictionary<string, Dictionary<string, dynamic>> NewMainConfigurationDictionary);

        /// <summary>
        /// Save Specific Pos Configuration
        /// </summary>
        /// <param name="PosJsonConfigurationDictionary"></param>
        /// <returns>true for successful update or false for fail</returns>
        bool SaveSpecificPos(Dictionary<string, Dictionary<string, dynamic>> PosJsonConfigurationDictionary);

        /// <summary>
        /// Get All Configuration Files from PosInfo
        /// </summary>
        /// <returns>List of Pos Info Configuration Files</returns>
        List<string> GetAllPosInfoConfig(DBInfoModel DBInfo);

        /// <summary>
        /// Get pos configuration file names
        /// </summary>
        /// <returns></returns>
        List<string> GetAllPosFiles();

        /// <summary>
        /// Add Pos Configiguration File
        /// </summary>
        /// <param name="posJsonFile"></param>
        /// <returns>true for success or false for fail</returns>
        bool AddPosConfig(string posJsonFile);

        /// <summary>
        /// Delete Selected Pos Configiguration File
        /// </summary>
        /// <param name="PosConfigFile"></param>
        /// <returns>true for success or false for fail</returns>
        bool DeletePosConfig(DBInfoModel DBInfo, string PosConfigFile);

        /// <summary>
        /// Get store id and first pos id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        DA_GetStorePosModel GetStorePos(DBInfoModel dbInfo);

        /// <summary>
        ///  Get cancel statuses
        /// </summary>
        /// <returns></returns>
        List<string> GetCancelStatus();

    }
}
