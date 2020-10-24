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
using AutoMapper;
using DAtingApp.API.Dtos;

namespace DAtingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _datingRepository;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository datingRepository,IMapper mapper)
        {
            _datingRepository = datingRepository;
            _mapper= mapper;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult> GetUsers()
        {
            //var a = User.Identity.Name;
            var users= await _datingRepository.Users();

            var userForList=_mapper.Map<IEnumerable<UserForList>>(users);
            return Ok(userForList);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetUser(int id)
        {
            var user = await _datingRepository.User(id);

            if (user == null)
            {
                return NotFound();
            }


            var UserForDetailed =_mapper.Map<UserForDetailed>(user);
            return Ok(UserForDetailed);
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
        public async Task<ActionResult> PostUser(User user)
        {
            //_context.Users.Add(user);
            //await _context.SaveChangesAsync();

            _datingRepository.Add<User>(user);
            await _datingRepository.SaveAll();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var user = await _datingRepository.User(id);
            if (user == null)
            {
                return NotFound();
            }

            _datingRepository.Delete<User>(user);
            await _datingRepository.SaveAll();
            var UserForDetailed = _mapper.Map<UserForDetailed>(user);

            return Ok(UserForDetailed);
        }

        private bool UserExists(int id)
        {
            return _datingRepository.UserExists(id); //_context.Users.Any(e => e.Id == id);
        }
    }
}
