using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace MasterRad.Attributes
{
    /// <summary>
    /// Filter used on AJAX endpoint to inform client that page reload is required. 
    /// AuthorizeForScopes attribute on the MVC controller action will then trigger incremental consent.
    /// </summary>
    public class AjaxMsGraphProxyAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            MsalUiRequiredException msalUiRequiredException = context.Exception as MsalUiRequiredException;

            if (msalUiRequiredException == null)
                msalUiRequiredException = context.Exception?.InnerException as MsalUiRequiredException;

            if (msalUiRequiredException != null && AuthorizeForScopesAttribute.CanBeSolvedByReSignInOfUser(msalUiRequiredException))
            {
                context.Result = new JsonResult("ErrorMessage: A page reload is required in order to refresh (or gain additional) consent for Ms Graph on behalf of the user.");
                context.HttpContext.Response.StatusCode = (int)HttpCodes.ReloadRequired;
            }

            base.OnException(context);
        }
    }
}
