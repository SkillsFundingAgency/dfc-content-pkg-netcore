﻿using DFC.Content.Pkg.Netcore.Data.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;

namespace DFC.Content.Pkg.Netcore.Services.ApiProcessorService
{
    public class ApiDataProcessorService : IApiDataProcessorService
    {
        private readonly IApiService apiService;

        public ApiDataProcessorService(IApiService apiService)
        {
            this.apiService = apiService;
        }

        public async Task<TApiModel?> GetAsync<TApiModel>(HttpClient? httpClient, Uri url)
            where TApiModel : class
        {
            _ = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            var response = await apiService.GetAsync(httpClient, url, MediaTypeNames.Application.Json).ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(response))
            {
                return JsonConvert.DeserializeObject<TApiModel>(response);
            }

            return default;
        }

        public async Task<TApiModel?> GetAsync<TApiModel>(Type type, HttpClient? httpClient, Uri url)
            where TApiModel : class
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));
            _ = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            var response = await apiService.GetAsync(httpClient, url, MediaTypeNames.Application.Json).ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(response))
            {
                return (TApiModel?)JsonConvert.DeserializeObject(response, type);
            }

            return default;
        }

        public async Task<TApiModel?> PostAsync<TApiModel>(
            HttpClient? httpClient,
            Uri url,
            Dictionary<string, object> parameters)
                where TApiModel : class
        {
            _ = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            var response = await apiService.PostAsync(httpClient, url, MediaTypeNames.Application.Json, parameters).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(response))
            {
                throw new NullReferenceException();
            }

            return JsonConvert.DeserializeObject<TApiModel>(response);
        }

        public async Task<HttpStatusCode> PostAsync(HttpClient? httpClient, Uri url)
        {
            _ = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            return await apiService.PostAsync(httpClient, url).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> PostAsync<TModel>(HttpClient? httpClient, Uri url, TModel model)
            where TModel : class
        {
            _ = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            return await apiService.PostAsync(httpClient, url, model).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> DeleteAsync(HttpClient? httpClient, Uri url)
        {
            _ = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            return await apiService.DeleteAsync(httpClient, url).ConfigureAwait(false);
        }
    }
}
