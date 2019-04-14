using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        private readonly IAuthService _repository;

        public AuthController(IAuthService repository)
        {
            this._repository = repository;
        }

        // POST: api/Auth
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegister userForRegister)
        {
            //TODO: pasar la lógica al servicio y hacer la llamada a un solo método
            /* try {
                User createdUser = await _repository.Register(userToCreate, userpassword);
                return StatusCode(201);
            } catch (Exception ex) { 
                return BadRequest(ex.Message);
            }*/



            userForRegister.Username = userForRegister.Username.ToLower();

            if (await _repository.UserExists(userForRegister.Username))
                return BadRequest("Username already exists");

            User userToCreate = new User
            {
                Username = userForRegister.Username,
                
            };

            User createdUser = await _repository.Register(userToCreate, userForRegister.Userpassword);

            return StatusCode(201);


        }
    }

}