using CashCenter.DataMigration;

namespace CashCenter.IvEnergySales.DataMigrationControls
{
    public class ExportTargetItem
    {
        public string Name { get; private set; }
        public IExporter Exporter { get; private set; }

        public ExportTargetItem(string name, IExporter exporter)
        {
            Name = name;
            Exporter = exporter;
        }
    }
}
