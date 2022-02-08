using AutoMapper;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

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

            return await GetSummaryAsync<TApiModel>(url).ConfigureAwait(false);
        }

        public async Task<IList<TApiModel>?> GetSummaryAsync<TApiModel>()
            where TApiModel : class, IApiDataModel
        {
            var url = new Uri(
                  $"{cmsApiClientOptions.BaseAddress}{cmsApiClientOptions.SummaryEndpoint}",
                  UriKind.Absolute);

            return await GetSummaryAsync<TApiModel>(url).ConfigureAwait(false);
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
            return GetItemAsync<TModel>(url, false);
        }

        public async Task<TModel?> GetItemAsync<TModel>(Uri url, bool preventRecursion)
           where TModel : class, IBaseContentItemModel
        {
            var apiDataModel = await apiDataProcessorService.GetAsync<TModel>(httpClient, url)
                .ConfigureAwait(false);

            const int level = 0;
            var retrievedPaths = new Dictionary<int, List<string>> { { level, new List<string> { url.AbsolutePath, } }, };

            if (apiDataModel != null)
            {
                await GetSharedChildContentItems(
                    apiDataModel.ContentLinks,
                    apiDataModel.ContentItems,
                    retrievedPaths,
                    preventRecursion,
                    level).ConfigureAwait(false);
            }

            return apiDataModel;
        }

        public async Task<TChild?> GetContentItemAsync<TChild>(Uri? uri)
             where TChild : class, IBaseContentItemModel
        {
            if (uri != null)
            {
                if (uri.ToString().Contains(@"//skill", StringComparison.CurrentCultureIgnoreCase) || uri.ToString().Contains(@"//knowledge", StringComparison.CurrentCultureIgnoreCase))
                {
                    return null;
                }

                return await apiDataProcessorService.GetAsync<TChild>(httpClient, uri)
                    .ConfigureAwait(false);
            }

            return null;
        }

        public async Task<TChild?> GetContentItemAsync<TChild>(Type type, Uri? uri)
             where TChild : class, IBaseContentItemModel
        {
            if (uri != null)
            {
                if (uri.ToString().Contains(@"//skill", StringComparison.OrdinalIgnoreCase) || uri.ToString().Contains(@"//knowledge", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                return await apiDataProcessorService.GetAsync<TChild>(type, httpClient, uri)
                    .ConfigureAwait(false);
            }

            return null;
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

        private bool IsAncestor(string absolutePath, Dictionary<int, List<string>> retrievedPaths, int level)
        {
            for (var currentLevel = level - 1; currentLevel >= 0; currentLevel--)
            {
                if (!retrievedPaths.ContainsKey(currentLevel))
                {
                    continue;
                }

                var levelsList = retrievedPaths[currentLevel];

                if (levelsList.Contains(absolutePath))
                {
                    return true;
                }
            }

            return false;
        }

        private async Task GetSharedChildContentItems(
            ContentLinksModel? model,
            IList<IBaseContentItemModel> contentItem,
            Dictionary<int, List<string>> retrievedPaths,
            bool preventRecursion,
            int level)
        {
            var filteredLinkDetails = model?.ContentLinks?
                .Where(x => !contentTypeMappingService.IgnoreRelationship.Any(z => z == x.Key));

            var linkDetails = filteredLinkDetails?
                .SelectMany(contentLink => contentLink.Value)
                .Where(x => !preventRecursion || !IsAncestor(x.Uri.AbsolutePath, retrievedPaths, level));

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
                        await GetAndMapContentItem(contentItem, linkDetail, retrievedPaths, preventRecursion, level + 1).ConfigureAwait(false);
                    }
                }
            }
        }

        private async Task GetAndMapContentItem(
            IList<IBaseContentItemModel> contentItem,
            ILinkDetails linkDetail,
            Dictionary<int, List<string>> retrievedPaths,
            bool preventRecursion,
            int level)
        {
            var mappingToUse = contentTypeMappingService.GetMapping(linkDetail.ContentType!);
            var path = linkDetail.Uri!.AbsolutePath;
            var passedRecursionCheck = !preventRecursion || !IsAncestor(path, retrievedPaths, level);

            if (mappingToUse != null && passedRecursionCheck)
            {
                if (!retrievedPaths.ContainsKey(level))
                {
                    retrievedPaths.Add(level, new List<string>());
                }

                retrievedPaths[level].Add(path);
                var pagesApiContentItemModel = GetFromApiCache<IBaseContentItemModel>(mappingToUse, linkDetail.Uri!) ?? AddToApiCache(await GetContentItemAsync<IBaseContentItemModel>(mappingToUse!, linkDetail!.Uri!).ConfigureAwait(false));

                if (pagesApiContentItemModel != null)
                {
                    mapper.Map(linkDetail, pagesApiContentItemModel);

                    if (pagesApiContentItemModel.ContentLinks != null)
                    {
                        pagesApiContentItemModel.ContentLinks.ExcludePageLocation = true;

                        await GetSharedChildContentItems(
                            pagesApiContentItemModel.ContentLinks,
                            pagesApiContentItemModel.ContentItems,
                            retrievedPaths,
                            preventRecursion,
                            level).ConfigureAwait(false);
                    }

                    contentItem.Add(pagesApiContentItemModel!);
                }
            }
        }

        private async Task<IList<TApiModel>?> GetSummaryAsync<TApiModel>(Uri url)
          where TApiModel : class, IApiDataModel
        {
            var result = await apiDataProcessorService.GetAsync<IList<TApiModel>>(httpClient, url)
              .ConfigureAwait(false);

            apiCacheService.Clear();

            return result;
        }

        private TModel? AddToApiCache<TModel>(TModel? model)
            where TModel : class, IBaseContentItemModel
        {
            if (model == null)
            {
                return null;
            }

            apiCacheService.AddOrUpdate(model!.Url!, model);
            return model;
        }

        private TModel? GetFromApiCache<TModel>(Type type, Uri uri)
           where TModel : class, IBaseContentItemModel
        {
            return apiCacheService.Retrieve<TModel>(type, uri);
        }
    }
}
