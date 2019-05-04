using DatingApp.API.Model;
using DatingApp.API.Services;
using DAtingApp.API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        //
        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        // POST: api/Auth
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegister userForRegister)
        {

            //TODO: pasar la lógica al servicio y hacer la llamada a un solo método
            try
            {
                User userToCreate = new User
                {
                    Username = userForRegister.Username,

                };

                User createdUser = await _authService.Register(userToCreate, userForRegister.Password);

                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLogin userForLogin)
        {



            try
            {
                //throw new Exception("error en el servidor");
                //TODO: pasar la lógica al servicio y hacer la llamada a un solo método
                User user = await _authService.Login(userForLogin.Username.ToLower(), userForLogin.Password);

                if (user == null)
                    return Unauthorized();

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

                var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = cred

                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Ok(
                    new
                    {
                        token = tokenHandler.WriteToken(token)
                    }
                    );
            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }

}