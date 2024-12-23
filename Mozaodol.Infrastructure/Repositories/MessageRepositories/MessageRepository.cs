using MongoDB.Bson;
using MongoDB.Driver;
using Mozaodol.Domain.Entities.MessageEntities;
using Mozaodol.Domain.Enums;
using Mozaodol.Domain.Repositories;
using Mozaodol.Domain.Repositories.MessageRepositories;
using Mozaodol.Infrastructure.Contexts.MongoDBContexts;

namespace Mozaodol.Infrastructure.Repositories.MessageRepositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MongoDBContext _context;
        private readonly IMongoCollection<Message> _collection;

        public MessageRepository(MongoDBContext dbContext)
        {
            _context = dbContext;
            _collection = _context.Database.GetCollection<Message>(nameof(Message));
        }

        public async Task<Message> Get(ObjectId _id)
        {
            return (await _context.Database.GetCollection<Message>(nameof(Message)).FindAsync(x => x._id == _id)).FirstOrDefault();
        }

        public async Task<List<Message>> GetByChat(ObjectId chatId, Pagination pagination)
        {
            var pipeline = new EmptyPipelineDefinition<Message>()
                .Match(x => x.ChatId == chatId)
                .Sort(new SortDefinitionBuilder<Message>().Descending(x => x.Timestamp));

            if (pagination.PageSize > 0 && pagination.PageNumber > 0)
            {
                var skip = (pagination.PageNumber - 1) * pagination.PageSize;
                var limit = pagination.PageSize;

                pipeline = pipeline
                    .Skip(skip)
                    .Limit(limit);
            }

            var result = await _collection.AggregateAsync(pipeline);
            return await result.ToListAsync();
        }

        public async Task Insert(Message message)
        {
            await _context.Database.GetCollection<Message>(nameof(Message)).InsertOneAsync(message);
        }

        public async Task UpdateStatus(ObjectId _id, MessageStatus status)
        {
            await _context.Database.GetCollection<Message>(nameof(Message)).UpdateOneAsync(
                x => x._id == _id,
                new UpdateDefinitionBuilder<Message>().Set(x => x.Status, status));
        }
    }
}
