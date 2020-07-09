using DatingAppApi.Helper;
using DatingAppApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DatingAppApi.DataContext.Data
{
    public  class Seed
    {
        public static void SeedUsers(DatingAppContext context)
        {
            if (!context.users.Any())
            {
                var userdata = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userdata);
                foreach(var user in users)
                {
                     byte[] hashpassword, saltpassword;
                    Hashing.CreateHashPassword("password", out hashpassword, out saltpassword);
                    user.PasswordHash = hashpassword;
                    user.PasswordSalt = saltpassword;
                    user.UserName = user.UserName.ToLower();
                    context.users.Add(user);

                }
                context.SaveChanges();
            }

        }

        public static void CreateHashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            }
        }
    }
}
