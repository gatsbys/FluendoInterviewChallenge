using System;

namespace PUBG.Stats.Worker.RateManager.Entities
{
    public class ApiRateLimitStatus
    {
        public ApiRateLimitStatus()
        {
            
        }

        public ApiRateLimitStatus(int remainingCalls, double unixResetsAt)
        {
            RemainingCalls = remainingCalls;
            ResetsAt = UnixTimeStampToDateTime(unixResetsAt);
        }

        public ApiRateLimitStatus(int remainingCalls)
        {
            RemainingCalls = remainingCalls;
            ResetsAt = DateTime.UtcNow;
        }

        public int RemainingCalls { get; set; } = -1;

        public DateTime ResetsAt { get; set; } = DateTime.MaxValue;

        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}
