using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using CashCenter.IvEnergySales.Logging;

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
					mappingModel = LoadDbCodeMappingModel();

				return mappingModel;
			}
		}

		public static DepartmentModel GetCurrentDepartment()
        {
            var departmentCode = Config.CurrentDepartmentCode;
            var targetDepartment = MappingModel.Departments.FirstOrDefault(item => item.DepartmentCode == departmentCode);
            if (targetDepartment != null)
                return targetDepartment;

            Log.Error($"Отделение с кодом {departmentCode} не найдено. Отредактируйте ключ {Config.CURRENT_DEPARTMENT_CODE} раздела appSettings в App.config");
            return new DepartmentModel();
        }

		private static DbCodeMappingModel LoadDbCodeMappingModel()
		{
			var xmlFilePath = Config.DbCodeMappingXmlPath;
			if (!File.Exists(xmlFilePath))
			{
				Log.Error($"XML с базами данных по отделениям не найдены. Путь к файлу: {xmlFilePath}. Создайте файл или отредатируйте ключ {Config.DBCODE_MAPPING_XML_PATH} раздела appSettings в App.config.");
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
				Log.Error($"Ошибка загрузки XML с базами данных по отделениям:\n{e.Message}\nStack trace:\n{e.StackTrace}");
				return new DbCodeMappingModel();
			}
		}
	}
}
