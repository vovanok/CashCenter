using System;
using System.Configuration;

namespace CashCenter.Common
{
	public static class Config
	{
        public static readonly DateTime DeathDate = new DateTime(2017, 10, 1);

        public static string DbfConnectionStringFormat => GetAppSettingByKey("DbfConnectionStringFormat", string.Empty);
        public static string OutputDirectory => GetAppSettingByKey("OutputDirectory", string.Empty);
        public static string EnergyCustomerOffOutputFileFormat => GetAppSettingByKey("EnergyCustomerOffOutputFileFormat", string.Empty);
        public static string WaterCustomerDbfOutputFileFormat => GetAppSettingByKey("WaterCustomerDbfOutputFileFormat", string.Empty);
        public static string ArticlesDbfOutputFileFormat => GetAppSettingByKey("ArticlesDbfOutputFileFormat", string.Empty);
        public static string EnergyCustomersReportTemplateFilename => GetAppSettingByKey("EnergyCustomersReportTemplateFilename", string.Empty);
        public static string WaterCustomersReportTemplateFilename => GetAppSettingByKey("WaterCustomersReportTemplateFilename", string.Empty);
        public static string ArticlesSalesReportTemplateFilename => GetAppSettingByKey("ArticlesSalesReportTemplateFilename", string.Empty);
        public static int CurrentRegionId => GetAppSettingByKeyAsInt("CurrentRegionId", 100);
		public static int CheckPrinterPassword => GetAppSettingByKeyAsInt("CheckPrinterPassword", 0);
		public static int CheckPrinterMaxLineLength => GetAppSettingByKeyAsInt("CheckPrinterMaxLineLength", 10);
		public static int CheckPrinterCountEmptyLinesAfterCheck => GetAppSettingByKeyAsInt("CheckPrinterCountEmptyLinesAfterCheck", 0);
        public static int NdsPercent => GetAppSettingByKeyAsInt("NdsPercent", 0);
        public static string ChecksFilename => GetAppSettingByKey("ChecksFilename", string.Empty);
        public static bool IsShowWaterPayments => GetAppSettingByKeyAsBool("IsShowWaterPayments", false);
        public static bool IsShowArticles => GetAppSettingByKeyAsBool("IsShowArticles", false);

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
