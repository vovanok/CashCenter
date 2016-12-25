using System.Configuration;

namespace CashCenter.IvEnergySales
{
	public static class Config
	{
        public const string DB_CONNECTION_STRING_FORMAT = "DbConnectionStringFormat";
        public const string DBCODE_MAPPING_XML_PATH = "DbCodeMappingXmlPath";
		public const string CURRENT_DEPARTMENT_CODE = "CurrentDepartmentCode";
		public const string CHECK_PRINTER_PASSWORD = "CheckPrinterPassword";
		public const string CHECK_PRINTER_MAX_LINE_LENGTH = "CheckPrinterMaxLineLength";
		public const string CHECK_PRINTER_COUNT_EMPTY_LINES_AFTER_CHECK = "CheckPrinterCountEmptyLinesAfterCheck";

        public const string RECIPIENT_NAME = "RecipientName";
        public const string RECIPIENT_NAME_SHORT = "RecipientNameShort";
        public const string RECIPIENT_INN = "RecipientInn";
        public const string RECIPIENT_ADDRESS_LINE1 = "RecipientAddressLine1";
        public const string RECIPIENT_ADDRESS_LINE2 = "RecipientAddressLine2";
        public const string SELLER_NAME = "SellerName";
        public const string SELLER_INN = "SellerInn";
        public const string SELLER_ADDRESS_LINE1 = "SellerAddressLine1";
        public const string SELLER_ADDRESS_LINE2 = "SellerAddressLine2";
        public const string CASHIER_NAME = "CashierName";

        public static string DbConnectionStringFormat => GetAppSettingByKey(DB_CONNECTION_STRING_FORMAT, string.Empty);
        public static string DbCodeMappingXmlPath => GetAppSettingByKey(DBCODE_MAPPING_XML_PATH, "DbCodeMapping.xml");
        public static string CurrentDepartmentCode => GetAppSettingByKey(CURRENT_DEPARTMENT_CODE, "0000");
		public static int CheckPrinterPassword => GetAppSettingByKeyAsInt(CHECK_PRINTER_PASSWORD, 0);
		public static int CheckPrinterMaxLineLength => GetAppSettingByKeyAsInt(CHECK_PRINTER_MAX_LINE_LENGTH, 10);
		public static int CheckPrinterCountEmptyLinesAfterCheck => GetAppSettingByKeyAsInt(CHECK_PRINTER_COUNT_EMPTY_LINES_AFTER_CHECK, 0);

        public static string RecipientName => GetAppSettingByKey(RECIPIENT_NAME, string.Empty);
        public static string RecipientNameShort => GetAppSettingByKey(RECIPIENT_NAME_SHORT, string.Empty);
        public static string RecipientInn => GetAppSettingByKey(RECIPIENT_INN, string.Empty);
        public static string RecipientAddressLine1 => GetAppSettingByKey(RECIPIENT_ADDRESS_LINE1, string.Empty);
        public static string RecipientAddressLine2 => GetAppSettingByKey(RECIPIENT_ADDRESS_LINE2, string.Empty);
        public static string SellerName => GetAppSettingByKey(SELLER_NAME, string.Empty);
        public static string SellerInn => GetAppSettingByKey(SELLER_INN, string.Empty);
        public static string SellerAddressLine1 => GetAppSettingByKey(SELLER_ADDRESS_LINE1, string.Empty);
        public static string SellerAddressLine2 => GetAppSettingByKey(SELLER_ADDRESS_LINE2, string.Empty);
        public static string CashierName => GetAppSettingByKey(CASHIER_NAME, string.Empty);

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
