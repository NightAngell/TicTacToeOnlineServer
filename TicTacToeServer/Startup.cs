﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicTacToeServer.Database;
using TicTacToeServer.Hubs;
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

            services.AddCors(options => options.AddPolicy("CorsPolicy",
               builder =>
               {
                   builder.AllowAnyMethod().AllowAnyHeader()
                          //.AllowAnyOrigin()
                          .WithOrigins("http://localhost:4200")
                          .AllowCredentials();
               }));
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
    }
}
