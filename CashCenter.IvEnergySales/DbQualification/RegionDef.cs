using System.Collections.Generic;
using System.Xml.Serialization;

namespace CashCenter.IvEnergySales.DbQualification
{
	public class RegionDef
	{
		[XmlAttribute("code")]
		public string Code { get; set; }

		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlArray("Departments")]
		[XmlArrayItem("Department")]
		public List<DepartmentDef> Departments { get; set; }

		public RegionDef()
		{
			Code = string.Empty;
			Name = string.Empty;
			Departments = new List<DepartmentDef>();
		}
	}
}
