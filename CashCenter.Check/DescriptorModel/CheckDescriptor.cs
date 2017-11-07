using System.Collections.Generic;
using System.Xml.Serialization;

namespace CashCenter.Check.DescriptorModel
{
    public class CheckDescriptor
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("paySection")]
        public int PaySection { get; set; }

        [XmlArray("Lines")]
        [XmlArrayItem("Line")]
        public List<CheckLineDescriptor> Lines { get; set; }
    }
}
