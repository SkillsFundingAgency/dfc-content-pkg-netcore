using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.Content.Pkg.Netcore.Data.Contracts
{
    public interface IApiService
    {
        Task<string?> GetAsync(HttpClient? httpClient, Uri url, string acceptHeader);

        Task<string?> PostAsync(HttpClient? httpClient, Uri url, string acceptHeader, Dictionary<string, object> parameters);

        Task<HttpStatusCode> PostAsync(HttpClient? httpClient, Uri url);

        Task<HttpStatusCode> PostAsync<TApiModel>(HttpClient? httpClient, Uri url, TApiModel model)
            where TApiModel : class;

        Task<HttpStatusCode> DeleteAsync(HttpClient? httpClient, Uri url);
    }
}
