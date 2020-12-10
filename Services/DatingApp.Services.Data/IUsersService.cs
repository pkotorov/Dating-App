namespace DatingApp.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using DatingApp.Data.Models;

    public interface IUsersService
    {
        Task<IEnumerable<T>> GetUsersAsync<T>();

        Task<T> GetUserAsync<T>(string id);
    }
}
