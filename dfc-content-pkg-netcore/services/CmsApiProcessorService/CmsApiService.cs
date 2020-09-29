using AutoMapper;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using System;
using System.Collections.Generic;
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
        private readonly AutoMapper.IMapper mapper;
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

        public async Task<TModel?> GetItemAsync<TModel>(Uri url)
           where TModel : class, IBaseContentItemModel
        {
            var apiDataModel = await apiDataProcessorService.GetAsync<TModel>(httpClient, url)
                .ConfigureAwait(false);

            if (apiDataModel != null)
            {
                await GetSharedChildContentItems(apiDataModel.ContentLinks, apiDataModel.ContentItems).ConfigureAwait(false);
            }

            return apiDataModel;
        }

        public async Task<TChild?> GetContentItemAsync<TChild>(Uri? uri)
             where TChild : class, IBaseContentItemModel
        {
            if (uri != null)
            {
                if (uri.ToString().Contains(@"//skill") || uri.ToString().Contains(@"//knowledge")) {
                    return null;
                }

                return await apiDataProcessorService.GetAsync<TChild>(httpClient, uri)
                    .ConfigureAwait(false);
            }

            return null;
        }

        public async Task<TChild?> GetContentItemAsync<TChild>(TChild type, Uri? uri)
             where TChild : class, IBaseContentItemModel
        {
            if (uri != null)
            {
                if (uri.ToString().Contains(@"//skill") || uri.ToString().Contains(@"//knowledge"))
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

        private async Task GetSharedChildContentItems(ContentLinksModel? model, IList<IBaseContentItemModel> contentItem)
        {
            var linkDetails = model?.ContentLinks.SelectMany(contentLink => contentLink.Value);

            if (linkDetails != null && linkDetails.Any())
            {
                if (!contentTypeMappingService.Mappings.Any())
                {
                    throw new InvalidOperationException($"No mappings have been added to {nameof(contentTypeMappingService)}. Please add mappings before calling {nameof(GetSharedChildContentItems)}");
                }

                foreach (var linkDetail in linkDetails)
                {
                    if (linkDetail.ContentType != null && linkDetail.ContentType.StartsWith("esco__"))
                    {
                        var newLink = linkDetail.Uri.ToString().Replace("esco__", "");
                        linkDetail.Uri = new Uri(newLink);
                    }

                    if (linkDetail.Uri != null)
                    {
                        var mappingToUse = contentTypeMappingService.Mappings[linkDetail.ContentType!];

                        var pagesApiContentItemModel = GetFromApiCache(mappingToUse, linkDetail.Uri) ?? AddToApiCache(await GetContentItemAsync(mappingToUse, linkDetail!.Uri!).ConfigureAwait(false));

                        if (pagesApiContentItemModel != null)
                        {
                            mapper.Map(linkDetail, pagesApiContentItemModel);

                            if (pagesApiContentItemModel.ContentLinks != null)
                            {
                                await GetSharedChildContentItems(pagesApiContentItemModel.ContentLinks, pagesApiContentItemModel.ContentItems).ConfigureAwait(false);
                            }

                            contentItem.Add(pagesApiContentItemModel!);
                        }
                    }
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

        private TModel? GetFromApiCache<TModel>(TModel type, Uri uri)
           where TModel : class, IBaseContentItemModel
        {
            return apiCacheService.Retrieve(type, uri);
        }
    }
}
