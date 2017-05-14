using System.Configuration;

namespace CashCenter.Common
{
	public static class Config
	{
        public static string DbConnectionStringFormat => GetAppSettingByKey("DbConnectionStringFormat", string.Empty);
        public static string DbfConnectionStringFormat => GetAppSettingByKey("DbfConnectionStringFormat", string.Empty);
        public static string DbCodeMappingXmlPath => GetAppSettingByKey("DbCodeMappingXmlPath", "DbCodeMapping.xml");
        public static string OutputDirectory => GetAppSettingByKey("OutputDirectory", string.Empty);
        public static string EnergyCustomerOffOutputFileFormat => GetAppSettingByKey("EnergyCustomerOffOutputFileFormat", string.Empty);
        public static string WaterCustomerDbfOutputFileFormat => GetAppSettingByKey("WaterCustomerDbfOutputFileFormat", string.Empty);
        public static string CustomersReportTemplateFilename => GetAppSettingByKey("CustomersReportTemplateFilename", string.Empty);

        public static int CurrentRegionId => GetAppSettingByKeyAsInt("CurrentRegionId", 100);
		public static int CheckPrinterPassword => GetAppSettingByKeyAsInt("CheckPrinterPassword", 0);
		public static int CheckPrinterMaxLineLength => GetAppSettingByKeyAsInt("CheckPrinterMaxLineLength", 10);
		public static int CheckPrinterCountEmptyLinesAfterCheck => GetAppSettingByKeyAsInt("CheckPrinterCountEmptyLinesAfterCheck", 0);

        public static string SalesMainInfo => GetAppSettingByKey("SalesMainInfo", string.Empty);
        public static string SalesDepartmentInfo => GetAppSettingByKey("SalesDepartmentInfo", string.Empty);

        public static int NdsPercent => GetAppSettingByKeyAsInt("NdsPercent", 0);

        private static string GetAppSettingByKey(string key, string defaultValue)
		{
			return ConfigurationManager.AppSettings[key] ?? defaultValue;
		}

		private static int GetAppSettingByKeyAsInt(string key, int defaultValue)
		{
			var stringValue = GetAppSettingByKey(key, defaultValue.ToString());

			if (!int.TryParse(stringValue, out int result))
				result = defaultValue;

			return result;
		}

        private static bool GetAppSettingByKeyAsBool(string key, bool defaultValue)
        {
            var stringValue = GetAppSettingByKey(key, defaultValue.ToString());

            if (!bool.TryParse(stringValue, out bool result))
                result = defaultValue;

            return result;
        }
    }
}
