using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Clinical.Data;
using Clinical.Models;

namespace Clinical.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ClinicalContext _context;

        public LoginModel(ClinicalContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LoginInput Input { get; set; }

        public bool Registered { get; set; }

        public class LoginInput
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public void OnGet(bool registered = false)
        {
            Registered = registered;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // ✅ Check if database is connected
            if (_context.Users == null)
            {
                ModelState.AddModelError(string.Empty, "Database connection failed. Please check configuration.");
                return Page();
            }

            // ✅ Verify user credentials (no hashing)
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == Input.Email && u.Password == Input.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }

            // ✅ Create authentication claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // ✅ Redirect user based on role
            return user.Role switch
            {
                "Admin" => RedirectToPage("/Admin/Dashboard"),
                "Resolver" => RedirectToPage("/Resolver/Dashboard"),
                "Reviewer" => RedirectToPage("/Reviewer/Dashboard"),
                _ => RedirectToPage("/Index")
            };
        }
    }
}
