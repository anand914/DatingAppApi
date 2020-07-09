using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DatingAppApi.Models.ViewModel
{

    public class UserForLoginVM
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }



}
