using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using PUBG.Stats.Worker.RateManager;
using PUBG.Stats.Worker.Services;
using Xunit;

namespace PUBG.Stats.Tests
{
    public class CallGovernorTests
    {
        private static readonly string RemainingCallHeader = "X-Ratelimit-Remaining";
        private static readonly string ResetLimitHeader = "X-Ratelimit-Reset";

        public CallGovernorTests()
        {
            CallGovernor.Init();
        }

        [Fact]
        public async Task Call_WhenTooManyRequests_ThenWait()
        {
            Stopwatch sw = new Stopwatch();

            //Arrange
            Mock<HttpMessageHandler> handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.TooManyRequests,
                    Headers =
                    {
                        {RemainingCallHeader, new[] {"0"}},
                        {ResetLimitHeader, new[] {GetUnixFromDateTime(DateTime.UtcNow.AddSeconds(10)).ToString()}}
                    }
                }).ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(string.Empty)
                });

            var httpClient = new HttpClient(handlerMock.Object);

            Func<Task<HttpResponseMessage>> mockCall = async () =>
            {
                HttpResponseMessage apiResponse = await httpClient.GetAsync("https://fluendo.com/en/");
                return apiResponse;
            };

            //Act
            sw.Start();
            HttpResponseMessage testResponse = await CallGovernor.Call(mockCall, CancellationToken.None);
            sw.Stop();

            //Assert
            sw.Elapsed.Seconds.Should().BeGreaterThan(5).And.BeLessThan(15);
            testResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        private int GetUnixFromDateTime(DateTime dt)
        {
            int unixTimestamp = (int)dt.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            return unixTimestamp;
        }
    }
}
