using System.Collections.Generic;
using System.Xml.Serialization;

namespace CashCenter.IvEnergySales.DbQualification
{
	[XmlRoot("Qualifier")]
	public class QualifierDef
	{
		[XmlArray("Regions")]
		[XmlArrayItem("Region")]
		public List<RegionDef> Regions { get; set; }

		public QualifierDef()
		{
			Regions = new List<RegionDef>();
		}
	}
}
