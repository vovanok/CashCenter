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

        public static string EnergyPerformerInn
        {
            get { return Properties.Settings.Default.EnergyPerformerInn; }
            set { Properties.Settings.Default.EnergyPerformerInn = value; }
        }

        public static string EnergyPerformerName
        {
            get { return Properties.Settings.Default.EnergyPerformerName; }
            set { Properties.Settings.Default.EnergyPerformerName = value; }
        }

        public static string EnergyPerformerKpp
        {
            get { return Properties.Settings.Default.EnergyPerformerKpp; }
            set { Properties.Settings.Default.EnergyPerformerKpp = value; }
        }

        public static string EnergyReceiverInn
        {
            get { return Properties.Settings.Default.EnergyReceiverInn; }
            set { Properties.Settings.Default.EnergyReceiverInn = value; }
        }

        public static string EnergyReceiverName
        {
            get { return Properties.Settings.Default.EnergyReceiverName; }
            set { Properties.Settings.Default.EnergyReceiverName = value; }
        }

        public static string EnergyReceiverBankName
        {
            get { return Properties.Settings.Default.EnergyReceiverBankName; }
            set { Properties.Settings.Default.EnergyReceiverBankName = value; }
        }

        public static string EnergyReceiverBankBik
        {
            get { return Properties.Settings.Default.EnergyReceiverBankBik; }
            set { Properties.Settings.Default.EnergyReceiverBankBik = value; }
        }

        public static string EnergyReceiverAccount
        {
            get { return Properties.Settings.Default.EnergyReceiverAccount; }
            set { Properties.Settings.Default.EnergyReceiverAccount = value; }
        }

        public static void Save()
        {
            Properties.Settings.Default.Save();
        }
    }
}