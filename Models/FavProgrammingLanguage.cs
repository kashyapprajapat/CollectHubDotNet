using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CollecthubDotNet.Models
{
    public class FavProgrammingLanguage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userid")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("programminglanguagename")]
        public string ProgrammingLanguageName { get; set; } = string.Empty;

        [BsonElement("usecase")]
        public string UseCase { get; set; } = string.Empty;

        [BsonElement("reason")]
        public string Reason { get; set; } = string.Empty;
    }
}