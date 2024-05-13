using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using BuzzBid.Models;
using Microsoft.EntityFrameworkCore;

namespace BuzzBid
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices (IServiceCollection services)
        {
            // Register the BuzzBidContext
            services.AddDbContext<BuzzBidContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("BuzzBid")));

            // Add Hosted Service for BuzzBid Backgroup Services
            services.AddHostedService<BuzzBidBackgroundService>();

            // Register the UserManager with BuzzBidContext as a dependency
            services.AddScoped<UserManager>(sp => new UserManager(sp.GetRequiredService<BuzzBidContext>(),sp.GetRequiredService<IHttpContextAccessor>()));
            /*
            services.AddAuthorization(options =>
            {
                options.AddPolicy("MustBeAdmin", policy => policy.RequireRole("Admin"));
            });
            */
            services.AddAuthentication(
                CookieAuthenticationDefaults.AuthenticationScheme
            ).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                options =>
                {
                    options.LoginPath = "/Login";
                    options.LoginPath = "/Logout";
                });
            services.AddMvc();
            // Add MVC services to the application
            services.AddControllersWithViews();

            // Register your custom user manager as a service for dependency injection
            services.AddScoped<UserManager>();

            //authentication: https://stackoverflow.com/questions/36095076/custom-authentication-in-asp-net-core 
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });
            services.AddDistributedMemoryCache();
            
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
