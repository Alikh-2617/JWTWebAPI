namespace JWTWebAPI.Models
{
    public class Cookies
    {
        public string CookieToken { get; set; } = string.Empty;

        public DateTime Create { get; set; }

        public DateTime Expires { get; set; }
    }
}
