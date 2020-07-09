using DatingAppApi.Helper;
using DatingAppApi.Models;
using DatingAppApi.Models.DTOs;
using DatingAppApi.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAppApi.Repositories.Interface
{
   public interface IDataRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
       // void Delete(int id , int userId);

        Task<bool> SaveAll();
        Task<User> GetUser(int id);
        Task<PagedList<User>> GetUsers(UserParams userParams);

        Task<Photo> GetPhoto(int id);
        Task<Photo> GetMainPhotoForUser(int userId);

    }
}
