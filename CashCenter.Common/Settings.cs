namespace CashCenter.Common
{
    public static class Settings
    {
        public static string CasherName
        {
            get { return Properties.Settings.Default.CasherName; }
            set { Properties.Settings.Default.CasherName = value; }
        }

        public static int ArticlesDocumentNumberCurrentValue
        {
            get { return Properties.Settings.Default.ArticlesDocumentNumberCurrentValue; }
            set { Properties.Settings.Default.ArticlesDocumentNumberCurrentValue = value; }
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

        public static float WaterСommissionPercent
        {
            get { return Properties.Settings.Default.WaterСommissionPercent; }
            set { Properties.Settings.Default.WaterСommissionPercent = value; }
        }

        public static int GarbageCollectionFilialCode
        {
            get { return Properties.Settings.Default.GarbageCollectionFilialCode; }
            set { Properties.Settings.Default.GarbageCollectionFilialCode = value; }
        }

        public static float GarbageCollectionCommissionPercent
        {
            get { return Properties.Settings.Default.GarbageCollectionCommissionPercent; }
            set { Properties.Settings.Default.GarbageCollectionCommissionPercent = value; }
        }

        public static int RepairFilialCode
        {
            get { return Properties.Settings.Default.RepairFilialCode; }
            set { Properties.Settings.Default.RepairFilialCode = value; }
        }

        public static float RepairCommissionPercent
        {
            get { return Properties.Settings.Default.RepairCommissionPercent; }
            set { Properties.Settings.Default.RepairCommissionPercent = value; }
        }

        public static ManipulatorType ArticlesManipulatorType
        {
            get { return (ManipulatorType)Properties.Settings.Default.ArticlesManipulatorType; }
            set { Properties.Settings.Default.ArticlesManipulatorType = (int)value; }
        }

        public static string ArticlesZeusDbUrl
        {
            get { return Properties.Settings.Default.ArticlesZeusDbUrl; }
            set { Properties.Settings.Default.ArticlesZeusDbUrl = value; }
        }

        public static string ArticlesZeusDbPath
        {
            get { return Properties.Settings.Default.ArticlesZeusDbPath; }
            set { Properties.Settings.Default.ArticlesZeusDbPath = value; }
        }

        public static void Save()
        {
            Properties.Settings.Default.Save();
        }
    }
}
