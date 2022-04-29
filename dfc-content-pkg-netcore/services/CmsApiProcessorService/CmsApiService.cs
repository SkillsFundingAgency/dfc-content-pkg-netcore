using AutoMapper;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DFC.Content.Pkg.Netcore.Services.CmsApiProcessorService
{
    public class CmsApiService : ICmsApiService
    {
        private readonly CmsApiClientOptions cmsApiClientOptions;
        private readonly IApiDataProcessorService apiDataProcessorService;
        private readonly HttpClient httpClient;
        private readonly IMapper mapper;
        private readonly IApiCacheService apiCacheService;
        private readonly IContentTypeMappingService contentTypeMappingService;

        public CmsApiService(
            CmsApiClientOptions cmsApiClientOptions,
            IApiDataProcessorService apiDataProcessorService,
            HttpClient httpClient,
            IMapper mapper,
            IApiCacheService apiCacheService,
            IContentTypeMappingService contentTypeMappingService)
        {
            this.cmsApiClientOptions = cmsApiClientOptions;
            this.apiDataProcessorService = apiDataProcessorService;
            this.httpClient = httpClient;
            this.mapper = mapper;
            this.apiCacheService = apiCacheService;
            this.contentTypeMappingService = contentTypeMappingService;
        }

        public async Task<IList<TApiModel>?> GetSummaryAsync<TApiModel>(string contentType)
          where TApiModel : class, IApiDataModel
        {
            var url = new Uri(
                     $"{cmsApiClientOptions.BaseAddress}{cmsApiClientOptions.SummaryEndpoint}{contentType}",
                     UriKind.Absolute);

            return await GetSummaryAsync<TApiModel>(url, true).ConfigureAwait(false);
        }

        public async Task<IList<TApiModel>?> GetSummaryAsync<TApiModel>(string contentType, bool clearCache)
            where TApiModel : class, IApiDataModel
        {
            var url = new Uri(
                $"{cmsApiClientOptions.BaseAddress}{cmsApiClientOptions.SummaryEndpoint}{contentType}",
                UriKind.Absolute);

            return await GetSummaryAsync<TApiModel>(url, clearCache).ConfigureAwait(false);
        }

        public async Task<IList<TApiModel>?> GetSummaryAsync<TApiModel>()
            where TApiModel : class, IApiDataModel
        {
            var url = new Uri(
                  $"{cmsApiClientOptions.BaseAddress}{cmsApiClientOptions.SummaryEndpoint}",
                  UriKind.Absolute);

            return await GetSummaryAsync<TApiModel>(url, true).ConfigureAwait(false);
        }

        public async Task<TModel?> GetItemAsync<TModel>(string contentType, Guid id)
           where TModel : class, IApiDataModel
        {
            var uri = new Uri($"{cmsApiClientOptions.BaseAddress}/{contentType}/{id}", UriKind.Absolute);

            return await apiDataProcessorService.GetAsync<TModel>(httpClient, uri).ConfigureAwait(false);
        }

        public Task<TModel?> GetItemAsync<TModel>(Uri url)
            where TModel : class, IBaseContentItemModel
        {
            return GetItemAsync<TModel>(url, new CmsApiOptions { PreventRecursion = false });
        }

        public async Task<TModel?> GetItemAsync<TModel>(Uri url, CmsApiOptions options)
           where TModel : class, IBaseContentItemModel
        {
            var useExpandFunction = !IsSummaryRequest(url.ToString());

            if (useExpandFunction)
            {
                var (contentType, id) = GetContentTypeAndId(url!.ToString());
                var multiDirectional = url.ToString().EndsWith("/true", StringComparison.InvariantCultureIgnoreCase);

                var hostWithPort = url.IsDefaultPort ? url.Host : $"{url.Host}:{url.Port}";
                var uri = new Uri($"{url.Scheme}://{hostWithPort}/api/expand/{contentType}/{id}");

                const int maxDepth = 5;

                var typesToInclude = contentTypeMappingService.Mappings
                    .Select(mapping => mapping.Key.ToLowerInvariant())
                    .ToArray();

                var bodyParameters = new Dictionary<string, object>
                {
                    { "MaxDepth", maxDepth },
                    { "MultiDirectional", multiDirectional },
                    { "TypesToInclude", typesToInclude },
                };

                var responseJObject = await apiDataProcessorService.PostAsync<JObject>(httpClient, uri, bodyParameters)
                    .ConfigureAwait(false);

                if (responseJObject == null)
                {
                    throw new NullReferenceException();
                }

                return (TModel?)ContentItemWithMappedChildren(typeof(TModel), responseJObject);
            }

            var apiDataModel = await apiDataProcessorService.GetAsync<TModel>(httpClient, url)
                .ConfigureAwait(false);

            const int level = 0;
            var retrievedContentTypes = new Dictionary<int, List<string>> { { level, new List<string> { GetContentType(url.AbsolutePath), } }, };

            if (apiDataModel != null)
            {
                await GetSharedChildContentItems(
                    apiDataModel.ContentLinks,
                    apiDataModel.ContentItems,
                    retrievedContentTypes,
                    options,
                    level).ConfigureAwait(false);
            }

            return apiDataModel;
        }

        private IBaseContentItemModel ContentItemWithMappedChildren(Type parentMappedType, JToken parentContentItemToken)
        {
            // Clear out the content items, after taking a cut of them, as we need to map them separately.
            var contentItems = parentContentItemToken["ContentItems"];
            parentContentItemToken["ContentItems"] = null;

            var children = new List<IBaseContentItemModel>();

            if (contentItems != null)
            {
                foreach (var contentItemToken in contentItems)
                {
                    var contentType = (contentItemToken["ContentType"] as JValue) !.Value as string;
                    var mappedType = contentTypeMappingService.GetMapping(contentType!) !;

                    var contentItem = ContentItemWithMappedChildren(mappedType, contentItemToken);
                    children.Add(contentItem);
                }
            }

            try
            {
                var parentContentItem = (IBaseContentItemModel)parentContentItemToken.ToObject(parentMappedType) !;
                parentContentItem.ContentItems = children;

                return parentContentItem;
            }
            catch
            {
                return default;
            }
        }

        private static string GetContentType(string path)
        {
            var pathParts = path.Split('/');
            return pathParts.Length > 3 ? pathParts[3] : null;
        }

        public async Task<TChild?> GetContentItemAsync<TChild>(Uri? uri)
             where TChild : class, IBaseContentItemModel
        {
            if (uri == null)
            {
                return null;
            }

            return await apiDataProcessorService.GetAsync<TChild>(httpClient, uri)
                .ConfigureAwait(false);
        }

        public async Task<TChild?> GetContentItemAsync<TChild>(Type type, Uri? uri)
             where TChild : class, IBaseContentItemModel
        {
            if (uri == null)
            {
                return null;
            }

            return await apiDataProcessorService.GetAsync<TChild>(type, httpClient, uri)
                .ConfigureAwait(false);
        }

        public async Task<List<TApiModel>?> GetContentAsync<TApiModel>()
            where TApiModel : class
        {
            var contentList = new List<TApiModel>();

            var ids = cmsApiClientOptions.ContentIds != null ? cmsApiClientOptions.ContentIds.Split(",").ToList() : new List<string>();

            foreach (var id in ids)
            {
                var url = new Uri(
                    $"{cmsApiClientOptions.BaseAddress}{cmsApiClientOptions.StaticContentEndpoint}{id}",
                    UriKind.Absolute);

                var content = await apiDataProcessorService.GetAsync<TApiModel>(httpClient, url).ConfigureAwait(false);

                if (content != null)
                {
                    contentList.Add(content);
                }
            }

            return contentList;
        }

        private static bool AncestorsContainsType(string absolutePath, Dictionary<int, List<string>> retrievedContentTypes, int level)
        {
            var contentType = GetContentType(absolutePath);

            for (var currentLevel = level - 1; currentLevel >= 0; currentLevel--)
            {
                if (!retrievedContentTypes.ContainsKey(currentLevel))
                {
                    continue;
                }

                var levelsList = retrievedContentTypes[currentLevel];

                if (levelsList.Contains(contentType))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsSummaryRequest(string? uri)
        {
            var (_, id) = GetContentTypeAndId(uri);
            return id == Guid.Empty;
        }

        private static (string ContentType, Guid Id) GetContentTypeAndId(string? uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return (string.Empty, Guid.Empty);
            }

            var pathOnly = uri.StartsWith("http", StringComparison.InvariantCultureIgnoreCase) ?
                new Uri(uri, UriKind.Absolute).AbsolutePath
                : uri;

            pathOnly = pathOnly
                .ToLowerInvariant()
                .Replace("/api/execute", string.Empty, StringComparison.InvariantCultureIgnoreCase);

            var uriParts = pathOnly.Trim('/').Split('/');
            var contentType = uriParts[0];

            var id = uriParts.Length >= 2 && Guid.TryParse(uriParts[1], out var guidId) ?
                guidId
                : Guid.Empty;

            return (contentType, id);
        }

        private async Task GetSharedChildContentItems(
            ContentLinksModel? model,
            IList<IBaseContentItemModel> contentItem,
            Dictionary<int, List<string>> retrievedPaths,
            CmsApiOptions options,
            int level)
        {
            var filteredLinkDetails = model?.ContentLinks?
                .Where(x => !contentTypeMappingService.IgnoreRelationship.Any(z => z == x.Key));

            var linkDetails = filteredLinkDetails?
                .SelectMany(contentLink => contentLink.Value)
                .Where(x => !options.PreventRecursion || !AncestorsContainsType(x.Uri.AbsolutePath, retrievedPaths, level));

            if (linkDetails != null && linkDetails.Any())
            {
                if (!contentTypeMappingService.Mappings.Any())
                {
                    throw new InvalidOperationException($"No mappings have been added to {nameof(contentTypeMappingService)}. Please add mappings before calling {nameof(GetSharedChildContentItems)}");
                }

                foreach (var linkDetail in linkDetails)
                {
                    if (linkDetail.ContentType != null && linkDetail.ContentType.StartsWith("esco__", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var newLink = linkDetail.Uri!.ToString().Replace("esco__", string.Empty, StringComparison.CurrentCultureIgnoreCase);
                        linkDetail.Uri = new Uri(newLink);
                    }

                    if (linkDetail.Uri != null)
                    {
                        await GetAndMapContentItem(contentItem, linkDetail, retrievedPaths, options, level + 1).ConfigureAwait(false);
                    }
                }
            }
        }

        private async Task GetAndMapContentItem(
            IList<IBaseContentItemModel> contentItem,
            ILinkDetails linkDetail,
            Dictionary<int, List<string>> retrievedPaths,
            CmsApiOptions options,
            int level)
        {
            var mappingToUse = contentTypeMappingService.GetMapping(linkDetail.ContentType!);

            var isOwnAncestor = AncestorsContainsType(linkDetail.Uri!.AbsolutePath, retrievedPaths, level);
            var passedRecursionCheck = !options.PreventRecursion || !isOwnAncestor;

            if (mappingToUse == null || !passedRecursionCheck)
            {
                return;
            }

            if (!retrievedPaths.ContainsKey(level))
            {
                retrievedPaths.Add(level, new List<string>());
            }

            var contentType = GetContentType(linkDetail.Uri!.AbsolutePath) ?? "Unknown";
            retrievedPaths[level].Add(contentType);

            var keyName = (options.ContentTypeOptions?.ContainsKey(contentType) == true ?
                options.ContentTypeOptions[contentType].KeyName : null) ?? "Uri";

            var key = keyName.ToUpperInvariant() switch
            {
                "TITLE" => linkDetail.Title!,
                _ => linkDetail.Uri.ToString()
            };

            if (options.ContentTypeOptions?.ContainsKey(contentType) == true &&
                options.ContentTypeOptions[contentType].Transform != null)
            {
                key = options.ContentTypeOptions[contentType].Transform!(key);
            }

            var pagesApiContentItemModel = GetFromApiCache<IBaseContentItemModel>(mappingToUse, key)
                ?? AddToApiCache(key, await GetContentItemAsync<IBaseContentItemModel>(mappingToUse!, linkDetail.Uri!).ConfigureAwait(false));

            if (pagesApiContentItemModel == null)
            {
                return;
            }

            mapper.Map(linkDetail, pagesApiContentItemModel);

            if (pagesApiContentItemModel.ContentLinks != null)
            {
                pagesApiContentItemModel.ContentLinks.ExcludePageLocation = true;

                await GetSharedChildContentItems(
                    pagesApiContentItemModel.ContentLinks,
                    pagesApiContentItemModel.ContentItems,
                    retrievedPaths,
                    options,
                    level).ConfigureAwait(false);
            }

            contentItem.Add(pagesApiContentItemModel!);
        }

        private async Task<IList<TApiModel>?> GetSummaryAsync<TApiModel>(Uri url, bool clearCache)
          where TApiModel : class, IApiDataModel
        {
            var result = await apiDataProcessorService.GetAsync<IList<TApiModel>>(httpClient, url)
              .ConfigureAwait(false);

            if (clearCache)
            {
                apiCacheService.Clear();
            }

            return result;
        }

        private TModel? AddToApiCache<TModel>(string key, TModel? model)
            where TModel : class, IBaseContentItemModel
        {
            if (model == null)
            {
                return null;
            }

            apiCacheService.AddOrUpdate(key, model);
            return model;
        }

        private TModel? GetFromApiCache<TModel>(Type type, string key)
           where TModel : class, IBaseContentItemModel
        {
            return apiCacheService.Retrieve<TModel>(type, key);
        }
    }
}