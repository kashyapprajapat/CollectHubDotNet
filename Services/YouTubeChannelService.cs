using CollecthubDotNet.Models;
using MongoDB.Driver;
using MongoDB.Bson;

namespace CollecthubDotNet.Services
{
    public class YouTubeChannelService
    {
        private readonly IMongoCollection<YouTubeChannel> _youtubeChannels;

        public YouTubeChannelService(MongoDbService mongoDbService)
        {
            _youtubeChannels = mongoDbService.GetCollection<YouTubeChannel>("youtubechannels");
        }

        // Create a new YouTube channel
        public async Task<YouTubeChannel> CreateAsync(CreateYouTubeChannelRequest request)
        {
            var youtubeChannel = new YouTubeChannel
            {
                UserId = request.UserId,
                ChannelName = request.ChannelName,
                CreatorName = request.CreatorName,
                Genre = request.Genre,
                Reason = request.Reason,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _youtubeChannels.InsertOneAsync(youtubeChannel);
            return youtubeChannel;
        }

        // Get all YouTube channels by userId
        public async Task<List<YouTubeChannel>> GetByUserIdAsync(string userId)
        {
            var filter = Builders<YouTubeChannel>.Filter.Eq(x => x.UserId, userId);
            return await _youtubeChannels.Find(filter).ToListAsync();
        }

        // Get a specific YouTube channel by id
        public async Task<YouTubeChannel?> GetByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out _))
                return null;

            var filter = Builders<YouTubeChannel>.Filter.Eq(x => x.Id, id);
            return await _youtubeChannels.Find(filter).FirstOrDefaultAsync();
        }

        // Update a YouTube channel
        public async Task<YouTubeChannel?> UpdateAsync(string id, string userId, UpdateYouTubeChannelRequest request)
        {
            if (!ObjectId.TryParse(id, out _))
                return null;

            var filter = Builders<YouTubeChannel>.Filter.And(
                Builders<YouTubeChannel>.Filter.Eq(x => x.Id, id),
                Builders<YouTubeChannel>.Filter.Eq(x => x.UserId, userId)
            );

            var updateBuilder = Builders<YouTubeChannel>.Update
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            if (!string.IsNullOrWhiteSpace(request.ChannelName))
                updateBuilder = updateBuilder.Set(x => x.ChannelName, request.ChannelName);

            if (!string.IsNullOrWhiteSpace(request.CreatorName))
                updateBuilder = updateBuilder.Set(x => x.CreatorName, request.CreatorName);

            if (!string.IsNullOrWhiteSpace(request.Genre))
                updateBuilder = updateBuilder.Set(x => x.Genre, request.Genre);

            if (request.Reason != null)
                updateBuilder = updateBuilder.Set(x => x.Reason, request.Reason);

            var options = new FindOneAndUpdateOptions<YouTubeChannel>
            {
                ReturnDocument = ReturnDocument.After
            };

            return await _youtubeChannels.FindOneAndUpdateAsync(filter, updateBuilder, options);
        }

        // Delete a YouTube channel
        public async Task<bool> DeleteAsync(string id)
        {
            if (!ObjectId.TryParse(id, out _))
                return false;

            var filter = Builders<YouTubeChannel>.Filter.Eq(x => x.Id, id);
            var result = await _youtubeChannels.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        // Check if a YouTube channel exists and belongs to the user
        public async Task<bool> ExistsAndBelongsToUserAsync(string id, string userId)
        {
            if (!ObjectId.TryParse(id, out _))
                return false;

            var filter = Builders<YouTubeChannel>.Filter.And(
                Builders<YouTubeChannel>.Filter.Eq(x => x.Id, id),
                Builders<YouTubeChannel>.Filter.Eq(x => x.UserId, userId)
            );

            var count = await _youtubeChannels.CountDocumentsAsync(filter);
            return count > 0;
        }
    }
}