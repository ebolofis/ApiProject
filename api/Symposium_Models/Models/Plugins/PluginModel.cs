using System;
using System.Collections.Generic;
using System.Reflection;

namespace Symposium.Models.Models.Plugins
{
    public class PluginModel
    {
        /// <summary>
        /// plugin class
        /// </summary>
        public TypeInfo TypeInfo { get; set; }

        /// <summary>
        /// reference assembly
        /// </summary>
        public Assembly Assembly { get; set; }

        /// <summary>
        /// Plugin's unique Category Name
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// Plugin's version
        /// </summary>
        public string Version { get; set; }

        //constructor
        public PluginModel(TypeInfo TypeInfoArray, Assembly assembly)
        {
            this.TypeInfo = TypeInfoArray;
            this.Assembly = assembly;
        }
    }
}
