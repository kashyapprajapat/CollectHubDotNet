using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CollecthubDotNet.Models
{
    public class Game
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userid")]
        [Required]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("gamename")]
        [Required]
        public string GameName { get; set; } = string.Empty;

        [BsonElement("platform")]
        [Required]
        [RegularExpression("^(indoor|outdoor)$", ErrorMessage = "Platform must be either 'indoor' or 'outdoor'")]
        public string Platform { get; set; } = string.Empty;

        [BsonElement("reason")]
        [Required]
        public string Reason { get; set; } = string.Empty;

        [BsonElement("isdigital")]
        [Required]
        public bool IsDigital { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    // DTO for creating a game (without Id)
    public class CreateGameDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string GameName { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(indoor|outdoor)$", ErrorMessage = "Platform must be either 'indoor' or 'outdoor'")]
        public string Platform { get; set; } = string.Empty;

        [Required]
        public string Reason { get; set; } = string.Empty;

        [Required]
        public bool IsDigital { get; set; }
    }

    // DTO for updating a game
    public class UpdateGameDto
    {
        public string? GameName { get; set; }

        [RegularExpression("^(indoor|outdoor)$", ErrorMessage = "Platform must be either 'indoor' or 'outdoor'")]
        public string? Platform { get; set; }

        public string? Reason { get; set; }

        public bool? IsDigital { get; set; }
    }
}