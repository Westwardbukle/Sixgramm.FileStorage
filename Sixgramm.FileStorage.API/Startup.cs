using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sixgramm.FileStorage.Core.Options;
using Sixgramm.FileStorage.Core.ProFiles;
using Sixgramm.FileStorage.Core.Services;
using Sixgramm.FileStorage.Core.Token;
using Sixgramm.FileStorage.Database;

namespace Sixgramm.FileStorage.API
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
            // Configure App Options
            services.Configure<AppOptions>(Configuration.GetSection(AppOptions.App));
            var appOptions = Configuration.GetSection(AppOptions.App).Get<AppOptions>();
            services.AddSingleton(appOptions);
            
            // Configure Nlog Options
            services.Configure<NLogConfigOptions>(Configuration.GetSection(NLogConfigOptions.Nlog));
            var nlogOptions = Configuration.GetSection(NLogConfigOptions.Nlog).Get<NLogConfigOptions>();
            services.AddSingleton(nlogOptions);
            
            // Configure Authentication
            ConfigureAuthentication(services);
            
            // Configure Swagger
            ConfigureSwagger(services);
            
            // Configure Repositories & Services
            
            /*services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRestoringCodeRepository, RestoringCodeRepository>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IRestorePasswordService, RestorePasswordService>();
            services.AddScoped<IUserService, UserService>();*/
            
            var connection = Configuration.GetConnectionString("DefaultConnection");
            //services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(connection));
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connection,  
                x => x.MigrationsAssembly("Sixgram.Auth.Database")));
            
            //Configure AutoMapper Profile
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AppProfile());
            });
            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
            
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            services.AddControllers();
            
            services.AddControllers();
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(
                    opt => {
                        foreach (var description in provider.ApiVersionDescriptions) {
                            opt.SwaggerEndpoint(
                                $"/swagger/{description.GroupName}/swagger.json", 
                                description.GroupName.ToUpperInvariant()); 
                        } 
                    }
                );
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
        
        
        private void ConfigureAuthentication(IServiceCollection services) 
        {
            services.AddAuthentication(option => 
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; 
            }).AddJwtBearer(options => 
            {
                options.TokenValidationParameters = new TokenValidationParameters 
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])) //Configuration["JwtToken:SecretKey"]
                }; 
                options.SaveToken = true;
            });
        }

        private static void ConfigureSwagger(IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });
            
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
            
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo {Title = "Sixgramm.FileStorage.API", Version = "v1"});
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Autorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "Bearer {authToken}",
                    In = ParameterLocation.Header,
                    Description = "JSON Web Token to access resources. Example: Bearer {token}",
                    //Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
               /* var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);*/
            });
        }
    }
}