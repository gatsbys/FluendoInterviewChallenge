using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PUBG.Stats.Core.Hosting.Exceptions;

namespace PUBG.Stats.Core.Hosting
{
    public static class AppBuilderExtensions
    {
        private static bool _isJobUsed = false;

        /// <summary>
        /// Wires the starts and the end hooks to the start and the end IJob methods.
        /// </summary>
        /// <param name="app"></param>
        public static void UseJob(this IApplicationBuilder app)
        {
            if (_isJobUsed)
            {
                throw new ForbiddenHostInitializationException("A job has already been registered to use on the host as background worker.");
            }

            IJob job = app.ApplicationServices.GetRequiredService<IJob>();
            IApplicationLifetime lifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();

            lifetime.ApplicationStarted.Register(() => job.Start());
            lifetime.ApplicationStopping.Register(() => job.Stop());

            _isJobUsed = true;
        }
    }
}
