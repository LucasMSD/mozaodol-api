﻿using MongoDB.Bson;
using Mozaodol.Application.DTOs.MessageDTOs;

namespace Mozaodol.Application.UseCases.ChatUseCases
{
    public interface IListChatMessagesUseCase : IUseCase<ObjectId, List<MessageDto>>
    {
    }
}