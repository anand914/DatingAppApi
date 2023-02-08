using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AutoMapper;
using DatingAppApi.Helper;
using DatingAppApi.Models;
using DatingAppApi.Models.DTOs;
using DatingAppApi.Models.ViewModel;
using DatingAppApi.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Recommendations;

namespace DatingAppApi.Controllers
{
   // [ServiceFilter(typeof(LogUserActivity))]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly IDataRepository _dataRepository;

        private readonly IMapper _mapper;
        public UserController(IDataRepository dataRepository, IMapper mapper)
        {
            _dataRepository = dataRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUser(int UserId, [FromQuery] UserParams userParams)
        {
            try
            {
            //    var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            //    var user = await _dataRepository.GetUser(currentUserId);
            //    userParams.UserId = currentUserId;
                if (string.IsNullOrEmpty(userParams.Gender))
                {
                    userParams.Gender = userParams.Gender == "male" ? "female" : "male";
                }
                
                
                var usr = await _dataRepository.GetUsers(userParams);
                var userToReturn = _mapper.Map<IEnumerable<UserListVM>>(usr);
                if (userToReturn != null)
                {
                    Response.AddPagination(usr.CurrentPage, usr.PageSize, usr.TotalCount, usr.TotalPages);
                    return Ok(userToReturn);
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, false, new List<string> { ex.Message }, null, null));
            }
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUserByID(int id)
        {
            try
            {
                var data = await _dataRepository.GetUser(id);
                var usertoReturn = _mapper.Map<UserForDetailsVM>(data);
                if (usertoReturn != null)
                {
                    return Ok(usertoReturn);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, false, new List<string> { ex.Message }, null, null));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(UserForUpdateVM model)
        {
            try
            {
                var userFromRepo = await _dataRepository.GetUser(model.Id);
                _mapper.Map(model, userFromRepo);
                if (await _dataRepository.SaveAll())
                    return NoContent();
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                throw new Exception($"Updating user failed on save");
            }
        }

        [HttpPost("{id}/like/{recepientId}")]
        public async Task<IActionResult> LikeUser(int id, int recepientId)
        {
            //if (id !=  int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            //return Unauthorized();

            var like = await _dataRepository.GetLike(id, recepientId);
            if (like != null)
            {
                return BadRequest("You Already Like the User");
            }
            if (await _dataRepository.GetUser(recepientId) == null)
            {
                return NotFound();
            }
            like = new Like
            {
                LikerId = id,
                LikeeId = recepientId
            };
            _dataRepository.Add<Like>(like);
            if (await _dataRepository.SaveAll())
                return Ok();

            return BadRequest("Failed To Like User");
        }
    }
}