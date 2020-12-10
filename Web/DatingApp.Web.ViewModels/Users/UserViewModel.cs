namespace DatingApp.Web.ViewModels.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AutoMapper;
    using DatingApp.Data.Models;
    using DatingApp.Services.Mapping;
    using DatingApp.Web.ViewModels.Photos;

    public class UserViewModel : IMapFrom<ApplicationUser>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string PhotoUrl { get; set; }

        public int Age => this.CalculateAge();

        public string Allias { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime LastActive { get; set; }

        public string Gender { get; set; }

        public string AboutMe { get; set; }

        public string LookingFor { get; set; }

        public string Interests { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public DateTime DateOfBirth { get; set; }

        public IEnumerable<PhotoViewModel> Photos { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, UserViewModel>()
                .ForMember(x => x.PhotoUrl, opt => opt.MapFrom(x => x.Photos
                    .FirstOrDefault(x => x.IsMain).Url));
        }

        private int CalculateAge()
        {
            var today = DateTime.Today;
            var age = today.Year - this.DateOfBirth.Year;

            if (this.DateOfBirth.Date > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }
}
