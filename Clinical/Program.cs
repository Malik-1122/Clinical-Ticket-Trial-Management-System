using Clinical.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages
builder.Services.AddRazorPages();

// Database
builder.Services.AddDbContext<ClinicalContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ClinicalContext")));

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

// Authorization
builder.Services.AddAuthorization();

// ? ADD SESSION HERE ?
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Authentication + Authorization
app.UseAuthentication();
app.UseAuthorization();

// ? ENABLE SESSION HERE ?
app.UseSession();

app.MapRazorPages();
app.Run();
