using System.Collections.Generic;
using System.Xml.Serialization;

namespace CashCenter.IvEnergySales.DbCodeMapping
{
	[XmlRoot("DbCodeMapping")]
	public class DbCodeMappingModel
	{
		[XmlArray("Departments")]
		[XmlArrayItem("Department")]
		public List<DepartmentModel> Departments { get; set; }

		public DbCodeMappingModel()
		{
			Departments = new List<DepartmentModel>();
		}
	}
}
