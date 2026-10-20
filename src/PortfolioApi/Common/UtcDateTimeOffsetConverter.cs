using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PortfolioApi.Common;

public class UtcDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.GetDateTimeOffset();

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture));
    
}