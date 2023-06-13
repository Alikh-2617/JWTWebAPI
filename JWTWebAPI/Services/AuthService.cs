using JWTWebAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace JWTWebAPI.Services
{
    public class AuthService : IAuthService
    {
        static User user = new User();
        static List<User> users = new List<User>();
        private readonly IConfiguration _config;

        public AuthService(IConfiguration configuration)
        {
            _config = configuration;
        }


        public User CreatePasswordHash(string username, string password, out byte[] passwordHas, out byte[] passwordSalt)
        {
            using (var hash = new HMACSHA512())
            {
                passwordSalt = hash.Key;
                passwordHas = hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            User user = new User { UserName = username, PasswordHash = passwordHas, PasswordSalt = passwordSalt };
            users.Add(user);
            return user;
        }

        public string? FineUser(string request, string pass)
        {
            // we can check the username if this Exist , here or we can have filter before Action execioting
            foreach (var user in users)
            {
                if (request == user.UserName)
                {
                    if (VarifyPasswordHassh(pass, user.PasswordHash!, user.PasswordSalt!))
                    {
                        var token = CreateToken(user);
                        return token;
                    }
                }
            }
            return null;
        }

        public bool VarifyPasswordHassh(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hash = new HMACSHA512(passwordSalt)) // Create instance of hash metod WITH SAME KEY !!! 
            {

                // we create one Encoding passwordHash with Same Key to compari the passwords 
                var computHash = hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                // check if the new passHash is same the old one (return is bool)
                return computHash.SequenceEqual(passwordHash);
            }
        }

        public string CreateToken(User user)
        {
            // Cleam is users propertis like rol osv.
            List<Claim> claim = new List<Claim>
            {
                new Claim(ClaimTypes.Name , user.UserName),
                new Claim(ClaimTypes.Role , "user")  // user Rolle can put from User Tabel or Cleam Tabel 

            };
            // pakeg = Microsoft.Identity.Token
            // Key puts in appsettingjson or somewhere else
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:TokenKey").Value));

            var signing_Credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken   // put token detaljer and install pakage for it
                (
                    claims: claim,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: signing_Credentials
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token); // the Token to send
            return jwt;
        }

        public Cookies GetCookies()
        {
            var cookie = new Cookies
            {
                CookieToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Create = DateTime.Now,
                Expires = DateTime.Now.AddDays(7)
            };
            return cookie;
        }

        public CookieOptions SetCookies(Cookies cookie)
        {
            // CookieOption är inbyggd class
            var cookieOption = new CookieOptions
            {
                HttpOnly = true,
                Expires = cookie.Expires,
            };

            // can save in database
            foreach(var user in users)
            {
                user.Cookie = cookie.CookieToken;
                user.CookieCreated = cookie.Create;
                user.CookieExpires = cookie.Expires;
            }

            return cookieOption;
        }

        public bool VarifyCookie(string cookie)
        {
            foreach(var userCookie in users)
            {
                if(userCookie.Cookie == cookie && userCookie.CookieExpires > DateTime.Now)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
