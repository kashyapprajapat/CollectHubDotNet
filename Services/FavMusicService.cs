using CollecthubDotNet.Models;
using MongoDB.Driver;

namespace CollecthubDotNet.Services
{
    public class FavMusicService
    {
        private readonly IMongoCollection<FavMusic> _favMusicCollection;

        public FavMusicService(MongoDbService mongoDbService)
        {
            _favMusicCollection = mongoDbService.GetCollection<FavMusic>("FavMusic");
        }

        // GET: Get all favorite music by userId
        public async Task<List<FavMusic>> GetByUserIdAsync(string userId)
        {
            var filter = Builders<FavMusic>.Filter.Eq(fm => fm.UserId, userId);
            return await _favMusicCollection.Find(filter).ToListAsync();
        }

        // POST: Create new favorite music
        public async Task<FavMusic> CreateAsync(FavMusic favMusic)
        {
            await _favMusicCollection.InsertOneAsync(favMusic);
            return favMusic;
        }

        // PUT: Update favorite music by id and userId
        public async Task<bool> UpdateAsync(string id, string userId, FavMusic updatedFavMusic)
        {
            var filter = Builders<FavMusic>.Filter.And(
                Builders<FavMusic>.Filter.Eq(fm => fm.Id, id),
                Builders<FavMusic>.Filter.Eq(fm => fm.UserId, userId)
            );

            var update = Builders<FavMusic>.Update
                .Set(fm => fm.MusicName, updatedFavMusic.MusicName)
                .Set(fm => fm.Singer, updatedFavMusic.Singer)
                .Set(fm => fm.Reason, updatedFavMusic.Reason);

            var result = await _favMusicCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        // DELETE: Delete favorite music by id
        public async Task<bool> DeleteAsync(string id)
        {
            var filter = Builders<FavMusic>.Filter.Eq(fm => fm.Id, id);
            var result = await _favMusicCollection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        // Helper method to get by id (optional, for validation)
        public async Task<FavMusic?> GetByIdAsync(string id)
        {
            var filter = Builders<FavMusic>.Filter.Eq(fm => fm.Id, id);
            return await _favMusicCollection.Find(filter).FirstOrDefaultAsync();
        }
    }
}