using OfficeOpenXml;
namespace StudentApp.Services
{
    public class ExcelExportService
    {
        public byte[] ExportToExcel<T>(IEnumerable<T> data)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Load data into worksheet
                _ = worksheet.Cells["A1"].LoadFromCollection(data, true);

                return package.GetAsByteArray();
            }
        }
    }
}
