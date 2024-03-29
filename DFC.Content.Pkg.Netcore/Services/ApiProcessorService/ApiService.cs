﻿using DFC.Content.Pkg.Netcore.Data.Contracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace DFC.Content.Pkg.Netcore.Services.ApiProcessorService
{
    public class ApiService : IApiService
    {
        private readonly ILogger<ApiService> logger;

        public ApiService(ILogger<ApiService> logger)
        {
            this.logger = logger;
        }

        public async Task<string?> GetAsync(HttpClient? httpClient, Uri url, string acceptHeader)
        {
            _ = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            logger.LogInformation("Loading data from {Url}", url);

            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(acceptHeader));

            try
            {
                var response = await httpClient.SendAsync(request).ConfigureAwait(false);
                string? responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError($"Failed to get {acceptHeader} data from {url}, received error : '{responseString}', returning empty content.");
                    responseString = null;
                }
                else if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    logger.LogInformation($"Status - {response.StatusCode} with response '{responseString}' received from {url}, returning empty content.");
                    responseString = null;
                }

                return responseString;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error received getting {acceptHeader} data '{ex.InnerException?.Message}'. Received from {url}, returning empty content");
            }

            return default;
        }

        public async Task<string?> PostAsync(HttpClient? httpClient, Uri url, string acceptHeader, Dictionary<string, object> parameters)
        {
            _ = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            logger.LogInformation("Loading data from {Url}", url);

            using var request = new HttpRequestMessage(HttpMethod.Post, url);

            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(acceptHeader));

            request.Content = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.SendAsync(request).ConfigureAwait(false);
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                switch (response.IsSuccessStatusCode)
                {
                    case true:
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            return responseString;
                        }

                        const string infoMsg =
                            "Status - {StatusCode} with response '{responseString}' received from {url}, returning empty content.";

                        logger.LogInformation(infoMsg, response.StatusCode, responseString, url);
                        throw new Exception("Response returned something other than okay");
                    case false:
                    {
                        const string errorMsg =
                            "Failed to get {acceptHeader} data from {url}, received error : '{responseString}', returning empty content.";

                        logger.LogError(errorMsg, acceptHeader, url, responseString);
                        throw new Exception("Response returned an error code.");
                    }
                }
            }
            catch (Exception ex)
            {
                const string errorMsg =
                    "Error received getting {acceptHeader} data '{Message}'. Received from {url}, returning empty content";

                logger.LogError(ex, errorMsg, acceptHeader, ex.InnerException?.Message, url);
                throw;
            }
        }

        public async Task<HttpStatusCode> PostAsync(HttpClient? httpClient, Uri url)
        {
            _ = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            logger.LogInformation($"Posting to {url}");

            HttpResponseMessage? response = null;
            try
            {
                using var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = url,
                };

                response = await httpClient.SendAsync(request).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content != null ? await response.Content.ReadAsStringAsync().ConfigureAwait(false) : string.Empty;
                    logger.LogError($"Failure status code '{response.StatusCode}' received with content '{responseContent}', for POST: {url}");
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error received posting '{ex.InnerException?.Message}'. Received from {url}");
            }

            return response?.StatusCode ?? HttpStatusCode.BadRequest;
        }

        public async Task<HttpStatusCode> PostAsync<TModel>(HttpClient? httpClient, Uri url, TModel model)
            where TModel : class
        {
            _ = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            logger.LogInformation($"Posting data to {url}");

            HttpResponseMessage? response = null;
            try
            {
                using var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = url,
                    Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, MediaTypeNames.Application.Json),
                };

                response = await httpClient.SendAsync(request).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content != null ? await response.Content.ReadAsStringAsync().ConfigureAwait(false) : string.Empty;
                    logger.LogError($"Failure status code '{response.StatusCode}' received with content '{responseContent}', for POST: {url}");
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error received posting data '{ex.InnerException?.Message}'. Received from {url}");
            }

            return response?.StatusCode ?? HttpStatusCode.BadRequest;
        }

        public async Task<HttpStatusCode> DeleteAsync(HttpClient? httpClient, Uri url)
        {
            _ = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            logger.LogInformation($"Deleting data from {url}");

            HttpResponseMessage? response = null;
            try
            {
                using var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = url,
                };

                response = await httpClient.SendAsync(request).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content != null ? await response.Content.ReadAsStringAsync().ConfigureAwait(false) : string.Empty;
                    logger.LogError($"Failure status code '{response.StatusCode}' received with content '{responseContent}', for DELETE: {url}");
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error received deleting data '{ex.InnerException?.Message}'. Received from {url}");
            }

            return response?.StatusCode ?? HttpStatusCode.BadRequest;
        }
    }
}
