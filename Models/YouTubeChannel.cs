using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CollecthubDotNet.Models
{
    public class YouTubeChannel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required]
        [BsonElement("userid")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [BsonElement("channelName")]
        public string ChannelName { get; set; } = string.Empty;

        [Required]
        [BsonElement("creatorname")]
        public string CreatorName { get; set; } = string.Empty;

        [Required]
        [BsonElement("genre")]
        public string Genre { get; set; } = string.Empty;

        [BsonElement("reason")]
        public string Reason { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    // DTOs for API requests
    public class CreateYouTubeChannelRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string ChannelName { get; set; } = string.Empty;

        [Required]
        public string CreatorName { get; set; } = string.Empty;

        [Required]
        public string Genre { get; set; } = string.Empty;

        public string Reason { get; set; } = string.Empty;
    }

    public class UpdateYouTubeChannelRequest
    {
        public string? ChannelName { get; set; }
        public string? CreatorName { get; set; }
        public string? Genre { get; set; }
        public string? Reason { get; set; }
    }
}