﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Symposium_Test.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.8.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("{\"DataBase\":\"Pos_NikkiBeach\",\"DataBasePassword\":\"111111\",\"DataBaseUsername\":\"sqla" +
            "dmin\",\"DataSource\":\"sisifos\",\"Id\":\"dd07eeec-752a-45cf-a219-2a868731f089\",\"IsInte" +
            "grated\":\"true\",\"Password\":\"12\",\"Role\":\"role1\",\"Username\":\"12\"}\r\n")]
        public string Store_NikkiBeach {
            get {
                return ((string)(this["Store_NikkiBeach"]));
            }
            set {
                this["Store_NikkiBeach"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public long PosInfo {
            get {
                return ((long)(this["PosInfo"]));
            }
            set {
                this["PosInfo"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("{\"DataBase\":\"DeliveryAgent\",\"DataBasePassword\":\"111111\",\"DataBaseUsername\":\"sqlad" +
            "min\",\"DataSource\":\"SISIFOS\",\"Id\":\"dd07eeec-752a-45cf-a229-2a868731f081\",\"IsInteg" +
            "rated\":\"true\",\"Password\":\"12\",\"Role\":\"role1\",\"Username\":\"12\"}")]
        public string Store_DeliveryAgent {
            get {
                return ((string)(this["Store_DeliveryAgent"]));
            }
            set {
                this["Store_DeliveryAgent"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("{\"DataBase\":\"Pos_NikkiBeach\",\"DataBasePassword\":\"111111\",\"DataBaseUsername\":\"sa\"," +
            "\"DataSource\":\"KMANOLATOS-PC\",\"Id\":\"dd07eeec-752a-45cf-a219-2a868731f089\",\"IsInte" +
            "grated\":\"true\",\"Password\":\"12\",\"Role\":\"role1\",\"Username\":\"12\"}\r\n")]
        public string Store_NikkiBeach_Local {
            get {
                return ((string)(this["Store_NikkiBeach_Local"]));
            }
            set {
                this["Store_NikkiBeach_Local"] = value;
            }
        }
    }
}
