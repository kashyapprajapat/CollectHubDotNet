using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CollecthubDotNet.Models
{
    public class MobileApp
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required]
        [BsonElement("userid")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [BsonElement("appname")]
        public string AppName { get; set; } = string.Empty;

        [Required]
        [BsonElement("platform")]
        public string Platform { get; set; } = string.Empty; // Android/iOS

        [Required]
        [BsonElement("category")]
        public string Category { get; set; } = string.Empty;

        [Required]
        [BsonElement("reason")]
        public string Reason { get; set; } = string.Empty;

        [BsonElement("createdat")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedat")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    // DTO for update operations (without Id and UserId)
    public class MobileAppUpdateDto
    {
        public string? AppName { get; set; }
        public string? Platform { get; set; }
        public string? Category { get; set; }
        public string? Reason { get; set; }
    }
}