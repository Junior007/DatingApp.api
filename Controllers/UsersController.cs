using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatingApp.API.Data;
using DatingApp.API.Model;
using Microsoft.AspNetCore.Authorization;
using DAtingApp.API.Data;

namespace DAtingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _datingRepository;

        public UsersController(IDatingRepository datingRepository)
        {
            _datingRepository = datingRepository;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            //var a = User.Identity.Name;
            var users= await _datingRepository.Users();
            return Ok(users);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _datingRepository.User(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            //_context.Entry(user).State = EntityState.Modified;
            _datingRepository.Update<User>(user);
            try
            {
                await _datingRepository.SaveAll();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            //_context.Users.Add(user);
            //await _context.SaveChangesAsync();

            _datingRepository.Add<User>(user);
            await _datingRepository.SaveAll();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _datingRepository.User(id);
            if (user == null)
            {
                return NotFound();
            }

            _datingRepository.Delete<User>(user);
            await _datingRepository.SaveAll();

            return user;
        }

        private bool UserExists(int id)
        {
            return _datingRepository.UserExists(id); //_context.Users.Any(e => e.Id == id);
        }
    }
}
