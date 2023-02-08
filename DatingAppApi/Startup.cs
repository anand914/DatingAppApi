using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingAppApi.DataContext;
using DatingAppApi.DataContext.Data;
using DatingAppApi.Helper;
using DatingAppApi.Repositories.Interface;
using DatingAppApi.Repositories.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace DatingAppApi
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

            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value);
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new Info
            //    {
            //        Version = "v1",
            //        Title = "MyAPI",
            //        Description = "Testing"
            //    });
            //    c.AddSecurityDefinition("Bearer", new ApiKeyScheme()
            //    {
            //        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            //        Name = "Authorization",
            //        In = "header",
            //        Type = "apiKey"
            //    });
            //    c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
            //    {
            //        { "Bearer", new string[] { } }
            //    });
            //});
            ////services.AddCors(options =>
            ////{
            ////    options.AddPolicy("CorsPolicy", builder => builder.WithOrigins("http://localhost:4200")
            ////        .AllowAnyHeader()
            ////        .AllowAnyMethod()
            ////        .AllowCredentials()
            ////        .SetIsOriginAllowed((host) => true));
            ////});
            //services.AddCors(o => o.AddPolicy("AllowOrigin", builder =>
            //{
            //    builder.AllowAnyOrigin()
            //           .AllowAnyMethod()
            //           .AllowAnyHeader();
            //}));
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //services.Configure<IISOptions>(options =>
            //{
            //    options.ForwardClientCertificate = false;
            //});
            //services.AddDbContext<DatingAppContext>(Options => Options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
            //   AddJwtBearer(options =>
            //   {
            //       options.TokenValidationParameters = new TokenValidationParameters
            //       {
            //           ValidateIssuerSigningKey = true,
            //           IssuerSigningKey = new SymmetricSecurityKey(key),
            //           ValidateIssuer = false,
            //           ValidateAudience = false
            //       };
            //   });
            ////services.AddCors(c =>
            ////{
            ////    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());
            ////});
            ///var key = Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Dating API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", new string[] { } }
                });
            });
          
            //services.AddHttpContextAccessor();
            //services.AddSingleton(mapper);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
           services.AddDbContext<DatingAppContext>(Options => Options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            var corsBuilder = new CorsPolicyBuilder();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            //string[] originUrl =
            //{
            //    Configuration.GetSection("BaseURI:Url").Value,
            //};
            //corsBuilder.WithOrigins(originUrl);
            corsBuilder.AllowAnyOrigin();
            corsBuilder.AllowCredentials();
            services.AddCors(options =>
            {
                options.AddPolicy("SiteCorsPolicy", corsBuilder.Build());
            });

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("SiteCorsPolicy"));
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
               AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(key),
                       ValidateIssuer = false,
                       ValidateAudience = false
                   };
               });

            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            services.AddAutoMapper();
            services.AddTransient<Seed>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IDataRepository, DataRepository>();
           // services.AddScoped<LogUserActivity>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
            }
            
           // app.UseCors("CorsPolicy");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DatingAPI  V1");
               
                c.DefaultModelsExpandDepth(-1);
            });
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseCors("SiteCorsPolicy");
            app.UseMvc();
        }
    }
}
