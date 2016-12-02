using System.Collections.Generic;
using System.Xml.Serialization;

namespace CashCenter.IvEnergySales.DbQualification
{
	[XmlRoot("DbQualifier")]
	public class DbQualifierModel
	{
		[XmlArray("Departments")]
		[XmlArrayItem("Department")]
		public List<DepartmentModel> Departments { get; set; }

		public DbQualifierModel()
		{
			Departments = new List<DepartmentModel>();
		}
	}
}
