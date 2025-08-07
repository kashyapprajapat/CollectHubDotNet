using CollecthubDotNet.Models;
using MongoDB.Driver;

namespace CollecthubDotNet.Services
{
    public class MobileAppService
    {
        private readonly IMongoCollection<MobileApp> _mobileAppsCollection;

        public MobileAppService(MongoDbService mongoDbService)
        {
            _mobileAppsCollection = mongoDbService.GetCollection<MobileApp>("MobileApps");
        }

        // Create a new mobile app
        public async Task<MobileApp> CreateAsync(MobileApp mobileApp)
        {
            mobileApp.CreatedAt = DateTime.UtcNow;
            mobileApp.UpdatedAt = DateTime.UtcNow;
            await _mobileAppsCollection.InsertOneAsync(mobileApp);
            return mobileApp;
        }

        // Get all mobile apps by user ID
        public async Task<List<MobileApp>> GetByUserIdAsync(string userId)
        {
            var filter = Builders<MobileApp>.Filter.Eq(x => x.UserId, userId);
            return await _mobileAppsCollection.Find(filter).ToListAsync();
        }

        // Get a single mobile app by its own ID
        public async Task<MobileApp?> GetByIdAsync(string id)
        {
            var filter = Builders<MobileApp>.Filter.Eq(x => x.Id, id);
            return await _mobileAppsCollection.Find(filter).FirstOrDefaultAsync();
        }

        // Update a mobile app
        public async Task<MobileApp?> UpdateAsync(string id, string userId, MobileAppUpdateDto updateDto)
        {
            // First check if the mobile app exists and belongs to the user
            var existingApp = await GetByIdAsync(id);
            if (existingApp == null || existingApp.UserId != userId)
            {
                return null;
            }

            var updateBuilder = Builders<MobileApp>.Update.Set(x => x.UpdatedAt, DateTime.UtcNow);

            if (!string.IsNullOrEmpty(updateDto.AppName))
                updateBuilder = updateBuilder.Set(x => x.AppName, updateDto.AppName);

            if (!string.IsNullOrEmpty(updateDto.Platform))
                updateBuilder = updateBuilder.Set(x => x.Platform, updateDto.Platform);

            if (!string.IsNullOrEmpty(updateDto.Category))
                updateBuilder = updateBuilder.Set(x => x.Category, updateDto.Category);

            if (!string.IsNullOrEmpty(updateDto.Reason))
                updateBuilder = updateBuilder.Set(x => x.Reason, updateDto.Reason);

            var filter = Builders<MobileApp>.Filter.Eq(x => x.Id, id);
            var options = new FindOneAndUpdateOptions<MobileApp>
            {
                ReturnDocument = ReturnDocument.After
            };

            return await _mobileAppsCollection.FindOneAndUpdateAsync(filter, updateBuilder, options);
        }

        // Delete a mobile app
        public async Task<bool> DeleteAsync(string id)
        {
            var filter = Builders<MobileApp>.Filter.Eq(x => x.Id, id);
            var result = await _mobileAppsCollection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        // Check if mobile app exists and get it
        public async Task<MobileApp?> GetByIdAndUserIdAsync(string id, string userId)
        {
            var filter = Builders<MobileApp>.Filter.And(
                Builders<MobileApp>.Filter.Eq(x => x.Id, id),
                Builders<MobileApp>.Filter.Eq(x => x.UserId, userId)
            );
            return await _mobileAppsCollection.Find(filter).FirstOrDefaultAsync();
        }
    }
}