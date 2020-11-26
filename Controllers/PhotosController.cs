using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatingApp.API.Data;
using DatingApp.API.Model;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using DatingApp.API.Helpers;
using CloudinaryDotNet;
using DatingApp.API.Dtos;
using System.Security.Claims;
using CloudinaryDotNet.Actions;

namespace DatingApp.API.Controllers
{
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    [Authorize]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _datingRepository;
        private readonly Cloudinary _cloudinary; private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinarySettings;


        public PhotosController(IDatingRepository datingRepository, IMapper mapper, IOptions<CloudinarySettings> cloudinarySettings)
        {
            //_context = context; 
            _datingRepository = datingRepository;
            _mapper = mapper;
            _cloudinarySettings = cloudinarySettings;

            var account = new CloudinaryDotNet.Account(
                _cloudinarySettings.Value.CloudName,
                _cloudinarySettings.Value.ApiKey,
                _cloudinarySettings.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }

        // GET: api/Photos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Photo>> GetPhoto(int id)
        {
            var photo = await _datingRepository.GetPhoto(id);

            if (photo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PhotoForReturn>(photo));
        }
        // POST: api/Photos
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Photo>> PostPhoto(int userId
            , [FromForm] PhotoForCreation photoForCreation)
        {

            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var user = await _datingRepository.User(userId);

            var file = photoForCreation.file;
            var uploadResult = new ImageUploadResult();


            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);

                }

                photoForCreation.Url = uploadResult.Url.ToString();
                photoForCreation.PublicId = uploadResult.PublicId;

                var photo = _mapper.Map<Photo>(photoForCreation);

                if (!user.Photos.Any(ph => ph.IsMain))
                {
                    photo.IsMain = true;
                }

                user.Photos.Add(photo);

                if (await _datingRepository.SaveAll())
                {
                    var photoForReturn = _mapper.Map<PhotoForReturn>(photo);
                    return CreatedAtRoute(new { userId = userId, id = photo.Id }, photoForReturn);
                }
            }

            return BadRequest("Could not add the photo");
        }
        //
        [HttpPost("{photoId}/setMain")]
        public async Task<ActionResult<Photo>> SetMainPhoto(int userId, int photoId)
        {

            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var user = await _datingRepository.User(userId);

            if (!user.Photos.Any(ph => ph.Id == photoId))
            {
                return Unauthorized();
            }

            var photoFromRepo = await _datingRepository.GetPhoto(photoId);

            if (photoFromRepo.IsMain)
            {
                return BadRequest("This is already the main photo");
            }

            Photo currentMainPhoto = await _datingRepository.GetMainPhotoForUser(userId);

            currentMainPhoto.IsMain = false;
            photoFromRepo.IsMain = true;

            if (await _datingRepository.SaveAll())
                return NoContent();

            return BadRequest("Could not set photo to main");

        }
        //

        // DELETE: api/Photos/5
        [HttpDelete("{photoId}/DeletePhoto")]
        public async Task<ActionResult<Photo>> DeletePhoto(int userId, int photoId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var user = await _datingRepository.User(userId);

            if (!user.Photos.Any(ph => ph.Id == photoId))
            {
                return Unauthorized();
            }

            var photoFromRepo = await _datingRepository.GetPhoto(photoId);

            if (photoFromRepo.IsMain)
            {
                return BadRequest("You cannot delete your main photo");
            }

            if (photoFromRepo.PublicId != null)
            {
                var deletionParams = new DeletionParams(photoFromRepo.PublicId);

                var result = await _cloudinary.DestroyAsync(deletionParams);

                if (result.Result == "ok")
                {
                    _datingRepository.Delete<Photo>(photoFromRepo);
                }
            }
            else
            {
                _datingRepository.Delete<Photo>(photoFromRepo);
            }

            if (await _datingRepository.SaveAll())
            {

                return Ok();
            }

            return BadRequest("Failed to delete the photo");

        }

        /*
        //
        // GET: api/Photos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Photo>>> GetPhoto()
        {
            return await _contextBORRAR.Photo.ToListAsync();
        }

        // GET: api/Photos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Photo>> GetPhoto(int id)
        {
            var photo = await _contextBORRAR.Photo.FindAsync(id);

            if (photo == null)
            {
                return NotFound();
            }

            return photo;
        }

        // PUT: api/Photos/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPhoto(int id, Photo photo)
        {
            if (id != photo.Id)
            {
                return BadRequest();
            }

            _contextBORRAR.Entry(photo).State = EntityState.Modified;

            try
            {
                await _contextBORRAR.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhotoExists(id))
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
        
        private bool PhotoExists(int id)
        {
            return _contextBORRAR.Photo.Any(e => e.Id == id);
        }*/
    }
}
