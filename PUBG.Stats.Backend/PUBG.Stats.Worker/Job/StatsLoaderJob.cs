using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PUBG.Stats.Core.Hosting;
using PUBG.Stats.Worker.Job.Configuration;
using PUBG.Stats.Worker.Services.Abstractions;
using Serilog;

namespace PUBG.Stats.Worker.Job
{
    public class StatsLoaderJob : IJob
    {
        private readonly JobConfiguration _jobConfiguration;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _iterationWaitCancellationTokenSource = new CancellationTokenSource();
        private bool _running = false;
        private readonly IETLService _etlService;

        public StatsLoaderJob(
            IOptions<JobConfiguration> options,
            IETLService etlService)
        {
            _etlService = etlService;
            _jobConfiguration = options.Value;
        }

        public void ForceStart()
        {
            if (!_running)
            {
                _iterationWaitCancellationTokenSource.Cancel();
                _iterationWaitCancellationTokenSource = new CancellationTokenSource();
            }
        }

        public void Start()
        {
            Log.Information($"Started job {_jobConfiguration.JobName}, the main loop wait time between iterations is set to {_jobConfiguration.RunEachMinutes} minutes.");

            CancellationToken cancellationToken = _cancellationTokenSource.Token;

            Task.Factory.StartNew(() => RunMainFlow(cancellationToken), cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        public void Stop()
        {
            var sw = new Stopwatch();
            sw.Start();

            _cancellationTokenSource.Cancel();
            _iterationWaitCancellationTokenSource.Cancel();
            while (_running && sw.Elapsed < TimeSpan.FromSeconds(_jobConfiguration.GracefulSecondsThreshold))
            {
                Log.Information("Elapsed while waiting {0}", sw.Elapsed);
                Log.Information("Waiting for job stop");
                Thread.Sleep(1000);
            }

            if (_running)
            {
                Log.Warning("Job stopped but graceful shutdown was not guaranteed");
            }

            Log.Information("Job stopped!");
        }

        private async Task RunMainFlow(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    Log.Information("Starting iteration...");
                    _running = true;
                    try
                    {
                        await _etlService.RunAsync(cancellationToken);
                    }
                    catch (Exception jobEx)
                    {
                        Log.Error("Error on job execution", jobEx);
                    }
                    _running = false;
                    await Task
                        .Delay(TimeSpan.FromMinutes(_jobConfiguration.RunEachMinutes),
                            _iterationWaitCancellationTokenSource.Token).ContinueWith(
                            (t) => { Log.Information("Restarting job."); }, cancellationToken);
                }

                _running = false;

                Log.Information("Job canceled");
            }
            catch (Exception ex)
            {
                Log.Error("Error job Start {ex}", ex);
                throw;
            }
        }
    }
}
