using System.Collections.Generic;
using System.Xml.Serialization;

namespace CashCenter.Check.DescriptorModel
{
    [XmlRoot("ChecksSet")]
    public class ChecksSetDescriptor
    {
        [XmlArray("Checks")]
        [XmlArrayItem("Check")]
        public List<CheckDescriptor> Checks { get; set; }
    }
}
