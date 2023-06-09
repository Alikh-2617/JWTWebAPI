using JWTWebAPI.Models;
using JWTWebAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JWTWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _service;
        private readonly IConfiguration _config;
        List<User> userList = new List<User>(); // den kan vara DataBase sålänge


        public AuthController(ILogger<AuthController> logger, IAuthService service , IConfiguration configuration)
        {
            _logger = logger;
            _service = service;
            _config = configuration;
        }


        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto user)
        {
            CreatePasswordHash(user.Password, out byte[] passwordHash, out byte[] passwordSalt);
            userList.Add(new User
            {
                UserName = user.UserName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            });
            var user2 = new User { UserName = user.UserName, PasswordHash = passwordHash, PasswordSalt = passwordSalt };
            return Ok(user2);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHas, out byte[] passwordSalt)
        {
            using (var hash = new HMACSHA512())
            {
                passwordSalt = hash.Key;
                passwordHas = hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }



        // get back the token !!
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            // we can check the username if this Exist , here or we can have filter before Action execioting
            foreach (var user in userList)
            {
                if (user.UserName == user.UserName)
                {
                    if(VarifyPasswordHassh(request.Password , user.PasswordHash! , user.PasswordSalt!))
                    {
                        var token = CreateToken(user); 
                        return Ok(token);
                    }
                    return BadRequest("pass is not correct");
                }
            }
            return BadRequest("user not find !");  
        }

        // varify passWord if is corekt with passHash !! 
        private bool VarifyPasswordHassh(string password , byte[] passwordHash , byte[] passwordSalt)
        {   // passwordSalt can we give it from here from appjson or from Database Or pass it to funktion like I did

            using(var hash = new HMACSHA512(passwordSalt)) // Create instance of hash metod WITH SAME KEY !!! 
            {

                // we create one Encoding passwordHash with Same Key to compari the passwords 
                var computHash  = hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes (password));
                 // check if the new passHash is same the old one (return is bool)
                return computHash.SequenceEqual(passwordHash); 

            }
        }

        // for Create Token we need the User Model 
        private string CreateToken(User user)
        {
            // Cleam is users propertis like rol osv.
            List<Claim> claim = new List<Claim>
            {
                new Claim(ClaimTypes.Name , user.UserName),
                new Claim(ClaimTypes.Role , "user")  // user Rolle can put from User Tabel or Cleam Tabel

            };
            // pakeg = Microsoft.Identity.Token
            // Key puts in appsettingjson
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSetting:TokenKey").Value)); 

            var signing_Credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken   // put token detaljer and install pakage for it
                (
                    claims : claim,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials : signing_Credentials
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token); // the Token to send
            return jwt;
        }
    }
}
