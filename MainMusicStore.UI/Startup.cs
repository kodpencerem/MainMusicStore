using System;
using MainMusicStore.DataAccess.Data;
using MainMusicStore.DataAccess.IMainRepository;
using MainMusicStore.DataAccess.MainRepository;
using MainMusicStore.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Office.Interop.Word;

namespace MainMusicStore.UI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
             services.AddDbContext<MainMusicStoreDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders()
                .AddEntityFrameworkStores<MainMusicStoreDbContext>();
            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();

            services.Configure<Utility.EmailOptions>(Configuration);
            //services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            //services.Configure<BrainTreeSettings>(Configuration.GetSection("BrainTree"));
            //services.Configure<TwilioSettings>(Configuration.GetSection("Twilio"));
            //services.AddSingleton<IBrainTreeGate, BrainTreeGate>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            //services.AddScoped<IDbInitiliazer, DbInitializer>();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddRazorPages();
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });
            //services.AddAuthentication().AddFacebook(options =>
            //{
            //    options.AppId = "479144716347128";
            //    options.AppSecret = "8888cefba55e9cfa06a2b28f0495e533";
            //});
            //services.AddAuthentication().AddGoogle(options =>
            //{
            //    options.ClientId = "751413081977-ct8rrlcf8cgt8f42b5evots13mg458lt.apps.googleusercontent.com";
            //    options.ClientSecret = "LPRLug47n8OQsYAirUVGofLw";

            //});
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
