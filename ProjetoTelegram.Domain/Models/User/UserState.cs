﻿using MongoDB.Bson;

namespace ProjetoTelegram.Domain.Models.User
{
    public class UserState
    {
        public string UserId { get; set; }
        public string PushToken { get; set; }
        public string OpenedChatId { get; set; }
        public bool Connected { get; set; }
    }
}
