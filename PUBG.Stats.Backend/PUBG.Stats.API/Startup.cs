using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PUBG.Stats.API.Configuration;
using PUBG.Stats.API.Filters;
using PUBG.Stats.API.Services;
using PUBG.Stats.API.Services.Abstractions;
using PUBG.Stats.Core.Hosting;
using PUBG.Stats.Core.Services.Data;
using PUBG.Stats.Core.Services.Data.Abstractions;
using PUBG.Stats.Core.Services.Data.Configuration;
using Swashbuckle.AspNetCore.Swagger;

namespace PUBG.Stats.API
{
    public class Startup
    {
        public Startup()
        {
            Configuration = BuildConfiguration();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => { options.Filters.Add<RequiredTokenActionFilter>(); });
            services.AddOptions();

            services.AddConfiguration<MongoDbConfiguration>("DataConfiguration");
            services.AddConfiguration<CacheConfiguration>("CacheConfiguration");
            services.AddConfiguration<InternalComConfiguration>("InternalComConfiguration");
            services.AddConfiguration<SecurityConfiguration>("SecurityConfiguration");
            services.AddConfiguration<RedisCacheOptions>("RedisCacheConfiguration");


            services.AddSingleton<IDistributedCache, RedisCache>();
            services.AddTransient<IReadOnlyMongoDbService, ReadOnlyMongoDbService>();
            services.AddTransient<ILeaderboardService, LeaderboardService>();
            services.AddTransient<IPlayerLifetimeStatsService, PlayerLifetimeStatsService>();
            services.AddTransient<IPlayerLifetimeStatsService, PlayerLifetimeStatsService>();

            services.AddTransient<IFallbackPlayerService, FallbackPlayerService>();
            services.AddTransient<IReloadStatsService, ReloadStatsService>();

            services.AddTransient<RequiredTokenActionFilter>();

            services.AddSwaggerGen(options =>
            {
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "PUBG.Stats.API.xml"));
                options.SwaggerDoc("v1", new Info() { Title = "PUBG Fluendo Stats API", Version = "v1" });
                options.AddSecurityDefinition("Token", new ApiKeyScheme
                {
                    In = "header",
                    Description = "Please enter Api Token",
                    Name = "Authorization"
                });
                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Token", Enumerable.Empty<string>() },
                });

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PUBG Fluendo Stats API V1");
            });
            app.UseMvc();
            app.UseAuthentication();
        }

        private IConfiguration BuildConfiguration()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json");

            return builder.Build();
        }
    }
}
