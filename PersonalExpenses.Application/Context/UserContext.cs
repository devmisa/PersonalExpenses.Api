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
                Claim? userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
                return int.TryParse(userIdClaim?.Value, out int userId) ? userId : null;
            }
        }

        public string? Email => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    }
}
