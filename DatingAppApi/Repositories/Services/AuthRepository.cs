using AutoMapper;
using DatingAppApi.DataContext;
using DatingAppApi.Helper;
using DatingAppApi.Models;
using DatingAppApi.Models.DTOs;
using DatingAppApi.Models.ViewModel;
using DatingAppApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DatingAppApi.Repositories.Services
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DatingAppContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public AuthRepository(DatingAppContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }
        public async Task<bool> IsUserExist(string username)
        {
            if (await _context.users.AnyAsync(x => x.UserName == username))
            {
                return true;
            }

            return false;
        }

        public async Task<ApiResponse> Login(UserForLoginVM model)
        {
            try
            {
                var data = _context.users.Include(p => p.Photos).Where(x => x.UserName == model.Username).FirstOrDefault();
                if (data != null)
                {
                    var tokenhandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Token").Value);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                          new Claim(ClaimTypes.NameIdentifier,data.ID.ToString()),
                           new Claim(ClaimTypes.Name,data.UserName),

                        }),
                        NotBefore = DateTime.Now,
                        Expires = DateTime.Now.AddDays(7),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha512Signature)
                    };
                    var token = tokenhandler.CreateToken(tokenDescriptor);
                    var UAT = tokenhandler.WriteToken(token);
                    var user = _mapper.Map<UserListVM>(data);
                    return new ApiResponse(200, true, null, user, UAT);
                }
                else
                {
                    return new ApiResponse(201, false, new List<string> { "Wrong User And/Or Password." }, null, null);
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, false, new List<string> { ex.Message }, null, null);
            }
        }

        //public async Task<UserVM> Register(UserVM model, string password)
        //{

        //    try
        //    {
        //        byte[] hashpassword, saltpassword;
        //        Hashing.CreateHashPassword(password, out hashpassword, out saltpassword);
        //        await _context.users.AddAsync(model);
        //        await _context.SaveChangesAsync();
        //        return model;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}
        public async Task<User> Register(User user, string password)
        {
            byte[] hashpassword, saltpassword;
            Hashing.CreateHashPassword(password, out hashpassword, out saltpassword);
            user.PasswordHash = hashpassword;
            user.PasswordSalt = saltpassword;

            await _context.users.AddAsync(user);

            await _context.SaveChangesAsync();

            return user;
        }
    }
}
