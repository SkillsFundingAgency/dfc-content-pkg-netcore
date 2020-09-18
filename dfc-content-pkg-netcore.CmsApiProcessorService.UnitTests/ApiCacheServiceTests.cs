using DFC.Content.Pkg.Netcore.Data.Models;
using DFC.Content.Pkg.Netcore.Services;
using System;
using Xunit;

namespace DFC.Content.Pkg.Netcore.CmsApiProcessorService.UnitTests
{
    [Trait("Category", "Api Cache Service Unit Tests")]
    public class ApiCacheServiceTests
    {
        [Fact]
        public void ApiCacheServiceWhenAddOrUpdateAddsItem()
        {
            //Arrange
            var apiCacheService = new ApiCacheService();
            var itemCacheUri = new Uri("http://somehost.com/somewhere/aresource");

            //Act
            apiCacheService.AddOrUpdate(itemCacheUri, new BaseContentItemModel() { Url = itemCacheUri });

            //Assert
            var cachedItem = apiCacheService.Retrieve<BaseContentItemModel>(itemCacheUri);
            Assert.Equal(itemCacheUri, cachedItem!.Url);
            Assert.Equal(1, apiCacheService.Count);
        }

        [Fact]
        public void ApiCacheServiceWhenGetReturnsNull()
        {
            //Arrange
            var apiCacheService = new ApiCacheService();

            //Act
            var item = apiCacheService.Retrieve<BaseContentItemModel>(new Uri("http://somewhere.com/somewhere/aresource"));

            //Assert
            Assert.Null(item);
            Assert.Equal(0, apiCacheService.Count);
        }

        [Fact]
        public void ApiCacheServiceWhenClearedNoItems()
        {
            //Arrange
            var apiCacheService = new ApiCacheService();
            var itemCacheUri = new Uri("http://somehost.com/somewhere/aresource");

            //Act
            apiCacheService.AddOrUpdate(itemCacheUri, new BaseContentItemModel() { Url = itemCacheUri });
            apiCacheService.Clear();

            //Assert
            Assert.Equal(0, apiCacheService.Count);
        }

        [Fact]
        public void APiCacheServiceWhenRemovedRemovesItem()
        {
            //Arrange
            var apiCacheService = new ApiCacheService();
            var itemCacheUri = new Uri("http://somehost.com/somewhere/aresource");

            //Act
            apiCacheService.AddOrUpdate(itemCacheUri, new BaseContentItemModel() { Url = itemCacheUri });
            apiCacheService.Remove(itemCacheUri);

            //Assert
            Assert.Equal(0, apiCacheService.Count);
        }
    }
}
