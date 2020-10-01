using AutoMapper;
using DFC.Content.Pkg.Netcore.CmsApiProcessorService.UnitTests.Models;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using DFC.Content.Pkg.Netcore.Services;
using DFC.Content.Pkg.Netcore.Services.CmsApiProcessorService;
using FakeItEasy;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.Content.Pkg.Netcore.CmsApiProcessorService.UnitTests
{
    [Trait("Category", "CMS API Service Unit Tests")]
    public class CmsApiServiceTests
    {
        private readonly IApiDataProcessorService fakeApiDataProcessorService = A.Fake<IApiDataProcessorService>();
        private readonly IContentTypeMappingService fakeMappingService = A.Fake<IContentTypeMappingService>();
        private readonly HttpClient fakeHttpClient = A.Fake<HttpClient>();
        private readonly AutoMapper.Mapper mapper = A.Fake<Mapper>();

        private CmsApiClientOptions CmsApiClientOptions => new CmsApiClientOptions
        {
            BaseAddress = new Uri("https://localhost/", UriKind.Absolute),
            SummaryEndpoint = "api/something",
        };

        [Fact]
        public async Task CmsApiServiceGetSummaryReturnsNullFornNData()
        {
            // arrange
            IList<ApiSummaryModel>? nullExpectedResults = null;

            A.CallTo(() => fakeApiDataProcessorService.GetAsync<IList<ApiSummaryModel>>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(nullExpectedResults);

            var cmsApiService = new CmsApiService(CmsApiClientOptions, fakeApiDataProcessorService, fakeHttpClient, mapper, A.Fake<IApiCacheService>(), A.Fake<IContentTypeMappingService>());

            // act
            var result = await cmsApiService.GetSummaryAsync<ApiSummaryModel>().ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<IList<ApiSummaryModel>>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(result, nullExpectedResults);
        }

        [Fact]
        public async Task CmsApiServiceGetSummaryReturnsSuccess()
        {
            // arrange
            var expectedResults = A.CollectionOfFake<ApiSummaryModel>(2);

            A.CallTo(() => fakeApiDataProcessorService.GetAsync<IList<ApiSummaryModel>>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(expectedResults);

            var cmsApiService = new CmsApiService(CmsApiClientOptions, fakeApiDataProcessorService, fakeHttpClient, mapper, A.Fake<IApiCacheService>(), A.Fake<IContentTypeMappingService>());

            // act
            var result = await cmsApiService.GetSummaryAsync<ApiSummaryModel>().ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<IList<ApiSummaryModel>>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResults);
        }

        [Fact]
        public async Task CmsApiServiceGetItemNoChildrenReturnsSuccess()
        {
            // arrange
            var expectedResult = A.Fake<ApiItemNoChildrenModel>();

            A.CallTo(() => fakeApiDataProcessorService.GetAsync<ApiItemNoChildrenModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(expectedResult);

            var cmsApiService = new CmsApiService(CmsApiClientOptions, fakeApiDataProcessorService, fakeHttpClient, mapper, new ApiCacheService(), A.Fake<IContentTypeMappingService>());

            // act
            var result = await cmsApiService.GetItemAsync<ApiItemNoChildrenModel>("root-item-only", Guid.NewGuid()).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<ApiItemNoChildrenModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task CmsApiServiceGetItemNoChildrenByUrlReturnsSuccess()
        {
            // arrange
            var expectedResult = A.Fake<ApiItemNoChildrenModel>();
            var url = new Uri("https://www.somewhere.com", UriKind.Absolute);

            A.CallTo(() => fakeApiDataProcessorService.GetAsync<ApiItemNoChildrenModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(expectedResult);

            var fakeContentTypeMappingService = A.Fake<IContentTypeMappingService>();
            A.CallTo(() => fakeContentTypeMappingService.IgnoreRelationship).Returns(new List<string>());

            var cmsApiService = new CmsApiService(CmsApiClientOptions, fakeApiDataProcessorService, fakeHttpClient, mapper, new ApiCacheService(), A.Fake<IContentTypeMappingService>());

            // act
            var result = await cmsApiService.GetItemAsync<ApiItemNoChildrenModel>(url).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<ApiItemNoChildrenModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task CmsApiServiceGetItemReturnsSuccess()
        {
            // arrange
            var expectedResult = A.Fake<ApiItemModel>();
            var expectedItemResult = A.Fake<ApiContentItemModel>();
            expectedItemResult.Url = new Uri("http://www.testChild.com");

            var url = new Uri($"{CmsApiClientOptions.BaseAddress}api/someitem/", UriKind.Absolute);
            expectedResult.Url = url;
            var contentUrl = new Uri("http://www.test.com");

            var childContentUrl = new Uri("http://www.testChild.com");
            var fakeDictionary = new Dictionary<string, Type>();
            fakeDictionary.Add("test", typeof(ApiContentItemModel));

            A.CallTo(() => fakeMappingService.Mappings).Returns(fakeDictionary);
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<ApiItemModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(expectedResult);
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<ApiContentItemModel>(A<HttpClient>.Ignored, contentUrl)).Returns(expectedItemResult);
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<ApiContentItemModel>(A<HttpClient>.Ignored, childContentUrl)).Returns(new ApiContentItemModel());
            expectedResult.ContentLinks = new ContentLinksModel(new JObject())
            {
                ContentLinks = new List<KeyValuePair<string, List<LinkDetails>>>()
                {
                    new KeyValuePair<string, List<LinkDetails>>(
                        "test",
                        new List<LinkDetails>
                        {
                            new LinkDetails
                            {
                                Uri = contentUrl,
                                ContentType = "test"
                            },
                        }),
                },
            };
            expectedItemResult.ContentLinks = new ContentLinksModel(new JObject())
            {
                ContentLinks = new List<KeyValuePair<string, List<LinkDetails>>>()
                {
                    new KeyValuePair<string, List<LinkDetails>>(
                        "Child",
                        new List<LinkDetails>
                        {
                            new LinkDetails
                            {
                                Uri = new Uri("http://www.testChild.com"),
                                ContentType = "test"
                            },
                        }),
                },
            };

            var cmsApiService = new CmsApiService(CmsApiClientOptions, fakeApiDataProcessorService, fakeHttpClient, mapper, new ApiCacheService(), fakeMappingService);

            // act
            var result = await cmsApiService.GetItemAsync<ApiItemModel>(url).ConfigureAwait(false);

            // assert
            var expectedCount =
                expectedResult.ContentLinks.ContentLinks.SelectMany(contentLink => contentLink.Value).Count() +
                expectedItemResult.ContentLinks.ContentLinks.SelectMany(contentLink => contentLink.Value).Count();

            A.CallTo(() => fakeApiDataProcessorService.GetAsync<ApiItemModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            // Account for cache hit on child item
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<ApiItemModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappened(expectedCount - 1, Times.Exactly);
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task CmsApiServiceGetContentItemReturnsSuccess()
        {
            // arrange
            var expectedResult = A.Fake<ApiContentItemModel>();
            var url = new Uri($"{CmsApiClientOptions.BaseAddress}api/someitemcontent", UriKind.Absolute);

            A.CallTo(() => fakeApiDataProcessorService.GetAsync<ApiContentItemModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(expectedResult);

            var cmsApiService = new CmsApiService(CmsApiClientOptions, fakeApiDataProcessorService, fakeHttpClient, mapper, new ApiCacheService(), A.Fake<IContentTypeMappingService>());

            // act
            var result = await cmsApiService.GetContentItemAsync<ApiContentItemModel>(url).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<ApiContentItemModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task CmsApiServiceGetContentItemReturnsNull()
        {
            // arrange
            ApiContentItemModel? expectedResult = null;

            var cmsApiService = new CmsApiService(CmsApiClientOptions, fakeApiDataProcessorService, fakeHttpClient, mapper, A.Fake<IApiCacheService>(), A.Fake<IContentTypeMappingService>());

            // act
            var result = await cmsApiService.GetContentItemAsync<ApiContentItemModel>(null).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<ApiContentItemModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task CmsApiServiceGetContentReturnsSuccess()
        {
            // arrange
            var expectedResult = A.Fake<ApiItemModel>();
            var cmsApiClientOptions = CmsApiClientOptions;
            cmsApiClientOptions.ContentIds = Guid.NewGuid().ToString();

            A.CallTo(() => fakeApiDataProcessorService.GetAsync<ApiItemModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(expectedResult);

            var cmsApiService = new CmsApiService(cmsApiClientOptions, fakeApiDataProcessorService, fakeHttpClient, mapper, A.Fake<IApiCacheService>(), A.Fake<IContentTypeMappingService>());

            // act
            var result = await cmsApiService.GetContentAsync<ApiItemModel>().ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<ApiItemModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task CmsApiServiceGetContentItemReturnsNullWhenSkillsPassed()
        {
            // arrange
            var expectedResult = A.Fake<ApiContentItemModel>();
            var cmsApiClientOptions = CmsApiClientOptions;
            cmsApiClientOptions.ContentIds = Guid.NewGuid().ToString();
            var url = new Uri($"{CmsApiClientOptions.BaseAddress}api//skills", UriKind.Absolute);

            A.CallTo(() => fakeApiDataProcessorService.GetAsync<ApiContentItemModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(expectedResult);

            var cmsApiService = new CmsApiService(cmsApiClientOptions, fakeApiDataProcessorService, fakeHttpClient, mapper, A.Fake<IApiCacheService>(), A.Fake<IContentTypeMappingService>());

            // act
            var result = await cmsApiService.GetContentItemAsync<ApiContentItemModel>(url).ConfigureAwait(false);

            // assert
            A.Equals(result, null);
        }


        [Fact]
        public async Task CmsApiServiceGetContentItemReturnsNullWhenKnowledgePassed()
        {
            // arrange
            var expectedResult = A.Fake<ApiContentItemModel>();
            var cmsApiClientOptions = CmsApiClientOptions;
            cmsApiClientOptions.ContentIds = Guid.NewGuid().ToString();
            var url = new Uri($"{CmsApiClientOptions.BaseAddress}api//knowledge", UriKind.Absolute);

            A.CallTo(() => fakeApiDataProcessorService.GetAsync<ApiContentItemModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(expectedResult);

            var cmsApiService = new CmsApiService(cmsApiClientOptions, fakeApiDataProcessorService, fakeHttpClient, mapper, A.Fake<IApiCacheService>(), A.Fake<IContentTypeMappingService>());

            // act
            var result = await cmsApiService.GetContentItemAsync<ApiContentItemModel>(url).ConfigureAwait(false);

            // assert
            A.Equals(result, null);
        }
    }
}
