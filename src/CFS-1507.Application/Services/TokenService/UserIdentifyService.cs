using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CFS_1507.Domain.Entities;
using CFS_1507.Infrastructure.Persistence;
using CTCore.DynamicQuery.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CFS_1507.Contract.Helper
{
    public class UserIdentifyService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _dbContext;
        public UserIdentifyService(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }
        public string GetUserId()
        {
            var user_id = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(user_id))
                throw new InvalidOperationException("User_id from token not found!");
            return user_id;
        }
        public async Task<UserEntity> FindCurrentUser(string user_id)
        {
            var currentUser = await _dbContext.UserEntities.Where(x => x.user_id == user_id).FirstOrDefaultAsync();
            if (currentUser is null)
                throw new NotFoundException("Current user not found!");
            return currentUser;
        }
    }
}