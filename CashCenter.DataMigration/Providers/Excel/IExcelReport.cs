using NetOffice.ExcelApi;

namespace CashCenter.DataMigration.Providers.Excel
{
    public interface IExcelReport
    {
        void ExportToExcel(Workbook excelWorkbook);
    }
}
