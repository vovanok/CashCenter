using System.Configuration;

namespace CashCenter.IvEnergySales
{
	public static class Config
	{
		public const string DBCODE_MAPPING_XML_PATH = "DbCodeMappingXmlPath";

        public const string CURRENT_DEPARTMENT_CODE = "CurrentDepartmentCode";
        
        public static string DbCodeMappingXmlPath => GetAppSettingByKey(DBCODE_MAPPING_XML_PATH, "DbCodeMapping.xml");

        public static string CurrentDepartmentCode => GetAppSettingByKey(CURRENT_DEPARTMENT_CODE, "0000");
        
        private static string GetAppSettingByKey(string key, string defaultValue)
		{
			return ConfigurationManager.AppSettings[key] ?? defaultValue;
		}
	}
}