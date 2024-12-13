using FluentResults;
using MongoDB.Bson;
using MongoDB.Driver;
using Mozaodol.Domain.Entities.MessageEntities;
using Mozaodol.Domain.Enums;
using Mozaodol.Domain.Repositories.MessageRepositories;
using Mozaodol.Infrastructure.Contexts.MongoDBContexts;

namespace Mozaodol.Infrastructure.Repositories.MessageRepositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MongoDBContext _context;

        public MessageRepository(MongoDBContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<Message> Get(ObjectId _id)
        {
            return (await _context.Database.GetCollection<Message>(nameof(Message)).FindAsync(x => x._id == _id)).FirstOrDefault();
        }

        public async Task<List<Message>> GetByChat(ObjectId chatId)
        {
            var result = await _context.Database.GetCollection<Message>(nameof(Message)).FindAsync(x => x.ChatId == chatId);
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
