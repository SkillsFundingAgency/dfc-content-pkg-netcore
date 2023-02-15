using DFC.Content.Pkg.Netcore.ApiProcessorService.UnitTests.Models;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Services;
using System;
using Xunit;

namespace DFC.Content.Pkg.Netcore.UnitTests
{
    [Trait("Category", "Api Cache Service (with cache stopped) Unit Tests")]
    public class ApiCacheServiceWithCacheStoppedTests
    {
        [Fact]
        public void ApiCacheServiceAddOrUpdateAddsItem()
        {
            //Arrange
            var serviceToTest = BuildApiCacheService();

            //Act
            serviceToTest.AddOrUpdate(new Uri("http://somewhere.com/aresource").ToString(), new ApiItemModel());

            //Assert
            Assert.Equal(0, serviceToTest.Count);
        }

        [Fact]
        public void ApiCacheServiceAddOrUpdateUpdatesItem()
        {
            //Arrange
            var serviceToTest = BuildApiCacheService();
            var itemToCache = new ApiItemModel() { Description = "a test item" };

            //Act
            serviceToTest.AddOrUpdate(new Uri("http://somewhere.com/aresource").ToString(), new ApiItemModel());
            serviceToTest.AddOrUpdate(new Uri("http://somewhere.com/aresource").ToString(), itemToCache);
            var result = serviceToTest.Retrieve<ApiItemModel>(new Uri("http://somewhere.com/aresource").ToString());

            //Assert
            Assert.Equal(0, serviceToTest.Count);
            Assert.Null(result);
        }

        [Fact]
        public void ApiCacheServiceClearClearsItems()
        {
            //Arrange
            var serviceToTest = BuildApiCacheService();

            //Act
            serviceToTest.AddOrUpdate(new Uri("http://somewhere.com/aresource").ToString(), new ApiItemModel());
            serviceToTest.Clear();

            //Assert
            Assert.Equal(0, serviceToTest.Count);
        }

        [Fact]
        public void ApiCacheServiceRemoveRemovesItem()
        {
            //Arrange
            var serviceToTest = BuildApiCacheService();

            //Act
            serviceToTest.AddOrUpdate(new Uri("http://somewhere.com/aresource").ToString(), new ApiItemModel());
            serviceToTest.Remove(new Uri("http://somewhere.com/aresource").ToString());

            //Assert
            Assert.Equal(0, serviceToTest.Count);
        }

        [Fact]
        public void ApiCacheServiceRetrieveRetrievesItem()
        {
            //Arrange
            var serviceToTest = BuildApiCacheService();
            var itemToCache = new ApiItemModel() { Description = "a test item" };

            //Act
            serviceToTest.AddOrUpdate(new Uri("http://somewhere.com/aresource").ToString(), itemToCache);
            var result = serviceToTest.Retrieve<ApiItemModel>(new Uri("http://somewhere.com/aresource").ToString());

            //Assert
            Assert.Equal(0, serviceToTest.Count);
            Assert.Null(result);
        }

        [Fact]
        public void ApiCacheServiceRetrieveByTypeRetrievesItem()
        {
            //Arrange
            var serviceToTest = BuildApiCacheService();
            var itemToCache = new ApiItemModel() { Description = "a test item" };

            //Act
            serviceToTest.AddOrUpdate(new Uri("http://somewhere.com/aresource").ToString(), itemToCache);
            var result = serviceToTest.Retrieve<ApiItemModel>(itemToCache.GetType(), new Uri("http://somewhere.com/aresource").ToString());

            //Assert
            Assert.Equal(0, serviceToTest.Count);
            Assert.Null(result);
        }

        [Fact]
        public void ApiCacheServiceRetrieveByTypeReturnsNullItem()
        {
            //Arrange
            var serviceToTest = BuildApiCacheService();
            var itemToCache = new ApiItemModel() { Description = "a test item" };

            //Act
            serviceToTest.AddOrUpdate(new Uri("http://somewhere.com/aresource").ToString(), itemToCache);
            var result = serviceToTest.Retrieve<IBaseContentItemModel>(itemToCache.GetType(), new Uri("http://somewhere.com/aresource1").ToString());

            //Assert
            Assert.Equal(0, serviceToTest.Count);
            Assert.Null(result);
        }

        private ApiCacheService BuildApiCacheService()
        {
            var serviceTotest = new ApiCacheService();

            serviceTotest.StopCache();

            return serviceTotest;
        }
    }
}
