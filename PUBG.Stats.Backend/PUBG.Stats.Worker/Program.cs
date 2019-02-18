using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;

namespace PUBG.Stats.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo
                .Console(LogEventLevel.Debug)
                .CreateLogger();

            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>().Build().Run();
        }
    }
}
