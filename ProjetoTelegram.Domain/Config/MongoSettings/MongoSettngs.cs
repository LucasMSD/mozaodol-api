using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace ProjetoTelegram.Domain.Config.MongoSettings
{
    public class MongoSettngs   
    {
        public static void ConfigureDateSerialization()
        {
            var conventionPack = new ConventionPack();
            conventionPack.Add(new IgnoreIfDefaultConvention(true));
            conventionPack.Add(new DateSerializationOptionsConvention());

            ConventionRegistry.Register("DateSerializationConventions", conventionPack, _ => true);
        }

        // Classe para configurar a serialização de datas
        public class DateSerializationOptionsConvention : IMemberMapConvention
        {
            public string Name => "DateSerializationOptions";

            public void Apply(BsonMemberMap memberMap)
            {
                if (memberMap.MemberType == typeof(DateTime) || memberMap.MemberType == typeof(DateTime?))
                {
                    memberMap.SetSerializer(new DateTimeSerializer(DateTimeKind.Local));
                }
            }
        }
    }
}
