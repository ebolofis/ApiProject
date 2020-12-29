using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Symposium.Models.Models
{
    public class Installations 
    {
        /// <summary>
        /// Model that represents the list of Stores/Installations into file UsersToDatabases.xml
        /// </summary>
        [XmlElement("Store")]
        public List<DBInfoModel> Stores { get; set; }
    }

    /// <summary>
    /// Model that represents a Store/Installation into file UsersToDatabases.xml. 
    /// Previous name: Store
    /// </summary>
    public class DBInfoModel
    {
        [XmlAttribute("Id")]
        public Guid Id { get; set; }
        [XmlAttribute("Username")]
        public string Username { get; set; }
        [XmlAttribute("Password")]
        public string Password { get; set; }
        [XmlAttribute("Role")]
        public string Role { get; set; }
        [XmlAttribute("DataSource")]
        public string DataSource { get; set; }
        [XmlAttribute("DataBase")]
        public string DataBase { get; set; }
        [XmlAttribute("DataBaseUsername")]
        public string DataBaseUsername { get; set; }
        [XmlAttribute("DataBasePassword")]
        public string DataBasePassword { get; set; }
        [XmlAttribute("IsIntegrated")]
        public string IsIntegrated { get; set; }
    }
}
