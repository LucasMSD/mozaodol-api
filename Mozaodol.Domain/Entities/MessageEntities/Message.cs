﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Mozaodol.Domain.Enums;

namespace Mozaodol.Domain.Entities.MessageEntities
{
    [BsonIgnoreExtraElements]
    public class Message
    {
        public ObjectId _id { get; set; }
        public string Text { get; set; }
        public MessageMedia Media { get; set; }
        public ObjectId UserId { get; set; }
        public ObjectId ChatId { get; set; }
        public DateTime Timestamp { get; set; }
        public MessageStatus Status { get; set; }
        public string ExternalId { get; set; }
    }
}
