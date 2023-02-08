using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAppApi.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth  { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }

        public string Country { get; set; }
        public string City { get; set; }

        public  virtual ICollection<Photo> Photos { get; set; }

        public virtual  ICollection<Like> Likers { get; set; }
        public virtual  ICollection<Like> Likees { get; set; }
    }

   
}
