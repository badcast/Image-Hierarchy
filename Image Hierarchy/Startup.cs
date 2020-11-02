using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.IO;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;

using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ImageHierarchy
{
    class AuthOptions
    {
        public const string ISSUER = "Image Hierarchy for Test";
        public const string AUDIENCE = "client";
        const string _SECURTY_KEY = "fool_back_the_good_and_stop_introvert";
        public const int LIFETIME_TOKEN = 1; // 1 - minutes
        public static SymmetricSecurityKey Get_Symmetric_SecurityKey()
        {
            return new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_SECURTY_KEY));
        }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = AuthOptions.ISSUER,
                    ValidateAudience = true,
                    ValidAudience = AuthOptions.AUDIENCE,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = AuthOptions.Get_Symmetric_SecurityKey()

                };
            });
            services.AddDirectoryBrowser();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            string path = env.ContentRootPath+"/static_files";

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(path),
            });

            path=path.Replace('\\', '/');

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UsePathBase("/static_files");

            app.UseEndpoints(endpoints =>
            {
                Server.register_route(endpoints);

                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
 