using DatingApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Services.Data
{
    public interface IUsersService
    {
        Task<IEnumerable<T>> GetUsersAsync<T>();
    }
}
