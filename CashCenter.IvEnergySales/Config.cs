using System.Configuration;

namespace CashCenter.IvEnergySales
{
	public static class Config
	{
		public const string DBCODE_MAPPING_XML_PATH = "DbCodeMappingXmlPath";
		public const string CURRENT_DEPARTMENT_CODE = "CurrentDepartmentCode";
		public const string CHECK_PRINTER_PASSWORD = "CheckPrinterPassword";
		public const string CHECK_PRINTER_MAX_LINE_LENGTH = "CheckPrinterMaxLineLength";
		public const string CHECK_PRINTER_COUNT_EMPTY_LINES_AFTER_CHECK = "CheckPrinterCountEmptyLinesAfterCheck";

		public static string DbCodeMappingXmlPath => GetAppSettingByKey(DBCODE_MAPPING_XML_PATH, "DbCodeMapping.xml");

        public static string CurrentDepartmentCode => GetAppSettingByKey(CURRENT_DEPARTMENT_CODE, "0000");

		public static int CheckPrinterPassword => GetAppSettingByKeyAsInt(CHECK_PRINTER_PASSWORD, 0);

		public static int CheckPrinterMaxLineLength => GetAppSettingByKeyAsInt(CHECK_PRINTER_MAX_LINE_LENGTH, 10);

		public static int CheckPrinterCountEmptyLinesAfterCheck 
			=> GetAppSettingByKeyAsInt(CHECK_PRINTER_COUNT_EMPTY_LINES_AFTER_CHECK, 0);

        private static string GetAppSettingByKey(string key, string defaultValue)
		{
			return ConfigurationManager.AppSettings[key] ?? defaultValue;
		}

		private static int GetAppSettingByKeyAsInt(string key, int defaultValue)
		{
			var stringValue = GetAppSettingByKey(key, defaultValue.ToString());

			int result;
			if (!int.TryParse(stringValue, out result))
				result = defaultValue;

			return result;
		}
	}
}
