using CashCenter.Common;
using System;
using System.Text;
using System.Windows.Controls;

namespace CashCenter.IvEnergySales.DataMigrationControls
{
    public class BaseImportControl : UserControl
    {
        protected string ImportItem(ImportTargetItem importTarget)
        {
            var resultStatistic = new StringBuilder();
            var waiter = new OperationWaiter();
            using (waiter)
            {
                try
                {
                    Logger.Info($"Импорт \"{importTarget.Name}\"");

                    var importResult = importTarget.Importer.Import();
                    resultStatistic.AppendLine($"Результат импортирования \"{importTarget.Name}\"");
                    resultStatistic.AppendLine($"  Добавлено: {importResult.AddedCount}");
                    resultStatistic.AppendLine($"  Обновлено: {importResult.UpdatedCount}");
                    resultStatistic.AppendLine($"  Удалено: {importResult.DeletedCount}");
                }
                catch (Exception ex)
                {
                    var errorHead = $"Ошибка импортирования \"{importTarget.Name}\"";

                    Logger.Error($"{errorHead}.\n{ex.Message}");
                    resultStatistic.AppendLine(errorHead);
                }
                finally
                {
                    resultStatistic.AppendLine($"Затрачено времени: {waiter.TimeFromStart}");
                }
            }

            return resultStatistic.ToString();
        }
    }
}
