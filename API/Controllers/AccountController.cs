using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username))
            {
                return this.BadRequest("This username is taken");
            }

            var user = this.mapper.Map<AppUser>(registerDto);

            var result = await this.userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return this.BadRequest(result.Errors);
            }

            var roleResult = await this.userManager.AddToRoleAsync(user, "Member");

            if (!roleResult.Succeeded)
            {
                return this.BadRequest(result.Errors);
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = await this.tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await this.userManager.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            if (user == null)
            {
                return this.Unauthorized("Invalid username");
            }

            var result = await this.signInManager
               .CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                return this.Unauthorized();
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = await this.tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await this.userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}