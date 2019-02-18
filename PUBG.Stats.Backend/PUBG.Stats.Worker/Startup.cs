using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PUBG.Stats.Core.Hosting;
using PUBG.Stats.Core.Services.Data;
using PUBG.Stats.Core.Services.Data.Abstractions;
using PUBG.Stats.Core.Services.Data.Configuration;
using PUBG.Stats.Worker.Configuration;
using PUBG.Stats.Worker.Job;
using PUBG.Stats.Worker.Job.Configuration;
using PUBG.Stats.Worker.RateManager;
using PUBG.Stats.Worker.Services;
using PUBG.Stats.Worker.Services.Abstractions;

namespace PUBG.Stats.Worker
{
    public class Startup
    {
        public Startup()
        {
            Configuration = BuildConfiguration();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddOptions();
            services.AddJob<StatsLoaderJob>();

            services.AddConfiguration<JobConfiguration>("JobConfiguration");
            services.AddConfiguration<WorkerConfiguration>("WorkerConfiguration");
            services.AddConfiguration<MongoDbConfiguration>("DataConfiguration");

            services.AddSingleton<IETLService, ETLService>();
            services.AddSingleton<IPUBGApiCaller, PUBGApiCaller>();
            services.AddSingleton<IReadOnlyMongoDbService, ReadOnlyMongoDbService>();
            services.AddSingleton<IWriteOnlyMongoDbService, WriteOnlyMongoDbService>();
            services.AddSingleton<IDocumentExtractorService, DocumentExtractorService>();

            services.AddSingleton((collection) =>
            {
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                    collection.GetService<IOptions<WorkerConfiguration>>().Value.PUBGApiKey);
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/vnd.api+json"));

                return httpClient;
            });

            CallGovernor.Init();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseJob();
            app.UseMvc();
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
