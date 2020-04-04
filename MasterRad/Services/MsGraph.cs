using MasterRad.Models;
using MasterRad.Models.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp_OpenIDConnect_DotNet.Services;
using Graph = Microsoft.Graph;

namespace MasterRad.Services
{

    public interface IMsGraph
    {
        Task<SearchStudentsRS> SearchStudentsAsync(SearchStudentsRQ model);
        Task<SearchStudentsRS> ListStudentsByPageAsync(string pageUrl);
    }

    public class MsGraph : IMsGraph
    {

        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly WebOptions _webOptions;

        public MsGraph
        (
             ITokenAcquisition tokenAcquisition,
             IOptions<WebOptions> webOptions
        )
        {
            _tokenAcquisition = tokenAcquisition;
            _webOptions = webOptions.Value;
        }

        public async Task<SearchStudentsRS> SearchStudentsAsync(SearchStudentsRQ model)
        {
            if (model == null || model.PageSize <= 0)
                throw new ArgumentException();

            GraphServiceClient graphClient = GetGraphServiceClient(new[] { Constants.ScopeUserReadBasicAll });
            var qry = graphClient.Users.Request();

            #region SetUserFilters
            if (!string.IsNullOrEmpty(model.FirstNameFilter))
                qry = qry.Filter($"startswith(givenName,+'{model.FirstNameFilter}')");

            if (!string.IsNullOrEmpty(model.LastNameFilter))
                throw new NotImplementedException();

            if (!string.IsNullOrEmpty(model.EmailFilter))
                throw new NotImplementedException();

            qry = qry.Top(model.PageSize);
            #endregion

            var qryRes = await qry.GetAsync();

            #region MapResult
            var nextPageUrl = qryRes.NextPageRequest
                                   .GetHttpRequestMessage()
                                   .RequestUri
                                   .AbsoluteUri
                                   .ToString();

            var students = qryRes.Select(x => new Student(x.Id, x.GivenName, x.Surname, x.Mail, x.UserPrincipalName));
            #endregion

            return new SearchStudentsRS(students, nextPageUrl);
        }

        public async Task<SearchStudentsRS> ListStudentsByPageAsync(string pageUrl)
        {
            GraphServiceClient graphClient = GetGraphServiceClient(new[] { Constants.ScopeUserReadBasicAll });

            var httpRqMethod_GET = new System.Net.Http.HttpMethod("GET");
            var nextPageHttpRq = new System.Net.Http.HttpRequestMessage(httpRqMethod_GET, pageUrl);
            var nextPageHttpRs = await graphClient.HttpProvider.SendAsync(nextPageHttpRq);
            var responseJSON = await nextPageHttpRs.Content.ReadAsStringAsync();

            throw new NotImplementedException();
        }

        private GraphServiceClient GetGraphServiceClient(string[] scopes)
        {
            return GraphServiceClientFactory.GetAuthenticatedGraphClient(async () =>
            {
                string result = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
                return result;
            }, _webOptions.GraphApiUrl);
        }
    }
}
