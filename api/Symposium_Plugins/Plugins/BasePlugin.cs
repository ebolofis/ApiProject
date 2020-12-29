
using log4net;
using Newtonsoft.Json;
using Symposium.Models.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Symposium.Plugins
{
    public abstract class BasePlugin
    {
        /// <summary>
        /// plugin's configuration
        /// </summary>
        public Dictionary<string, dynamic> Config;
        /// <summary>
        /// plugin's descriptor
        /// </summary>
        public Dictionary<string, List<DescriptorsModel>> Descriptor;

        /// <summary>
        /// full path for settings.json
        /// </summary>
        protected string settingsPath;
        /// <summary>
        /// full path for descriptor.json
        /// </summary>
        protected string descriptorPath;

        /// <summary>
        /// folder Path
        /// </summary>
        protected string folderPath;


        public BasePlugin()
        {
            getPaths();
            readConfig();
        }

        /// <summary>
        /// Plugin unique name based on Plugin Interface, ex: GoogleMaps and TerraMaps plugins will have CategoryName='MapGeocodePlugin'.
        /// If Plugin Interface is not suitable for CategoryName then another description should be selected.
        /// MUST ALWAYS ends WITH suffix 'Plugin' otherwise backoffice will do not recognize as plugin config  ...  
        /// </summary>
        public abstract string CategoryName();

        /// <summary>
        /// Plugin's version
        /// </summary>
        public abstract string Version();

        /// <summary>
        /// Plugin's path
        /// </summary>
        /// <returns></returns>
        public abstract string PluginBasePath();

        /// <summary>
        /// Add a main dictionary key with plugin's configuration. key must be the unique plugin name
        /// </summary>
        /// <param name="configurationDictionary">main configuration dictionary. Key SHOULD BE the Name of the plugin</param>
        /// <param name="descriptorsDictionary">descriptors dictionary. Key SHOULD BE the Name of the plugin</param>
        /// <param name="logger"></param>
        public virtual void AppendConfiguration(Dictionary<string, Dictionary<string, dynamic>> configurationDictionary, Dictionary<string, Dictionary<string, List<DescriptorsModel>>> descriptorsDictionary, ILog logger)
        {
            if (Config != null && Config.Keys.Count > 0)
            {
                if (configurationDictionary.ContainsKey(CategoryName())) throw new Exception($"ConfigurationDictionary already contains key '{CategoryName()}'. See plugin {PluginBasePath()}");
                lock (configurationDictionary)
                {
                    configurationDictionary.Add(CategoryName(), Config);
                }
            }

            if (Descriptor != null && Descriptor.Keys.Count > 0) descriptorsDictionary.Add(CategoryName(), Descriptor);
        }

        public virtual void SaveConfiguration(Dictionary<string, Dictionary<string, dynamic>> configurationDictionary, ILog logger)
        {
            string json = JsonConvert.SerializeObject(configurationDictionary[CategoryName()]);
            System.IO.File.WriteAllText(settingsPath, json, Encoding.Default);
            Config = configurationDictionary[CategoryName()];
        }

        protected void getPaths()
        {
            string codeBase = PluginBasePath();
            codeBase = codeBase.Substring(0, codeBase.LastIndexOf("/"));
            var uri = new Uri(codeBase).LocalPath;
            settingsPath = uri + "\\settings.json";
            descriptorPath = uri + "\\descriptor.json";
        }

        protected void readConfig()
        {
            string conf = System.IO.File.ReadAllText(settingsPath, Encoding.Default);
            Config = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(conf);

            string descr = System.IO.File.ReadAllText(descriptorPath, Encoding.Default);
            Descriptor = JsonConvert.DeserializeObject<Dictionary<string, List<DescriptorsModel>>>(descr);

        }

    }
}
