using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.Content.Pkg.Netcore.Data.Contracts
{
    public interface IApiDataProcessorService
    {
        Task<TApiModel?> GetAsync<TApiModel>(HttpClient? httpClient, Uri url)
            where TApiModel : class;

        Task<TApiModel?> GetAsync<TApiModel>(Type type, HttpClient? httpClient, Uri url)
            where TApiModel : class;

        Task<TApiModel?> PostAsync<TApiModel>(HttpClient? httpClient, Uri url, Dictionary<string, object> parameters)
            where TApiModel : class;

        Task<HttpStatusCode> PostAsync(HttpClient? httpClient, Uri url);

        Task<HttpStatusCode> PostAsync<TModel>(HttpClient? httpClient, Uri url, TModel model)
            where TModel : class;

        Task<HttpStatusCode> DeleteAsync(HttpClient? httpClient, Uri url);
    }
}
