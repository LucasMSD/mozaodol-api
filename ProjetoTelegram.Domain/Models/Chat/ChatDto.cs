﻿using MongoDB.Bson;

namespace ProjetoTelegram.Domain.Models.Chat
{
    public class ChatDto
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
    }
}
