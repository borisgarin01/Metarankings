namespace ExcelProcessors;

public interface IExcelDataReader<T> where T : class
{
    IEnumerable<T> GetFromExcel(string fileName);
}
