using CollecthubDotNet.Models;
using MongoDB.Driver;

namespace CollecthubDotNet.Services
{
    public class GameService
    {
        private readonly IMongoCollection<Game> _gamesCollection;

        public GameService(MongoDbService mongoDbService)
        {
            _gamesCollection = mongoDbService.GetCollection<Game>("games");
        }

        // Create a new game
        public async Task<Game> CreateGameAsync(CreateGameDto gameDto)
        {
            var game = new Game
            {
                UserId = gameDto.UserId,
                GameName = gameDto.GameName,
                Platform = gameDto.Platform,
                Reason = gameDto.Reason,
                IsDigital = gameDto.IsDigital,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _gamesCollection.InsertOneAsync(game);
            return game;
        }

        // Get all games for a specific user
        public async Task<List<Game>> GetGamesByUserIdAsync(string userId)
        {
            var filter = Builders<Game>.Filter.Eq(g => g.UserId, userId);
            return await _gamesCollection.Find(filter).ToListAsync();
        }

        // Get a specific game by id
        public async Task<Game?> GetGameByIdAsync(string id)
        {
            var filter = Builders<Game>.Filter.Eq(g => g.Id, id);
            return await _gamesCollection.Find(filter).FirstOrDefaultAsync();
        }

        // Update a game
        public async Task<Game?> UpdateGameAsync(string id, string userId, UpdateGameDto updateGameDto)
        {
            var filter = Builders<Game>.Filter.And(
                Builders<Game>.Filter.Eq(g => g.Id, id),
                Builders<Game>.Filter.Eq(g => g.UserId, userId)
            );

            var updateDefinitionList = new List<UpdateDefinition<Game>>
            {
                Builders<Game>.Update.Set(g => g.UpdatedAt, DateTime.UtcNow)
            };

            if (!string.IsNullOrEmpty(updateGameDto.GameName))
                updateDefinitionList.Add(Builders<Game>.Update.Set(g => g.GameName, updateGameDto.GameName));

            if (!string.IsNullOrEmpty(updateGameDto.Platform))
                updateDefinitionList.Add(Builders<Game>.Update.Set(g => g.Platform, updateGameDto.Platform));

            if (!string.IsNullOrEmpty(updateGameDto.Reason))
                updateDefinitionList.Add(Builders<Game>.Update.Set(g => g.Reason, updateGameDto.Reason));

            if (updateGameDto.IsDigital.HasValue)
                updateDefinitionList.Add(Builders<Game>.Update.Set(g => g.IsDigital, updateGameDto.IsDigital.Value));

            var updateDefinition = Builders<Game>.Update.Combine(updateDefinitionList);

            var options = new FindOneAndUpdateOptions<Game>
            {
                ReturnDocument = ReturnDocument.After
            };

            return await _gamesCollection.FindOneAndUpdateAsync(filter, updateDefinition, options);
        }

        // Delete a game
        public async Task<bool> DeleteGameAsync(string id)
        {
            var filter = Builders<Game>.Filter.Eq(g => g.Id, id);
            var result = await _gamesCollection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        // Check if a game exists and belongs to a user
        public async Task<bool> GameExistsForUserAsync(string id, string userId)
        {
            var filter = Builders<Game>.Filter.And(
                Builders<Game>.Filter.Eq(g => g.Id, id),
                Builders<Game>.Filter.Eq(g => g.UserId, userId)
            );
            var count = await _gamesCollection.CountDocumentsAsync(filter);
            return count > 0;
        }
    }
}