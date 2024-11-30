using MongoDB.Bson;
using MongoDB.Driver;
using ProjetoTelegram.Domain.Entities.ChatEntities;

namespace ProjetoTelegram.Domain.Repositories.ChatRepositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly DbContext _context;

        public ChatRepository(DbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<Chat> Get(ObjectId chatId)
        {
            return (await _context.Database.GetCollection<Chat>(nameof(Chat)).FindAsync(x => x._id == chatId)).FirstOrDefault();
        }

        public async Task<IEnumerable<Chat>> Get(IEnumerable<ObjectId> chatsIds)
        {
            return (await _context.Database.GetCollection<Chat>(nameof(Chat)).FindAsync(x => chatsIds.Contains(x._id))).ToEnumerable();
        }

        public async Task Insert(Chat chat)
        {
            await _context.Database.GetCollection<Chat>(nameof(Chat)).InsertOneAsync(chat);
        }
    }
}
