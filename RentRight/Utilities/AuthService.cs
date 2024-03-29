using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using RentRight.Data;
using System.Security.Claims;

namespace RentRight.Utilities
{
    public class AuthService
    {
        private readonly RentRightContext _context;

        public AuthService(RentRightContext context)
        {
            _context = context;
        }

        public async Task<bool> AuthenticateUserAsync(string email, string password, HttpContext httpContext)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email && u.Password == password && u.IsActive);
            if (user != null)
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(ClaimTypes.Role, user.Type.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("Type", user.Type.ToString()),
            };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var claimsPrincipal = new ClaimsPrincipal(identity);

                await httpContext.SignInAsync("MyCookieAuth", claimsPrincipal);
                
                return true;
            }
            return false;
        }
    }
}
