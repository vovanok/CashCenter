using System;
using System.Configuration;

namespace CashCenter.Common
{
	public static class Config
	{
        public static readonly DateTime DeathDate = new DateTime(2018, 4, 1);

        public static string DbfConnectionStringFormat => GetAppSettingByKey("DbfConnectionStringFormat", string.Empty);
        public static string OutputDirectory => GetAppSettingByKey("OutputDirectory", string.Empty);
        public static string EnergyCustomerOffOutputFileFormat => GetAppSettingByKey("EnergyCustomerOffOutputFileFormat", string.Empty);
        public static string AllPaymentsRfcOutputFileFormat => GetAppSettingByKey("AllPaymentsRfcOutputFileFormat", string.Empty);
        public static string WaterCustomerDbfOutputFileFormat => GetAppSettingByKey("WaterCustomerDbfOutputFileFormat", string.Empty);
        public static string ArticlesDbfOutputFileFormat => GetAppSettingByKey("ArticlesDbfOutputFileFormat", string.Empty);
        public static string HotWaterPaymentTxtOutputFileFormat => GetAppSettingByKey("HotWaterPaymentTxtOutputFileFormat", string.Empty);
        public static string ArticlesSeparatedDbfOutputFileFormat => GetAppSettingByKey("ArticlesSeparatedDbfOutputFileFormat", string.Empty);
        public static string GarbageCollectionPaymentsDbfOutputFileFormat => GetAppSettingByKey("GarbageCollectionPaymentsDbfOutputFileFormat", string.Empty);
        public static string RepairPaymentsDbfOutputFileFormat => GetAppSettingByKey("RepairPaymentsDbfOutputFileFormat", string.Empty);
        public static string EnergyCustomersReportTemplateFilename => GetAppSettingByKey("EnergyCustomersReportTemplateFilename", string.Empty);
        public static string EnergyCustomersReportGisHusTemplateFilename => GetAppSettingByKey("EnergyCustomersReportGisHusTemplateFilename", string.Empty);
        public static string WaterCustomersReportTemplateFilename => GetAppSettingByKey("WaterCustomersReportTemplateFilename", string.Empty);
        public static string ArticlesSalesReportTemplateFilename => GetAppSettingByKey("ArticlesSalesReportTemplateFilename", string.Empty);
        public static string CommonPaymentsReportTemplateFilename => GetAppSettingByKey("CommonPaymentsReportTemplateFilename", string.Empty);
        public static int CurrentRegionId => GetAppSettingByKeyAsInt("CurrentRegionId", 100);
		public static int CheckPrinterPassword => GetAppSettingByKeyAsInt("CheckPrinterPassword", 0);
		public static int CheckPrinterMaxLineLength => GetAppSettingByKeyAsInt("CheckPrinterMaxLineLength", 10);
		public static int CheckPrinterCountEmptyLinesAfterCheck => GetAppSettingByKeyAsInt("CheckPrinterCountEmptyLinesAfterCheck", 0);
        public static NdsPercent EnergyNdsPercent => GetAppSettingByKeyAsNdsPercent("EnergyNdsPercent", NdsPercent.Nds18);
        public static NdsPercent WaterNdsPercent => GetAppSettingByKeyAsNdsPercent("WaterNdsPercent", NdsPercent.Nds18);
        public static NdsPercent ArticlesNdsPercent => GetAppSettingByKeyAsNdsPercent("ArticlesNdsPercent", NdsPercent.Nds18);
        public static NdsPercent HotWaterNdsPercent => GetAppSettingByKeyAsNdsPercent("HotWaterNdsPercent", NdsPercent.Nds18);
        public static string ChecksFilename => GetAppSettingByKey("ChecksFilename", string.Empty);
        public static bool IsShowWaterPayments => GetAppSettingByKeyAsBool("IsShowWaterPayments", false);
        public static bool IsShowArticles => GetAppSettingByKeyAsBool("IsShowArticles", false);
        public static bool IsShowGarbageCollection => GetAppSettingByKeyAsBool("IsShowGarbageCollection", false);
        public static bool IsShowHotWater => GetAppSettingByKeyAsBool("IsShowHotWater", false);

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

        private static NdsPercent GetAppSettingByKeyAsNdsPercent(string key, NdsPercent defaultValue)
        {
            var stringValue = GetAppSettingByKey(key, ((int)defaultValue).ToString());

            if (!Enum.TryParse<NdsPercent>(stringValue, out NdsPercent result))
                result = defaultValue;

            return result;

        }
    }
}
