using DatingAppApi.Models;
using DatingAppApi.Models.DTOs;
using DatingAppApi.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAppApi.Repositories.Interface
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password);
        //Task<UserVM> Register(UserVM model);
        Task<ApiResponse> Login(UserForLoginVM model);
        Task<bool> IsUserExist(string username);

    }
}
