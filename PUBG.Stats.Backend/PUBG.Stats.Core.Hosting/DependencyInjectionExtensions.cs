using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PUBG.Stats.Core.Hosting.Exceptions;

namespace PUBG.Stats.Core.Hosting
{
    public static class DependencyInjectionExtensions
    {
        private static bool _isJobAdded = false;
        private static IConfiguration _configurationInstance;

        public static void AddJob<T>(this IServiceCollection services)
            where T : class, IJob
        {
            if (_isJobAdded)
            {
                throw new ForbiddenHostInitializationException("A job has already been registered.");
            }

            if (typeof(T).IsInterface)
            {
                throw new ForbiddenHostInitializationException(
                    "You must specify an implementation instead of an interface");
            }

            services.AddSingleton<IJob, T>();
            _isJobAdded = true;
        }

        public static void AddConfiguration<T>(this IServiceCollection services, string section)
            where T : class
        {
            if (_configurationInstance == null)
            {
                CacheConfiguration(services);
            }

            services.Configure<T>(options => _configurationInstance.GetSection(section).Bind(options));
        }

        private static void CacheConfiguration(IServiceCollection services)
        {
            IConfiguration configuration = services.First(service => service.ServiceType == typeof(IConfiguration))
                .ImplementationInstance as IConfiguration;

            _configurationInstance = configuration;
        }
    }
}
