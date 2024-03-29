﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAppApi.Models.ViewModel
{
    public class UserListVM
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
       // public string Gender { get; set; }
        public int Age { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Country { get; set; }
        public string City { get; set; }

        public string PhotoUrl { get; set; }
    }
}
