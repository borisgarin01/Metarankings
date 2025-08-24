using Domain.Games;
using Domain.RequestsModels.Games.Publishers;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ExcelProcessors.Derived;

public sealed class PublishersDataReader : IExcelDataReader<AddPublisherModel>
{
    public IEnumerable<AddPublisherModel> GetFromExcel(string fileName)
    {
        using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
            IWorkbook workbook = fileName.EndsWith(".xlsx") ?
                new XSSFWorkbook(fileStream) : new HSSFWorkbook(fileStream);

            ISheet sheet = workbook.GetSheetAt(0);

            if (sheet is not null)
            {
                var publishersToUpload = new List<AddPublisherModel>();
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    IRow currentRow = sheet.GetRow(i);

                    var publisherName = currentRow.GetCell(0).StringCellValue.Trim();

                    publishersToUpload.Add(new AddPublisherModel { Name = publisherName });
                }

                return publishersToUpload;
            }

            return Enumerable.Empty<AddPublisherModel>();
        }
    }
}