using AutoMapper;
using dfc_content_pkg_netcore.contracts;
using dfc_content_pkg_netcore.models;
using dfc_content_pkg_netcore.models.clientOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace dfc_content_pkg_netcore.CmsApiProcessorService
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

        public async Task<IList<T>> GetSummaryAsync<T>() where T : class
        {
            var url = new Uri(
                $"{cmsApiClientOptions.BaseAddress}{cmsApiClientOptions.SummaryEndpoint}",
                UriKind.Absolute);

            return await apiDataProcessorService.GetAsync<IList<T>>(httpClient, url)
                .ConfigureAwait(false);
        }

        public async Task<T> GetItemAsync<T>(Uri url) where T : class, ICmsApiDataModel
        {
            var apiDataModel = await apiDataProcessorService.GetAsync<T>(httpClient, url)
                .ConfigureAwait(false);

            await GetSharedChildContentItems(apiDataModel.ContentLinks, apiDataModel.ContentItems).ConfigureAwait(false);

            return apiDataModel;
        }

        public async Task<BaseContentItemModel> GetContentItemAsync(LinkDetails details)
        {
            return await apiDataProcessorService.GetAsync<BaseContentItemModel>(httpClient, details.Uri)
                .ConfigureAwait(false);
        }

        public async Task<BaseContentItemModel> GetContentItemAsync(Uri uri)
        {
            return await apiDataProcessorService.GetAsync<BaseContentItemModel>(httpClient, uri)
                .ConfigureAwait(false);
        }

        public async Task<List<T>> GetContentAsync<T>() where T : class
        {
            var contentList = new List<T>();

            var ids = cmsApiClientOptions.ContentIds.Split(",").ToList();

            foreach (var id in ids)
            {
                var url = new Uri(
                    $"{cmsApiClientOptions.BaseAddress}{cmsApiClientOptions.StaticContentEndpoint}{id}",
                    UriKind.Absolute);

                var content = await apiDataProcessorService.GetAsync<T>(httpClient, url).ConfigureAwait(false);

                if (content != null)
                {
                    contentList.Add(content);
                }
            }

            return contentList;
        }

        private async Task GetSharedChildContentItems(ContentLinksModel model, IList<BaseContentItemModel> contentItem)
        {
            if (model != null && model.ContentLinks.Any())
            {
                foreach (var linkDetail in model.ContentLinks.SelectMany(contentLink => contentLink.Value))
                {
                    if (linkDetail.ContentType.StartsWith("esco__"))
                    {
                        var newLink = linkDetail.Uri.ToString().Replace("esco__", "");
                        linkDetail.Uri = new Uri(newLink);
                    }

                    var pagesApiContentItemModel = await GetContentItemAsync(linkDetail).ConfigureAwait(false);

                    if (pagesApiContentItemModel != null)
                    {
                        mapper.Map(linkDetail, pagesApiContentItemModel);
                        await GetSharedChildContentItems(pagesApiContentItemModel.ContentLinks, pagesApiContentItemModel.ContentItems).ConfigureAwait(false);
                        contentItem.Add(pagesApiContentItemModel);
                    }
                }
            }
        }
    }
}
