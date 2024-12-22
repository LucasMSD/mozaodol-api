using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Mozaodol.Domain.Repositories;

namespace Mozaodol.Application.DTOs.MessageDTOs
{
    public class ListChatMessagesDto : IPagination
    {
        [FromRoute]
        public ObjectId ChatId { get; set; }
        [FromQuery]
        public int PageSize { get; set; } = 5;
        [FromQuery]
        public int PageNumber { get; set; } = 1;
    }
}
