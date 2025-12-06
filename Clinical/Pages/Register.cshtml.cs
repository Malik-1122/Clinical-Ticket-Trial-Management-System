using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Clinical.Data;
using Clinical.Models;
using Clinical.Helpers;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace Clinical.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly ClinicalContext _db;

        public RegisterModel(ClinicalContext db) => _db = db;

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required, StringLength(100)]
            public string Name { get; set; } = null!;

            [Required, EmailAddress]
            public string Email { get; set; } = null!;

            [Required, MinLength(6)]
            public string Password { get; set; } = null!;

            [Required]
            public string Role { get; set; } = "User";
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Check for existing email
            var exists = await _db.Users.AnyAsync(u => u.Email == Input.Email);
            if (exists)
            {
                ModelState.AddModelError(string.Empty, "An account with this email already exists.");
                return Page();
            }

            var user = new Users
            {
                Name = Input.Name,
                Email = Input.Email,
                Role = Input.Role,
                Password = PasswordHelper.HashPassword(Input.Password),
                CreateDate = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // Optionally sign in user automatically - redirect to login for now
            return RedirectToPage("/Login", new { registered = true });
        }
    }
}
