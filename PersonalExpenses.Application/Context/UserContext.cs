using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace PersonalExpenses.Application.Context
{
    public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

        public int? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdClaim?.Value, out var userId))
                {
                    return userId;
                }
                return null;
            }
        }

        public string? Email
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
            }
        }
    }
}
