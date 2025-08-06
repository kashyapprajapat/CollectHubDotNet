using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CollecthubDotNet.Models
{
    public class FavMusic
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userid")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("musicname")]
        public string MusicName { get; set; } = string.Empty;

        [BsonElement("singer")]
        public string Singer { get; set; } = string.Empty;

        [BsonElement("reason")]
        public string Reason { get; set; } = string.Empty;
    }
}