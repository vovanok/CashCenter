using System.Configuration;

namespace CashCenter.IvEnergySales
{
	public static class Config
	{
        private const string DB_CONNECTION_STRING_FORMAT = "DbConnectionStringFormat";
        private const string DBCODE_MAPPING_XML_PATH = "DbCodeMappingXmlPath";
        private const string IS_USE_OFFLINE_MODE = "IsUseOfflineMode";
        private const string OUTPUT_PATH = "OutputPath";
        private const string CUSTOMER_OUTPUT_FILE_FORMAT = "CustomerOutputFileFormat";

        private const string CURRENT_DEPARTMENT_CODE = "CurrentDepartmentCode";
        private const string CHECK_PRINTER_PASSWORD = "CheckPrinterPassword";
        private const string CHECK_PRINTER_MAX_LINE_LENGTH = "CheckPrinterMaxLineLength";
        private const string CHECK_PRINTER_COUNT_EMPTY_LINES_AFTER_CHECK = "CheckPrinterCountEmptyLinesAfterCheck";

        public const string SALES_DEPARTAMENT_INFO = "SalesDepartmentInfo";
        public const string CASHIER_NAME = "CashierName";

        public static string DbConnectionStringFormat => GetAppSettingByKey(DB_CONNECTION_STRING_FORMAT, string.Empty);
        public static string DbCodeMappingXmlPath => GetAppSettingByKey(DBCODE_MAPPING_XML_PATH, "DbCodeMapping.xml");
        public static bool IsUseOfflineMode => GetAppSettingByKeyAsBool(IS_USE_OFFLINE_MODE, false);
        public static string OutputPath => GetAppSettingByKey(OUTPUT_PATH, string.Empty);
        public static string CustomerOutputFileFormat => GetAppSettingByKey(CUSTOMER_OUTPUT_FILE_FORMAT, string.Empty);

        public static string CurrentDepartmentCode => GetAppSettingByKey(CURRENT_DEPARTMENT_CODE, "0000");
		public static int CheckPrinterPassword => GetAppSettingByKeyAsInt(CHECK_PRINTER_PASSWORD, 0);
		public static int CheckPrinterMaxLineLength => GetAppSettingByKeyAsInt(CHECK_PRINTER_MAX_LINE_LENGTH, 10);
		public static int CheckPrinterCountEmptyLinesAfterCheck => GetAppSettingByKeyAsInt(CHECK_PRINTER_COUNT_EMPTY_LINES_AFTER_CHECK, 0);

        public static string SalesDepartmentInfo => GetAppSettingByKey(SALES_DEPARTAMENT_INFO, string.Empty);
        public static string CashierName => GetAppSettingByKey(CASHIER_NAME, string.Empty);

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
