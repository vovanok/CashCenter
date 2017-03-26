using System.Configuration;

namespace CashCenter.Common
{
	public static class Config
	{
        private const string DB_CONNECTION_STRING_FORMAT = "DbConnectionStringFormat";
        private const string DBF_CONNECTION_STRING_FORMAT = "DbfConnectionStringFormat";
        private const string DBCODE_MAPPING_XML_PATH = "DbCodeMappingXmlPath";
        private const string OUTPUT_DIRECTORY = "OutputDirectory";
        private const string CUSTOMER_OUTPUT_FILE_FORMAT = "CustomerOutputFileFormat";
        private const string ORGANIZATION_OUTPUT_FILE_FORMAT = "OrganizationOutputFileFormat";

        private const string CURRENT_REGION_ID = "CurrentRegionId";
        private const string CHECK_PRINTER_PASSWORD = "CheckPrinterPassword";
        private const string CHECK_PRINTER_MAX_LINE_LENGTH = "CheckPrinterMaxLineLength";
        private const string CHECK_PRINTER_COUNT_EMPTY_LINES_AFTER_CHECK = "CheckPrinterCountEmptyLinesAfterCheck";

        public const string SALES_MAIN_INFO = "SalesMainInfo";
        public const string SALES_DEPARTAMENT_INFO = "SalesDepartmentInfo";

        public const string NDS_PERCENT = "NdsPercent";

        public static string DbConnectionStringFormat => GetAppSettingByKey(DB_CONNECTION_STRING_FORMAT, string.Empty);
        public static string DbfConnectionStringFormat => GetAppSettingByKey(DBF_CONNECTION_STRING_FORMAT, string.Empty);
        public static string DbCodeMappingXmlPath => GetAppSettingByKey(DBCODE_MAPPING_XML_PATH, "DbCodeMapping.xml");
        public static string OutputDirectory => GetAppSettingByKey(OUTPUT_DIRECTORY, string.Empty);
        public static string CustomerOutputFileFormat => GetAppSettingByKey(CUSTOMER_OUTPUT_FILE_FORMAT, string.Empty);
        public static string OrganizationOutputFileFormat => GetAppSettingByKey(ORGANIZATION_OUTPUT_FILE_FORMAT, string.Empty);

        public static int CurrentRegionId => GetAppSettingByKeyAsInt(CURRENT_REGION_ID, 100);
		public static int CheckPrinterPassword => GetAppSettingByKeyAsInt(CHECK_PRINTER_PASSWORD, 0);
		public static int CheckPrinterMaxLineLength => GetAppSettingByKeyAsInt(CHECK_PRINTER_MAX_LINE_LENGTH, 10);
		public static int CheckPrinterCountEmptyLinesAfterCheck => GetAppSettingByKeyAsInt(CHECK_PRINTER_COUNT_EMPTY_LINES_AFTER_CHECK, 0);

        public static string SalesMainInfo => GetAppSettingByKey(SALES_MAIN_INFO, string.Empty);
        public static string SalesDepartmentInfo => GetAppSettingByKey(SALES_DEPARTAMENT_INFO, string.Empty);

        public static int NdsPercent => GetAppSettingByKeyAsInt(NDS_PERCENT, 0);

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
