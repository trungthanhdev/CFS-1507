using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CFS_1507.Contract.Helper
{
    public class UserIdentifyService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserIdentifyService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string GetUserId()
        {
            var user_id = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(user_id))
                throw new InvalidOperationException("User_id from token not found!");
            return user_id;
        }
    }
}