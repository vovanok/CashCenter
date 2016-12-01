using System.Xml.Serialization;

namespace CashCenter.IvEnergySales.DbCodeMapping
{
	public class DbModel
	{
		[XmlAttribute("dbCode")]
		public string DbCode { get; set; }

		[XmlAttribute("url")]
		public string Url { get; set; }

        [XmlAttribute("path")]
        public string Path { get; set; }

        public DbModel()
		{
			DbCode = string.Empty;
			Url = string.Empty;
            Path = string.Empty;
		}
	}
}
