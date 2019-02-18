using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PUBG.Stats.Worker.RateManager.Entities;

namespace PUBG.Stats.Worker.RateManager
{
    public static class CallGovernor
    {
        private static SemaphoreSlim _semaphore;
        private static ApiRateLimitStatus _apiRateLimitStatus;
        private static int _tokenEachSeconds = 6;
        private static DateTime _lastCall;

        public static void Init()
        {
            if (_apiRateLimitStatus == null)
            {
                _apiRateLimitStatus = new ApiRateLimitStatus(10);
                _semaphore = new SemaphoreSlim(1);
                _lastCall = DateTime.UtcNow.AddSeconds(-6);
            }
        }

        public static async Task<HttpResponseMessage> Call(Func<Task<HttpResponseMessage>> apiCall,
            CancellationToken cancellationToken)
        {
            try
            {
                await _semaphore.WaitAsync(cancellationToken);

                double elapsedSecondsSinceLastCall = (DateTime.UtcNow - _lastCall).Seconds;
                if (elapsedSecondsSinceLastCall <= 6)
                {
                    double secondsToNewToken = _tokenEachSeconds - elapsedSecondsSinceLastCall;
                    await Task.Delay(TimeSpan.FromSeconds(secondsToNewToken), cancellationToken);
                }

                HttpResponseMessage response = await apiCall();
                UpdateLastCall();
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    await Task.Delay(TimeSpan.FromSeconds(_tokenEachSeconds), cancellationToken);
                    response = await apiCall();
                    UpdateLastCall();
                }

                AssertResponse(response);

                return response;
            }
            finally
            {
                _semaphore.Release(1);
            }
        }

        private static void AssertResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                throw new Exception("Waited two renew token cycles and the PUBG still in 429");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Status code from PUBG is not a valid one {response.StatusCode}");
            }
        }

        private static void UpdateLastCall()
        {
            _lastCall = DateTime.UtcNow;
        }
    }
}
