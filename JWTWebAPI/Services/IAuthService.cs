using JWTWebAPI.Models;

namespace JWTWebAPI.Services
{
    public interface IAuthService
    {
        //protected void CreatePasswordHash(string password, out byte[] passwordHas, out byte[] passwordSalt);
        User CreatePasswordHash(string userName, string password, out byte[] passwordHas, out byte[] passwordSalt);
        bool VarifyPasswordHassh(string password, byte[] passwordHash, byte[] passwordSalt);
        string? FineUser(string userName, string pass);
        string CreateToken(User user);

        Cookies GetCookies();
        CookieOptions SetCookies(Cookies cookie);
        bool VarifyCookie(string cookie);
    }
}
