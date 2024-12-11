using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;

namespace Mozaodol.Infrastructure.Contexts.MongoDBContexts
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

        public static void ConfigureDateSerialization()
        {
            var conventionPack = new ConventionPack
            {
                new IgnoreIfDefaultConvention(true),
                new DateSerializationOptionsConvention()
            };

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
