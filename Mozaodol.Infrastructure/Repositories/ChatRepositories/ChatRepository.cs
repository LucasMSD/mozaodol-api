﻿using MongoDB.Bson;
using MongoDB.Driver;
using Mozaodol.Domain.Entities.ChatEntities;
using Mozaodol.Domain.Repositories.ChatRepositories;
using Mozaodol.Infrastructure.Contexts.MongoDBContexts;

namespace Mozaodol.Infrastructure.Repositories.ChatRepositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly MongoDBContext _context;

        public ChatRepository(MongoDBContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<Chat?> Get(ObjectId chatId)
        {
            return (await _context.Database.GetCollection<Chat>(nameof(Chat)).FindAsync(x => x._id == chatId)).FirstOrDefault();
        }

        public async Task<List<Chat>> Get(IEnumerable<ObjectId> chatsIds)
        {
            var result = await _context.Database.GetCollection<Chat>(nameof(Chat)).FindAsync(x => chatsIds.Contains(x._id));
            return await result.ToListAsync();
        }

        public async Task Insert(Chat chat)
        {
            await _context.Database.GetCollection<Chat>(nameof(Chat)).InsertOneAsync(chat);
        }
    }
}
