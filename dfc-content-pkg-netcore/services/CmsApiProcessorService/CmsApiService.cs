﻿using AutoMapper;
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

        public CmsApiService(
            CmsApiClientOptions cmsApiClientOptions,
            IApiDataProcessorService apiDataProcessorService,
            HttpClient httpClient,
            IMapper mapper)
        {
            this.cmsApiClientOptions = cmsApiClientOptions;
            this.apiDataProcessorService = apiDataProcessorService;
            this.httpClient = httpClient;
            this.mapper = mapper;
        }

        public async Task<IList<TApiModel>?> GetSummaryAsync<TApiModel>()
            where TApiModel : class, IApiDataModel
        {
            var url = new Uri(
                $"{cmsApiClientOptions.BaseAddress}{cmsApiClientOptions.SummaryEndpoint}",
                UriKind.Absolute);

            return await apiDataProcessorService.GetAsync<IList<TApiModel>>(httpClient, url)
                .ConfigureAwait(false);
        }

        public async Task<TModel?> GetItemAsync<TModel, TChild>(Uri url)
            where TModel : class, IBaseContentItemModel<TChild>
            where TChild : class, IBaseContentItemModel<TChild>
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
             where TChild : class, IBaseContentItemModel<TChild>
        {
            if (uri != null)
            {
                return await apiDataProcessorService.GetAsync<TChild>(httpClient, uri)
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

        private async Task GetSharedChildContentItems<TModel>(ContentLinksModel? model, IList<TModel> contentItem)
            where TModel : class, IBaseContentItemModel<TModel>
        {
            var linkDetails = model?.ContentLinks.SelectMany(contentLink => contentLink.Value);

            if (linkDetails != null && linkDetails.Any())
            {
                foreach (var linkDetail in linkDetails)
                {
                    if (linkDetail.Uri != null)
                    {
                        var pagesApiContentItemModel = await GetContentItemAsync<TModel>(linkDetail.Uri).ConfigureAwait(false);

                        if (pagesApiContentItemModel != null)
                        {
                            mapper.Map(linkDetail, pagesApiContentItemModel);

                            if (pagesApiContentItemModel.ContentLinks != null)
                            {
                                pagesApiContentItemModel.ContentLinks.ExcludePageLocation = true;

                                await GetSharedChildContentItems(pagesApiContentItemModel.ContentLinks, pagesApiContentItemModel.ContentItems).ConfigureAwait(false);
                            }

                            contentItem.Add(pagesApiContentItemModel);
                        }
                    }
                }
            }
        }
    }
}
