namespace JWTWebAPI.Models
{
    public class User
    {
        // user Tabel Model
        public string UserName { get; set; } = string.Empty;
        public byte[]? PasswordHash { get; set; } 
        public byte[]? PasswordSalt { get; set; }

        public string Cookie { get; set; } = string.Empty;
        public DateTime CookieCreated { get; set; }
        public DateTime CookieExpires { get; set; }
    }
}
