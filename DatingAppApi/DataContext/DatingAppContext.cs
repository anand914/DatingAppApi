using DatingAppApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAppApi.DataContext
{
    public class DatingAppContext : DbContext
    {
        public DatingAppContext(DbContextOptions<DatingAppContext> options) : base(options)
        {

        }

        public DbSet<User> users { get; set; }
        public DbSet<Photo> Photos { get; set; }
    }
}
