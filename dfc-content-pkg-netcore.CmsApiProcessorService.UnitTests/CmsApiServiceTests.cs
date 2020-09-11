using AutoMapper;
using DFC.Content.Pkg.Netcore.CmsApiProcessorService.UnitTests.Models;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
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

            var cmsApiService = new CmsApiService(CmsApiClientOptions, fakeApiDataProcessorService, fakeHttpClient, mapper);

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

            var cmsApiService = new CmsApiService(CmsApiClientOptions, fakeApiDataProcessorService, fakeHttpClient, mapper);

            // act
            var result = await cmsApiService.GetSummaryAsync<ApiSummaryModel>().ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<IList<ApiSummaryModel>>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResults);
        }

        [Fact]
        public async Task CmsApiServiceGetItemReturnsSuccess()
        {
            // arrange
            var expectedResult = A.Fake<ApiItemModel>();
            var expectedItemResult = A.Fake<ApiContentItemModel>();
            var url = new Uri($"{CmsApiClientOptions.BaseAddress}api/someitem", UriKind.Absolute);
            var contentUrl = new Uri("http://www.test.com");
            var childContentUrl = new Uri("http://www.testChild.com");
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
                            },
                        }),
                },
            };

            var cmsApiService = new CmsApiService(CmsApiClientOptions, fakeApiDataProcessorService, fakeHttpClient, mapper);

            // act
            var result = await cmsApiService.GetItemAsync<ApiItemModel, ApiContentItemModel>(url).ConfigureAwait(false);

            // assert
            var expectedCount =
                expectedResult.ContentLinks.ContentLinks.SelectMany(contentLink => contentLink.Value).Count() +
                expectedItemResult.ContentLinks.ContentLinks.SelectMany(contentLink => contentLink.Value).Count();

            A.CallTo(() => fakeApiDataProcessorService.GetAsync<ApiItemModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<ApiContentItemModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappened(expectedCount, Times.Exactly);
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task CmsApiServiceGetContentItemReturnsSuccess()
        {
            // arrange
            var expectedResult = A.Fake<ApiContentItemModel>();
            var url = new Uri($"{CmsApiClientOptions.BaseAddress}api/someitemcontent", UriKind.Absolute);

            A.CallTo(() => fakeApiDataProcessorService.GetAsync<ApiContentItemModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(expectedResult);

            var cmsApiService = new CmsApiService(CmsApiClientOptions, fakeApiDataProcessorService, fakeHttpClient, mapper);

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

            var cmsApiService = new CmsApiService(CmsApiClientOptions, fakeApiDataProcessorService, fakeHttpClient, mapper);

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

            var cmsApiService = new CmsApiService(cmsApiClientOptions, fakeApiDataProcessorService, fakeHttpClient, mapper);

            // act
            var result = await cmsApiService.GetContentAsync<ApiItemModel>().ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<ApiItemModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}
