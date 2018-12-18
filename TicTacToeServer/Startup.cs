using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Threading.Tasks;
using TicTacToeServer.Database;
using TicTacToeServer.Hubs;
using TicTacToeServer.Models;
using TicTacToeServer.Services;

namespace TicTacToeServer
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IGameService, GameService>();
            services.AddScoped<IHubService, HubService>();

            services.AddDbContext<Db>(options => options.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;
                Database=TicTacToeProject;
                Trusted_Connection=True;
                ConnectRetryCount=0"
             ));

            _addIdentity(services);
            _addJwtAuth(services);
            _addCors(services);

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");
            app.UseWebSockets();
            app.UseSignalR(options =>
            {
                options.MapHub<RoomHub>("/roomHub");
                options.MapHub<GameHub>("/gameHub");
            });
            app.UseMvc();
        }

        private void _addCors(IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("CorsPolicy",
               builder =>
               {
                   builder
                        .AllowAnyOrigin()
                        //.WithOrigins("http://localhost:4200")
                        .AllowCredentials()
                        .AllowAnyMethod()
                        .AllowAnyHeader();   
               }));
        }

        private void _addIdentity(IServiceCollection services)
        {
            services.AddIdentity<AppUser, IdentityRole>(
               option =>
               {
                   option.Password.RequireDigit = false;
                   option.Password.RequiredLength = 6;
                   option.Password.RequireNonAlphanumeric = false;
                   option.Password.RequireUppercase = false;
                   option.Password.RequireLowercase = false;
               }
           )
           .AddEntityFrameworkStores<Db>()
           .AddDefaultTokenProviders();
        }

        private void _addJwtAuth(IServiceCollection services)
        {
            services.AddAuthentication(option => {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = _getTokenValidationParameters(options);
                options.Events = _getJwtBearerEvents(options);
            });
        }

        private TokenValidationParameters _getTokenValidationParameters(JwtBearerOptions options)
        {
            return new TokenValidationParameters()
            {
                LifetimeValidator = (before, expires, token, param) =>
                {
                    return expires > DateTime.UtcNow;
                },
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateActor = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SigningKey"]))
            };

        }

        private JwtBearerEvents _getJwtBearerEvents(JwtBearerOptions options)
        {
            return new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && _pathContainHubPath(path))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        }

        private bool _pathContainHubPath(PathString path )
        {
            return path.StartsWithSegments("/roomHub") || path.StartsWithSegments("/gameHub");
        }
    }
}
