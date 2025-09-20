using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace FlowForge.UI.Filters.ActionFilters
{
    public class SkipControllerAuthorizationFilter : IAuthorizationFilter
    {
        private readonly string _controllerName;

        public SkipControllerAuthorizationFilter(string controllerName)
        {
            _controllerName = controllerName;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var controllerName = context.RouteData.Values["controller"]?.ToString();
            
            // If this is the specified controller, skip authorization by setting the endpoint to null
            if (string.Equals(controllerName, _controllerName, StringComparison.OrdinalIgnoreCase))
            {
                // This effectively bypasses the authorization check for this controller
                context.HttpContext.SetEndpoint(null);
            }
        }
    }
}