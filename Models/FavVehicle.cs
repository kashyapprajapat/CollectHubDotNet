using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CollecthubDotNet.Models
{
    public class FavVehicle
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userid")]
        [Required(ErrorMessage = "User ID is required")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("vehiclename")]
        [Required(ErrorMessage = "Vehicle name is required")]
        [StringLength(100, ErrorMessage = "Vehicle name cannot exceed 100 characters")]
        public string VehicleName { get; set; } = string.Empty;

        [BsonElement("typeofvehicle")]
        [Required(ErrorMessage = "Type of vehicle is required")]
        public string TypeOfVehicle { get; set; } = string.Empty;

        [BsonElement("launchyear")]
        [Required(ErrorMessage = "Launch year is required")]
        [Range(1886, 2030, ErrorMessage = "Launch year must be between 1886 and 2030")]
        public int LaunchYear { get; set; }

        [BsonElement("reason")]
        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        public string? Reason { get; set; }

        [BsonElement("createdat")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedat")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}