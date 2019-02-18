namespace PUBG.Stats.Core.Hosting
{
    public interface IJob
    {
        /// <summary>
        /// Starts the job.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the job.
        /// </summary>
        void Stop();

        /// <summary>
        /// Stops the job.
        /// </summary>
        void ForceStart();
    }
}
