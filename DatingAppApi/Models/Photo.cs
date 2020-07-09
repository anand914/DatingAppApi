using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAppApi.Models
{
    public class Photo
    {
        public int ID { get; set; }
        public string   URL { get; set; }
        public string Description { get; set; }
        public bool IsMain { get; set; }
        public DateTime DateAdded { get; set; }
        public string PublicId { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
    }
}
