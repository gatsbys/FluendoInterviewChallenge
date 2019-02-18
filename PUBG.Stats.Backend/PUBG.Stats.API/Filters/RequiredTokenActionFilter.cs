using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using PUBG.Stats.API.Configuration;

namespace PUBG.Stats.API.Filters
{
    public class RequiredTokenActionFilter : IActionFilter
    {
        private readonly SecurityConfiguration _securityConfiguration;

        public RequiredTokenActionFilter(IOptions<SecurityConfiguration> options)
        {
            _securityConfiguration = options.Value;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            ControllerActionDescriptor controllerDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

            if (controllerDescriptor?.MethodInfo.GetCustomAttribute<RequiredTokenAttribute>() == null)
            {
                return;
            }

            if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues apiKeyValues))
            {
                string receivedApiKey = apiKeyValues.FirstOrDefault();
                if (string.IsNullOrEmpty(receivedApiKey) || apiKeyValues != _securityConfiguration.Token)
                {
                    context.Result = new UnauthorizedResult();
                }
                return;
            }
            context.Result = new UnauthorizedResult();
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
