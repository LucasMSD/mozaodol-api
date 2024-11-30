using MongoDB.Bson;
using ProjetoTelegram.Domain.Models.ContactList;
using ProjetoTelegram.Domain.Models.User;

namespace ProjetoTelegram.Application.Interfaces.UserInterfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> AddContact(ObjectId userId, AddContactModel addContact);
        Task<IEnumerable<UserDto>> GetAll();
        Task<IEnumerable<UserDto>> GetContacts(ObjectId userId);
        Task<IEnumerable<UserDto>> RemoveContact(ObjectId userId, ObjectId contactId);
        Task<UserDto> Get(ObjectId userId);
        Task UpdatePushToken(ObjectId userId, UpdatePushTokenModel updatePushTokenModel);
    }
}
