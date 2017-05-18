﻿using CashCenter.Common;
using System;
using System.Text;

namespace CashCenter.IvEnergySales.DataMigrationControls
{
    public static class MigrationHelper
    {
        public static string ImportItem(ImportTargetItem importTarget)
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

                    Logger.Error($"{errorHead}", ex);
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