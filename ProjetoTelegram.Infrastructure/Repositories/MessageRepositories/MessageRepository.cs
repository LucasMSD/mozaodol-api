using MongoDB.Bson;
using MongoDB.Driver;
using ProjetoTelegram.Domain.Entities.MessageEntities;
using ProjetoTelegram.Domain.Enums;
using ProjetoTelegram.Domain.Repositories.MessageRepositories;
using ProjetoTelegram.Infrastructure.Contexts.MongoDBContexts;

namespace ProjetoTelegram.Infrastructure.Repositories.MessageRepositories
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

        public async Task<IEnumerable<Message>> GetByChat(ObjectId chatId)
        {
            return (await _context.Database.GetCollection<Message>(nameof(Message)).FindAsync(x => x.ChatId == chatId)).ToEnumerable();
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
