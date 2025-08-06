using CollecthubDotNet.Models;
using MongoDB.Driver;

namespace CollecthubDotNet.Services
{
    public class FavVehicleService
    {
        private readonly IMongoCollection<FavVehicle> _favVehicleCollection;

        public FavVehicleService(MongoDbService mongoDbService)
        {
            _favVehicleCollection = mongoDbService.GetCollection<FavVehicle>("FavVehicle");
        }

        // Get all vehicles for a specific user
        public async Task<List<FavVehicle>> GetByUserIdAsync(string userId)
        {
            var filter = Builders<FavVehicle>.Filter.Eq(v => v.UserId, userId);
            return await _favVehicleCollection.Find(filter).ToListAsync();
        }

        // Get a specific vehicle by ID
        public async Task<FavVehicle?> GetByIdAsync(string id)
        {
            var filter = Builders<FavVehicle>.Filter.Eq(v => v.Id, id);
            return await _favVehicleCollection.Find(filter).FirstOrDefaultAsync();
        }

        // Create a new vehicle
        public async Task<FavVehicle> CreateAsync(FavVehicle vehicle)
        {
            vehicle.CreatedAt = DateTime.UtcNow;
            vehicle.UpdatedAt = DateTime.UtcNow;
            await _favVehicleCollection.InsertOneAsync(vehicle);
            return vehicle;
        }

        // Update an existing vehicle
        public async Task<bool> UpdateAsync(string id, FavVehicle vehicle)
        {
            vehicle.UpdatedAt = DateTime.UtcNow;
            var filter = Builders<FavVehicle>.Filter.Eq(v => v.Id, id);
            var update = Builders<FavVehicle>.Update
                .Set(v => v.VehicleName, vehicle.VehicleName)
                .Set(v => v.TypeOfVehicle, vehicle.TypeOfVehicle)
                .Set(v => v.LaunchYear, vehicle.LaunchYear)
                .Set(v => v.Reason, vehicle.Reason)
                .Set(v => v.UpdatedAt, vehicle.UpdatedAt);

            var result = await _favVehicleCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        // Delete a vehicle
        public async Task<bool> DeleteAsync(string id)
        {
            var filter = Builders<FavVehicle>.Filter.Eq(v => v.Id, id);
            var result = await _favVehicleCollection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        // Check if vehicle exists
        public async Task<bool> ExistsAsync(string id)
        {
            var filter = Builders<FavVehicle>.Filter.Eq(v => v.Id, id);
            var count = await _favVehicleCollection.CountDocumentsAsync(filter);
            return count > 0;
        }
    }
}