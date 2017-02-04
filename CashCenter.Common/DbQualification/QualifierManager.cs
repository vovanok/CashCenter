using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace CashCenter.Common.DbQualification
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

		public static RegionDef GetCurrentRegion()
        {
            var regionCode = Config.CurrentRegionCode;
            var targetRegion = MappingModel.Regions.FirstOrDefault(item => item.Code == regionCode);
            if (targetRegion != null)
                return targetRegion;

            Log.Error($"Район с кодом {regionCode} не найден");
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
