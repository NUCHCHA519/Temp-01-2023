using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MasterCoreMVC.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Rotativa.AspNetCore;

namespace MasterCoreMVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            this.FileServicePath = string.IsNullOrEmpty(Configuration["FileServicePath"])  
                ? Path.Combine(env.WebRootPath,"fileservice/") 
                : Path.Combine(Configuration["FileServicePath"]);
        }

        public IConfiguration Configuration { get; }
        public string FileServicePath { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options=>{
                    options.Cookie.Name = "_auth_"+System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                    options.LoginPath = "/Auth/Login";
                    options.AccessDeniedPath = "/Auth/Denied";
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                });
            services.AddSingleton<Helpers.IFileService,Helpers.FileService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // using Microsoft.Extensions.FileProviders;
            // using System.IO;
            // ======= Seting File Upload in Temp Folder ===========
            if (!Directory.Exists(FileServicePath)){
                Directory.CreateDirectory(FileServicePath);
            }
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    FileServicePath),
                RequestPath = "/fileservice"
            });
            // ======= Seting File Upload in Temp Folder ===========

            RotativaConfiguration.Setup(env.WebRootPath, "Rotativa");

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
