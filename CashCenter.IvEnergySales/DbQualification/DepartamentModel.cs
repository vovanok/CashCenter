using System.Collections.Generic;
using System.Xml.Serialization;

namespace CashCenter.IvEnergySales.DbQualification
{
	public class DepartmentModel
	{
		[XmlAttribute("departmentCode")]
		public string DepartmentCode { get; set; }

		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlArray("Dbs")]
		[XmlArrayItem("Db")]
		public List<DbModel> Dbs { get; set; }

		public DepartmentModel()
		{
			DepartmentCode = string.Empty;
			Name = string.Empty;
			Dbs = new List<DbModel>();
		}
	}
}
