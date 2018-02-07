using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;

namespace CashCenter.DataMigration.Providers.Csv
{
    public class CsvController
    {
        private string filename;
        private char separator;
        private Func<string, bool> exclusionLineFunction;

        public CsvController(string filename, Func<string, bool> exclusionLineFunction = null, char separator = ';')
        {
            this.filename = filename;
            this.separator = separator;
            this.exclusionLineFunction = exclusionLineFunction ?? (line => true);
        }

        public IEnumerable<IEnumerable<string>> LoadRows()
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
                throw new FileNotFoundException($"Файл для загрузки не найден {filename}", filename);

            return File.ReadLines(filename)
                .Where(line => !exclusionLineFunction(line))
                .Select(line => line.Split(separator));
        }

        public void SaveRows(IEnumerable<IEnumerable<string>> rows)
        {
            if (string.IsNullOrEmpty(filename))
                throw new FileNotFoundException("Имя файла для сохранения не задано", filename);

            string directory = Path.GetDirectoryName(filename);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllLines(filename,
                rows.Select(row => string.Join(separator.ToString(), row)));
        }
    }
}
