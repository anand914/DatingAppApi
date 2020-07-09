using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingAppApi.Helper;
using DatingAppApi.Models;
using DatingAppApi.Models.ViewModel;
using DatingAppApi.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingAppApi.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly IDataRepository _dataRepository;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;
        public PhotoController(IDataRepository dataRepository, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _mapper = mapper;
            _dataRepository = dataRepository;

            Account account = new Account
            (
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var PhotofromRepo = await _dataRepository.GetPhoto(id);
            var photo = Mapper.Map<PhotoFromReturnVM>(PhotofromRepo);
            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm] PhotoforCreationVM model)
        {
            //if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            //    return Unauthorized();

            var userFromRepo = await _dataRepository.GetUser(userId);
            var file = model.File;

            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation()
                            .Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            model.Url = uploadResult.Url.ToString();
            model.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(model);

            if (!userFromRepo.Photos.Any(u => u.IsMain))
                photo.IsMain = true;

            userFromRepo.Photos.Add(photo);

            if (await _dataRepository.SaveAll())
            {
                var phototoReturn = _mapper.Map<PhotoFromReturnVM>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.ID }, phototoReturn);
                //return CreatedAtAction(nameof(GetPhoto), new { id = photo.ID }, phototoReturn);

            }

            return BadRequest("Could not add the photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            //if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            //    return Unauthorized();

            var user = await _dataRepository.GetUser(userId);
            if (!user.Photos.Any(p => p.ID == id))
                return Unauthorized();

            var photofromRepo = await _dataRepository.GetPhoto(id);
            if (photofromRepo.IsMain)
                return BadRequest("This is already the main photo");

            var currentMainPhoto = await _dataRepository.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;

            photofromRepo.IsMain = true;
            if (await _dataRepository.SaveAll())
                return NoContent();

            return BadRequest("Could not set photo to main");

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            //if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            //    return Unauthorized();

            var user = await _dataRepository.GetUser(userId);
            if (!user.Photos.Any(p => p.ID == id))
                return Unauthorized();

            var photofromRepo = await _dataRepository.GetPhoto(id);
            if (photofromRepo.IsMain)
                return BadRequest("you cannot delete your main photo");

            var deleteParams = new DeletionParams(photofromRepo.PublicId);
            var result = _cloudinary.Destroy(deleteParams);

            if (result.Result == "ok")
            {
                _dataRepository.Delete(photofromRepo);
              
             
            }
            if (photofromRepo.PublicId == null)
            {
                _dataRepository.Delete(photofromRepo);
            }

            if (await _dataRepository.SaveAll())
            {
                return Ok();
            }
            return BadRequest("Failed to  delete the  photo");

        }

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeletePhoto(int userId, int id)
        //{
        //    if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        //        return Unauthorized();

        //    var user = await _dataRepository.GetUser(userId);

        //    if (!user.Photos.Any(p => p.ID == id))
        //        return Unauthorized();

        //    var photoFromRepo = await _dataRepository.GetPhoto(id);

        //    if (photoFromRepo.IsMain)
        //        return BadRequest("You cannot delete your main photo");

        //    if (photoFromRepo.PublicId != null)
        //    {
        //        var deleteParams = new DeletionParams(photoFromRepo.PublicId);

        //        var result = _cloudinary.Destroy(deleteParams);
        //         _dataRepository.Delete(id, userId);
        //        //if (result.Result == "ok")
        //        //{
        //        //    _dataRepository.Delete(id);
        //        //}
        //    }

        //    if (photoFromRepo.PublicId == null)
        //    {
        //        _dataRepository.Delete(id, userId);
        //    }

        //    if (await _dataRepository.SaveAll())
        //        return Ok();

        //    return BadRequest("Failed to delete the photo");
        //}

    }

}


