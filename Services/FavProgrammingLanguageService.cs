using CollecthubDotNet.Models;
using MongoDB.Driver;

namespace CollecthubDotNet.Services
{
    public class FavProgrammingLanguageService
    {
        private readonly IMongoCollection<FavProgrammingLanguage> _favProgrammingLanguages;

        public FavProgrammingLanguageService(MongoDbService mongoDbService)
        {
            _favProgrammingLanguages = mongoDbService.GetCollection<FavProgrammingLanguage>("FavProgrammingLanguage");
        }

        // Create a new favorite programming language
        public async Task<FavProgrammingLanguage> CreateAsync(FavProgrammingLanguage favProgrammingLanguage)
        {
            await _favProgrammingLanguages.InsertOneAsync(favProgrammingLanguage);
            return favProgrammingLanguage;
        }

        // Get all favorite programming languages by user ID
        public async Task<List<FavProgrammingLanguage>> GetByUserIdAsync(string userId)
        {
            var filter = Builders<FavProgrammingLanguage>.Filter.Eq(x => x.UserId, userId);
            return await _favProgrammingLanguages.Find(filter).ToListAsync();
        }

        // Get a specific favorite programming language by ID
        public async Task<FavProgrammingLanguage?> GetByIdAsync(string id)
        {
            var filter = Builders<FavProgrammingLanguage>.Filter.Eq(x => x.Id, id);
            return await _favProgrammingLanguages.Find(filter).FirstOrDefaultAsync();
        }

        // Update a favorite programming language
        public async Task<bool> UpdateAsync(string id, FavProgrammingLanguage favProgrammingLanguage)
        {
            var filter = Builders<FavProgrammingLanguage>.Filter.Eq(x => x.Id, id);
            
            var update = Builders<FavProgrammingLanguage>.Update
                .Set(x => x.ProgrammingLanguageName, favProgrammingLanguage.ProgrammingLanguageName)
                .Set(x => x.UseCase, favProgrammingLanguage.UseCase)
                .Set(x => x.Reason, favProgrammingLanguage.Reason);

            var result = await _favProgrammingLanguages.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        // Delete a favorite programming language
        public async Task<bool> DeleteAsync(string id)
        {
            var filter = Builders<FavProgrammingLanguage>.Filter.Eq(x => x.Id, id);
            var result = await _favProgrammingLanguages.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        // Check if a favorite programming language exists
        public async Task<bool> ExistsAsync(string id)
        {
            var filter = Builders<FavProgrammingLanguage>.Filter.Eq(x => x.Id, id);
            var count = await _favProgrammingLanguages.CountDocumentsAsync(filter);
            return count > 0;
        }
    }
}