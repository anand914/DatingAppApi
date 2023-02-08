using DatingAppApi.DataContext;
using DatingAppApi.Helper;
using DatingAppApi.Models;
using DatingAppApi.Models.DTOs;
using DatingAppApi.Models.ViewModel;
using DatingAppApi.Repositories.Interface;
using Microsoft.AspNetCore.Identity.UI.Pages.Internal.Account;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAppApi.Repositories.Services
{
    public class DataRepository : IDataRepository
    {
        private readonly DatingAppContext _context;
        public DataRepository(DatingAppContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.ID == id);
            return user;
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.users.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();
            users = _context.users.Where(u => u.ID != userParams.UserId);
            users = users.Where(u => u.Gender == userParams.Gender);

            if (userParams.Likers)
            {
                var userlikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userlikers.Contains(u.ID));
            }
            if (userParams.Likees)
            {
                var userlikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userlikees.Contains(u.ID));
            }
            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }

            if (string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _context.users.Include(x => x.Likers)
                                       .Include(x => x.Likees)
                                       .FirstOrDefaultAsync(u => u.ID == id);

            if (likers)
            {
                return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
            }
            else
            {
                return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
            }
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.ID == id);
            return photo;
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            try
            {
                var data = await _context.Photos.Where(p => p.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Like> GetLike(int userId, int recepientId)
        {
            try
            {
                var result = await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recepientId);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public async void Delete(int id, int userId)
        //{
        //    var data = _context.Photos.Where(p => p.ID == id && p.UserId == userId).FirstOrDefault();
        //    if (data != null)
        //    {
        //        _context.Photos.Remove(data);
        //        _context.SaveChanges();

        //    }

        //}

    }
}
