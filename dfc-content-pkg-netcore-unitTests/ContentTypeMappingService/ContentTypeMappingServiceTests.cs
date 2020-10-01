using DFC.Content.Pkg.Netcore.ApiProcessorService.UnitTests.Models;
using DFC.Content.Pkg.Netcore.Services;
using Xunit;

namespace DFC.Content.Pkg.Netcore.UnitTests
{
    [Trait("Category", "Content Type Mapping Service Unit Tests")]
    public class ContentTypeMappingServiceTests
    {
        [Fact]
        public void ContentTypeMappingServiceAddMappingAddsMapping()
        {
            //Arrange
            var serviceToTest = new ContentTypeMappingService();

            //Act
            serviceToTest.AddMapping("foo", typeof(ApiItemModel));

            //Assert
            Assert.Single(serviceToTest.Mappings);
        }

        [Fact]
        public void ContentTypeMappingServiceRemoveMappingRemovesMapping()
        {
            //Arrange
            var serviceToTest = new ContentTypeMappingService();

            //Act
            serviceToTest.AddMapping("foo", typeof(ApiItemModel));
            serviceToTest.RemoveMapping("foo");

            //Assert
            Assert.Empty(serviceToTest.Mappings);
        }

        [Fact]
        public void ContentTypeMappingServiceGetMappingReturnsMapping()
        {
            //Arrange
            var serviceToTest = new ContentTypeMappingService();

            //Act
            serviceToTest.AddMapping("foo", typeof(ApiItemModel));
            var result = serviceToTest.GetMapping("foo");

            //Assert
            Assert.True(result! == typeof(ApiItemModel));
        }

        [Fact]
        public void ContentTypeMappingServiceGetMappingReturnsNull()
        {
            //Arrange
            var serviceToTest = new ContentTypeMappingService();

            //Act
            serviceToTest.AddMapping("foo", typeof(ApiItemModel));
            var result = serviceToTest.GetMapping("bar");

            //Assert
            Assert.Null(result);
        }
    }
}
