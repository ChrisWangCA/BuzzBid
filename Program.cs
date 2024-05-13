using Microsoft.EntityFrameworkCore;
using BuzzBid.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using BuzzBid;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<BuzzBidContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BuzzBid") ?? throw new InvalidOperationException("Connection string 'BuzzBid' not found.")));
builder.Services.AddScoped<BuzzBid.UserManager>(); // Adjust the scope as needed
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
    });
builder.Services.AddControllersWithViews();
// Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();


// Add Hosted Service for BuzzBid Backgroup Services
builder.Services.AddHostedService<BuzzBidBackgroundService>();

// Register UserManager
builder.Services.AddScoped<UserManager>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.MapRazorPages();

app.Run();
