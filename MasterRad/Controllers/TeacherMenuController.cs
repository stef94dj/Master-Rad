using MasterRad.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace MasterRad.Controllers
{
    [Authorize(Roles = UserRole.Professor)]
    public class TeacherMenuController : BaseController
    {
        [AuthorizeForScopes(Scopes = new[] { Constants.ScopeUserReadBasicAll })]
        [ImplicitAuthoriseForScopesTrigger(Scopes = new[] { Constants.ScopeUserReadBasicAll })]
        public IActionResult Templates()
            => View();

        [AuthorizeForScopes(Scopes = new[] { Constants.ScopeUserReadBasicAll })]
        [ImplicitAuthoriseForScopesTrigger(Scopes = new[] { Constants.ScopeUserReadBasicAll })]
        public IActionResult Tasks()
            => View();

        [AuthorizeForScopes(Scopes = new[] { Constants.ScopeUserReadBasicAll })]
        [ImplicitAuthoriseForScopesTrigger(Scopes = new[] { Constants.ScopeUserReadBasicAll })]
        public IActionResult SynthesisTests()
            => View();

        [AuthorizeForScopes(Scopes = new[] { Constants.ScopeUserReadBasicAll })]
        [ImplicitAuthoriseForScopesTrigger(Scopes = new[] { Constants.ScopeUserReadBasicAll })]
        public IActionResult AnalysisTests()
            => View();
    }
}
