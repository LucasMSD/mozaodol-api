using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson;
using ProjetoTelegram.Application.DTOs.ContactDTOs;
using ProjetoTelegram.Application.DTOs.UserDTOs;
using ProjetoTelegram.Application.Interfaces.UserInterfaces;
using ProjetoTelegram.Domain.Repositories.UserRepositories;
using System.Text.Json;

namespace ProjetoTelegram.Application.Implementations.UserImplementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDistributedCache _distributedCache;

        public UserService(IUserRepository userRepository, IDistributedCache distributedCache)
        {
            _userRepository = userRepository;
            _distributedCache = distributedCache;
        }

        public async Task<IEnumerable<UserDto>> AddContact(ObjectId userId, AddContactModel addContact)
        {
            var user = await _userRepository.Get(userId);

            if (user == null)
                return [];

            user.Contacts = user.Contacts == null ? new List<ObjectId>() : user.Contacts;
            user.Contacts.Add(addContact._id);
            _userRepository.UpdateContacts(user);

            return await GetContacts(user._id);
        }

        public async Task<UserDto> Get(ObjectId userId)
        {
            var user = await _userRepository.Get(userId);

            return new UserDto
            {
                Name = user.Name,
                Username = user.Username,
                _id = user._id,
                PushToken = user.PushToken,
            };
        }

        public async Task<IEnumerable<UserDto>> GetAll()
        {
            return (await _userRepository.GetAll()).Select(x => new UserDto
            {
                _id = x._id,
                Username = x.Username,
                Name = x.Name,
                PushToken = x.PushToken,
            });
        }

        public async Task<IEnumerable<UserDto>> GetContacts(ObjectId userId)
        {
            var user = await _userRepository.Get(userId);

            if (user == null || user.Contacts == null || user.Contacts.Count == 0)
                return [];

            return (await _userRepository.Get(user.Contacts)).Select(x => new UserDto
            {
                _id = x._id,
                Name = x.Name,
                Username = x.Username
            });
        }

        public async Task<IEnumerable<UserDto>> RemoveContact(ObjectId userId, ObjectId contactId)
        {
            var user = await _userRepository.Get(userId);

            if (user == null)
                return [];

            user.Contacts.Remove(contactId);
            _userRepository.UpdateContacts(user);

            return await GetContacts(user._id);
        }

        public async Task UpdatePushToken(ObjectId userId, UpdatePushTokenModel updatePushTokenModel)
        {
            var user = await _userRepository.Get(userId);

            if (user == null)
                return;

            await _userRepository.UpdatePushToken(userId, updatePushTokenModel.PushToken);

            var userStateJson = await _distributedCache.GetStringAsync(userId.ToString());

            if (string.IsNullOrEmpty(userStateJson)) return;

            var userState = JsonSerializer.Deserialize<UserState>(userStateJson);
            userState.PushToken = updatePushTokenModel.PushToken;

            await _distributedCache.RemoveAsync(userId.ToString());
            await _distributedCache.SetStringAsync(userId.ToString(), JsonSerializer.Serialize(userState));
            return;
        }
    }
}
