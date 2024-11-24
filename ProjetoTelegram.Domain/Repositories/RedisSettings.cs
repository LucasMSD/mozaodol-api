using System.Reflection.Metadata.Ecma335;

namespace ProjetoTelegram.Domain.Repositories
{
    public class RedisSettings
    {
        public string ConnectionString { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}
