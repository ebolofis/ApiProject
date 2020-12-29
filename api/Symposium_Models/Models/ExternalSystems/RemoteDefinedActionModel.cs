using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Symposium.Models.Models
{
    public class RemoteDefinedActionModel
    {
        [XmlElement("Action")]
        public List<Action> Actions { get; set; }
    }

    public class Action 
    {
        [XmlAttribute("Id")]
        public string Id { get; set; }
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlAttribute("Description")]
        public string Description { get; set; }
        [XmlAttribute("Information")]
        public string Information { get; set; }
        [XmlAttribute("Query")]
        public string Query { get; set; }
        [XmlAttribute("Qtype")]
        public string Qtype { get; set; }

        [XmlAttribute("SecurityPolicy")]
        public string SecurityPolicy { get; set; }
    }
}
