using MasterRad.DTO;
using MasterRad.DTO.RQ;
using MasterRad.DTO.RS;
using MasterRad.Models.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp_OpenIDConnect_DotNet.Services;
using Graph = Microsoft.Graph;
using MasterRad.Extensions;

namespace MasterRad.Services
{

    public interface IMsGraph
    {
        Task<SearchStudentsRS> SearchStudentsAsync(SearchStudentsRQ model);
        Task<SearchStudentsRS> ListStudentsByPageAsync(string pageUrl);
        Task<IEnumerable<StudentDTO>> GetStudentsByIds(List<Guid> studendIds);
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
            var filters = new List<string>();

            if (!string.IsNullOrEmpty(model.FirstNameStartsWith))
                filters.Add($"startswith(givenName,+'{model.FirstNameStartsWith}')");

            if (!string.IsNullOrEmpty(model.LastNameStartsWith))
                filters.Add($"startswith(surname,+'{model.LastNameStartsWith}')");

            if (!string.IsNullOrEmpty(model.EmailStartsWith))
                filters.Add($"startswith(mail,+'{model.EmailStartsWith}')");

            if (filters.Any())
                qry = qry.Filter(string.Join(" and ", filters));

            qry = qry.Top(model.PageSize);
            #endregion

            var qryRes = await qry.GetAsync();

            #region MapResult
            var nextPageUrl = qryRes.NextPageRequest
                                    ?.GetHttpRequestMessage()
                                    ?.RequestUri
                                    ?.AbsoluteUri
                                    ?.ToString();

            var students = qryRes.Select(x => new StudentDTO(x));
            #endregion

            return new SearchStudentsRS(students, nextPageUrl);
        }

        public async Task<SearchStudentsRS> ListStudentsByPageAsync(string pageUrl)
        {
            GraphServiceClient graphClient = GetGraphServiceClient(new[] { Constants.ScopeUserReadBasicAll });

            await graphClient.Users.Request().Top(1).GetAsync(); //gets the authenthication token (to do: find better way)

            var httpRqMethod_GET = new System.Net.Http.HttpMethod("GET");
            var nextPageHttpRq = new System.Net.Http.HttpRequestMessage(httpRqMethod_GET, pageUrl);
            var nextPageHttpRs = await graphClient.HttpProvider.SendAsync(nextPageHttpRq);
            var responseJSON = await nextPageHttpRs.Content.ReadAsStringAsync();

            #region MapJsonToResult
            var responseJObject = JObject.Parse(responseJSON);
            var nextPageURL = responseJObject["@odata.nextLink"]?.ToString();
            var currentPageJSON = responseJObject["value"]?.ToString() ?? string.Empty;
            var currentPageData = JsonConvert.DeserializeObject<IList<Graph.User>>(currentPageJSON);
            var studentDTOs = currentPageData?.Select(s => new StudentDTO(s));
            #endregion

            return new SearchStudentsRS(studentDTOs, nextPageURL);
        }

        public async Task<IEnumerable<StudentDTO>> GetStudentsByIds(List<Guid> studendIds)
        {
            if (studendIds == null || !studendIds.Where(id => id != Guid.Empty).Any())
                throw new ArgumentException();

            GraphServiceClient graphClient = GetGraphServiceClient(new[] { Constants.ScopeUserReadBasicAll });

            studendIds = studendIds.Distinct()
                                   .ToList();

            var studentIdChunks = studendIds.ToChunks(15);
            var requests = new List<IGraphServiceUsersCollectionRequest>();
            foreach (var chunkIds in studentIdChunks)
            {
                var qry = graphClient.Users.Request();
                var idEqExpressions = chunkIds.Select(id => $"(id eq '{id.ToString()}')");
                qry = qry.Filter(string.Join(" or ", idEqExpressions));
                requests.Add(qry);
            }

            var studentChunks = await Task.WhenAll(requests.Select(rq => rq.GetAsync()));
            return studentChunks.SelectMany(x => x)
                                .Select(x => new StudentDTO(x));
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
