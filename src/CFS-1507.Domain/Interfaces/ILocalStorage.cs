using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CFS_1507.Domain.Interfaces
{
    public interface ILocalStorage
    {
        Task<string> SaveImageAsync(IFormFile imageFile, string folder);
    }
}