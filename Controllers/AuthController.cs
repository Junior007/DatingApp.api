using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Model;
using DatingApp.API.Services;
using DAtingApp.API.Dtos;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            this._authService = authService;
        }

        // POST: api/Auth
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegister userForRegister)
        {
            

            //TODO: pasar la lógica al servicio y hacer la llamada a un solo método
            try {
                User userToCreate = new User
                {
                    Username = userForRegister.Username,

                };

                User createdUser = await _authService.Register(userToCreate, userForRegister.Password);

                return StatusCode(201);
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }






        }
    }

}