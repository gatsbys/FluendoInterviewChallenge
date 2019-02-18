namespace PUBG.Stats.Worker.RateManager.Entities
{
    public class ApiResponse<T>
    {
        public ApiResponse()
        {
            
        }

        public ApiResponse(T response, ApiRateLimitStatus apiRateLimitStatus)
        {
            Response = response;
            ApiRateLimitStatus = apiRateLimitStatus;
        }

        public T Response { get; set; }

        public ApiRateLimitStatus ApiRateLimitStatus { get; set; }
    }
}
