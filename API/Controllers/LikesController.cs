using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUserService userService;

        private readonly ILikesService likesService;

        public LikesController(IUserService userService, ILikesService likesService)
        {
            this.likesService = likesService;
            this.userService = userService;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await this.userService.GetUserByUsernameAsync(username);
            var sourceUser = await this.likesService.GetUserWithLikes(sourceUserId);

            if (likedUser == null)
            {
                return this.NotFound();
            }

            if (sourceUser.UserName == username)
            {
                return this.BadRequest("You cannot like yourself");
            }

            var userLike = await this.likesService.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null)
            {
                return this.BadRequest("You already like this user");
            }

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            if (await this.userService.SaveAllAsync())
            {
                return this.Ok();
            }

            return this.BadRequest("Failed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var users = await this.likesService.GetUserLikes(likesParams);

            Response.AddPaginationHeader(users.CurrentPage,
                users.PageSize, users.TotalCount, users.TotalPages);

            return this.Ok(users);
        }
    }
}