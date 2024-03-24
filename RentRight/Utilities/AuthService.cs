using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using RentRight.Data;
using System.Security.Claims;

namespace RentRight.Utilities
{
    public class AuthService
    {
        private readonly RentRightContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(RentRightContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> AuthenticateUserAsync(string email, string password)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("Type", user.Type),
            };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var claimsPrincipal = new ClaimsPrincipal(identity);

                await _httpContextAccessor.HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);
                return true;
            }
            return false;
        }
    }
}
