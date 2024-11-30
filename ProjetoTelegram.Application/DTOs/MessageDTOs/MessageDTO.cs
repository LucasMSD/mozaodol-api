﻿using MongoDB.Bson;
using ProjetoTelegram.Domain.Enums;

namespace ProjetoTelegram.Application.DTOs.MessageDTOs
{
    public class MessageDto
    {
        public ObjectId _id { get; set; }
        public string Text { get; set; }
        public ObjectId UserId { get; set; }
        public string UserUsername { get; set; }
        public string Timestamp { get; set; }
        public ObjectId ChatId { get; set; }
        public MessageStatus Status { get; set; }
        public string ExternalId { get; set; }
    }
}