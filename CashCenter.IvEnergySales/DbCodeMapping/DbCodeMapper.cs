using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CashCenter.IvEnergySales.DbCodeMapping
{
	class DbCodeMapper
	{
		private static DbCodeMappingModel mappingModel;

		private static DbCodeMappingModel MappingModel
		{
			get
			{
				if (mappingModel == null)
				{
					mappingModel = LoadDbCodeMappingModel();
				}

				return mappingModel;
			}
		}

		public static DbCodeMappingModel Test()
		{
			return MappingModel;
		}

		//public List<DepartmentModel> GetDepartamentsByDbCode(string dbCode)
		//{

		//}

		private static DbCodeMappingModel LoadDbCodeMappingModel()
		{
			var xmlFilePath = Config.DbCodeMappingXmlPath;
			if (!File.Exists(xmlFilePath))
			{
				Log.Log.Error($"XML с базами данных по отделениям не найдены. Путь к файлу: {xmlFilePath}");
				return new DbCodeMappingModel();
			}

			try
			{
				using (var fs = new FileStream(Config.DbCodeMappingXmlPath, FileMode.Open))
				{
					using (var reader = XmlReader.Create(fs))
					{
						var serializer = new XmlSerializer(typeof(DbCodeMappingModel));
						return (DbCodeMappingModel)serializer.Deserialize(reader);
					}
				}
			}
			catch (Exception e)
			{
				Log.Log.Error($"Load DbCodeMappingModel error: {e.Message}\nStack trace:\n{e.StackTrace}");
				return new DbCodeMappingModel();
			}
		}
	}
}
