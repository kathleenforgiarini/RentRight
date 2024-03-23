using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RentRight.Data;
using System.Security.Claims;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<RentRightContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RentRightContext") ?? throw new InvalidOperationException("Connection string 'RentRightContext' not found.")));


// Authentication and Authorization configurations
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireOwnerRole", policy =>
        policy.RequireClaim(ClaimTypes.Role, "owner"));

    options.AddPolicy("RequireManagerRole", policy =>
       policy.RequireClaim(ClaimTypes.Role, "manager"));

    options.AddPolicy("RequireTenantRole", policy =>
       policy.RequireClaim(ClaimTypes.Role, "tenant"));
});


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "login",
    pattern: "login",
    defaults: new { controller = "Account", action = "Login" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
