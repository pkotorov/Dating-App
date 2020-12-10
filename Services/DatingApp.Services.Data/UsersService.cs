namespace DatingApp.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using DatingApp.Data.Common.Repositories;
    using DatingApp.Data.Models;
    using DatingApp.Services.Mapping;
    using Microsoft.EntityFrameworkCore;

    public class UsersService : IUsersService
    {
        private readonly IRepository<ApplicationUser> userRepository;

        public UsersService(IRepository<ApplicationUser> userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<T> GetUserAsync<T>(string id)
        {
            return await this.userRepository
                .AllAsNoTracking()
                .Where(i => i.Id == id)
                .To<T>()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetUsersAsync<T>()
        {
            var users = await this.userRepository
                .AllAsNoTracking()
                .Where(p => p.Photos.Any())
                .Include(p => p.Photos)
                .To<T>()
                .ToListAsync();

            return users;
        }
    }
}
