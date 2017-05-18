using CashCenter.DataMigration.Import;

namespace CashCenter.IvEnergySales.DataMigrationControls
{
    public class ImportTargetItem
    {
        public bool IsChecked { get; set; }
        public string Name { get; private set; }
        public IImporter Importer { get; private set; }
        public bool IsNeedDepartmentInfo { get; private set; }

        public ImportTargetItem(string name, IImporter importer, bool isNeedDepartmentInfo)
        {
            IsChecked = false;
            Name = name;
            Importer = importer;
            IsNeedDepartmentInfo = isNeedDepartmentInfo;
        }
    }
}
