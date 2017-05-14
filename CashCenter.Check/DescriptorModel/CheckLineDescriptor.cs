using System.Xml.Serialization;

namespace CashCenter.Check.DescriptorModel
{
    public class CheckLineDescriptor
    {
        public enum Align
        {
            Left,
            Center,
            Repeated
        }

        [XmlAttribute("content")]
        public string Content { get; set; }

        [XmlAttribute("align")]
        public Align ContentAlign { get; set; }
    }
}
