namespace DatingApp.Data.DTOs
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using DatingApp.Data.Models;

    public class AppUserDto
    {
        public string Email { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Allias { get; set; }

        public DateTime LastActive { get; set; }

        public string Gender { get; set; }

        public string AboutMe { get; set; }

        public string LookingFor { get; set; }

        public string Interests { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public ICollection<Photo> Photos { get; set; }
    }
}
