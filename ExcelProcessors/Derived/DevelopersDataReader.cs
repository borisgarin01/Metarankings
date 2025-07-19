using Domain.Games;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ExcelProcessors.Derived;

public sealed class DevelopersDataReader : IExcelDataReader<Developer>
{
    public IEnumerable<Developer> GetFromExcel(string fileName)
    {
        using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
            IWorkbook workbook = fileName.EndsWith(".xlsx") ?
                new XSSFWorkbook(fileStream) : new HSSFWorkbook(fileStream);

            ISheet sheet = workbook.GetSheetAt(0);

            if (sheet is not null)
            {
                var developersToUpload = new List<Developer>();
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    IRow currentRow = sheet.GetRow(i);

                    var publisherName = currentRow.GetCell(0).StringCellValue.Trim();

                    developersToUpload.Add(new Developer { Name = publisherName });
                }

                return developersToUpload;
            }

            return Enumerable.Empty<Developer>();
        }
    }
}
