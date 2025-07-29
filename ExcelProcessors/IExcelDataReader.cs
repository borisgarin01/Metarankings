namespace ExcelProcessors;

public interface IExcelDataReader<T> where T : class, new()
{
    IEnumerable<T> GetFromExcel(string fileName);
}
