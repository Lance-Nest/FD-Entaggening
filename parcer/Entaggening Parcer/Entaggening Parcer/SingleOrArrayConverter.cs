using System.Text.Json.Serialization;
using System.Text.Json;
using System.Formats.Asn1;

public class SingleOrArrayConverter<T> : JsonConverter<T[]>
{

    public override T[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        List<T> retVal = new List<T>();
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            return new T[] { JsonSerializer.Deserialize<T>(ref reader, options) };
        }
        else if (reader.TokenType == JsonTokenType.StartArray)
        {
            return JsonSerializer.Deserialize<T[]>(ref reader, options);
        }
        return retVal.ToArray();
    }
    public override void Write(Utf8JsonWriter writer, T[] value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}