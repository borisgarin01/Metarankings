namespace BlazorClient
{
    public class TextTruncater
    {
        public string Truncate(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...";
        }
    }
}
