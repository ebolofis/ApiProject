using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Symposium.Models.Models.Plugins;
using System.Reflection;

using System.Threading.Tasks;
using Symposium.Helpers;
using log4net;

namespace Symposium.Plugins
{
    public class PluginHelper
    {
        //logger
        public  ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //directory where plugin dlls are stored
        public  string ApplicationPluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\Plugins";

        //list of full paths of files of specified type in specified directory, disposed when finished loading dlls
        public  List<string> FilePathsList { get; set; }

        //dll files loaded, persist through program execution
        public  List<Assembly> LoadedPluginList { get; set; }

        //list of plugin models
        public static List<PluginModel> PluginList;

        /// <summary>
        /// collect plugin dlls and load them
        /// </summary>
        public void LoadDLLFiles()
        {
            FilePathsList = new List<string>();

            LoadedPluginList = new List<Assembly>();

            PluginList = new List<PluginModel>();


            bool filesTraversed = FileCollector(ApplicationPluginPath);

            if (filesTraversed)
            {
                foreach (string path in FilePathsList)
                {
                    PluginLoader(path);
                }

                FilePathsList = null;

                CreatePluginReference();
                AppendWithPluginConfiguration();
            }
        }

        /// <summary>
        /// collects full path name of files in specified directory and of specified type
        /// </summary>
        /// <param name="dir">string: directory to search for files</param>
        /// <param name="fileType">string: file type to search</param>
        /// <returns>bool: true if path read successfully, false if fail to read path</returns>
        private bool FileCollector(string dir, string fileType = "dll")
        {
            DirectoryInfo d;

            d = new DirectoryInfo(new Uri(dir).LocalPath);

            if (!d.Exists)
            {
                return false;
            }
            else
            {
                FileInfo[] fileList = d.GetFiles("*." + fileType, SearchOption.AllDirectories);

                foreach (FileInfo file in fileList)
                {
                    FilePathsList.Add(file.FullName);
                }
            }

            return true;
        }

        /// <summary>
        /// load dll file
        /// </summary>
        /// <param name="path">string: path to load dll file from</param>
        private void PluginLoader(string path)
        {
            Assembly LoadedAssembly;

            try
            {
                LoadedAssembly = Assembly.LoadFile(path);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return;
            }

            LoadedPluginList.Add(LoadedAssembly);
        }

        /// <summary>
        /// fill plugin model with information of loaded assemblies
        /// </summary>
        private void CreatePluginReference()
        {
            Type masterPluginInterface = typeof(Plugins.BasePlugin);

            PluginList.Clear();

            foreach (Assembly loadedAssembly in LoadedPluginList)
            {
                logger.Info($"Loading {loadedAssembly.ToString()} ...");
                foreach (TypeInfo type in loadedAssembly.GetTypes().Where(t => !t.IsInterface && !t.IsAbstract).Where(p => masterPluginInterface.IsAssignableFrom(p)))
                {
                    PluginModel pluginInst = new PluginModel(type, loadedAssembly);

                    PluginList.Add(pluginInst);
                }
            }
        }

        /// <summary>
        /// instanciate plugins and append their configuration in Main Configuration
        /// </summary>
        public void AppendWithPluginConfiguration()
        {
            foreach (PluginModel plugin in PluginList)
            {
                Plugins.BasePlugin instance = (Plugins.BasePlugin) Activator.CreateInstance(plugin.TypeInfo);
                plugin.CategoryName = instance.CategoryName();
                plugin.Version = instance.Version();
                instance.AppendConfiguration(MainConfigurationHelper.MainConfigurationDictionary, MainConfigurationHelper.DescriptorsDictionary, logger);
            }
        }

        /// <summary>
        /// instanciate plugins and save new configuration in each plugin settings file
        /// </summary>
        /// <param name="NewMainConfigurationDictionary">Dictionary<string, Dictionary<string, dynamic>>: updated configuration dictionary</param>
        public void SaveConfiguration(Dictionary<string, Dictionary<string, dynamic>> NewMainConfigurationDictionary)
        {
            foreach (PluginModel plugin in PluginList)
            {
                Plugins.BasePlugin instance = (Plugins.BasePlugin)Activator.CreateInstance(plugin.TypeInfo);
                plugin.CategoryName = instance.CategoryName();
                plugin.Version = instance.Version();
                instance.SaveConfiguration(NewMainConfigurationDictionary, logger); 
            }
        }

