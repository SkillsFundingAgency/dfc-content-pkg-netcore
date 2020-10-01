using DFC.Content.Pkg.Netcore.ApiProcessorService.UnitTests.Models;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Services;
using System;
using Xunit;

namespace DFC.Content.Pkg.Netcore.UnitTests
{
    [Trait("Category", "Api Cache Service Unit Tests")]
    public class ApiCacheServiceTests
    {
        [Fact]
        public void ApiCacheServiceAddOrUpdateAddsItem()
        {
            //Arrange
            var serviceToTest = new ApiCacheService();

            //Act
            serviceToTest.AddOrUpdate(new Uri("http://somewhere.com/aresource"), new ApiItemModel());

            //Assert
            Assert.Equal(1, serviceToTest.Count);
        }

        [Fact]
        public void ApiCacheServiceAddOrUpdateUpdatesItem()
        {
            //Arrange
            var serviceToTest = new ApiCacheService();
            var itemToCache = new ApiItemModel() { Description = "a test item" };

            //Act
            serviceToTest.AddOrUpdate(new Uri("http://somewhere.com/aresource"), new ApiItemModel());
            serviceToTest.AddOrUpdate(new Uri("http://somewhere.com/aresource"), itemToCache);
            var result = serviceToTest.Retrieve<ApiItemModel>(new Uri("http://somewhere.com/aresource"));

            //Assert
            Assert.Equal(1, serviceToTest.Count);
            Assert.Equal(itemToCache.Description, result!.Description);
        }

        [Fact]
        public void ApiCacheServiceClearClearsItems()
        {
            //Arrange
            var serviceToTest = new ApiCacheService();

            //Act
            serviceToTest.AddOrUpdate(new Uri("http://somewhere.com/aresource"), new ApiItemModel());
            serviceToTest.Clear();

            //Assert
            Assert.Equal(0, serviceToTest.Count);
        }

        [Fact]
        public void ApiCacheServiceRemoveRemovesItem()
        {
            //Arrange
            var serviceToTest = new ApiCacheService();

            //Act
            serviceToTest.AddOrUpdate(new Uri("http://somewhere.com/aresource"), new ApiItemModel());
            serviceToTest.Remove(new Uri("http://somewhere.com/aresource"));

            //Assert
            Assert.Equal(0, serviceToTest.Count);
        }

        [Fact]
        public void ApiCacheServiceRetrieveRetrievesItem()
        {
            //Arrange
            var serviceToTest = new ApiCacheService();
            var itemToCache = new ApiItemModel() { Description = "a test item" };
            //Act
            serviceToTest.AddOrUpdate(new Uri("http://somewhere.com/aresource"), itemToCache);
            var result = serviceToTest.Retrieve<ApiItemModel>(new Uri("http://somewhere.com/aresource"));

            //Assert
            Assert.Equal(1, serviceToTest.Count);
            Assert.Equal(itemToCache.Description, result!.Description);
        }


        [Fact]
        public void ApiCacheServiceRetrieveByTypeRetrievesItem()
        {
            //Arrange
            var serviceToTest = new ApiCacheService();
            var itemToCache = new ApiItemModel() { Description = "a test item" };
            //Act
            serviceToTest.AddOrUpdate(new Uri("http://somewhere.com/aresource"), itemToCache);
            var result = serviceToTest.Retrieve<ApiItemModel>(itemToCache.GetType(), new Uri("http://somewhere.com/aresource"));

            //Assert
            Assert.Equal(1, serviceToTest.Count);
            Assert.Equal(itemToCache.Description, result!.Description);
        }

        [Fact]
        public void ApiCacheServiceRetrieveByTypeReturnsNullItem()
        {
            //Arrange
            var serviceToTest = new ApiCacheService();
            var itemToCache = new ApiItemModel() { Description = "a test item" };
            //Act
            serviceToTest.AddOrUpdate(new Uri("http://somewhere.com/aresource"), itemToCache);
            var result = serviceToTest.Retrieve<IBaseContentItemModel>(itemToCache.GetType(), new Uri("http://somewhere.com/aresource1"));

            //Assert
            Assert.Equal(1, serviceToTest.Count);
            Assert.Null(result);
        }
    }
}
