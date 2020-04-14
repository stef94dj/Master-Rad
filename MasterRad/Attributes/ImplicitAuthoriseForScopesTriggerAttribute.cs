using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace MasterRad.Attributes
{
    /// <summary>
    /// Filter used on AJAX endpoint to inform client-side that page reload is required.
    /// Code on client-side should trigger a reload/resubmit to the page's MVC controller action.
    /// AuthorizeForScopes attribute on the MVC controller action will then handle incremental consent.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ImplicitAuthoriseForScopesTriggerAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Scopes to request
        /// </summary>
        public string[] Scopes { get; set; }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (Scopes == null && Scopes.Length <= 0)
            {
                throw new InvalidOperationException("Provide at least one scope to request access for.");
            }

            var svc = context.HttpContext.RequestServices;
            var _tokenAcquisition = svc.GetService(typeof(ITokenAcquisition)) as ITokenAcquisition;

            await _tokenAcquisition.GetAccessTokenForUserAsync(Scopes);

            await next();
        }
    }
}
