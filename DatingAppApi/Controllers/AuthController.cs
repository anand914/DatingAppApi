using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DatingAppApi.Models;
using DatingAppApi.Models.DTOs;
using DatingAppApi.Models.ViewModel;
using DatingAppApi.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DatingAppApi.Controllers
{
   //[Authorize]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository authRepository, IConfiguration configuration, IMapper mapper)
        {
            _authRepository = authRepository;
            _configuration = configuration;
            _mapper = mapper;
        }

        //[HttpPost]
        //public async Task<IActionResult> Register(UserVM model, string password)
        //{
        //    try
        //    {
        //        var user = _mapper.Map<User>(model);
        //        var result = await _authRepository.Register(user, password);
        //        var userToReturn = _mapper.Map<UserForDetailsVM>(user);
        //        return CreatedAtRoute("GetUser",
        //            new { controller = "User", id = result.ID }, userToReturn);
        //        if (result.StatusCode == 500)
        //        {
        //            return StatusCode(500, result);
        //        }

        //        //return Ok(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ApiResponse(500, false, new List<string> { ex.Message }, null, null));
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> Register(UserVM model)
        {
            var userToCreate = _mapper.Map<User>(model);

            var result = await _authRepository.Register(userToCreate, model.password);

            var userToReturn = _mapper.Map<UserForDetailsVM>(userToCreate);

            return CreatedAtRoute("GetUser",
                    new { controller = "User", id = userToCreate.ID }, userToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserForLoginVM model)
        {
            try
            {
                var user = await _authRepository.Login(model);

                if (user.StatusCode == 500)
                {
                    return StatusCode(500, user);
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, false, new List<string> { ex.Message }, null, null));
            }
        }
    }
}