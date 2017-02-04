using System.Xml.Serialization;

namespace CashCenter.Common.DbQualification
{
	public class DepartmentDef
	{
		[XmlAttribute("code")]
		public string Code { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("url")]
		public string Url { get; set; }

        [XmlAttribute("path")]
        public string Path { get; set; }

        public DepartmentDef()
		{
			Code = string.Empty;
			Url = string.Empty;
            Path = string.Empty;
		}
	}
}
