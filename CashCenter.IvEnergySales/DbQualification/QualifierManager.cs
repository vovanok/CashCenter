using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using CashCenter.IvEnergySales.Logging;

namespace CashCenter.IvEnergySales.DbQualification
{
	public class QualifierManager
	{
		private static QualifierDef mappingModel;

		private static QualifierDef MappingModel
		{
			get
			{
				if (mappingModel == null)
					mappingModel = LoadDbCodeMappingModel();

				return mappingModel;
			}
		}

		public static RegionDef GetCurrentDepartment()
        {
            var departmentCode = Config.CurrentDepartmentCode;
            var targetDepartment = MappingModel.Regions.FirstOrDefault(item => item.Code == departmentCode);
            if (targetDepartment != null)
                return targetDepartment;

            Log.Error($"Отделение с кодом {departmentCode} не найдено");
            return null;
        }

		private static QualifierDef LoadDbCodeMappingModel()
		{
			var xmlFilePath = Config.DbCodeMappingXmlPath;
			if (!File.Exists(xmlFilePath))
			{
				Log.Error($"XML с базами данных по отделениям не найдены. Путь к файлу: {xmlFilePath}");
				return new QualifierDef();
			}

			try
			{
				using (var fs = new FileStream(Config.DbCodeMappingXmlPath, FileMode.Open))
				{
					using (var reader = XmlReader.Create(fs))
					{
						var serializer = new XmlSerializer(typeof(QualifierDef));
						return (QualifierDef)serializer.Deserialize(reader);
					}
				}
			}
			catch (Exception e)
			{
				Log.Error($"Ошибка загрузки XML с базами данных по отделениям:\n{e.Message}\nStack trace:\n{e.StackTrace}");
				return new QualifierDef();
			}
		}
	}
}
