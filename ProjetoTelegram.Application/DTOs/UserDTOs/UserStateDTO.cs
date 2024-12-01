namespace ProjetoTelegram.Application.DTOs.UserDTOs
{
    public class UserState
    {
        public string UserId { get; set; }
        public string PushToken { get; set; }
        public string OpenedChatId { get; set; }
        public bool Connected { get; set; }
    }
}
