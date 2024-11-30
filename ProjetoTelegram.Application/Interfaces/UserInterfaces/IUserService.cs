using FluentResults;
using MongoDB.Bson;
using ProjetoTelegram.Application.DTOs.ContactDTOs;
using ProjetoTelegram.Application.DTOs.UserDTOs;

namespace ProjetoTelegram.Application.Interfaces.UserInterfaces
{
    public interface IUserService
    {
        Task<Result<List<UserDto>>> AddContact(ObjectId userId, AddContactModel addContact);
        Task<Result<List<UserDto>>> GetAll();
        Task<Result<List<UserDto>>> GetContacts(ObjectId userId);
        Task<Result<List<UserDto>>> RemoveContact(ObjectId userId, ObjectId contactId);
        Task<Result<UserDto>> Get(ObjectId userId);
        Task<Result> UpdatePushToken(ObjectId userId, UpdatePushTokenModel updatePushTokenModel);
    }
}
