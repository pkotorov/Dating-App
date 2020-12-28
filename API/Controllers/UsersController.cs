using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserService userService;

        private readonly IMapper mapper;

        private readonly IPhotoService photoService;

        public UsersController(
            IUserService userService,  
            IMapper mapper,
            IPhotoService photoService)
        {
            this.mapper = mapper;
            this.photoService = photoService;
            this.userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            var user = await userService.GetUserByUsernameAsync(User.GetUserName());

            userParams.CurrentUsername = user.UserName;

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = user.Gender == "male" ? "female" : "male";
            }

            var users = await userService.GetMembersAsync(userParams);

            Response.AddPaginationHeader(
                users.CurrentPage, 
                users.PageSize, 
                users.TotalCount, 
                users.TotalPages);

            return this.Ok(users);
        }

        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await userService.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var user = await userService.GetUserByUsernameAsync(User.GetUserName());

            this.mapper.Map(memberUpdateDto, user);

            userService.Update(user);

            if (await userService.SaveAllAsync())
            {
                return this.NoContent();
            }

            return this.BadRequest("Failed to update user.");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await userService.GetUserByUsernameAsync(User.GetUserName());

            var result = await photoService.AddPhotoAsync(file);

            if (result.Error != null)
            {
                return this.BadRequest(result.Error.Message);
            }

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if (await userService.SaveAllAsync())
            {
                return this.CreatedAtRoute("GetUser", new {username = user.UserName}, this.mapper.Map<PhotoDto>(photo));
            }

            return this.BadRequest("Problem adding photo.");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await userService.GetUserByUsernameAsync(User.GetUserName());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo.IsMain)
            {
                return this.BadRequest("This is already your main photo.");
            }

            var currentMainPhoto = user.Photos.FirstOrDefault(x => x.IsMain);

            if (currentMainPhoto != null)
            {
                currentMainPhoto.IsMain = false;
            }

            photo.IsMain = true;

            if(await userService.SaveAllAsync())
            {
                return this.NoContent();
            }

            return this.BadRequest("Failed to set main photo.");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await userService.GetUserByUsernameAsync(User.GetUserName());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null)
            {
                return this.NotFound();
            }

            if (photo.IsMain)
            {
                return this.BadRequest("You cannot delete your main photo.");
            }

            if (photo.PublicId != null)
            {
                var result = await photoService.DeletePhotoAsync(photo.PublicId);
                
                if (result.Error != null)
                {
                    return this.BadRequest(result.Error.Message);
                }
            }

            user.Photos.Remove(photo);

            if (await userService.SaveAllAsync())
            {
                return this.Ok();
            }

            return this.BadRequest("Failed to delete the photo.");
        }
    }
}