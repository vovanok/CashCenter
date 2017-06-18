namespace CashCenter.Common
{
    public static class Settings
    {
        public static string CasherName
        {
            get { return Properties.Settings.Default.CasherName; }
            set { Properties.Settings.Default.CasherName = value; }
        }

        public static string ArticlesDocumentNumber
        {
            get { return Properties.Settings.Default.ArticlesDocumentNumber; }
            set { Properties.Settings.Default.ArticlesDocumentNumber = value; }
        }

        public static string ArticlesWarehouseCode
        {
            get { return Properties.Settings.Default.ArticlesWarehouseCode; }
            set { Properties.Settings.Default.ArticlesWarehouseCode = value; }
        }

        public static string ArticlesWarehouseName
        {
            get { return Properties.Settings.Default.ArticlesWarehouseName; }
            set { Properties.Settings.Default.ArticlesWarehouseName = value; }
        }

        public static void Save()
        {
            Properties.Settings.Default.Save();
        }
    }
}
