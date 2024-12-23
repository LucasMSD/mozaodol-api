using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Mozaodol.Domain.Repositories;

namespace Mozaodol.Application.DTOs.MessageDTOs
{
    public class ListChatMessagesDto : Pagination
    {
        [FromRoute]
        public ObjectId ChatId { get; set; }
        [FromQuery]
        public new int PageSize { get; set; }
        [FromQuery]
        public new int PageNumber { get; set; }
    }
}
