using MongoDB.Bson;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mozaodol.Infrastructure.Config.Serialization.JsonSerialization
{
    public class ObjectIdToStringConverter : JsonConverter<ObjectId>
    {
        public override ObjectId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Converter string para ObjectId durante a desserialização
            var objectIdString = reader.GetString();
            return ObjectId.TryParse(objectIdString, out var objectId) ? objectId : ObjectId.Empty;
        }

        public override void Write(Utf8JsonWriter writer, ObjectId value, JsonSerializerOptions options)
        {
            // Converter ObjectId para string durante a serialização
            writer.WriteStringValue(value.ToString());
        }
    }
}
