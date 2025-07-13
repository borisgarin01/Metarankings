namespace API.Json;

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private readonly string format;
    public DateOnlyJsonConverter(string format)
    {
        this.format = format;
    }

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateOnly.ParseExact(reader.GetString(), format, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(format, CultureInfo.InvariantCulture));
    }
}
