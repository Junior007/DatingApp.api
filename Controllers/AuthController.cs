using DatingApp.API.Model;
using DatingApp.API.Services;
using DatingApp.API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        //
        public AuthController(IAuthService authService, IConfiguration configuration, IMapper mapper)
        {
            _authService = authService;
            _configuration = configuration;
            _mapper = mapper;
        }

        // POST: api/Auth
        [HttpPost]
        [Route("register")]
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

        [HttpPost]
        [Route("login")]
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
                var userForList = _mapper.Map<UserForList>(user);
                return Ok(
                    new
                    {
                        token = tokenHandler.WriteToken(token),
                        //userForList
                        user= userForList
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