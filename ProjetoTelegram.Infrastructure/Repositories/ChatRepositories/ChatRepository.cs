using FluentResults;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjetoTelegram.Domain.Entities.ChatEntities;
using ProjetoTelegram.Domain.Repositories.ChatRepositories;
using ProjetoTelegram.Infrastructure.Contexts.MongoDBContexts;

namespace ProjetoTelegram.Infrastructure.Repositories.ChatRepositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly MongoDBContext _context;

        public ChatRepository(MongoDBContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<Result<Chat>> Get(ObjectId chatId)
        {
            return (await _context.Database.GetCollection<Chat>(nameof(Chat)).FindAsync(x => x._id == chatId)).FirstOrDefault();
        }

        public async Task<Result<List<Chat>>> Get(IEnumerable<ObjectId> chatsIds)
        {
            var result = await _context.Database.GetCollection<Chat>(nameof(Chat)).FindAsync(x => chatsIds.Contains(x._id));
            return Result.Ok(await result.ToListAsync());
        }

        public async Task<Result> Insert(Chat chat)
        {
            await _context.Database.GetCollection<Chat>(nameof(Chat)).InsertOneAsync(chat);
            return Result.Ok();
        }
    }
}
