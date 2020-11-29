namespace DatingApp.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using DatingApp.Services.Data;
    using DatingApp.Web.ViewModels.Users;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUsersService usersService;

        public UsersController(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        [ActionName("All")]
        public async Task<IActionResult> AllAsync()
        {
            var usersViewModel = new AllUsersInListViewModel
            {
                Users = await this.usersService.GetUsersAsync<AllUsersViewModel>(),
            };

            return this.View(usersViewModel);
        }
    }
}