        /// <summary>
        /// Instanciates first plugin with respective implemented class
        /// </summary>
        /// <param name="implementedClass">Type: implemented class to instanciate</param>
        /// <returns>object: instance of plugin</returns>
        public object InstanciatePlugin(Type implementedClass)
        {
            List<PluginModel> availablePlugins = SearchPluginClassesByClass(implementedClass.Name);
            if (availablePlugins.Count < 1)
            {
                return null;
            }
            PluginModel firstPlugin = availablePlugins[0];
            object instanciatedPlugin = Activator.CreateInstance(firstPlugin.TypeInfo);
            logger.Debug("Instanciated " + implementedClass.ToString() + " plugin.");
            return instanciatedPlugin;
        }

        /// <summary>
        /// Instanciates all plugins with respective implemented class
        /// </summary>
        /// <param name="implementedClass">Type: implemented class to instanciate</param>
        /// <returns>List<object>: instances of plugins</returns>
        public List<object> InstanciatePluginList(Type implementedClass)
        {
            List<object> instanciatedPlugins = new List<object>();
            List<PluginModel> availablePlugins = SearchPluginClassesByClass(implementedClass.Name);
            if (availablePlugins.Count < 1)
            {
                return instanciatedPlugins;
            }
            foreach (PluginModel plugin in availablePlugins)
            {
                object instanciatedPlugin = Activator.CreateInstance(plugin.TypeInfo);
                instanciatedPlugins.Add(instanciatedPlugin);
            }
            logger.Debug("Instanciated " + implementedClass.ToString() + " plugin(s).");
            return instanciatedPlugins;
        }

        /// <summary>
        /// search a list of dll files for classes with implemented class
        /// </summary>
        /// <param name="implementedClass">Type: implemented class to look for</param>
        /// <returns>List<PluginModel>: list of plugin models: classes implementing it</returns>
        private List<PluginModel> SearchPluginClassesByClass(string implementedClass)
        {
           return PluginList.Where(x => x.TypeInfo.BaseType.Name == implementedClass).ToList();
        }

        /// <summary>
        /// search a list of dll files for classes with given category name
        /// </summary>
        /// <param name="categoryName">string: category name to look for</param>
        /// <returns>List<PluginModel>: list of plugin models: classes implementing it</returns>
        private List<PluginModel> SearchPluginClassesByCategoryName(string categoryName)
        {
            return PluginList.Where(x => x.CategoryName == categoryName).ToList();
        }

        /// <summary>
        /// invokes plugin method and returns result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instanciatedClass">object: instance of class</param>
        /// <param name="methodName">string: method to invoke</param>
        /// <param name="parameterTypes">object[]: types of parameters for method</param>
        /// <param name="parameters">object[]: parameters for method</param>
        /// <returns></returns>
        public T InvokePluginMethod<T>(object instanciatedClass, string methodName, Type[] parameterTypes, object[] parameters)
        {
            try
            {
                logger.Debug("Invoking plugin method: " + methodName + ".");
                var method = instanciatedClass.GetType().GetMethod(methodName, parameterTypes);
                return (T)method.Invoke(instanciatedClass, parameters);
            }
            catch (Exception e)
            {
                logger.Error($"Error invoking method {methodName} with {parameterTypes.Count()} params : " + e.ToString());
               throw;
            }
        }

        /// <summary>
        /// invokes plugin method asynchronously and returns result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instanciatedClass">object: instance of class</param>
        /// <param name="methodName">string: method to invoke</param>
        /// <param name="parameterTypes">object[]: types of parameters for method</param>
        /// <param name="parameters">object[]: parameters for method</param>
        /// <returns></returns>
        public async Task<T> InvokePluginMethodAsync<T>(object instanciatedClass, string methodName, Type[] parameterTypes, object[] parameters)
        {
           Task<T> myTask = Task.Run(() => InvokePluginMethod<T>(instanciatedClass, methodName, parameterTypes, parameters));
           // T res = InvokePluginMethod<T>(instanciatedClass, methodName, parameterTypes, parameters);
            return await myTask;
        }

      

    }
}
