namespace DatingApp.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DatingApp.Data.Models;
    using DatingApp.Services.Data;
    using DatingApp.Web.ViewModels.Users;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUsersService usersService;
        private readonly UserManager<ApplicationUser> userManager;

        public UsersController(
            IUsersService usersService,
            UserManager<ApplicationUser> userManager)
        {
            this.usersService = usersService;
            this.userManager = userManager;
        }

        [ActionName("All")]
        public async Task<IActionResult> AllAsync()
        {
            var usersViewModel = new AllUsersInListViewModel
            {
                Users = await this.usersService.GetUsersAsync<UserViewModel>(),
            };

            return this.View(usersViewModel);
        }

        [ActionName("ById")]
        public async Task<IActionResult> GetUserAsync(string id)
        {
            var user = await this.usersService.GetUserAsync<UserViewModel>(id);
            return this.View(user);
        }

        [ActionName("MyProfile")]
        public async Task<IActionResult> CurrentUserProfile()
        {
            var userId = this.userManager.GetUserId(this.User);

            var user = await this.usersService.GetUserAsync<UserViewModel>(userId);

            return this.View(user);
        }
    }
}
